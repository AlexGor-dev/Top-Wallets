using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public class TonTransactionDetailItem : TransactionDetailItem
    {
        public TonTransactionDetailItem(Wallet wallet, ITransactionDetail detail, IJettonSource jettonSource)
            :base(wallet, detail, null)
        {
            this.jettonSource = jettonSource;

            jettonButton = new CheckedButton(null, this.jettonSource.Jetton.Name, false);
            jettonButton.DrawBorder = false;
            jettonButton.Padding.Set(0, 4, 4, 4);
            //jettonButton.Inflate.width = 10;
            jettonButton.MaxWidth = 250;
            //jettonButton.MaxHeight = 32;
            jettonButton.TextComponent.Font = Theme.font10Bold;
            jettonButton.CheckedChanged += (s) =>
            {
                if (jettonButton.Checked)
                {
                    JettonMenu menu = new JettonMenu(this.wallet as TonUnknownWallet, this.jettonSource.Jetton, true);
                    menu.Hided += (s2) => jettonButton.Checked = false;
                    menu.Show(s as Component, MenuAlignment.TopLeft);
                }
            };
            jettonButton.Dock = DockStyle.Left;
            jettonButton.ImageComponent.MaxSize.Set(32, 32);
            jettonButton.ImageComponent.MinSize.Set(32, 32);
            bot.Insert(0, jettonButton);

            jettonSource.Jetton.LoadImage((image) =>
            {
                jettonButton.Image = image;
                jettonButton.Parent.Layout();
            });
            detail.Amount.SymbolChanged += Amount_SymbolChanged;
            wallet.Changed += Wallet_Changed;
        }

        protected override void OnDisposed()
        {
            detail.Amount.SymbolChanged -= Amount_SymbolChanged;
            wallet.Changed -= Wallet_Changed;
            base.OnDisposed();
        }

        private void Wallet_Changed(object sender)
        {
            jettonButton.Text = this.jettonSource.Jetton.Name;
            jettonSource.Jetton.LoadImage((image) =>
            {
                jettonButton.Image = image;
                jettonButton.Parent.Layout();
            });
            jettonButton.Parent.Layout();
        }

        private void Amount_SymbolChanged(object sender)
        {
            currencyLabel.CurrencyTextComponent.Text = this.detail.Amount.Symbol;
            currencyLabel.Parent.Layout();
        }



        private IJettonSource jettonSource;
        private CheckedButton jettonButton;
    }
}
