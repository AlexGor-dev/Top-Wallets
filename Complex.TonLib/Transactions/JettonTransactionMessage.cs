using System;
using Complex.Wallets;

namespace Complex.Ton
{
    public class JettonTransactionMessage : TransactionDetail, IJettonSource
    {
        public JettonTransactionMessage(string address, JettonInfo jetton, UInt128 amount, TransactionType type, bool isOut)
            : base(null, new Balance(jetton.Symbol, amount, jetton.Decimals, 3), null, null, isOut)
        {
            this.jetton = jetton;
            this.type = type;
        }
        private JettonInfo jetton;
        public JettonInfo Jetton
        {
            get => jetton;
            set => jetton = value;
        }

        public readonly TransactionType type;
    }
}
