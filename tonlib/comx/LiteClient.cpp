


#include "LiteClient.h"

namespace comx
{

    void LiteClientActor::send_raw_query(td::BufferSlice query, td::Promise<td::BufferSlice> promise)
    {
        auto query_id = queries.create(std::move(promise));
        td::Promise<td::BufferSlice> P = [query_id, self = this, actor_id = td::actor::actor_id()](td::Result<td::BufferSlice> result)
        {
            send_lambda(actor_id, [self, query_id, result = std::move(result)]() mutable
            {
                self->queries.extract(query_id).set_result(std::move(result));
            });
        };
        if (raw_client.empty())
        {
            return P.set_error(tonlib::TonlibError::NoLiteServers());
        }
        td::actor::send_closure(raw_client, &ton::adnl::AdnlExtClient::send_query, "query", std::move(query), td::Timestamp::in(10.0), std::move(P));
    }

    template<class QueryT>
    void LiteClientActor::send_query(QueryT query, td::Promise<typename QueryT::ReturnType> promise, td::int32 seq_no)
    {
        auto raw_query = ton::serialize_tl_object(&query, true);
        td::uint32 tag = td::Random::fast_uint32();
        if (seq_no >= 0)
        {
            auto wait = ton::lite_api::liteServer_waitMasterchainSeqno(seq_no, 5000);
            auto prefix = ton::serialize_tl_object(&wait, true);
            raw_query = td::BufferSlice(PSLICE() << prefix.as_slice() << raw_query.as_slice());
        }
        td::BufferSlice liteserver_query = ton::serialize_tl_object(ton::create_tl_object<liteServer_query>(std::move(raw_query)), true);

        send_raw_query(std::move(liteserver_query), [promise = std::move(promise), tag](td::Result<td::BufferSlice> R) mutable
                {
                    auto res = [&]() -> td::Result<typename QueryT::ReturnType>
                    {
                        TRY_RESULT_PREFIX(data, std::move(R), tonlib::TonlibError::LiteServerNetwork());
                        auto r_error = ton::fetch_tl_object<liteServer_error>(data.clone(), true);
                        if (r_error.is_ok())
                        {
                            auto f = r_error.move_as_ok();
                            return tonlib::TonlibError::LiteServer(f->code_, f->message_);
                        }
                        return ton::fetch_result<QueryT>(std::move(data));
                    }
                            ();
                    promise.set_result(std::move(res));
                });
    }

    td::Result<std::unique_ptr<block::BlockProofChain>> LiteClientActor::process_block_proof(
            ton::BlockIdExt from,
            td::Result<ton::ton_api::object_ptr<ton::lite_api::liteServer_partialBlockProof>> r_block_proof)
    {
        TRY_RESULT(block_proof, std::move(r_block_proof));  //TODO: it is fatal?
        TRY_RESULT(chain, liteclient::deserialize_proof_chain(std::move(block_proof)));
        if (chain->from != from)
            return td::Status::Error(PSLICE() << "block proof chain starts from block " << chain->from.to_str()
                                              << ", not from requested block " << from.to_str());
        TRY_STATUS(chain->validate(cancellation_token));
        return std::move(chain);
    }

    td::Result< std::unique_ptr<block::Config>> LiteClientActor::process_config_proof(ton::ton_api::object_ptr<ton::lite_api::liteServer_configInfo> raw_config)
    {
        auto blkid = create_block_id(raw_config->id_);
        if (!blkid.is_masterchain_ext())
        {
            return td::Status::Error(PSLICE() << "reference block " << blkid.to_str()
                                     << " for the configuration is not a valid masterchain block");
        }
        TRY_RESULT(state, block::check_extract_state_proof(blkid, raw_config->state_proof_.as_slice(),
                                                           raw_config->config_proof_.as_slice()));
        TRY_RESULT(config, block::Config::extract_from_state(std::move(state), 0));
        std::vector<td::int32> params_{ 4, 18, 20, 21, 24, 25 };

        for (auto i : params_)
        {
            //VLOG(last_config) << "ConfigParam(" << i << ") = ";
            auto value = config->get_config_param(i);
            if (value.is_null())
            {
                //VLOG(last_config) << "(null)\n";
            }
            else
            {
                std::ostringstream os;
                if (i >= 0)
                {
                    block::gen::ConfigParam{ i }.print_ref(os, value);
                    os << std::endl;
                }
                vm::load_cell_slice(value).print_rec(os);
                //VLOG(last_config) << os.str();
            }
        }
        return config;
    }

