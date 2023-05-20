using System;
using Complex.Collections;


namespace Complex.Wallets
{
    public class TransactionGroup : ITransactionGroup
    {
        public TransactionGroup(string name, string id, DateTime time, Balance fee, string transactionHash, Array<ITransactionDetail> details)
        {
            this.name = name;
            this.id = id;
            this.time = time;
            this.fee = fee;
            this.transactionHash = transactionHash;
            this.details = details;
        }

        private string name;
        public string Name => name;

        private string id;
        public string ID => id;

        private DateTime time;
        public DateTime Time => time;

        private Balance fee;
        public Balance Fee => fee;


        private string transactionHash;
        public string Hash => transactionHash;

        private Array<ITransactionDetail> details;
        public Array<ITransactionDetail> Details => details;

        public decimal GetAmount(string symbol)
        {
            decimal amount = 0;
            decimal fee = (decimal)this.fee;
            foreach (ITransactionDetail transaction in this.details)
            {
                if (transaction.Amount.Symbol == symbol)
                {
                    if (transaction.IsOut)
                    {
                        amount -= (decimal)(transaction.Amount + fee);
                        fee = 0;
                    }
                    else
                        amount += (decimal)transaction.Amount;
                }
            }
            return amount;
        }

        public virtual int CompareTo(ITransactionBase transaction)
        {
            TransactionGroup tr = transaction as TransactionGroup;
            int dir = this.Time.CompareTo(tr.Time);
            if (dir == 0)
                dir = this.id.CompareTo(tr.id);
            return dir;
        }

        public override string ToString()
        {
            return "" + time;
        }

    }
}
