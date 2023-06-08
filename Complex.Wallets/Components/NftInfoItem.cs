using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class NftInfoItem : Container, IFocusedComponent
    {
        public NftInfoItem(Wallet wallet, INftInfo nft, GridWaitEffect waitEffect)
        {
            this.wallet = wallet;
            this.nft = nft;
            this.waitEffect = waitEffect;

            this.Padding.Set(4);
            this.Inflate.height = 4;

            Caption caption = new Caption();
            caption.Padding.Set(2);
            caption.Dock = DockStyle.Top;

            TextButton addressButton = new TextButton(nft.Name);
            addressButton.ToolTipInfo = new ToolTipInfo("Nft", nft.Name);
            addressButton.DrawBorder = false;
            addressButton.Padding.Set(4);
            addressButton.MaxWidth = 250;
            addressButton.MaxHeight = 24;
            addressButton.Font = Theme.font10Bold;
            addressButton.Executed += (s) =>
            {
                if (this.waitEffect != null)
                    Controller.ShowAnyWallet(wallet.Adapter, wallet.Symbol, nft.Address, () => this.waitEffect.Start(), () => this.waitEffect.Stop());
            };
            addressButton.RightClick += (s) =>
            {
                this.wallet.CreateAddressMenu(nft.Address, (m) =>
                {
                    if (m != null)
                        Application.Invoke(() => m.Show(s as Component, MenuAlignment.Bottom));
                });
            };

            addressButton.Dock = DockStyle.Fill;
            caption.Add(addressButton);
            this.Add(caption);

            imageComponent = new ImageComponent("nft_token.svg");
            imageComponent.Dock = DockStyle.Fill;
            this.Add(imageComponent);

            errorComponent = new TextComponent();
            errorComponent.Visible = false;
            errorComponent.ForeColor = Theme.red0;
            errorComponent.Dock = DockStyle.Bottom;
            this.Add(errorComponent);

            nft.LoadImage(96, 16, (img, e) =>
            {
                if (this.IsDisposed)
                {
                    if (img != null && img.Disposable)
                        img.Dispose();
                }
                else
                {
                    if (imageComponent.Image != img && img != null)
                    {
                        imageComponent.Image = img;
                        imageComponent.Invalidate();
                    }
                    if (e != null)
                    {
                        errorComponent.Text = e;
                        errorComponent.Visible = true;
                    }
                }
            });
        }

        protected readonly Wallet wallet;
        protected readonly INftInfo nft;
        protected readonly GridWaitEffect waitEffect;
        private ImageComponent imageComponent;
        private TextComponent errorComponent;

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

        void IFocusedComponent.OnFocusedChanged()
        {
        }
    }
}
