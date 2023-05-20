using System;
using Complex.Wallets;
using Complex.Collections;

namespace Complex.Ton
{
    public class TonTransactionGroup : TransactionGroup, ITonTransaction
    {
        public TonTransactionGroup(string name, long lt, long utime, Gram fee, string hash, Array<ITransactionDetail> details, byte[] msgHash)
            : base(name, lt.ToString(), Calendar.FromMilliseconds(utime * 1000), fee, hash, details)
        {
            this.lt = lt;
            this.utime = utime;
            this.msgHash = msgHash;
        }

        private long lt;
        long ITonTransaction.Lt => lt;

        private long utime;
        public long UTime => utime;

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
            }
            return 0;
        }

    }
}
