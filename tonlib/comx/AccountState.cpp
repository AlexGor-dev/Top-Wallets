#include "objects.h"

namespace comx
{

    static block::AccountState create_account_state(ton::tl_object_ptr<liteServer_accountState> from)
    {
        block::AccountState res;
        res.blk = ton::create_block_id(from->id_);
        res.shard_blk = ton::create_block_id(from->shardblk_);
        res.shard_proof = std::move(from->shard_proof_);
        res.proof = std::move(from->proof_);
        res.state = std::move(from->state_);
        return res;
    }

    td::Result<td::int64> to_balance_or_throw(td::Ref<vm::CellSlice> balance_ref)
    {
        vm::CellSlice balance_slice = *balance_ref;
        auto balance = block::tlb::t_Grams.as_integer_skip(balance_slice);
        if (balance.is_null())
            return td::Status::Error("Failed to unpack balance");
        auto res = balance->to_long();
        if (res == td::int64(~0ULL << 63))
            return td::Status::Error("Failed to unpack balance (2)");
        return res;
    }

    td::Result<td::int64> to_balance(td::Ref<vm::CellSlice> balance_ref)
    {
        return TRY_VM(to_balance_or_throw(std::move(balance_ref)));
    }

    td::Result<RawAccountState> AccountState::create(tonlib::LastBlockState lastState, block::StdAddress address, td::Result<object_ptr<liteServer_accountState>> r_accountState)
    {
        object_ptr<liteServer_accountState> raw_account_state = r_accountState.move_as_ok();
        auto account_state = create_account_state(std::move(raw_account_state));
        TRY_RESULT(info, account_state.validate(lastState.last_block_id, address));
        auto serialized_state = account_state.state.clone();
        RawAccountState res;
        res.address = address;
//        res.block_id = block_id_.value();
        res.info = std::move(info);
        res.contractState = ContractState::Uninitialized;

        auto cell = res.info.root;
        //std::ostringstream outp;
        //block::gen::t_Account.print_ref(outp, cell);
        //LOG(INFO) << outp.str();
        if (cell.is_null()) {
            return res;
        }
        block::gen::Account::Record_account account;
        if (!tlb::unpack_cell(cell, account)) {
            return td::Status::Error("Failed to unpack Account");
        }
        {
            block::gen::StorageInfo::Record storage_info;
            if (!tlb::csr_unpack(account.storage_stat, storage_info)) {
                return td::Status::Error("Failed to unpack StorageInfo");
            }
            res.storage_last_paid = storage_info.last_paid;
            td::RefInt256 due_payment;
            if (storage_info.due_payment->prefetch_ulong(1) == 1) {
                vm::CellSlice& cs2 = storage_info.due_payment.write();
                cs2.advance(1);
                due_payment = block::tlb::t_Grams.as_integer_skip(cs2);
                if (due_payment.is_null() || !cs2.empty_ext()) {
                    return td::Status::Error("Failed to upack due_payment");
                }
            } else {
                due_payment = td::RefInt256{true, 0};
            }
            block::gen::StorageUsed::Record storage_used;
            if (!tlb::csr_unpack(storage_info.used, storage_used)) {
                return td::Status::Error("Failed to unpack StorageInfo");
            }
            unsigned long long u = 0;
            vm::CellStorageStat storage_stat;
            u |= storage_stat.cells = block::tlb::t_VarUInteger_7.as_uint(*storage_used.cells);
            u |= storage_stat.bits = block::tlb::t_VarUInteger_7.as_uint(*storage_used.bits);
            u |= storage_stat.public_cells = block::tlb::t_VarUInteger_7.as_uint(*storage_used.public_cells);
            //LOG(DEBUG) << "last_paid=" << res.storage_last_paid << "; cells=" << storage_stat.cells
            //<< " bits=" << storage_stat.bits << " public_cells=" << storage_stat.public_cells;
            if (u == std::numeric_limits<td::uint64>::max()) {
                return td::Status::Error("Failed to unpack StorageStat");
            }

            res.storage_stat = storage_stat;
        }

        block::gen::AccountStorage::Record storage;
        if (!tlb::csr_unpack(account.storage, storage)) {
            return td::Status::Error("Failed to unpack AccountStorage");
        }
        TRY_RESULT(balance, to_balance(storage.balance));
        res.balance = balance;

        auto state_tag = block::gen::t_AccountState.get_tag(*storage.state);
        if (state_tag < 0) {
            return td::Status::Error("Failed to parse AccountState tag");
        }

        switch (state_tag)
        {
            case block::gen::AccountState::account_frozen:
                res.contractState = ContractState::Frozen;
                block::gen::AccountState::Record_account_frozen state;
                if (!tlb::csr_unpack(storage.state, state))
                {
                    return td::Status::Error("Failed to parse AccountState");
                }
                res.frozen_hash = state.state_hash.as_slice().str();
                return res;
            case block::gen::AccountState::account_uninit:
                res.contractState = ContractState::Initialized;
                return res;
            case block::gen::AccountState::account_active:
                res.contractState = ContractState::Created;
                break;
            default:
                return res;
        }


        block::gen::AccountState::Record_account_active state;
        if (!tlb::csr_unpack(storage.state, state)) {
            return td::Status::Error("Failed to parse AccountState");
        }
        block::gen::StateInit::Record state_init;
        res.state = vm::CellBuilder().append_cellslice(state.x).finalize();
        if (!tlb::csr_unpack(state.x, state_init)) {
            return td::Status::Error("Failed to parse StateInit");
        }
        state_init.code->prefetch_maybe_ref(res.code);
        state_init.data->prefetch_maybe_ref(res.data);
        return res;
    }

