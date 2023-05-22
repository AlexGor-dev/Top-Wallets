#pragma once


#include "LiteClient.h"

namespace comx
{
    using namespace ton;
    using namespace block;
    using namespace td;

    static std::string privateKeyToString(const char* data)
    {
        BigInt256 bigInt256;
        bigInt256.import_bytes(reinterpret_cast<const unsigned char*>(data), (size_t)32, false);
        return bigInt256.to_dec_string_destroy();
    }

    static std::string publicKeyToString(const char* data)
    {
        block::PublicKey pubKey = block::PublicKey::from_bytes(Slice(data, (size_t)32)).move_as_ok();
        return pubKey.serialize(true);
    }

    static td::Result<block::PublicKey> get_public_key(td::Slice public_key)
    {
        TRY_RESULT_PREFIX(pubKey, block::PublicKey::parse(public_key), tonlib::TonlibError::InvalidPublicKey());
        return pubKey;
    }

    static td::Result<ton::RestrictedWallet::InitData> to_init_data(std::string& public_key, std::int64_t wallet_id)
    {
        std::string init_public_key = "Puasxr0QfFZZnYISRphVse7XHKfW7pZU5SJarVHXvQ+rpzkD";
        TRY_RESULT(init_key_bytes, get_public_key(init_public_key));
        TRY_RESULT(key_bytes, get_public_key(public_key));
        ton::RestrictedWallet::InitData init_data;
        init_data.init_key = td::SecureString(init_key_bytes.key);
        init_data.main_key = td::SecureString(key_bytes.key);
        init_data.wallet_id = static_cast<td::uint32>(wallet_id);
        return std::move(init_data);
    }

    td::Result<block::StdAddress> get_account_addressHighloadWalletV1(std::string& public_key, std::int64_t wallet_id, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::GenericAccount::get_address( workchain_id, ton::HighloadWallet::get_init_state(key, static_cast<td::uint32>(wallet_id), revision));
    }

    td::Result<block::StdAddress> get_account_addressHighloadWalletV2(std::string& public_key, std::int64_t wallet_id, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::GenericAccount::get_address(workchain_id, ton::HighloadWalletV2::get_init_state(key, static_cast<td::uint32>(wallet_id), revision));
    }

    td::Result<block::StdAddress> get_account_addressManualDns(std::string& public_key, std::int64_t wallet_id, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::ManualDns::create(key, static_cast<td::uint32>(wallet_id), revision)->get_address(workchain_id);
    }

    td::Result<block::StdAddress> get_account_addressWalletV1(std::string& public_key, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::GenericAccount::get_address(workchain_id, ton::TestWallet::get_init_state(key, revision));
    }

    td::Result<block::StdAddress> get_account_addressWalletV2(std::string& public_key, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::GenericAccount::get_address(workchain_id, ton::Wallet::get_init_state(key, revision));
    }

    static td::Result<block::StdAddress> get_account_addressWalletV3(std::string& public_key, std::int64_t wallet_id, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::GenericAccount::get_address(workchain_id,ton::WalletV3::get_init_state(key, static_cast<td::uint32>(wallet_id), revision));
    }

    static td::Result<block::StdAddress> get_account_addressWalletV4(std::string& public_key, std::int64_t wallet_id, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(key_bytes, get_public_key(public_key));
        auto key = td::Ed25519::PublicKey(td::SecureString(key_bytes.key));
        return ton::GenericAccount::get_address(workchain_id, ton::WalletV4::get_init_state(key, static_cast<td::uint32>(wallet_id), revision));
    }

    static td::Result<block::StdAddress> get_account_addressRestricted(std::string& public_key, std::int64_t wallet_id, td::int32 revision, ton::WorkchainId workchain_id)
    {
        TRY_RESULT(init_data, to_init_data(public_key, wallet_id));
        return ton::RestrictedWallet::create(init_data, revision)->get_address(workchain_id);
    }


