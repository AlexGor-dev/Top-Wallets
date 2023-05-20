#pragma once

#include "objects.h"

namespace comx
{

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