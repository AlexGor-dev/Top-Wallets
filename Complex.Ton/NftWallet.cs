using System;
using Complex.Themes;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;

namespace Complex.Ton
{
    public class NftWallet : TokenWallet
    {
        protected NftWallet(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.info = data["info"] as NftInfo;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["info"] = this.info;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public NftWallet(string adapterID, string address, NftInfo data, TonWallet parent)
            :base(adapterID, address, parent)
        {
            this.info = data;
            this.Init();
        }

        private void Init()
        {
            Controller.AddCoinImage(BannerImageID, 96, this.ThemeColor, this.ImageID);
            Controller.AddCoinImage(SmallImageID, 48, this.ThemeColor, this.ImageID);
        }

        private NftInfo info;
        public NftInfo Info => info;

        public override string Owner => info.Owner;

        public override string ImageID => "nft_token.svg";

        public override string SmallImageID => "Small_Nft";
        public override string BannerImageID => "Banner_Nft";

        public override ThemeColor ThemeColor => Theme.gray2;

        public override void LoadImage(ParamHandler<IImage> paramHandler)
        {
            paramHandler(Images.Get(SmallImageID));
        }

        public override ColorButton CreateMainLeftButton()
        {
            return null;
        }

        public override ColorButton CreateMainRightButton()
        {
            return null;
        }
    }
}
