#include <iostream>
#include <string>
#include <cstdint>
#include <cstdlib>
#include <utility>

#include <openssl/aes.h>
#include <openssl/evp.h>

#define TD_PORT_WINDOWS 1

#include "LiteClient.h"
#include "utils.h"

using namespace comx;
using namespace vm;
using namespace td;
using namespace tonlib;
//using namespace block;
using namespace ::tlb;

void SliceToArray(Slice slice, void** res, size_t* resLen)
{
    size_t s = slice.size();
    *resLen = s;
    *res = (void*)LocalAlloc(0, s);
    memcpy(*res, slice.data(), s);
}

void SliceToArray(Slice slice, const char* res, size_t* resLen)
{
    *resLen = slice.size();
    if(res)
        memcpy((void*)res, slice.data(), slice.size());
}

//***************************************************************************************
void LiteClientGetServerTime(LiteClient* client, QueryResultHandler resultHandler)
{
    client->getServerTime([resultHandler = resultHandler](td::Result<int64_t> rtime)
    {
        if (rtime.is_error())
            resultHandler(-1, rtime.move_as_error().to_string().c_str());
        else
            resultHandler(rtime.move_as_ok(), nullptr);
    });
}

LiteClient* LiteClientCreate(const char* name, const char* directory, const char* config, const char* validConfig, QueryLongHandler connectionHandler)
{
	return new LiteClient(std::string(name), std::string(directory), std::string(config), std::string(validConfig), connectionHandler);
}

void LiteClientDestroy(LiteClient* client)
{
	delete client;
}

int LiteClientGetServerIndex(LiteClient* client)
{
    return client->getServerIndex();
}

void LiteClientConnect(LiteClient* client)
{
    client->connect();
}

bool LiteClientIsConnected(LiteClient* client)
{
    return client->isConnected();
}

void LiteClientSend(LiteClient* client, vm::Cell* cell, QueryResultHandler resultHandler)
{
    td::Ref<vm::Cell> rcell(cell);
    client->send(rcell, [resultHandler = resultHandler](td::Result<std::string> rhash)
    {
        if (rhash.is_error())
            resultHandler(-1, rhash.move_as_error().to_string().c_str());
        else
            resultHandler((int64_t)rhash.move_as_ok().c_str(), nullptr);
    });
}

void LiteClientLast(LiteClient* client,  QueryResultHandler resultHandler)
{
    client->last([resultHandler = resultHandler](auto rlast)
    {
        if (rlast.is_error())
            resultHandler(-1, rlast.move_as_error().to_string().c_str());
        else
            resultHandler(0, nullptr);
    });
}

void LiteClientGetTransactions(LiteClient* client, const char* address, td::uint64 lt, const byte* prev_lt_hash, QueryResultHandler resultHandler, int count, QueryLongHandler handler)
{
    client->getTransactions(std::string(address), lt, ton::Bits256(prev_lt_hash), count, handler, [resultHandler = resultHandler](auto rtrans)
    {
        if (rtrans.is_error())
            resultHandler(-1, rtrans.move_as_error().to_string().c_str());
        else
            resultHandler(0, nullptr);
    });
}

comx::AccountState* LiteClientFindAddress(LiteClient* client)
{
    return findAddress(client);
}

void LiteClientGetAccountState(LiteClient* client, const char* address, QueryResultHandler resultHandler)
{
    client->getAccountState(std::string(address), [resultHandler = resultHandler](auto rsate)
    {
        if (rsate.is_error())
            resultHandler(-1, rsate.move_as_error().to_string().c_str());
        else
            resultHandler(toHandle<comx::AccountState>(rsate.move_as_ok().release()), nullptr);
    });
}

bool LiteClientGetImportKey(LiteClient* client, const char* mnemonicPassword, char** worlds, const char* publicKey, const char* password, const char* secret, LPTSTR* pBuffer, size_t* resLen, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();
    std::vector<SecureString> result;
    for (int i = 0; i < 24; i++)
        result.push_back(SecureString(Slice(worlds[i], strlen(worlds[i]))));


    td::Result<KeyStorage::Key> key_r = keyStorage.import_key(Slice(password, (size_t)64), Slice(mnemonicPassword, (size_t)0), KeyStorage::ExportedKey{ std::move(result) });
    if (key_r.is_error())
    {
        resultHandler(-1, key_r.move_as_error().to_string().c_str());
        return false;
    }
    KeyStorage::Key key = key_r.move_as_ok();
    td::Result<td::unique_ptr<comx::AccountState>> rstate = getAddress(client, publicKeyToString(key.public_key.as_slice().data()));
    if (rstate.is_error())
    {
        resultHandler(-1, rstate.move_as_error().to_string().c_str());
        return false;
    }
    td::unique_ptr<comx::AccountState> state = rstate.move_as_ok();
    std::string text = state->raw.address.rserialize(true);
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());

    size_t len = 32;
    SliceToArray(key.public_key.as_slice(), publicKey, &len);
    SliceToArray(key.secret.as_slice(), secret, &len);
    resultHandler(0, nullptr);
    return true;
}