    auto to_tonlib_api(const vm::StackEntry& entry) -> VmObject*
    {
        switch (entry.type())
        {
            case vm::StackEntry::Type::t_int:
                return new VmObjectNumber(dec_string(entry.as_int()));
            case vm::StackEntry::Type::t_cell:
                return new VmObjectSlice(new vm::CellSlice(td::Ref<vm::DataCell>((vm::DataCell*)entry.as_cell().get())));
            case vm::StackEntry::Type::t_slice:
                return new VmObjectSlice((vm::CellSlice*)entry.as_slice().get());
        }

    };
    std::vector<VmObject*> AccountState::runMethod(std::string methodName, std::vector<VmObject*> params)
    {
        std::vector<VmObject*> res_stack;

        ton::SmartContract smc(ton::SmartContract::State{raw.code, raw.data});
        td::Ref<vm::Stack> stack(true);

        ton::SmartContract::Args args;
        args.set_method_id(methodName);

        for (VmObject* entry : params)
        {
            switch (entry->getType())
            {
                case VmObjectType::Number:
                    stack.write().push_int(td::dec_string_to_int256(((VmObjectNumber*)entry)->getValue()));
                    break;
                case VmObjectType::Cell:
                    stack.write().push_cell(vm::Ref<vm::Cell>(((VmObjectCell*)entry)->getValue()));
                    break;
                case VmObjectType::Slice:
                    stack.write().push_cellslice(vm::Ref<vm::CellSlice>(((VmObjectSlice*)entry)->getValue()));
                    break;
            }
        }

        args.set_stack(std::move(stack));
        ton::SmartContract::Answer res = smc.run_get_method(std::move(args));

        for (auto &entry : res.stack->as_span())
        {
            switch (entry.type())
            {
            case vm::StackEntry::Type::t_int:
                res_stack.push_back(new VmObjectNumber(dec_string(entry.as_int())));
                break;
            case vm::StackEntry::Type::t_cell:
                res_stack.push_back(new VmObjectSlice(new vm::CellSlice(vm::CellBuilder()
                .append_data_cell(vm::Ref<vm::DataCell>(entry.as_cell())).finalize_novm())));
                break;
            case vm::StackEntry::Type::t_slice:
                res_stack.push_back(new VmObjectSlice(new vm::CellSlice(entry.as_slice().write())));
                break;
            }
        }
        return res_stack;
    }

}