    std::unique_ptr<ton::adnl::AdnlExtClient::Callback> LiteClientActor::make_callback()
    {
        class Callback : public ton::adnl::AdnlExtClient::Callback
        {
        public:
            void on_ready() override { td::actor::send_closure(id_, &LiteClientActor::conn_ready); }
            void on_stop_ready() override { td::actor::send_closure(id_, &LiteClientActor::conn_closed); }
            Callback(td::actor::ActorId<LiteClientActor> id) : id_(std::move(id)) {  }
        private:
            td::actor::ActorId<LiteClientActor> id_;
        };
        return std::make_unique<Callback>(actor_id(this));
    }

    tonlib::Config::LiteClient getServer(tonlib::Config& config)
    {
        unsigned int lite_clients_size = config.lite_clients.size();
        CHECK(lite_clients_size != 0);
        int serverIndex = td::Random::fast(0, td::narrow_cast<int>(lite_clients_size) - 1);
        return config.lite_clients[serverIndex];
    }

    td::Status LiteClientActor::createClient()
    {
        conn_closed();
        tonlib::Config::LiteClient& lc = getServer(validConfig);
        int x = 1;
        do
        {
            td::string host = lc.address.get_ip_host();
            x = pingIp(host.c_str(), 2000);
            if (x == 0)
                break;
            lc = getServer(config);
        }
        while (x != 0);
        raw_client = ton::adnl::AdnlExtClient::create(lc.adnl_id, lc.address, make_callback());
        return td::Status::OK();
    }

    void LiteClientActor::last(const std::function<void(td::Result<int64_t>)>& lambda)
    {
        send_query(liteServer_getBlockProof(0, create_tl_lite_block_id(state.last_key_block_id), nullptr),
                   [this, from = state.last_key_block_id, lambda = lambda](auto r_block_proof)
        {
            auto r_chain = process_block_proof(from, std::move(r_block_proof));
            if (r_chain.is_error())
            {
                lambda(r_chain.move_as_error());
            }
            else
            {
                auto chain = r_chain.move_as_ok();
                CHECK(chain);
                if (!state.last_block_id.is_valid() || state.last_block_id.id.seqno < chain->to.id.seqno)
                    state.last_block_id = chain->to;
                if (!state.last_key_block_id.is_valid() || state.last_key_block_id.id.seqno < chain->key_blkid.id.seqno)
                    state.last_key_block_id = chain->key_blkid;

                state.utime = chain->last_utime;
                last_block_storage.save_state(blockchain_name, state);
                if (chain->complete)
                    lambda(0);
                else
                    last(lambda);
            }
        });

    }

    void LiteClientActor::send(td::Ref<vm::Cell> cell, const std::function<void(td::Result<std::string>)> & lambda)
    {
        td::Result<td::BufferSlice> bs = vm::std_boc_serialize(cell);
        send_query(liteServer_sendMessage(bs.move_as_ok()),
        [lambda = lambda, cell = std::move(cell)](auto x)
        {
            if (x.is_ok())
                lambda(cell->get_hash().as_slice().str());
            else
                lambda(x.move_as_error());
        });
    }

    void LiteClientActor::getAccountState(block::StdAddress address, const std::function<void(td::Result<td::unique_ptr<AccountState>>)>& lambda)
    {
        send_query(liteServer_getAccountState(ton::create_tl_lite_block_id(state.last_block_id), ton::create_tl_object<liteServer_accountId>(address.workchain, address.addr)),
                   [this, address = std::move(address), lambda = lambda](auto r_state)
        {
            if (r_state.is_ok())
            {
                td::Result<RawAccountState> result = AccountState::create(state, address, r_state.move_as_ok());
                if (result.is_ok())
                    lambda(td::make_unique<AccountState>(result.move_as_ok()));
                else
                    lambda(result.move_as_error());
            }
            else
            {
                lambda(r_state.move_as_error());
            }
        }, state.last_block_id.id.seqno);
    }