bool LiteClientExportKey(LiteClient* client, const char* dataPassword, const char* publicKey, const char* password, const char* secret, char* keyData, size_t* keyDataLen, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();
    td::Result<KeyStorage::ExportedEncryptedKey> reuk = keyStorage.export_encrypted_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{ SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32) }, SecureString(password, (size_t)64) }, Slice(dataPassword));
    if (reuk.is_error())
    {
        resultHandler(-1, reuk.move_as_error().to_string().c_str());
        return false;
    }
    SliceToArray(reuk.move_as_ok().data.as_slice(), keyData, keyDataLen);
    resultHandler(0, nullptr);
    return true;
}

bool LiteClientImportKey(LiteClient* client, const char* dataPassword, const char* password, const char* outSecret, const char* keyData, size_t keyDataLen, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();
    td::Result<KeyStorage::Key> rkey =  keyStorage.import_encrypted_key(Slice(password), Slice(dataPassword), KeyStorage::ExportedEncryptedKey{ td::SecureString(keyData, keyDataLen) });
    if (rkey.is_error())
    {
        resultHandler(-1, rkey.move_as_error().to_string().c_str());
        return false;
    }
    KeyStorage::Key key = rkey.move_as_ok();
    size_t len = 32;
    SliceToArray(key.secret.as_slice(), outSecret, &len);
    resultHandler(0, nullptr);
    return true;
}

bool LiteClientDeleteKey(LiteClient* client, const char* publicKey, const char* secret, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();
    td::Status status = keyStorage.delete_key(tonlib::KeyStorage::Key{ SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32) });
    if (status.is_error())
    {
        resultHandler(-1, status.move_as_error().to_string().c_str());
        return false;
    }
    resultHandler(0, nullptr);
    return true;
}

bool LiteClientGetWords(LiteClient* client, const char* publicKey, const char* password, const char* secret, QueryStringHandler wordsHandler, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();

    td::Result<KeyStorage::ExportedKey> rexported_key = keyStorage.export_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{ SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32) }, SecureString(password, (size_t)64) });
    if (rexported_key.is_error())
    {
        resultHandler(-1, rexported_key.move_as_error().to_string().c_str());
        return false;
    }
    KeyStorage::ExportedKey exported_key = rexported_key.move_as_ok();

    for (int i = 0; i < 24; i++)
        wordsHandler(exported_key.mnemonic_words.at(i).as_slice().str().c_str());

    resultHandler(0, nullptr);
    return true;
}

bool LiteClientGetSeed(LiteClient* client, const char* publicKey, const char* password, const char* secret, void* outSeed, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();

    td::Result<KeyStorage::ExportedKey> rexported_key = keyStorage.export_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{ SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32) }, SecureString(password, (size_t)64) });
    if (rexported_key.is_error())
    {
        resultHandler(-1, rexported_key.move_as_error().to_string().c_str());
        return false;
    }
    KeyStorage::ExportedKey exported_key = rexported_key.move_as_ok();
    td::Result<Mnemonic> rm = Mnemonic::create(std::move(exported_key.mnemonic_words), td::SecureString(0));
    if (rm.is_error())
    {
        resultHandler(-1, rm.move_as_error().to_string().c_str());
        return false;
    }
    memcpy(outSeed, rm.move_as_ok().to_seed().as_slice().data(), 32);
    resultHandler(0, nullptr);
    return true;
}

bool LiteClientCreateWalletAddress(LiteClient* client, const char* mnemonicPassword, const char* publicKey, const char* password, const char* secret, const char* seed, LPTSTR* pBuffer, size_t* resLen, QueryStringHandler wordsHandler, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();
    td::Result<KeyStorage::Key> key_r = keyStorage.create_new_key(Slice(password, (size_t)64), Slice(mnemonicPassword, (size_t)0), Slice(seed, (size_t)32));
    if (key_r.is_error())
    {
        resultHandler(-1, key_r.move_as_error().to_string().c_str());
        return false;
    }
    KeyStorage::Key key = key_r.move_as_ok();

    size_t len = 32;
    SliceToArray(key.public_key.as_slice(), publicKey, &len);
    SliceToArray(key.secret.as_slice(), secret, &len);

    KeyStorage::InputKey input_key{ std::move(key), SecureString(Slice(password, (size_t)64)) };
    td::Result<KeyStorage::ExportedKey> rexported_key = keyStorage.export_key(std::move(input_key));
    if (rexported_key.is_error())
    {
        resultHandler(-1, rexported_key.move_as_error().to_string().c_str());
        return false;
    }
    KeyStorage::ExportedKey exported_key = rexported_key.move_as_ok();

    for (int i = 0; i < 24; i++)
    {
        td::SecureString& word = exported_key.mnemonic_words.at(i);
        std::string s = word.as_slice().str();
        wordsHandler(s.c_str());
    }
    std::string public_key = publicKeyToString((const char*)publicKey);

    td::Result<block::StdAddress> pa = getAddress(public_key, WalletType::WalletV4, client->getWalletID(), 2, ton::basechainId);
    if (pa.is_error())
    {
        resultHandler(-1, pa.move_as_error().to_string().c_str());
        return false;
    }
    std::string text = pa.move_as_ok().rserialize(true);
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());

    resultHandler(0, nullptr);
    return true;
}

