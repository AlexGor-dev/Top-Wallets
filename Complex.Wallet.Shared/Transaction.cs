using System;

namespace Complex.Wallets
{
    [Serializable]
    public class Transaction : TransactionDetail, ITransaction
    {
        public Transaction(string name, string id, DateTime time, Balance fee, string hash, string address, Balance amount, string message, string type, bool isOut)
            :base(address, amount, message, type, isOut)
        {
            this.name = name;
            this.id = id;
            this.time = time;
            this.fee = fee;
            this.hash = hash;
        }

        private string name;
        public string Name => name;

        private string id;
        public string ID => id;

        private DateTime time;
        public DateTime Time => time;

        private Balance fee;
        public Balance Fee => fee;

        private string hash;
        public string Hash => hash;

        public decimal GetAmount(string symbol)
        {
            if (this.Amount.Symbol == symbol)
            {
                decimal amount = (decimal)this.Amount;
                if (this.IsOut)
                    amount = -(decimal)(amount + fee);
                return amount;
            }
            return 0;
        }

        public virtual int CompareTo(ITransactionBase transaction)
        {
            Transaction tr = transaction as Transaction;
            int dir = this.Time.CompareTo(tr.Time);
            if (dir == 0)
                dir = this.id.CompareTo(tr.id);
            return dir;
        }

        public override string ToString()
        {
            return Time + " " + base.ToString();
        }
    }
}
