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

    template<typename Base, typename T>
    inline bool is(const T* ptr)
    {
        return static_cast<const Base*>(ptr) != nullptr;
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
