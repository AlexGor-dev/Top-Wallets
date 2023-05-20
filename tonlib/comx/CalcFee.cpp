
#include "CalcFee.h"

namespace comx
{
    static int output_actions_count(td::Ref<vm::Cell> list)
    {
        int i = -1;
        do
        {
            ++i;
            list = load_cell_slice(std::move(list)).prefetch_ref();
        }
        while (list.not_null());
        return i;
    }

    static td::RefInt256 compute_threshold(const block::GasLimitsPrices& cfg)
    {
        auto gas_price256 = td::RefInt256{ true, cfg.gas_price };
        if (cfg.gas_limit > cfg.flat_gas_limit)
        {
            return td::rshift(gas_price256 * (cfg.gas_limit - cfg.flat_gas_limit), 16, 1) +
                td::make_refint(cfg.flat_gas_price);
        }
        else
        {
            return td::make_refint(cfg.flat_gas_price);
        }
    }

    static td::uint64 gas_bought_for(td::RefInt256 nanograms, td::RefInt256 max_gas_threshold,
                                     const block::GasLimitsPrices& cfg)
    {
        if (nanograms.is_null() || sgn(nanograms) < 0)
        {
            return 0;
        }
        if (nanograms >= max_gas_threshold)
        {
            return cfg.gas_limit;
        }
        if (nanograms < cfg.flat_gas_price)
        {
            return 0;
        }
        auto gas_price256 = td::RefInt256{ true, cfg.gas_price };
        auto res = td::div((std::move(nanograms) - cfg.flat_gas_price) << 16, gas_price256);
        return res->to_long() + cfg.flat_gas_limit;
    }

    static td::RefInt256 compute_gas_price(td::uint64 gas_used, const block::GasLimitsPrices& cfg)
    {
        auto gas_price256 = td::RefInt256{ true, cfg.gas_price };
        return gas_used <= cfg.flat_gas_limit
            ? td::make_refint(cfg.flat_gas_price)
            : td::rshift(gas_price256 * (gas_used - cfg.flat_gas_limit), 16, 1) + cfg.flat_gas_price;
    }

    static vm::GasLimits compute_gas_limits(td::RefInt256 balance, const block::GasLimitsPrices& cfg)
    {
        vm::GasLimits res;
        // Compute gas limits
        if (false /*account.is_special*/)
        {
            res.gas_max = cfg.special_gas_limit;
        }
        else
        {
            res.gas_max = gas_bought_for(balance, compute_threshold(cfg), cfg);
        }
        res.gas_credit = 0;
        if (false /*trans_type != tr_ord*/)
        {
            // may use all gas that can be bought using remaining balance
            res.gas_limit = res.gas_max;
        }
        else
        {
            // originally use only gas bought using remaining message balance
            // if the message is "accepted" by the smart contract, the gas limit will be set to gas_max
            res.gas_limit = gas_bought_for(td::make_refint(0) /*msg balance remaining*/, compute_threshold(cfg), cfg);
            if (true /*!block::tlb::t_Message.is_internal(in_msg)*/)
            {
                // external messages carry no balance, give them some credit to check whether they are accepted
                res.gas_credit = std::min(static_cast<td::int64>(cfg.gas_credit), static_cast<td::int64>(res.gas_max));
            }
        }
        LOG(DEBUG) << "gas limits: max=" << res.gas_max << ", limit=" << res.gas_limit << ", credit=" << res.gas_credit;
        return res;
    }

    td::Result<td::int64> CalcFee::calc_fwd_fees(td::Ref<vm::Cell> list, block::MsgPrices** msg_prices, bool is_masterchain)
    {
        td::int64 res = 0;
        std::vector<td::Ref<vm::Cell>> actions;
        int n{ 0 };
        int max_actions = 20;
        while (true)
        {
            actions.push_back(list);
            auto cs = load_cell_slice(std::move(list));
            if (!cs.size_ext())
            {
                break;
            }
            if (!cs.have_refs())
            {
                return td::Status::Error("action list invalid: entry found with data but no next reference");
            }
            list = cs.prefetch_ref();
            n++;
            if (n > max_actions)
            {
                return td::Status::Error(PSLICE() << "action list too long: more than " << max_actions << " actions");
            }
        }
        for (int i = n - 1; i >= 0; --i)
        {
            vm::CellSlice cs = load_cell_slice(actions[i]);
            CHECK(cs.fetch_ref().not_null());
            int tag = block::gen::t_OutAction.get_tag(cs);
            CHECK(tag >= 0);
            switch (tag)
            {
            case block::gen::OutAction::action_set_code:
                return td::Status::Error("estimate_fee: action_set_code unsupported");
            case block::gen::OutAction::action_send_msg:
            {
                block::gen::OutAction::Record_action_send_msg act_rec;
                // mode: +128 = attach all remaining balance, +64 = attach all remaining balance of the inbound message, +1 = pay message fees, +2 = skip if message cannot be sent
                if (!tlb::unpack_exact(cs, act_rec) || (act_rec.mode & ~0xe3) || (act_rec.mode & 0xc0) == 0xc0)
                {
                    return td::Status::Error("estimate_fee: can't parse send_msg");
                }
                block::gen::MessageRelaxed::Record msg;
                if (!tlb::type_unpack_cell(act_rec.out_msg, block::gen::t_MessageRelaxed_Any, msg))
                {
                    return td::Status::Error("estimate_fee: can't parse send_msg");
                }

                bool dest_is_masterchain = false;
                if (block::gen::t_CommonMsgInfoRelaxed.get_tag(*msg.info) == block::gen::CommonMsgInfoRelaxed::int_msg_info)
                {
                    block::gen::CommonMsgInfoRelaxed::Record_int_msg_info info;
                    if (!tlb::csr_unpack(msg.info, info))
                    {
                        return td::Status::Error("estimate_fee: can't parse send_msg");
                    }
                    auto dest_addr = info.dest;
                    if (!dest_addr->prefetch_ulong(1))
                    {
                        return td::Status::Error("estimate_fee: messages with external addresses are unsupported");
                    }
                    int tag = block::gen::t_MsgAddressInt.get_tag(*dest_addr);

                    if (tag == block::gen::MsgAddressInt::addr_std)
                    {
                        block::gen::MsgAddressInt::Record_addr_std recs;
                        if (!tlb::csr_unpack(dest_addr, recs))
                        {
                            return td::Status::Error("estimate_fee: can't parse send_msg");
                        }
                        dest_is_masterchain = recs.workchain_id == ton::masterchainId;
                    }
                }
                vm::CellStorageStat sstat;                  // for message size
                sstat.add_used_storage(msg.init, true, 3);  // message init
                sstat.add_used_storage(msg.body, true, 3);  // message body (the root cell itself is not counted)
                res += msg_prices[is_masterchain || dest_is_masterchain]->compute_fwd_fees(sstat.cells, sstat.bits);
                break;
            }
            case block::gen::OutAction::action_reserve_currency:
                LOG(INFO) << "skip action_reserve_currency";
                continue;
            }
        }
        return res;
    }

