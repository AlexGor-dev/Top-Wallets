#pragma once

#include "headers.h"
#include "td/utils/port/detail/EventFdWindows.h"

namespace comx
{
    using namespace ton;
    using namespace ton::lite_api;

    typedef void(*QueryLongHandler)(int64_t);
    typedef void(*QueryStringHandler)(const char*);
    typedef void(*QueryResultHandler)(int64_t,const char*);
    typedef void(*QueryGetAccount)(void*, int64_t, const char*);

    template <class T> static int64_t toHandle(const T *object)
    {
        return static_cast<int64_t>(reinterpret_cast<std::uintptr_t>(object));
    }

    class MessageInfo
    {
    public:
        const char* destAddress;
        int64_t amount;
        const char* message;
        vm::Cell* body;
        vm::Cell* initState;
        bool is_encrypted;
    };

    class ManualResetEvent
    {
        HANDLE handle;

    public:
        ManualResetEvent(bool initState)
        {
            handle = CreateEvent(nullptr, true, initState, nullptr);
        }
        ~ManualResetEvent()
        {
            close();
        }
        void close()
        {
            if (handle != 0)
                CloseHandle(handle);
            handle = 0;
        }

        void wait(int timeout_ms)
        {
            WaitForSingleObject(handle, timeout_ms);
        }

        void reset()
        {
            ResetEvent(handle);
        }
        void set()
        {
            SetEvent(handle);
        }
    };

    enum class VmObjectType
    {
        Unknown,
        Number,
        Cell,
        Slice,
    };

    class Object
    {

    };

    class VmObject : public Object
    {
        VmObjectType type;
    public:
        VmObject(VmObjectType type)
            :type(type)
        {
        }
        ~VmObject()
        {

        }
        VmObjectType getType() { return type; }
    };

    class VmObjectNumber : public VmObject
    {
        char* value;
    public:
        VmObjectNumber(std::string value) 
            :VmObject(VmObjectType::Number)
        {
            this->value = new char[value.size() + 1];
            strcpy(this->value, value.c_str());
        }
        ~VmObjectNumber()
        {
            delete[] this->value;
        }
        std::string getValue() { return std::string(value); }
    };

    class VmObjectCell : public VmObject
    {
        vm::Cell* value;
    public:
        VmObjectCell(vm::Cell* value) 
            :value(value), VmObject(VmObjectType::Cell)
        {}
        vm::Cell* getValue() { return value; }
    };

    class VmObjectSlice : public VmObject
    {
        vm::CellSlice* value;
    public:
        VmObjectSlice(vm::CellSlice* value)
            :value(value), VmObject(VmObjectType::Slice) 
        {}
        vm::CellSlice* getValue() { return value; }
    };


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

        ton::UnixTime storage_last_paid{0};
        vm::CellStorageStat storage_stat;

