#include "AccountState.h"

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
    std::vector<td::unique_ptr<VmObject>> AccountState::runMethod(std::string methodName, std::vector<std::shared_ptr<VmObject>> params)
    {
        std::vector<td::unique_ptr<VmObject>> res_stack;

        ton::SmartContract smc(ton::SmartContract::State{raw.code, raw.data});
        td::Ref<vm::Stack> stack(true);

        ton::SmartContract::Args args;
        args.set_method_id(methodName);

        for (std::shared_ptr<VmObject> entry : params)
        {
            switch (entry->getType())
            {
                case VmObjectType::Number:
                    stack.write().push_int(td::dec_string_to_int256(((VmObjectNumber*)entry.get())->getValue()));
                    break;
                case VmObjectType::Cell:
                    stack.write().push_cell(vm::Ref<vm::Cell>(((VmObjectCell*)entry.get())->getValue()));
                    break;
                case VmObjectType::Slice:
                    stack.write().push_cellslice(vm::Ref<vm::CellSlice>(((VmObjectSlice*)entry.get())->getValue()));
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
                res_stack.push_back(td::make_unique<VmObjectNumber>(dec_string(entry.as_int())));
                break;
            case vm::StackEntry::Type::t_cell:
                res_stack.push_back(td::make_unique<VmObjectSlice>(new vm::CellSlice(vm::CellBuilder()
                .append_data_cell(vm::Ref<vm::DataCell>(entry.as_cell())).finalize_novm())));
                break;
            case vm::StackEntry::Type::t_slice:
                res_stack.push_back(td::make_unique<VmObjectSlice>(new vm::CellSlice(entry.as_slice().write())));
                break;
            }
        }
        return res_stack;
    }

    td::Result<td::Ref<vm::Cell>> AccountState::getStateInit(std::string& public_key, std::int64_t wallet_id)
    {
        TRY_RESULT_PREFIX(pubKey, block::PublicKey::parse(public_key), tonlib::TonlibError::InvalidPublicKey());
        if (wallet_type == WalletType::Unknown)
            guess_type();
        auto key = td::Ed25519::PublicKey(td::SecureString(pubKey.key));
        switch (wallet_type)
        {
        case WalletType::WalletV4:
            return ton::WalletV4::get_init_state(key, static_cast<td::uint32>(wallet_id), wallet_revision);
        case WalletType::WalletV3:
            return ton::WalletV3::get_init_state(key, static_cast<td::uint32>(wallet_id), wallet_revision);
        case WalletType::WalletV1:
            return ton::TestWallet::get_init_state(key, wallet_revision);
        case WalletType::WalletV2:
            return ton::Wallet::get_init_state(key, wallet_revision);
        case WalletType::HighloadWalletV1:
            return ton::HighloadWallet::get_init_state(key, static_cast<td::uint32>(wallet_id), wallet_revision);
        case WalletType::HighloadWalletV2:
            return ton::HighloadWalletV2::get_init_state(key, static_cast<td::uint32>(wallet_id), wallet_revision);
        }
        return td::Status::Error("unknownType");
    }

    td::unique_ptr<ton::WalletInterface> AccountState::getWallet()
    {
        switch (get_wallet_type())
        {
        case WalletType::WalletV4:
            return td::make_unique<WalletV4>(get_smc_state());
        case WalletType::WalletV3:
            return td::make_unique<WalletV3>(get_smc_state());
        case WalletType::HighloadWalletV2:
            return td::make_unique<HighloadWalletV2>(get_smc_state());
        case WalletType::HighloadWalletV1:
            return td::make_unique<HighloadWallet>(get_smc_state());
        case WalletType::RestrictedWallet:
            return td::make_unique<RestrictedWallet>(get_smc_state());
        case WalletType::Giver:
            return td::make_unique<TestGiver>(get_smc_state());
        case WalletType::WalletV1:
            return td::make_unique<TestWallet>(get_smc_state());
        case WalletType::WalletV2:
            return td::make_unique<Wallet>(get_smc_state());
        default:
            break;
        }
        return nullptr;
    }

    WalletType AccountState::guess_type()
    {
        if (raw.code.is_null())
        {
            wallet_type = WalletType::Empty;
            return wallet_type;
        }
        if (wallet_type == WalletType::Empty)
        {
            vm::Cell::Hash code_hash = raw.code->get_hash();

            if (guess_revision(WalletType::WalletV3, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::WalletV4, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::NftCollection, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::NftItem, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::NftSingle, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::NftMarketplace, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::NftSale, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::JettonMinter, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::JettonWallet, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::HighloadWalletV2, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::HighloadWalletV1, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::ManualDns, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::RestrictedWallet, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::Giver, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::WalletV1, code_hash))
                return wallet_type;

            if (guess_revision(WalletType::WalletV2, code_hash))
                return wallet_type;

            wallet_revision = -1;
            std::vector<std::shared_ptr<VmObject>> vparams;
            std::vector<td::unique_ptr<VmObject>> vres = runMethod("get_wallet_data", vparams);
            if (vres.size() >= 3 && is<VmObjectNumber>(vres[0].get()) && is<VmObjectSlice>(vres[1].get()) && is<VmObjectSlice>(vres[2].get()))
            {
                wallet_type = WalletType::JettonWallet;
                return wallet_type;
            }
            vres = runMethod("get_jetton_data", vparams);
            if (vres.size() >= 4 && is<VmObjectNumber>(vres[0].get()) && is<VmObjectSlice>(vres[1].get()) && is<VmObjectSlice>(vres[2].get()))
            {
                wallet_type = WalletType::JettonMinter;
                return wallet_type;
            }
            vres = runMethod("get_collection_data", vparams);
            if (vres.size() == 3 && is<VmObjectNumber>(vres[0].get()) && is<VmObjectSlice>(vres[1].get()) && is<VmObjectSlice>(vres[2].get()))
            {
                wallet_type = WalletType::NftCollection;
                return wallet_type;
            }
            vres = runMethod("get_nft_data", vparams);
            if (vres.size() >= 3 && is<VmObjectNumber>(vres[0].get()) && is<VmObjectNumber>(vres[1].get()) && is<VmObjectSlice>(vres[2].get()))
            {
                if (((VmObjectSlice*)vres[2].get())->getValue()->fetch_ulong(2) == 2)
                    wallet_type = WalletType::NftItem;
                else
                    wallet_type = WalletType::NftSingle;
                return wallet_type;
            }

            if (wallet_type == WalletType::Empty)
                wallet_type = WalletType::Unknown;
        }
        return wallet_type;
    }

    std::string AccountState::getVersion()
    {
        WalletType type = get_wallet_type();
        std::string s = std::to_string(wallet_revision);
        switch (type)
        {
        case WalletType::RestrictedWallet:
            return "Rw r" + s;
        case WalletType::WalletV4:
            return "V4 r" + s;
        case WalletType::WalletV3:
            return "V3 r" + s;
        case WalletType::WalletV1:
            return "V1 r" + s;
        case WalletType::WalletV2:
            return "V r" + s;
        case WalletType::HighloadWalletV1:
            return "Hv1 r" + s;
        case WalletType::HighloadWalletV2:
            return "Hv2 r" + s;
        case WalletType::ManualDns:
            return "Md r" + s;
        case WalletType::Giver:
            return "Giver";
        }
        return SmartContractCode::getWalletName(type);
    }

}