    void LiteClientActor::getTransactions(block::StdAddress address, ton::LogicalTime last_trans_lt, ton::Bits256 last_trans_hash, int count, QueryLongHandler handler, const std::function<void(td::Result<int64_t>)>& lambda)
    {
        send_query(ton::lite_api::liteServer_getTransactions(count, ton::create_tl_object<ton::lite_api::liteServer_accountId>(address.workchain, address.addr), last_trans_lt, last_trans_hash),
                   [this, handler = std::move(handler), lambda = lambda, last_lt = last_trans_lt, last_hash = last_trans_hash](auto r_transactions)
        {
            if (r_transactions.is_ok())
            {
                ton::lite_api::object_ptr<ton::lite_api::liteServer_transactionList> trans = r_transactions.move_as_ok();

                std::vector<ton::BlockIdExt> blkids;
                for (auto& id : trans->ids_)
                    blkids.push_back(ton::create_block_id(std::move(id)));

                block::TransactionList list;
                list.blkids = std::move(blkids);
                list.hash = last_hash;
                list.lt = last_lt;
                list.transactions_boc = std::move(trans->transactions_);
                td::Result<block::TransactionList::Info> info = list.validate();
                if (info.is_ok())
                {
                    td::optional<td::Ed25519::PrivateKey> private_key;
                    td::Result<object_ptr<raw_transactions>> raw = ToRawTransactions(std::move(private_key)).to_raw_transactions(info.move_as_ok());
                    if (raw.is_ok())
                    {
                        object_ptr<raw_transactions> transs = raw.move_as_ok();
                        for (object_ptr<raw_transaction>& trans : transs->transactions_)
                        {
                            raw_transaction* tr = trans.get();
                            rawtransaction r(tr);
                            handler(toHandle<rawtransaction>(&r));
                            for (object_ptr<raw_message>& message : tr->out_msgs_)
                            {
                                if (message->value_ > 0 || !message->destination_->account_address_.empty())
                                {
                                    rawmessage m(std::move(message), true);
                                    handler(toHandle<rawmessage>(&m));
                                }
                            }
                            if (tr->in_msg_ != NULL)
                            {
                                if (tr->in_msg_->value_ > 0 || !tr->in_msg_->source_->account_address_.empty())
                                {
                                    rawmessage m(std::move(tr->in_msg_), false);
                                    handler(toHandle<rawmessage>(&m));
                                }
                            }
                        }
                        lambda(0);
                    }
                    else
                    {
                        lambda(raw.move_as_error());
                    }
                }
                else
                {
                    lambda(info.move_as_error());
                }
            }
            else
            {
                lambda(r_transactions.move_as_error());
            }

        });
    }

    void LiteClientActor::calcFee(std::shared_ptr<CalcFee> calc, const std::function<void(td::Result<int64_t>)>& lambda)
    {
        send_query(ton::lite_api::liteServer_getConfigAll(0, create_tl_lite_block_id(state.last_key_block_id)), 
                   [this, calc = std::move(calc), lambda = lambda](td::Result<ton::ton_api::object_ptr<ton::lite_api::liteServer_configInfo>> r_config)
        { 
            if (r_config.is_ok())
            {
                td::Result<std::unique_ptr<block::Config>> conf = process_config_proof(r_config.move_as_ok());
                if (conf.is_ok())
                {
                    td::Result<td::int64> res = calc->calcFees(true, conf.move_as_ok());
                    if (res.is_ok())
                        lambda(res.move_as_ok());
                    else
                        lambda(res.move_as_error());
                }
                else
                {
                    lambda(conf.move_as_error());
                }
            }
            else
            {
                lambda(r_config.move_as_error());
            }
        });
    }

    void LiteClientActor::getServerTime(const std::function<void(td::Result<int64_t>)>& lambda)
    {
        send_query(liteServer_getTime(), [lambda = lambda](td::Result<ton::ton_api::object_ptr<ton::lite_api::liteServer_currentTime>> r_state)
        {
            if (r_state.is_ok())
                lambda(r_state.move_as_ok()->now_);
            else
                lambda(r_state.move_as_error());
        });
    }

    LiteClientActor::LiteClientActor(std::string& blockchain_name, std::string& directory, std::string& jsonConfig, std::string& validConfig, QueryLongHandler connectionHandler)
        :blockchain_name(blockchain_name), directory(directory), connectionHandler(connectionHandler)
    {
        config = tonlib::Config::parse(jsonConfig).move_as_ok();
        this->validConfig = tonlib::Config::parse(validConfig).move_as_ok();

        wallet_id = td::as<td::uint32>(config.zero_state_id.root_hash.as_slice().data());

        td::Result<td::unique_ptr<tonlib::KeyValue>> r_kv = tonlib::KeyValue::create_dir(directory);
        keyValue = std::shared_ptr<tonlib::KeyValue>(std::move(r_kv).move_as_ok().release());

        key_storage.set_key_value(keyValue);
        last_block_storage.set_key_value(keyValue);

        td::Result<tonlib::LastBlockState> r_state = last_block_storage.get_state(blockchain_name);
        if (r_state.is_error())
        {
            state.zero_state_id = ton::ZeroStateIdExt(config.zero_state_id.id.workchain, config.zero_state_id.root_hash, config.zero_state_id.file_hash);
            state.last_block_id = config.zero_state_id;
            state.last_key_block_id = config.zero_state_id;
            if (config.init_block_id.is_valid() && state.last_key_block_id.id.seqno < config.init_block_id.id.seqno)
                state.last_key_block_id = config.init_block_id;
            last_block_storage.save_state(blockchain_name, state);
        }
        else
        {
            state = r_state.move_as_ok();
        }
    }

