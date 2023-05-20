using System;

namespace Complex.Wallets
{
    public class TransactionDetail : ITransactionDetail
    {
        public TransactionDetail(string address, Balance amount, string message, string type, bool isOut)
        {
            this.address = address;
            this.amount = amount;
            this.message = message;
            this.type = type;
            this.isOut = isOut;
        }

        private string address;
        public string Address => address;

        private Balance amount;
        public Balance Amount => amount;

        private string message;
        public string Message => message;

        private string type;
        public string Type => type;

        private bool isOut;
        public bool IsOut => isOut;

        public override string ToString()
        {
            return (IsOut ? "Out" : "In") + " (" + Amount + ") " + Address;
        }
    }
}
