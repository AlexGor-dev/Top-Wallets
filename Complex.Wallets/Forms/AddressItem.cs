using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class AddressItem : Container, IFocusedComponent
    {
        public AddressItem(Wallet wallet, string address, decimal amount, string message)
        {
            this.wallet = wallet;
            this.address = address;
            this.amount = amount;
            this.message = message;

            this.Padding.Set(6);
            this.Inflate.Set(4);

            Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, address);
            string name = null;
            if (wt != null)
            {
                name = wt.Name;
                if (wt.IsMain)
                    name = Language.Current["myWallet"] + " " + name;
            }
            else
                name = wallet.Adapter.GetKnownAddress(address);

            TextComponent button = new TextComponent(name != null ? name : address);
            button.Dock = DockStyle.Fill;
            this.Add(button);

            if (!string.IsNullOrEmpty(message))
            {
                messgComponent = new TextComponent(message);
                messgComponent.MaxWidth = 150;
                messgComponent.Dock = DockStyle.Left;
                messgComponent.MaxHeight = 40;
                messgComponent.Padding.Set(4);
                this.Add(messgComponent);
            }

            if (amount > 0)
            {
                CurrencyLabel label = new CurrencyLabel(amount.GetTextSharps(8), wallet.Symbol);
                label.Dock = DockStyle.Right;
                this.Add(label);
            }
        }

        private Wallet wallet;
        private TextComponent messgComponent;

        public readonly string address;
        public readonly decimal amount;
        public readonly string message;

        private readonly Rect dispRect = new Rect();

        protected override void OnSizeChanged()
        {
            GetDisplayRectangle(dispRect);
            base.OnSizeChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            int color = Theme.unselectedItemBackColor;
            int back2 = Theme.selectedItemBackColor;
            if (this.Selected)
            {
                color = Theme.selectedItemBackColor;
                back2 = Theme.unselectedItemBackColor;
            }

            g.Smoosh(() =>
            {
                g.FillRoundRect(dispRect, 4, color);
                if (messgComponent != null)
                    g.FillRoundRect(messgComponent.Left, messgComponent.Top, messgComponent.Width, messgComponent.Height, 4, back2);

            });
        }

        void IFocusedComponent.OnFocusedChanged()
        {
        }
    }
}