void LiteClientCreateWallet(LiteClient* client, const char* publicKey, const char* password, const char* secret, QueryResultHandler resultHandler)
{
    tonlib::KeyStorage keyStorage = client->getKeyStorage();

    td::Result<KeyStorage::PrivateKey> privKey_r = keyStorage.load_private_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32)}, SecureString(password, (size_t)64) });
    if (privKey_r.is_error())
    {
        resultHandler(-1, privKey_r.move_as_error().to_string().c_str());
        return;
    }
    td::Ed25519::PrivateKey privKey = td::Ed25519::PrivateKey(privKey_r.move_as_ok().private_key);
    td::Ed25519::PublicKey pubKey = td::Ed25519::PublicKey(SecureString(publicKey, (size_t)32));

    td::Result<td::Ref<vm::Cell>> boc = createWallet(client, pubKey, privKey, ton::basechainId);
    if (boc.is_error())
    {
        resultHandler(-1, boc.move_as_error().to_string().c_str());
        return;
    }

    client->send(boc.move_as_ok(), [resultHandler = resultHandler](td::Result<std::string> rhash)
    {
        if (rhash.is_error())
            resultHandler(-1, rhash.move_as_error().to_string().c_str());
        else
            resultHandler((int64_t)rhash.move_as_ok().c_str(), nullptr);
    });
}

bool LiteClientGetPrivateKey(LiteClient* client, const char* publicKey, const char* password, const char* secret, const char* outPrivateKey, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();
    td::Result<KeyStorage::PrivateKey> privKey_r = keyStorage.load_private_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32)}, SecureString(password, (size_t)64) });
    if (privKey_r.is_error())
    {
        resultHandler(-1, privKey_r.move_as_error().to_string().c_str());
        return false;
    }
    td::Ed25519::PrivateKey privKey = td::Ed25519::PrivateKey(privKey_r.move_as_ok().private_key);
    size_t len = 32;
    SliceToArray(privKey.as_octet_string().as_slice(), outPrivateKey, &len);
    resultHandler(0, nullptr);
    return true;
}

bool LiteClientSign(LiteClient* client, const char* publicKey, const char* password, const char* secret, const char* message, size_t messageLen, void* res, QueryResultHandler resultHandler)
{
    KeyStorage keyStorage = client->getKeyStorage();

    td::Result<KeyStorage::PrivateKey> privKey_r = keyStorage.load_private_key(tonlib::KeyStorage::InputKey{ tonlib::KeyStorage::Key{SecureString(publicKey, (size_t)32), SecureString(secret, (size_t)32)}, SecureString(password, (size_t)64) });
    if (privKey_r.is_error())
    {
        resultHandler(-1, privKey_r.move_as_error().to_string().c_str());
        return false;
    }
    td::Ed25519::PrivateKey privKey = td::Ed25519::PrivateKey(privKey_r.move_as_ok().private_key);

    Result<SecureString> key = privKey.sign(td::Slice(message, messageLen));
    if (key.is_error())
    {
        resultHandler(-1, key.move_as_error().to_string().c_str());
        return false;
    }
    memcpy(res, key.move_as_ok().as_slice().data(), 64);
    resultHandler(0, nullptr);
    return true;
}

vm::Cell* LiteClientCreateSendMessageCell(LiteClient* client, const char* srcAddress, const char* publicKey, const char* password, const char* secret, MessageInfo* messages, int messagesLen, QueryResultHandler resultHandler)
{
    td::Result<MessageRaw> raw = getSendMessageRaw(client, srcAddress, publicKey, password, secret, messages, messagesLen);
    if (raw.is_error())
    {
        resultHandler(-1, raw.move_as_error().to_string().c_str());
        return nullptr;
    }
    resultHandler(0, nullptr);
    return raw.move_as_ok().message.release();
}