    LiteClientActor::~LiteClientActor()
    {
        queries.for_each([](auto id, auto& promise) { promise.set_error(tonlib::TonlibError::Cancelled()); });
    }

    void LiteClient::last(const std::function<void(td::Result<int64_t>)>& lambda)
    {
        scheduler.run_in_context([this, lambda = lambda]
        {
            td::actor::send_closure(this->actor, &LiteClientActor::last,  lambda);
        });
    }

    void LiteClient::getAccountState(std::string address, const std::function<void(td::Result<td::unique_ptr<AccountState>>)>& lambda)
    {
        scheduler.run_in_context([this, address = std::move(address), lambda = lambda]
        {
            td::actor::send_closure(this->actor, &LiteClientActor::getAccountState, block::StdAddress(address), lambda);
        });
    }

    void LiteClient::getServerTime(const std::function<void(td::Result<int64_t>)>& lambda)
    {
        scheduler.run_in_context([this, lambda = lambda]
        {
            td::actor::send_closure(this->actor, &LiteClientActor::getServerTime,  lambda);
        });
    }

    td::Result<td::unique_ptr<comx::AccountState>> LiteClient::getAccountState(std::string& address)
    {
        td::unique_ptr<comx::AccountState> state = nullptr;
        td::Status status = td::Status::Error("wait query timeout");
        ManualResetEvent wait_event(false);
        wait_event.reset();
        getAccountState(address, [&st = status, &s = state, &w = wait_event](td::Result<td::unique_ptr<comx::AccountState>> ra)
        {
            if (ra.is_ok())
                s = ra.move_as_ok();
            else
                st = ra.move_as_error();
            w.set();
        });
        wait_event.wait(5000);
        if (state)
            return state;
        return status;
        return td::Status::Error("wisClosed");
    }

    void LiteClient::getTransactions(std::string& address, ton::LogicalTime last_trans_lt, ton::Bits256 last_trans_hash, int count, QueryLongHandler handler, const std::function<void(td::Result<int64_t>)>& lambda)
    {
        scheduler.run_in_context([this, address = std::move(address), lambda = lambda, handler = std::move(handler), last_lt = last_trans_lt, last_hash = last_trans_hash, c = count]
        {
            td::actor::send_closure(this->actor, &LiteClientActor::getTransactions, block::StdAddress(address), last_lt, last_hash, c, handler, lambda);
        });
    }

    void LiteClient::send(td::Ref<vm::Cell> cell, const std::function<void(td::Result<std::string>)>& lambda)
    {
        scheduler.run_in_context([this, cell = std::move(cell), lambda = lambda]
        {
            td::actor::send_closure(this->actor, &LiteClientActor::send, cell, lambda);
        });
    }

    void LiteClient::calcFee(std::shared_ptr<CalcFee> query, const std::function<void(td::Result<int64_t>)>& lambda)
    {
        scheduler.run_in_context([this, query = std::move(query), lambda = lambda]
        {
            td::actor::send_closure(this->actor, &LiteClientActor::calcFee, query, lambda);
        });
    }

    void LiteClient::connect()
    {
        scheduler.run_in_context([&]
        {
            td::actor::send_closure(actor, &LiteClientActor::createClient);
        });
    }

    LiteClient::LiteClient(std::string& blockchain_name, std::string& directory, std::string &config, std::string& validConfig, QueryLongHandler connectionHandler)
    {
        scheduler.run_in_context([&]
        {
            actor = td::actor::create_actor<LiteClientActor>(td::actor::ActorOptions().with_name("LiteClient").with_poll(), blockchain_name, directory, config, validConfig, connectionHandler);
            td::actor::send_closure(actor, &LiteClientActor::createClient);
        });
        scheduler_thread_ = td::thread([&]  { scheduler.run(); });
    }

    LiteClient::~LiteClient()
    {
        scheduler.run_in_context_external([&] { actor.reset(); });
        scheduler.run_in_context_external([] { td::actor::SchedulerContext::get()->stop(); });
        scheduler_thread_.join();
    }
}


