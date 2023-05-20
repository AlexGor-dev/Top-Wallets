using System;
using Complex.Wallets;

namespace Complex.Ton
{
    public class TonTransactionMessage : TransactionDetail
    {
        public TonTransactionMessage(string address, Balance amount, string message, string type, bool isOut, long queryId)
            : base(address, amount, message, type, isOut)
        {
            this.queryId = queryId;
        }

        private long queryId;
        public long QueryId => queryId;
    }
}