    td::Result<std::pair<CalcFee::Fee, std::vector<CalcFee::Fee>>> CalcFee::estimate_fees(bool ignore_chksig, std::unique_ptr<block::Config> cfg)
    {
        // gas fees
        bool is_masterchain = raw_.source->get_address().workchain == ton::masterchainId;
        TRY_RESULT(gas_limits_prices, cfg->get_gas_limits_prices(is_masterchain));
        TRY_RESULT(storage_prices, cfg->get_storage_prices());
        TRY_RESULT(masterchain_msg_prices, cfg->get_msg_prices(true));
        TRY_RESULT(basechain_msg_prices, cfg->get_msg_prices(false));
        block::MsgPrices* msg_prices[2] = { &basechain_msg_prices, &masterchain_msg_prices };
        auto storage_fee_256 = block::StoragePrices::compute_storage_fees(
            raw_.source->get_sync_time(), storage_prices, raw_.source->raw.storage_stat,
            raw_.source->raw.storage_last_paid, false, is_masterchain);
        auto storage_fee = storage_fee_256.is_null() ? 0 : storage_fee_256->to_long();

        auto smc = ton::SmartContract::create(raw_.source->get_smc_state());

        td::int64 in_fwd_fee = 0;
        {
            vm::CellStorageStat sstat;                      // for message size
            sstat.add_used_storage(raw_.message, true, 3);  // message init
            in_fwd_fee += msg_prices[is_masterchain]->compute_fwd_fees(sstat.cells, sstat.bits);
        }

        vm::GasLimits gas_limits = compute_gas_limits(td::make_refint(raw_.source->get_balance()), gas_limits_prices);
        auto res = smc.write().send_external_message(raw_.message_body, ton::SmartContract::Args()
                                                     .set_limits(gas_limits)
                                                     .set_balance(raw_.source->get_balance())
                                                     .set_now(raw_.source->get_sync_time())
                                                     .set_ignore_chksig(ignore_chksig));
        td::int64 fwd_fee = 0;
        if (res.success)
        {
            LOG(DEBUG) << "output actions:\n"
                << block::gen::OutList{ output_actions_count(res.actions) }.as_string_ref(res.actions);

            TRY_RESULT_ASSIGN(fwd_fee, calc_fwd_fees(res.actions, msg_prices, is_masterchain));
        }

        auto gas_fee = res.accepted ? compute_gas_price(res.gas_used, gas_limits_prices)->to_long() : 0;
        LOG(INFO) << storage_fee << " " << in_fwd_fee << " " << gas_fee << " " << fwd_fee << " " << res.gas_used;

        Fee fee;
        fee.in_fwd_fee = in_fwd_fee;
        fee.storage_fee = storage_fee;
        fee.gas_fee = gas_fee;
        fee.fwd_fee = fwd_fee;

        std::vector<Fee> dst_fees;

        for (auto& destination : raw_.destinations)
        {
            bool dest_is_masterchain = destination && destination->get_address().workchain == ton::masterchainId;
            TRY_RESULT(dest_gas_limits_prices, cfg->get_gas_limits_prices(dest_is_masterchain));
            auto dest_storage_fee_256 =
                destination ? block::StoragePrices::compute_storage_fees(
                    destination->get_sync_time(), storage_prices, destination->raw.storage_stat,
                    destination->raw.storage_last_paid, false, is_masterchain)
                : td::make_refint(0);
            Fee dst_fee;
            auto dest_storage_fee = dest_storage_fee_256.is_null() ? 0 : dest_storage_fee_256->to_long();
            if (destination && destination->get_wallet_type() != ton::WalletType::Empty)
            {
                dst_fee.gas_fee = dest_gas_limits_prices.flat_gas_price;
                dst_fee.storage_fee = dest_storage_fee;
            }
            dst_fees.push_back(dst_fee);
        }
        return std::make_pair(fee, dst_fees);
    }

    td::Result<td::int64> CalcFee::calcFees(bool ignore_chksig, std::unique_ptr<block::Config> cfg)
    {
        TRY_RESULT(fees, estimate_fees(ignore_chksig, std::move(cfg)));
        return fees.first.getFees();
    }
}