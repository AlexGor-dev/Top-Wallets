using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class JettonWalletItem : WalletLiteItem
    {
        protected JettonWalletItem(IData data)
            : base(data)
        {

        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public JettonWalletItem(TokenWallet wallet)
            : base(wallet)
        {
            this.Init();
        }

        private void Init()
        {
            TokenWallet wallet = this.Wallet as TokenWallet;
            if (this.Wallet is JettonMinter jm && jm.OwnerAddress == jm.JettonInfo.OwnerAddress)
            {
                this.nameCaption.Padding.left = 2;
                ColorLabel colorLabel = new ColorLabel(wallet.Type == WalletType.JettonMinter ? "mint" : "burn", wallet.Type == WalletType.JettonMinter ? Theme.green2 : Theme.red0);
                colorLabel.Dock = DockStyle.Left;
                this.nameCaption.Add(colorLabel);
            }

            this.label = new Label(Wallet.ImageID, Wallet.Version);
            this.label.Inflate.height = 6;
            this.label.MinWidth = 100;
            this.label.ImageComponent.Dock = DockStyle.TopCenter;
            this.label.ImageComponent.MaxSize.Set(32, 32);
            this.label.TextAlignment = ContentAlignment.Center;
            this.label.Dock = DockStyle.Right;
            this.label.TextComponent.Font = Theme.font10Bold;
            topContainer.Add(this.label);

            wallet.LoadImage((img) =>
            {
                if (img != null && img != this.label.Image)
                {
                    this.label.Image = img;
                    this.topContainer.Layout();
                }
            });
            Images.ImageChanged += Images_ImageChanged;

        }

        protected override void OnDisposed()
        {
            Images.ImageChanged -= Images_ImageChanged;
            base.OnDisposed();
        }
        private void Images_ImageChanged(string imageID)
        {
            if (this.Wallet.SmallImageID == imageID)
            {
                this.label.Image = Images.Get(imageID);
                this.label.Layout();
            }

        }

        private Label label;

        private class ColorLabel : TextComponent
        {
            public ColorLabel(string textID, ThemeColor backColor)
                :base(textID)
            {
                this.backColor = backColor;
                this.Font = Theme.font9Bold;
                this.Padding.Set(4, 0, 4 ,0);
                this.MaxHeight = 18;
            }

            private ThemeColor backColor;

            protected override int GetColor()
            {
                return Color.Argb(245, 245, 245);
            }

            protected override void OnDrawBack(Graphics g)
            {
                g.Smoosh(() => g.FillRoundRect(0, 0, Width, Height, Height / 2, backColor));
            }
        }
    }
}
