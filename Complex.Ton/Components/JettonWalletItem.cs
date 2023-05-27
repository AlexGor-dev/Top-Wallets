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
            this.label = new Label(Wallet.ImageID, Wallet.Version);
            this.label.Inflate.height = 6;
            this.label.ImageComponent.Dock = DockStyle.TopCenter;
            this.label.ImageComponent.MaxSize.Set(32, 32);
            this.label.Dock = DockStyle.Right;
            this.label.TextComponent.Font = Theme.font10Bold;
            topContainer.Add(this.label);

            (this.Wallet as TokenWallet).LoadImage((img) =>
            {
                this.label.Image = img;
                this.topContainer.Layout();
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
    }
}
