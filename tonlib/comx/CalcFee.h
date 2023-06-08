#pragma once

#include "AccountState.h"

namespace comx
{
    struct MessageRaw
    {
        td::unique_ptr<AccountState> source;
        std::vector<td::unique_ptr<AccountState>> destinations;

        td::uint32 valid_until{ std::numeric_limits<td::uint32>::max() };

        td::Ref<vm::Cell> message;
        td::Ref<vm::Cell> new_state;
        td::Ref<vm::Cell> message_body;
    };

    class CalcFee
    {
    public:

        struct Fee
        {
            td::int64 in_fwd_fee{ 0 };
            td::int64 storage_fee{ 0 };
            td::int64 gas_fee{ 0 };
            td::int64 fwd_fee{ 0 };
            td::int64 getFees() {return in_fwd_fee + storage_fee + gas_fee + fwd_fee; }
        };
        td::Result<td::int64> calcFees(bool ignore_chksig, std::unique_ptr<block::Config> cfg);
        td::Result<std::pair<Fee, std::vector<Fee>>> estimate_fees(bool ignore_chksig, std::unique_ptr<block::Config> cfg);

        CalcFee(MessageRaw&& raw) : raw_(std::move(raw))
        {
        }

    private:
        MessageRaw raw_;
        td::Result<td::int64> calc_fwd_fees(td::Ref<vm::Cell> list, block::MsgPrices** msg_prices, bool is_masterchain);
    };


}