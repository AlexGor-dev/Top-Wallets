using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Animations;
using Complex.Trader;

namespace Complex.Wallets
{
    public class TokenItem : Container, IFocusedComponent, ITokenInfoSource
    {
        public TokenItem(Wallet wallet, ITokenInfo token, GridWaitEffect waitEffect)
        {
            this.wallet = wallet;
            this.token = token;
            this.waitEffect = waitEffect;
            this.Padding.Set(4, 4, 10, 4);
            this.Inflate.width = 6;

            tokenButton = new CheckedButton(null, this.token.Name, false);
            tokenButton.DrawBorder = false;
            tokenButton.Padding.Set(4);
            tokenButton.Inflate.width = 10;
            tokenButton.MaxWidth = 250;
            tokenButton.MinWidth = 250;
            tokenButton.TextComponent.Font = Theme.font10Bold;
            tokenButton.CheckedChanged += (s) =>
            {
                if (tokenButton.Checked)
                    this.OnNameButtonClicked(s as CheckedButton);
            };
            tokenButton.Dock = DockStyle.Left;
            tokenButton.ImageComponent.MaxSize.Set(40, 40);
            tokenButton.ImageComponent.MinSize.Set(40, 40);
            this.Add(tokenButton);

            Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, token.Address);

            addressButton = new TextButton(wt != null ? wt.Name : Controller.GetKnownAddress(wallet.Adapter, token.Address));
            addressButton.DrawBorder = false;
            addressButton.Padding.Set(4);
            addressButton.MaxWidth = 250;
            addressButton.MaxHeight = 24;
            addressButton.Font = Theme.font10Bold;
            addressButton.Executed += (s) =>
            {
                if (this.waitEffect != null)
                    Controller.ShowAnyWallet(wallet.Adapter, wallet.Symbol, token.Address, () => this.waitEffect.Start(), () => this.waitEffect.Stop());
            };
            addressButton.Dock = DockStyle.Left;
            this.Add(addressButton);

            token.LoadImage((image) =>
            {
                tokenButton.Image = image;
                tokenButton.Parent.Layout();
            });

            currencyLabel = new CurrencyLabel(token.Balance.GetTextSharps(9), token.Balance.Symbol);
            currencyLabel.ValueTextComponent.ForeColor = token.Color;
            currencyLabel.CurrencyTextComponent.MaxWidth = 100;
            currencyLabel.Dock = DockStyle.Right;
            this.Add(currencyLabel);
        }

        private Wallet wallet;
        private GridWaitEffect waitEffect;

        private CheckedButton tokenButton;
        private TextButton addressButton;
        CurrencyLabel currencyLabel;

        private ITokenInfo token;
        public ITokenInfo TokenInfo
        { 
            get => token;
            set
            {
                token = value;
                this.OnTokenInfoChanged();
            }
        }

        protected virtual void OnTokenInfoChanged()
        {
            tokenButton.Text = token.Name;
            token.LoadImage((image) =>
            {
                tokenButton.Image = image;
                tokenButton.Parent.Layout();
            });
            Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, token.Address);
            addressButton.Text = wt != null ? wt.Name : Controller.GetKnownAddress(wallet.Adapter, token.Address);
            currencyLabel.ValueTextComponent.Text = token.Balance.GetTextSharps(9);
            currencyLabel.CurrencyTextComponent.Text = token.Balance.Symbol;
            this.Layout();
        }

        protected virtual void OnNameButtonClicked(CheckedButton button)
        {

        }

        void IFocusedComponent.OnFocusedChanged()
        {
        }

        protected override void OnDrawBack(Graphics g)
        {
            g.Smoosh(() =>
            {
                int color = Theme.unselectedItemBackColor;
                if (this.Selected)
                    color = Theme.selectedItemBackColor;
                g.FillRoundRect(0, 0, Width, Height, 10, color);
            });
        }

    }
}