void LiteClientSendMessage(LiteClient* client, const char* srcAddress, const char* publicKey, const char* password, const char* secret, MessageInfo* messages, int messagesLen, QueryResultHandler resultHandler)
{
    td::Result<MessageRaw> raw = getSendMessageRaw(client, srcAddress, publicKey, password, secret, messages, messagesLen);
    if (raw.is_error())
    {
        resultHandler(-1, raw.move_as_error().to_string().c_str());
        return;
    }
    td::Ref<vm::Cell> cell = raw.move_as_ok().message;
    client->send(cell, [c = cell, resultHandler = resultHandler](td::Result<std::string> rhash)
    {
        if (rhash.is_error())
            resultHandler(-1, rhash.move_as_error().to_string().c_str());
        else
            resultHandler(toHandle<vm::Cell>(c.get()), nullptr);
    });
}

void LiteClientCalcFee(LiteClient* client, const char* srcAddress, MessageInfo* messages, int messagesLen, QueryResultHandler resultHandler)
{
    td::Result<td::Ed25519::PrivateKey> rprivKey = td::Ed25519::generate_private_key();
    if (rprivKey.is_error())
    {
        resultHandler(-1, rprivKey.move_as_error().to_string().c_str());
        return;
    }

    td::Result<MessageRaw> raw = getMessageRaw(client, srcAddress, rprivKey.move_as_ok(), messages, messagesLen);
    if (raw.is_error())
    {
        resultHandler(-1, raw.move_as_error().to_string().c_str());
        return;
    }

    client->calcFee(new CalcFee(raw.move_as_ok()), [resultHandler = resultHandler](td::Result<int64_t> rfees)
    {
        if (rfees.is_error())
            resultHandler(-1, rfees.move_as_error().to_string().c_str());
        else
            resultHandler(rfees.move_as_ok(), nullptr);
    });
}

void LiteClientGetStateInit(LiteClient* client, comx::AccountState* state, const char* publicKey, const char* arr, size_t* resLen)
{
    std::string text = state->getStateInitBase64(publicKeyToString(publicKey), client->getWalletID());
    *resLen = text.size();
    memcpy((void*)arr, text.c_str(), text.size());
}

    //**************************************************************************************

StdAddress* AddressCreate(int32_t wc, const unsigned char* rdata, bool bounceable, bool testnet)
{
    BitArray<256> address(reinterpret_cast<const unsigned char*>(rdata));
    return new StdAddress(wc, address, bounceable, testnet);
}

void AddressFromDataToString(int32_t wc, const unsigned char* rdata, bool bounceable, bool testnet, LPTSTR* pBuffer, size_t* resLen)
{
    BitArray<256> address(reinterpret_cast<const unsigned char*>(rdata));
    StdAddress addr(wc, address, bounceable, testnet);
    std::string text = addr.rserialize(true);
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}

void AddressFromHex(const char* hexAddress, LPTSTR* pBuffer, size_t* resLen)
{
    StdAddress addr;
    addr.parse_addr(std::string(hexAddress));
    std::string text = addr.rserialize(true);
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}

void AddressToHex(const char* address, LPTSTR* pBuffer, size_t* resLen)
{
    StdAddress addr;
    addr.parse_addr(std::string(address));
    std::string text = std::to_string(addr.workchain) + ":" + td::hex_encode(addr.addr.as_slice());
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}

StdAddress* AddressParse(const char* address)
{
    return new StdAddress(address);
}

void AddressDestroy(StdAddress* address)
{
    delete address;
}

bool AddressIsValid(const char* address)
{
    return StdAddress(address).is_valid();
}

void AddressToString(StdAddress* address, LPTSTR* pBuffer, size_t* resLen)
{
    std::string text = address->rserialize(true);
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}

bool AddressCompareTo(StdAddress* address, StdAddress* to)
{
    return address == to;
}

void AddressData(StdAddress* address, byte* data)
{
	memcpy(data, reinterpret_cast<const byte*>(&address->workchain), 1);
	memcpy(data + sizeof(byte), address->addr.data(), 32);
}

int AddressGetData(const char* address, byte* data)
{
    StdAddress addr;
    addr.parse_addr(std::string(address));
    memcpy(data, addr.addr.data(), 32);
    return addr.workchain;
}

td::int32 AddressWorkchain(StdAddress* address)
{
    return address->workchain;
}

//**************************************************************************************

void AccountDestroy(comx::AccountState* state)
{
    delete state;
}

int64_t AccountBalance(comx::AccountState* state)
{
    return state->balance();
}

int64_t AccountTransaction(comx::AccountState* state)
{
    return state->lastTransaqtionID();
}

int64_t AccountTime(comx::AccountState* state)
{
    return state->time();
}

void AccountTransactionHash(comx::AccountState* state, const char* arr)
{
    ton::Bits256 hash = state->raw.info.last_trans_hash;
    memcpy((void*)arr, hash.data(), 32);
}