        td::Ref<vm::Cell> code;
        td::Ref<vm::Cell> data;
        td::Ref<vm::Cell> state;
        std::string frozen_hash;
        block::AccountState::Info info;
        block::StdAddress address;
        ContractState contractState;
    };

    class AccountState : public Object
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


        td::Result<td::Ref<vm::Cell>> getStateInit(std::string& public_key, std::int64_t wallet_id)
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

        td::unique_ptr<ton::WalletInterface> getWallet()
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

        WalletType guess_type()
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

                std::vector<comx::VmObject*> vparams;
                std::vector<comx::VmObject*> vres = runMethod("get_wallet_data", vparams);
                if (vres.size() >= 3)
                {
                    wallet_type = WalletType::JettonWallet;
                    wallet_revision = -1;
                }
                for (int i = 0; i < vres.size(); i++)
                    delete vres[i];
                if (wallet_type == WalletType::Empty)
                {
                    std::vector<comx::VmObject*> vres = runMethod("get_jetton_data", vparams);
                    if (vres.size() >= 4)
                    {
                        wallet_type = WalletType::JettonMinter;
                        wallet_revision = -1;
                    }
                    for (int i = 0; i < vres.size(); i++)
                        delete vres[i];
                }

                if (wallet_type == WalletType::Empty)
                    wallet_type = WalletType::Unknown;
            }
            return wallet_type;
        }

        WalletType get_wallet_type()
        {
            if (wallet_type == WalletType::Empty)
                wallet_type = guess_type();
            return wallet_type;
        }

        std::string getVersion()
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

        td::int64 get_balance() const
        {
            return raw.balance;
        }

        std::vector<VmObject*> runMethod(std::string methodName, std::vector<VmObject*> params);
        static td::Result<RawAccountState> create(tonlib::LastBlockState state, block::StdAddress address, td::Result<object_ptr<liteServer_accountState>> r_accountState);

        RawAccountState raw;

        private:
            WalletType wallet_type{ WalletType::Empty };
            td::int32 wallet_revision{ 0 };
    };


    struct MessageRaw
    {
        td::unique_ptr<AccountState> source;
        std::vector<td::unique_ptr<AccountState>> destinations;

        td::uint32 valid_until{ std::numeric_limits<td::uint32>::max() };

        td::Ref<vm::Cell> message;
        td::Ref<vm::Cell> new_state;
        td::Ref<vm::Cell> message_body;
    };

    static td::Result<td::BufferSlice> decrypt(td::Ed25519::PrivateKey& pk_, td::Slice data)
    {
        if (data.size() < td::Ed25519::PublicKey::LENGTH + 32)
        {
            return td::Status::Error(501, "message is too short");
        }

        td::Slice pub = data.substr(0, td::Ed25519::PublicKey::LENGTH);
        data.remove_prefix(td::Ed25519::PublicKey::LENGTH);

        td::Slice digest = data.substr(0, 32);
        data.remove_prefix(32);

        TRY_RESULT_PREFIX(shared_secret,
                          td::Ed25519::compute_shared_secret(td::Ed25519::PublicKey(td::SecureString(pub)), pk_),
                          "failed to generate shared secret: ");

        td::SecureString key(32);
        key.as_mutable_slice().copy_from(td::Slice(shared_secret).substr(0, 16));
        key.as_mutable_slice().substr(16).copy_from(digest.substr(16, 16));

        td::SecureString iv(16);
        iv.as_mutable_slice().copy_from(digest.substr(0, 4));
        iv.as_mutable_slice().substr(4).copy_from(td::Slice(shared_secret).substr(20, 12));

        td::BufferSlice res(data.size());

        td::AesCtrState ctr;
        ctr.init(key, iv);
        ctr.encrypt(data, res.as_slice());

        td::UInt256 real_digest;
        td::sha256(res.as_slice(), as_slice(real_digest));

        if (as_slice(real_digest) != digest)
        {
            return td::Status::Error(501, "sha256 mismatch after decryption");
        }

        return std::move(res);
    }

    static td::Result<td::BufferSlice> encrypt(td::Ed25519::PublicKey& pub_, td::Slice data)
    {
        TRY_RESULT_PREFIX(pk, td::Ed25519::generate_private_key(), "failed to generate private key: ");
        TRY_RESULT_PREFIX(pubkey, pk.get_public_key(), "failed to get public key from private: ");
        auto pubkey_str = pubkey.as_octet_string();

        td::BufferSlice msg(pubkey_str.size() + 32 + data.size());
        td::MutableSlice slice = msg.as_slice();
        slice.copy_from(pubkey_str);
        slice.remove_prefix(pubkey_str.size());

        TRY_RESULT_PREFIX(shared_secret, td::Ed25519::compute_shared_secret(pub_, pk), "failed to compute shared secret: ");

        td::MutableSlice digest = slice.substr(0, 32);
        slice.remove_prefix(32);
        td::sha256(data, digest);

        td::SecureString key(32);
        {
            auto S = key.as_mutable_slice();
            S.copy_from(td::Slice(shared_secret).truncate(16));
            S.remove_prefix(16);
            S.copy_from(digest.copy().remove_prefix(16).truncate(16));
        }

        td::SecureString iv(16);
        {
            auto S = iv.as_mutable_slice();
            S.copy_from(digest.copy().truncate(4));
            S.remove_prefix(4);
            S.copy_from(td::Slice(shared_secret).remove_prefix(20).truncate(12));
        }

        td::AesCtrState ctr;
        ctr.init(key, iv);
        ctr.encrypt(data, slice);

        return std::move(msg);
    }


}
