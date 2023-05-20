using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class BalanceChangedLabel : CircleArrowComponent
    {
        public BalanceChangedLabel(Wallet wallet)
        {
            this.wallet = wallet;
            this.prevBalance = this.wallet.Balance;
        }

        public void Update()
        {
            if (this.Dir == 0)
            {
                decimal delta = 0;
                foreach (ITransactionBase last in wallet.Transactions)
                {
                    delta = last.GetAmount(this.wallet.Symbol);
                    if (delta != 0)
                        break;
                }
                if (delta != 0)
                    this.Start(delta > 0 ? 1 : -1, delta.ToKMBPlus(3), this.wallet.Symbol, false);
                else
                    this.Start(0, "0", this.wallet.Symbol, false);
            }
            else
            {
                if (this.prevBalance != this.wallet.Balance || this.BottomText != this.wallet.Symbol)
                {
                    this.deltaBalance = this.wallet.Balance - this.prevBalance;
                    this.prevBalance = this.wallet.Balance;
                    this.Start(this.deltaBalance > 0 ? 1 : -1, this.deltaBalance.ToKMBPlus(3), this.wallet.Symbol, true);
                }
            }
        }

        private Wallet wallet;
        private decimal prevBalance;
        private decimal deltaBalance;

    }
}