int64_t AccountSeqno(comx::AccountState* state)
{
    return state->seqno();
}

comx::ContractState AccountGetState(comx::AccountState* state)
{
    return state->getState();
}

void AccountAddress(comx::AccountState* state, const char* arr, size_t* resLen)
{
    std::string text = state->raw.address.rserialize(true);
    *resLen = text.size();
    memcpy((void*)arr, text.c_str(), text.size());
}

void AccountVersion(comx::AccountState* state, const char* arr, size_t* resLen)
{
    std::string text = state->getVersion();
    *resLen = text.size();
    memcpy((void*)arr, text.c_str(), text.size());
}

WalletType AccountType(comx::AccountState* state)
{
    return state->get_wallet_type();
}

void AccountGetMsgHash(comx::AccountState* state, const char* res)
{
    size_t resLen;
    SliceToArray(state->raw.info.root.get()->get_hash().as_slice(), res, &resLen);
}

void AccountRunMethod(comx::AccountState* state, const char* methodName, comx::VmObject** params, size_t paramsLen, comx::VmObject** res, size_t* resLen)
{
    std::vector<comx::VmObject*> vparams;
    for (int i = 0; i < paramsLen; i++)
        vparams.push_back((comx::VmObject*)params[i]);
    std::vector<comx::VmObject*> vres = state->runMethod(methodName, vparams);
    *resLen = vres.size();
    for (int i = 0; i < vres.size(); i++)
        res[i] = vres[i];
}

void DeleteVmObject(comx::VmObject* vmObject)
{
    switch (vmObject->getType())
    {
        case VmObjectType::Number:
            delete (VmObjectNumber*)vmObject;
            break;
        case VmObjectType::Cell:
            delete (VmObjectCell*)vmObject;
            break;
        case VmObjectType::Slice:
            delete (VmObjectSlice*)vmObject;
            break;
        default:
            break;
    }
}

//********************************************************************************


CellBuilder* CellBuilderCreate()
{
    return new CellBuilder();
}

void CellBuilderDestroy(CellBuilder* builder)
{
    Ref<CellBuilder>::release_shared(builder);
}

bool CellBuilderStoreLong(CellBuilder* builder, int64_t value, unsigned int bits)
{
    return builder->store_long_bool(value, bits);
}

bool CellBuilderStoreBigInt(CellBuilder* builder, const char* dec, unsigned int bits)
{
    BigInt256 big;
    big.parse_dec(dec);
    return builder->store_int256_bool(big, bits);
}

bool CellBuilderStoreAddress(CellBuilder* builder, const char* address)
{
    if (address)
    {
        StdAddress addr(address);
        return builder->store_long_bool(2, 2) &&
            builder->store_long_bool(0, 1) &&
            builder->store_long_bool(addr.workchain, 8) &&
            builder->store_bytes_bool(addr.addr.data(), 32);
    }
    return builder->store_long_bool(0, 2);
}

bool CellBuilderStoreRef(CellBuilder* builder, DataCell* cell)
{
    return builder->store_ref_bool(Ref<Cell>(cell));
}

bool CellBuilderStoreDict(CellBuilder* builder, Dictionary* dict)
{
    if (dict)
        return builder->store_long_bool(1, 1) && builder->store_ref_bool(dict->get_root_cell());
    return builder->store_long_bool(0, 1);

}

bool CellBuilderStoreDictFromCell(CellBuilder* builder, Cell* rootCell)
{
    if (rootCell)
        return builder->store_long_bool(1, 1) && builder->store_ref_bool(Ref<Cell>(rootCell));
    return builder->store_long_bool(0, 1);

}

bool CellBuilderStoreBytes(CellBuilder* builder, byte* data, size_t offset, size_t len)
{
    return builder->store_bytes_bool(data + offset, len);
}

bool CellBuilderStoreSlice(CellBuilder* builder, CellSlice* slice)
{
    return builder->append_cellslice_bool(Ref<CellSlice>(slice));
}

int64_t CellBuilderFinalize(CellBuilder* builder)
{
    return toHandle<DataCell>(builder->finalize_novm().release());
}

int32_t CellBuilderBits(CellBuilder* builder)
{
    return builder->get_bits();
}

int32_t CellBuilderRefs(CellBuilder* builder)
{
    return builder->get_refs_cnt();
}

//*************************************************************************


void CellDestroy(DataCell* cell)
{
    Ref<DataCell>::release_shared(cell);
}

int32_t CellBits(DataCell* cell)
{
    return cell->get_bits();
}

int32_t CellRefs(DataCell* cell)
{
    return cell->get_refs_cnt();
}

vm::Cell* CellFromBoc(byte* bocData, int32_t len)
{
    return vm::std_boc_deserialize(Slice(bocData, len)).move_as_ok().release();
}

