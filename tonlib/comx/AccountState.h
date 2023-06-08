#pragma once

#include "objects.h"

namespace comx
{

    enum class ContractState
    {
        Uninitialized,
        Initialized,
        Created,
        Frozen
    };

    struct RawAccountState
    {
        td::int64 balance = -1;

        ton::UnixTime storage_last_paid{ 0 };
        vm::CellStorageStat storage_stat;

        td::Ref<vm::Cell> code;
        td::Ref<vm::Cell> data;
        td::Ref<vm::Cell> state;
        std::string frozen_hash;
        block::AccountState::Info info;
        block::StdAddress address;
        ContractState contractState;
    };



    class AccountState
    {
    public:
        AccountState() {}
        AccountState(RawAccountState& raw)
            : raw(std::move(raw))
        {
        }

        ~AccountState()
        {

        }

        int64_t balance()
        {
            return raw.balance;
        }

        int64_t lastTransaqtionID()
        {
            return raw.info.last_trans_lt;
        }

        int64_t time()
        {
            return raw.storage_last_paid;
        }

        int64_t seqno()
        {
            if (!raw.data.is_null())
                return vm::load_cell_slice(raw.data).fetch_ulong(32);
            return -1;
        }

        ContractState getState()
        {
            return raw.contractState;
        }

        td::Slice getMsgHash()
        {
            return raw.info.root.get()->get_hash().as_slice();
        }

        bool is_frozen() const
        {
            return !raw.frozen_hash.empty();
        }

        const block::StdAddress& get_address() const
        {
            return raw.address;
        }

        void make_non_bounceable()
        {
            raw.address.bounceable = false;
        }

        td::uint32 get_sync_time() const
        {
            return raw.info.gen_utime;
        }

        std::string getStateInitBase64(std::string& public_key, std::int64_t wallet_id)
        {
            td::Result<td::Ref<vm::Cell>> r = getStateInit(public_key, wallet_id);
            if (r.is_ok())
            {
                td::Result<td::BufferSlice> bs = vm::std_boc_serialize(r.move_as_ok());
                return td::base64_encode(bs.move_as_ok().as_slice());
            }
            return "not";
        }

        SmartContract::State get_smc_state() const
        {
            return { raw.code, raw.data };
        }


        void setType(WalletType type, td::int32 revision)
        {
            this->wallet_type = type;
            this->wallet_revision = revision;
        }

        bool guess_revision(WalletType type, const vm::Cell::Hash& code_hash)
        {
            td::int32 revision = SmartContractCode::guess_revision(type, code_hash);
            if (revision != 0)
            {
                wallet_type = type;
                wallet_revision = revision;
                return true;
            }
            return false;
        }

        WalletType get_wallet_type()
        {
            if (wallet_type == WalletType::Empty)
                wallet_type = guess_type();
            return wallet_type;
        }

        td::int64 get_balance() const
        {
            return raw.balance;
        }

        td::Result<td::Ref<vm::Cell>> getStateInit(std::string& public_key, std::int64_t wallet_id);
        td::unique_ptr<ton::WalletInterface> getWallet();
        WalletType guess_type();
        std::string getVersion();
        std::vector<td::unique_ptr<VmObject>> runMethod(std::string methodName, std::vector<std::shared_ptr<VmObject>> params);
        static td::Result<RawAccountState> create(tonlib::LastBlockState state, block::StdAddress address, td::Result<object_ptr<liteServer_accountState>> r_accountState);

        RawAccountState raw;

    private:
        WalletType wallet_type{ WalletType::Empty };
        td::int32 wallet_revision{ 0 };
    };
}



