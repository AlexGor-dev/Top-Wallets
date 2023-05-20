#pragma once


#include "objects.h"
#include "td/actor/actor.h"
#include "RawTransaction.h"
#include "CalcFee.h"
#include "Ping.h"

namespace comx
{
    using namespace ton::lite_api;

    class LiteClientActor : public td::actor::Actor
    {
        std::string blockchain_name;
        std::string directory;

        QueryLongHandler connectionHandler;
        int serverIndex{ -1 };

        bool connected{ false };

        td::actor::ActorOwn<ton::adnl::AdnlExtClient> raw_client;
        td::Container<td::Promise<td::BufferSlice>> queries;

        tonlib::Config config;
        tonlib::Config validConfig;

        tonlib::LastBlockState state;

        std::shared_ptr<tonlib::KeyValue> keyValue;
        tonlib::LastBlockStorage last_block_storage;
        tonlib::KeyStorage key_storage;
        td::CancellationToken cancellation_token;

        td::uint32 wallet_id{ 0 };

        void send_raw_query(td::BufferSlice query, td::Promise<td::BufferSlice> promise);

        std::unique_ptr<ton::adnl::AdnlExtClient::Callback> make_callback();
        td::Result< std::unique_ptr<block::Config>> process_config_proof(ton::ton_api::object_ptr<ton::lite_api::liteServer_configInfo> raw_config);
        template<class QueryT> void send_query(QueryT query, td::Promise<typename QueryT::ReturnType> promise, td::int32 seq_no = -1);
        td::Result<std::unique_ptr<block::BlockProofChain>> process_block_proof(ton::BlockIdExt from,
                td::Result<ton::ton_api::object_ptr<ton::lite_api::liteServer_partialBlockProof>> r_block_proof);

    public:
        explicit LiteClientActor(std::string& blockchain_name, std::string& directory, std::string& jsonConfig, std::string& validConfig, QueryLongHandler connectionHandler);
        ~LiteClientActor();

        td::Status createClient();
        void send(td::Ref<vm::Cell> cell, const std::function<void(td::Result<std::string>)>& lambda);
        void getAccountState(block::StdAddress address, const std::function<void(td::Result<td::unique_ptr<AccountState>>)>& lambda);
        void getTransactions(block::StdAddress address, ton::LogicalTime last_trans_lt, ton::Bits256 last_trans_hash, int count, QueryLongHandler handler, const std::function<void(td::Result<int64_t>)>& lambda);
        void calcFee(CalcFee* query, const std::function<void(td::Result<int64_t>)>& lambda);
        void last(const std::function<void(td::Result<int64_t>)>& lambda);
        void getServerTime(const std::function<void(td::Result<int64_t>)>& lambda);

        std::shared_ptr<tonlib::KeyValue> getKeyValue()
        {
            return keyValue;
        }

        td::uint32 getWalletID()
        {
            return wallet_id;
        }

        void conn_ready()
        {
            if (!connected)
            {
                LOG(ERROR) << "conn ready";
                connected = true;
                connectionHandler(1);
            }
        }
        void conn_closed()
        {
            if (connected)
            {
                LOG(ERROR) << "conn closed";
                connected = false;
                connectionHandler(0);
            }
            else
            {
                connectionHandler(-1);
            }
        }

        bool isConnected()
        {
            return connected;
        }

        tonlib::KeyStorage& getKeyStorage()
        {
            return key_storage;
        }

        int getServerIndex()
        {
            return serverIndex;
        }
    };

    class LiteClient
    {

        td::actor::ActorOwn<LiteClientActor> actor;
        td::actor::Scheduler scheduler{{1}};
        td::thread scheduler_thread_;

    public:
        LiteClient(std::string &blockchain_name, std::string &directory, std::string &jsonConfig, std::string& validConfig, QueryLongHandler connectionHandler);

        ~LiteClient();

        //void send(Query *query);
        void connect();
        void getAccountState(std::string address, const std::function<void(td::Result<td::unique_ptr<AccountState>>)>& lambda);
        td::Result<td::unique_ptr<comx::AccountState>> getAccountState(std::string& address);
        void getTransactions(std::string& address, ton::LogicalTime last_trans_lt, ton::Bits256 last_trans_hash, int count, QueryLongHandler handler, const std::function<void(td::Result<int64_t>)>& lambda);
        void last(const std::function<void(td::Result<int64_t>)>& lambda);
        void send(td::Ref<vm::Cell> cell, const std::function<void(td::Result<std::string>)>& lambda);
        void calcFee(CalcFee* query, const std::function<void(td::Result<int64_t>)>& lambda);
        void getServerTime(const std::function<void(td::Result<int64_t>)>& lambda);

        bool isConnected()
        {
            return actor.get_actor_unsafe().isConnected();
        }
        td::uint32 getWalletID()
        {
            return actor.get_actor_unsafe().getWalletID();
        }
        tonlib::KeyStorage& getKeyStorage()
        {
            return actor.get_actor_unsafe().getKeyStorage();
        }
        int getServerIndex()
        {
            return actor.get_actor_unsafe().getServerIndex();
        }
        std::shared_ptr<tonlib::KeyValue> getKeyValue()
        {
            return actor.get_actor_unsafe().getKeyValue();
        }

    };


}



