using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class AdapterLabel : Container
    {
        public AdapterLabel(WalletAdapter adapter, IBannerImageSource bannerImageSource)
        {
            this.bannerImageSource = bannerImageSource;
            Images.ImageChanged += Images_ImageChanged;

            this.Padding.Set(4);
            this.Inflate.Set(0, 6);

            AdapterWaitLabel waitLabel = new AdapterWaitLabel(adapter);
            waitLabel.Dock = DockStyle.Top;
            waitLabel.Style = Theme.Get<CaptionForeTheme>();
            waitLabel.Font = Theme.font10Bold;
            this.Add(waitLabel);

            imageComponent = new ImageComponent(bannerImageSource.BannerImageID);
            imageComponent.Dock = DockStyle.Fill;
            imageComponent.MaxSize.Set(96, 96);
            this.Add(imageComponent);


        }

        protected override void OnDisposed()
        {
            Images.ImageChanged -= Images_ImageChanged;
            base.OnDisposed();
        }

        private void Images_ImageChanged(string imageID)
        {
            if (this.bannerImageSource.BannerImageID == imageID)
            {
                this.imageComponent.Image = Images.Get(imageID);
                this.imageComponent.Invalidate();
            }
        }


        private IBannerImageSource bannerImageSource;

        private ImageComponent imageComponent;
        public ImageComponent ImageComponent => imageComponent;
    }
}