void CellGetHash(DataCell* cell, byte* res)
{
    memcpy((void*)res, cell->get_hash().as_slice().data(), 32);
}

void CellGetData(DataCell* cell, void** res, size_t* len)
{
    Ref<Cell> ref(cell);
    SliceToArray(std_boc_serialize(ref).move_as_ok().as_slice(), res, len);
}


int64_t CellSliceCreate(DataCell* cell)
{
    Ref<DataCell> ref(cell);
    return toHandle<CellSlice>(new CellSlice(ref));
}

void CellSliceDestroy(CellSlice* slice)
{
    Ref<CellBuilder>::release_shared(slice);
}

int32_t CellSliceBits(CellSlice* slice)
{
    return slice->size();
}

bool CellSliceIsValid(CellSlice* slice)
{
    return slice->is_valid();
}

int32_t CellSliceRefs(CellSlice* slice)
{
    return slice->size_refs();
}

unsigned long long CellSliceLoadLong(CellSlice* slice, int32_t bits)
{
    unsigned long long res = 0;
    slice->fetch_ulong_bool(bits, res);
    return res;
}

unsigned long long CellSlicePreLoadLong(CellSlice* slice, int32_t bits)
{
    unsigned long long res = 0;
    slice->prefetch_ulong_bool(bits, res);
    return res;
}

void CellSliceLoadBigInt(CellSlice* slice, int32_t bits, LPTSTR pBuffer, size_t* resLen)
{
    td::RefInt256 refInt;
    slice->fetch_int256_to(bits, refInt);
    std::string text = refInt->to_dec_string();
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}

void CellSliceLoadBytes(CellSlice* slice, byte* data, size_t length)
{
    slice->fetch_bytes(data, length);
}

int64_t CellSliceLoadRef(CellSlice* slice)
{ 
    Ref<Cell> cell = slice->fetch_ref();
    return toHandle<CellSlice>(new CellSlice(NoVm(), std::move(cell)));
}

Cell* CellSliceLoadRefCell(CellSlice* slice)
{
    Ref<Cell> cell = slice->fetch_ref();
    return cell.release();
}

CellSlice* CellSliceLoadSlice(CellSlice* slice)
{
    Ref<CellSlice> res = slice->fetch_subslice_ext(block::gen::t_Maybe_Ref_Message_Any.get_size(*slice));
    return res.release();
}

Dictionary* CellSliceLoadDict(CellSlice* slice, int keySizeBits)
{
    if (slice->fetch_long(1))
    {
        Ref<Cell> cell = slice->fetch_ref();
        return new Dictionary(Ref<Cell>(cell), keySizeBits);
    }
    return nullptr;
}

int CellSliceBSelect(CellSlice* slice, int bits, int mask)
{
    return slice->bselect(bits, mask);
}

bool CellUnpackMessage(Cell* cell, block::gen::Message::Record* message)
{
    Ref<Cell> rcell(cell);
    if (!type_unpack_cell(rcell, block::gen::t_Message_Any, *message))
        return false;
    return  true;
}

void CellSliceLoadAddress(CellSlice* slice, LPTSTR* pBuffer, size_t* resLen)
{
    td::Result<std::string> text = to_std_address(Ref<CellSlice>(slice));
    //*resLen = text.size();
    //memcpy(pBuffer, text.c_str(), text.size());
}

void CellSliceLoadAddressRef(CellSlice* slice, LPTSTR* pBuffer, size_t* resLen)
{
    Ref<CellSlice> res;
    block::gen::t_MsgAddressInt.fetch_to(*slice, res);
    std::string text = to_std_address(res).move_as_ok();
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}

void CellSliceGetAddressExt(CellSlice* slice, LPTSTR* pBuffer, size_t* resLen)
{
    Ref<CellSlice> res;
    block::gen::t_MsgAddressExt.fetch_to(*slice, res);
    std::string text = to_std_address(res).move_as_ok();
    *resLen = text.size();
    memcpy(pBuffer, text.c_str(), text.size());
}
int64_t CellSliceLoadBalance(CellSlice* slice)
{
    return to_balance(Ref<CellSlice>(slice)).move_as_ok();
}

int64_t CellSliceLoadBalanceRef(CellSlice* slice)
{
    Ref<CellSlice> res;
    block::gen::t_CurrencyCollection.fetch_to(*slice, res);
    return to_balance(res).move_as_ok();
}

int64_t CellSliceGetFee(CellSlice* slice)
{
    Ref<CellSlice> res;
    block::gen::t_Grams.fetch_to(*slice, res);
    return to_balance(res).move_as_ok();
}

int64_t CellSliceGetStorageFeeRef(CellSlice* slice)
{
    Ref<Cell> cell = slice->fetch_ref();
    td::RefInt256 storage_fees;
    if (!block::tlb::t_TransactionDescr.get_storage_fees(cell, storage_fees))
        return 0;
    return storage_fees->to_long();
}

