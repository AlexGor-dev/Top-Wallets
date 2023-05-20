using System;
using Complex.Controls;
using Complex.Themes;
namespace Complex.Wallets
{
    public class LastTimeLabel : SwitchTextComponent
    {
        public LastTimeLabel(Wallet wallet)
        {
            this.wallet = wallet;
            this.Padding.Set(4);
            this.DrawShadow = true;
            this.RoundBack = true;
            this.Style = Theme.Get<RoundCaptionLabelTheme>();
        }

        private Wallet wallet;

        public bool Update()
        {
            string text = this.Text;
            //if (wallet.IsEmpty)
            //    this.Text = Language.Current["noChanges"];
            //else
            //{
            DateTime time = DateTime.MinValue;
            foreach (ITransactionBase last in wallet.Transactions)
            {
                if (last.GetAmount(this.wallet.Symbol) != 0)
                {
                    time = last.Time;
                    break;
                }
            }
            if(time == DateTime.MinValue)
                this.Text = Language.Current["noChanges"];
            else
                this.Text = (DateTime.UtcNow - time).ToYMD() + " " + Language.Current["ago"];
            //this.Text = (DateTime.UtcNow - this.wallet.LastActivityTime).ToYMD() + " " + Language.Current["ago"];
            return text != this.Text;
        }
    }
}