    static td::Result<block::StdAddress> getAddress(std::string& public_key, WalletType type, std::int64_t wallet_id, int revision, ton::WorkchainId workchain_id)
    {
        switch (type)
        {
        case WalletType::WalletV4:
            return get_account_addressWalletV4(public_key, wallet_id, revision, workchain_id);
        case WalletType::WalletV3:
            return get_account_addressWalletV3(public_key, wallet_id, revision, workchain_id);
        case WalletType::WalletV1:
            return get_account_addressWalletV1(public_key, revision, workchain_id);
        case WalletType::WalletV2:
            return get_account_addressWalletV2(public_key, revision, workchain_id);
        case WalletType::RestrictedWallet:
            return get_account_addressRestricted(public_key, wallet_id, revision, workchain_id);
        case WalletType::HighloadWalletV1:
            return get_account_addressHighloadWalletV1(public_key, wallet_id, revision, workchain_id);
        case WalletType::HighloadWalletV2:
            return get_account_addressHighloadWalletV2(public_key, wallet_id, revision, workchain_id);
        case WalletType::ManualDns:
            return get_account_addressManualDns(public_key, wallet_id, revision, workchain_id);

        }
        return tonlib::TonlibError::InvalidAccountAddress();
    }

    struct Source
    {
        WalletType type;
        std::int64_t wallet_id;
        int revision;
        ton::WorkchainId workchain_id;
    };

    static void addSource(std::vector<Source>& sources, WalletType type, std::int64_t wallet_id)
    {
        for (auto revision : ton::SmartContractCode::get_revisions(type))
        {
            sources.push_back(Source{ type, wallet_id + ton::masterchainId, revision,  ton::masterchainId });
            sources.push_back(Source{ type, wallet_id + ton::basechainId, revision,  ton::basechainId });
        }
    }

    static td::Result<block::StdAddress> createAddress(LiteClient* client, std::string& public_key, WalletType type, int revision, ton::WorkchainId workchain_id)
    {
        return getAddress(public_key, type, client->getWalletID(), revision, workchain_id);
    }

    static td::Result<MessageRaw> getMessageRaw(LiteClient* client, const char* srcAddress, td::Ed25519::PrivateKey privKey, MessageInfo* messages, int messagesLen)
    {
        TRY_RESULT(state, client->getAccountState(std::string(srcAddress)));
        if (state->getState() != ContractState::Created)
            return td::Status::Error("waleetNotCreated");
        tonlib::KeyStorage keyStorage = client->getKeyStorage();

        td::unique_ptr<ton::WalletInterface> wallet = state->getWallet();
        if (!wallet)
            td::Status::Error("notFoundWalletType");

        std::vector<td::unique_ptr<comx::AccountState>> destinations;

        std::vector<ton::WalletInterface::Gift> gifts;
        for (int i = 0; i < messagesLen; i++)
        {
            MessageInfo msg = messages[i];
            TRY_RESULT(dest, client->getAccountState(std::string(msg.destAddress)));

            ton::WalletInterface::Gift gift;
            gift.destination = block::StdAddress(msg.destAddress);
            if (dest->getState() != ContractState::Created)
                gift.destination.bounceable = false;
            gift.gramms = msg.amount;
            if(msg.message)
                gift.message = std::string(msg.message);
            gift.body = td::Ref<vm::Cell>(msg.body);
            gift.init_state = td::Ref<vm::Cell>(msg.initState);
            gift.is_encrypted = msg.is_encrypted;
            gifts.push_back(gift);
            destinations.push_back(std::move(dest));
        }

        td::uint32 valid_until = state->raw.info.gen_utime + 60;
        TRY_RESULT(message_body, wallet->make_a_gift_message(privKey, valid_until, gifts));

        MessageRaw raw;
        raw.valid_until = valid_until;
        raw.message_body = std::move(message_body);
        raw.new_state = {};
        raw.message = ton::GenericAccount::create_ext_message(state->get_address(), raw.new_state, raw.message_body);
        raw.source = std::move(state);
        raw.destinations = std::move(destinations);

        return raw;
    }