CellSlice* CellSliceFromBoc(byte* bocData, int32_t len)
{
    td::Ref<vm::Cell> rcell = vm::std_boc_deserialize(Slice(bocData, len)).move_as_ok();
    td::Ref<vm::DataCell> ref((vm::DataCell*)rcell.get());
    return new CellSlice(ref);

}

//*************************************************************************

bool PrivateKeyGenerate(const char* res)
{
    Result<Ed25519::PrivateKey> key = Ed25519::generate_private_key();
    if (key.is_error())
        return false;
    size_t len = 32;
    SliceToArray(key.move_as_ok().as_octet_string().as_slice(), res, &len);
    return true;
}

bool PrivateKeyGetPublicKey(const char* privKey, const char* res)
{
    Ed25519::PrivateKey privateKey(SecureString(privKey, (size_t)32));
    Result<Ed25519::PublicKey> key = privateKey.get_public_key();
    if (key.is_error())
        return false;
    size_t len = 32;
    SliceToArray(key.move_as_ok().as_octet_string().as_slice(), res, &len);
    return true;
}

bool PrivateKeySign(const char* privKey, DataCell* cell, void** res, size_t* resLen)
{
    Ed25519::PrivateKey privateKey(SecureString(privKey, (size_t)32));
    Result<SecureString> key = privateKey.sign(cell->get_hash().as_slice());
    if (key.is_error())
        return false;
    SliceToArray(key.move_as_ok().as_slice(), res, resLen);
    return true;
}

bool PublicKeyEncrypt(const char* pubKey, const char* data, size_t dataLen, void** res, size_t* resLen)
{
    Ed25519::PublicKey publicKey(SecureString(pubKey, (size_t)32));
    Result<BufferSlice> buff = encrypt(publicKey, Slice(data, dataLen));
    if (buff.is_error())
        return false;
    SliceToArray(buff.move_as_ok(), res, resLen);
    return true;
}

bool PrivateKeyDecrypt(const char* privKey, const char* data, size_t dataLen, void** res, size_t* resLen)
{
    Ed25519::PrivateKey privateKey(SecureString(privKey, (size_t)32));
    Result<BufferSlice> buff = decrypt(privateKey, Slice(data, dataLen));
    if (buff.is_error())
        return false;
    SliceToArray(buff.move_as_ok(), res, resLen);
    return true;
}

void KeyEncryptDecrypt(const char* pubKey, const char* privKey, const char* data, size_t dataLen, void** res, size_t* resLen)
{
    Ed25519::PrivateKey privateKey(SecureString(privKey, (size_t)32));
    Ed25519::PublicKey publicKey(SecureString(pubKey, (size_t)32));
    Result<BufferSlice> encData = encrypt(publicKey, Slice(data, dataLen));
    Result<BufferSlice> resData = decrypt(privateKey, encData.move_as_ok());
    SliceToArray(resData.move_as_ok(), res, resLen);
}

void PrivateKeyToString(const char* data, const char* pBuffer, size_t* resLen)
{
    string s = privateKeyToString(data);
    *resLen = s.size();
    memcpy((void*)pBuffer, s.c_str(), s.size());
}

void PublicKeyToString(const char* data, const char* pBuffer, size_t* resLen)
{
    string s = publicKeyToString(data);
    *resLen = s.size();
    memcpy((void*)pBuffer, s.c_str(), s.size());

}

bool PublicKeyVerify(const char* publicKey, const char* message, size_t messageLen, const char* signature)
{
    Ed25519::PublicKey pub(SecureString(publicKey, (size_t)32));
    Status s = pub.verify_signature(td::Slice(message, messageLen), td::Slice(signature, 64));
    if (s.is_error())
    {
        std::string text = s.move_as_error().message().c_str();
        return false;
    }
    return true;

}

bool PrivateKeyParse(const char* data, const char* res)
{
    BigInt256 bigInt256;
    bigInt256.parse_dec(data);
    return bigInt256.export_bytes((unsigned char*)res, (size_t)32);
}

bool PublicKeyParse(const char* data, const char* res)
{
    Result<block::PublicKey> pubKey = block::PublicKey::parse(Slice(data, 48));
    if (pubKey.is_error())
        return false;
    size_t* resLen;
    SliceToArray(pubKey.move_as_ok().key, res, resLen);
    return true;
}
//********************************************************************************

Dictionary* DictionaryCreate(int32_t keySizeBits)
{
    return new Dictionary(keySizeBits);
}

