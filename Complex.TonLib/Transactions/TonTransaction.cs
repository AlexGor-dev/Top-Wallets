using System;
using Complex.Wallets;

namespace Complex.Ton
{
    public class TonTransaction : Transaction, ITonTransaction
    {
        public TonTransaction(string name, long lt, long utime, Gram fee, string hash, TransactionDetail detail, byte[] msgHash)
            : base(name, lt.ToString(), Calendar.FromMilliseconds(utime * 1000), fee, hash, detail.Address, detail.Amount, detail.Message, detail.Type, detail.IsOut)
        {
            this.lt = lt;
            this.utime = utime;
            this.detail = detail;
            this.msgHash = msgHash;
        }

        private long lt;
        long ITonTransaction.Lt => lt;

        private long utime;
        public long UTime => utime;

        private TransactionDetail detail;
        public TransactionDetail Detail => detail;

        private byte[] msgHash;
        public byte[] MsgHash => msgHash;

        public override int CompareTo(ITransactionBase transaction)
        {
            if (transaction != this)
            {
                ITonTransaction tt = transaction as ITonTransaction;
                int dir = this.lt.CompareTo(tt.Lt);
                if (dir != 0)
                    return dir;
                dir = this.utime.CompareTo(tt.UTime);
                if (dir != 0)
                    return dir;
                ITransaction tr = transaction as ITransaction;
                if (tr != null)
                {
                    dir = this.IsOut.CompareTo(tr.IsOut);
                    if (dir != 0)
                        return dir;
                }
                return dir;
            }
            return 0;
        }

    }
}