    static td::Result<MessageRaw> getSendMessageRaw(LiteClient* client, const char* srcAddress, const char* publicKey, const char* password, const char* secret, MessageInfo* messages, int messagesLen)
    {
        TRY_RESULT(state, client->getAccountState(std::string(srcAddress)));
        if (state->getState() != ContractState::Created)
            return td::Status::Error("waleetNotCreated");

        tonlib::KeyStorage keyStorage = client->getKeyStorage();

        TRY_RESULT(prK, keyStorage.load_private_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32)}, SecureString(password, (size_t)64) }));
        td::Ed25519::PrivateKey privKey = td::Ed25519::PrivateKey(std::move(prK).private_key);
        return getMessageRaw(client, srcAddress, std::move(privKey), messages, messagesLen);
    }


    static td::Result<td::unique_ptr<comx::AccountState>> getAddress(LiteClient* client, std::string& public_key)
    {
        std::int64_t wallet_id = client->getWalletID();
        std::vector<Source> sources;

        addSource(sources, WalletType::WalletV4, wallet_id);
        addSource(sources, WalletType::WalletV3, wallet_id);
        addSource(sources, WalletType::WalletV1, wallet_id);
        addSource(sources, WalletType::WalletV1Ext, wallet_id);
        addSource(sources, WalletType::WalletV2, wallet_id);
        addSource(sources, WalletType::HighloadWalletV1, wallet_id);
        addSource(sources, WalletType::HighloadWalletV2, wallet_id);
        addSource(sources, WalletType::ManualDns, wallet_id);
        addSource(sources, WalletType::Multisig, wallet_id);
        addSource(sources, WalletType::PaymentChannel, wallet_id);
        addSource(sources, WalletType::RestrictedWallet, wallet_id);


        for (Source& source : sources)
        {
            td::Result<block::StdAddress> pa = getAddress(public_key, source.type, source.wallet_id, source.revision, source.workchain_id);
            if (pa.is_ok())
            {
                block::StdAddress address = pa.move_as_ok();
                td::Result<td::unique_ptr<comx::AccountState>> rstate = client->getAccountState(address.rserialize(true));
                if (rstate.is_ok())
                {
                    td::unique_ptr<comx::AccountState> state = rstate.move_as_ok();
                    if (state->getState() != ContractState::Uninitialized && state->balance() >= 0)
                    {
                        state->setType(source.type, source.revision);
                        return state;
                    }
                }
            }
        }
        return td::Status::Error("addressNotFoundFromPublicKey");
    }


    static td::Result<td::Ref<vm::Cell>> createWallet(LiteClient* client, const td::Ed25519::PublicKey& pubKey, const  td::Ed25519::PrivateKey& privKey, ton::WorkchainId workchain_id)
    {
        std::string pk = publicKeyToString(pubKey.as_octet_string().as_slice().data());
        TRY_RESULT(state, getAddress(client, pk));
        TRY_RESULT(stateInit, state->getStateInit(pk, client->getWalletID()));
        block::StdAddress addr = ton::GenericAccount::get_address(workchain_id, stateInit);
        td::Ref<vm::Cell> msg = vm::CellBuilder().store_long(client->getWalletID(), 32).store_long(-1, 32).store_long(0, 32).finalize();
        TRY_RESULT(signature, privKey.sign(msg->get_hash().as_slice()));
        return vm::CellBuilder().store_long(68, 7)
            .store_long(addr.workchain, 8)
            .store_bytes(addr.addr.as_slice())
            .store_long(2, 6)
            .append_cellslice(vm::load_cell_slice(stateInit))
            .store_long(0, 1)
            .store_bytes(signature)
            .append_cellslice(vm::load_cell_slice(msg))
            .finalize();
    }

    static comx::AccountState* findAddress(LiteClient* client)
    {
        std::int64_t wallet_id = client->getWalletID();
        std::vector<Source> sources;

        WalletType type = WalletType::WalletV3;
        for (auto revision : ton::SmartContractCode::get_revisions(type))
            sources.push_back(Source{ type, wallet_id + ton::basechainId, revision,  ton::basechainId });

        comx::AccountState* resState = NULL;

        do
        {
            Result<Ed25519::PrivateKey> pvKey = Ed25519::generate_private_key();
            if (pvKey.is_error())
                continue;
            Ed25519::PrivateKey privKey = pvKey.move_as_ok();
            std::string privKeys = privateKeyToString(privKey.as_octet_string().as_slice().data());


            Result<Ed25519::PublicKey> pbKey = privKey.get_public_key();
            if (pbKey.is_error())
                continue;

            Ed25519::PublicKey pubKey = pbKey.move_as_ok();

            std::string public_key = publicKeyToString(pubKey.as_octet_string().as_slice().data());
            for (Source& source : sources)
            {
                td::Result<block::StdAddress> pa = getAddress(public_key, source.type, source.wallet_id, source.revision, source.workchain_id);
                if (pa.is_ok())
                {
                    block::StdAddress address = pa.move_as_ok();
                    std::string addr = address.rserialize(true);
                    auto rstate = client->getAccountState(addr);
                    if (rstate.is_ok())
                    {
                        auto state = rstate.move_as_ok();
                        if (state->getState() != ContractState::Uninitialized && state->balance() >= 0)
                        {
                            resState = state.release();
                        }
                        if (resState)
                            break;
                    }
                }
            }
        }
        while (resState == NULL);

        return resState;
    }

}