void DictionaryStoreRef(Dictionary* dict, byte* key, size_t keyLen, DataCell* cell)
{
    BigInt256 big;
    big.import_bytes(key, keyLen);
    auto intkey = dict->integer_key(td::RefInt256{ big }, dict->get_key_bits());
    dict->set_ref(intkey.bits(), dict->get_key_bits(), td::Ref<Cell>(cell));
}

CellSlice* DictionaryFindRef(Dictionary* dict, byte* key, size_t keyLen)
{
    BigInt256 big;
    big.import_bytes(key, keyLen);
    auto intkey = dict->integer_key(td::RefInt256{ big }, dict->get_key_bits());
    return dict->lookup(intkey.bits(), dict->get_key_bits()).release();
}

void DictionaryStoreRefFromDecKey(Dictionary* dict, const char* decKey, DataCell* cell)
{
    BigInt256 big;
    big.parse_dec(decKey);
    auto key = dict->integer_key(td::RefInt256{ big }, dict->get_key_bits());
    dict->set_ref(key.bits(), dict->get_key_bits(), td::Ref<Cell>(cell));
}

Cell* DictionaryGetRootCell(Dictionary* dict)
{
    return dict->get_root_cell().release();
}

void DictionaryDestroy(Dictionary* dict)
{
    delete dict;
}


void BigIntegerGetData(int64_t value, byte* res, size_t* resLen)
{
    BigInt256 big(value);
    *resLen = big.bit_size() / 8;
    big.export_bytes(res, *resLen);
}

void BigIntegerGetDataFromString(const char* dec, byte* res, size_t* resLen)
{
    BigInt256 big;
    big.parse_dec(dec);
    int bits = big.bit_size();
    *resLen = ceil((double)bits / 8);
    big.export_bytes(res, *resLen);
}

//************************************************************************************


void UtilsAesIgeEncryption(unsigned char* buffer, unsigned char* key, unsigned char* iv, bool encrypt, int offset, int length)
{
    unsigned char* what = buffer + offset;
    AES_KEY akey;
    if (!encrypt)
    {
        AES_set_decrypt_key(key, 32 * 8, &akey);
        AES_ige_encrypt(what, what, length, &akey, iv, AES_DECRYPT);
    }
    else
    {
        AES_set_encrypt_key(key, 32 * 8, &akey);
        AES_ige_encrypt(what, what, length, &akey, iv, AES_ENCRYPT);
    }
}

void UtilsAesIgeEncryptionByteArray(unsigned char* buffer, unsigned char* key, unsigned char* iv, bool encrypt, int length)
{
    AES_KEY akey;
    if (!encrypt)
    {
        AES_set_decrypt_key(key, 32 * 8, &akey);
        AES_ige_encrypt(buffer, buffer, length, &akey, iv, AES_DECRYPT);
    }
    else
    {
        AES_set_encrypt_key(key, 32 * 8, &akey);
        AES_ige_encrypt(buffer, buffer, length, &akey, iv, AES_ENCRYPT);
    }
}

int UtilsPbkdf2(unsigned char* password, size_t passwordLength, unsigned char* salt, size_t saltLength, unsigned char* dst, size_t dstLength, int iterations)
{
    int result = PKCS5_PBKDF2_HMAC((char*)password, passwordLength, (uint8_t*)salt, saltLength, (unsigned int)iterations, EVP_sha512(), dstLength, (uint8_t*)dst);
    return result;
}

void RandomSecureBytes(unsigned char* ptr, size_t size)
{
    Random::secure_bytes(ptr, size);
}

void Sha256ComputeHash(const char* data, size_t dataLen, const char* pBuffer)
{
    std::string hash = sha256(td::Slice(data, dataLen));
    memcpy((void*)pBuffer, hash.c_str(), hash.size());
}

bool EncryptData(const char* data, const char* public_key, const char* private_key, const char* salt, void** res, size_t* resLen)
{
    Ed25519::PrivateKey privateKey(SecureString(private_key, (size_t)32));
    Ed25519::PublicKey publicKey(SecureString(public_key, (size_t)32));
    td::Result<td::SecureString> rres = SimpleEncryptionV2::encrypt_data(std::string(data), publicKey, privateKey, std::string(salt));
    if (rres.is_error())
        return false;
    SliceToArray(rres.move_as_ok(), res, resLen);
    return true;
}

bool DecryptData(const char* data, const char* public_key, const char* private_key, const char* salt, void** res, size_t* resLen)
{
    Ed25519::PrivateKey privateKey(SecureString(private_key, (size_t)32));
    Ed25519::PublicKey publicKey(SecureString(public_key, (size_t)32));
    td::Result<SimpleEncryptionV2::Decrypted> rres = SimpleEncryptionV2::decrypt_data(std::string(data), publicKey, privateKey, std::string(salt));
    if (rres.is_error())
        return false;
    SliceToArray(rres.move_as_ok().data, res, resLen);
    return true;
}


//************************************************************************************
