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

        private NftInfo prevInfo;
        protected NftInfo PrevInfo => prevInfo;

        private NftInfo info;
        public NftInfo Info
        {
            get => info;
            private set
            {
                if (value == null) return;
                this.prevInfo = this.info;
                this.info = value;
            }
        }

        public override string OwnerAddress => info.OwnerAddress;

        public override string ImageID => "nft_token.svg";

        public override string SmallImageID => "Small_Nft";
        public override string BannerImageID => "Banner_Nft";

        public override ThemeColor ThemeColor => Theme.gray2;


        public override bool Update(AccountState state)
        {
            bool firstUpdate = this.State == WalletState.None;
            if (base.Update(state))
            {
                if (!firstUpdate)
                {
                    NftInfo info = this.Adapter.GetNftInfo(state);
                    this.Info = info;
                    if (this.CheckInfoChanged())
                        this.OnInfoChanged();
                }
                return true;
            }
            return false;
        }

        protected virtual bool CheckInfoChanged()
        {
            return this.Parent != null && this.info != null && this.info.OwnerAddress != this.Parent.Address;
        }

        protected virtual void OnInfoChanged()
        {

        }
        public override void LoadImage(ParamHandler<IImage> paramHandler)
        {
            paramHandler(Images.Get(SmallImageID));
        }

        //public override string GetBalanceMarketPrice()
        //{
        //    if (this.Parent != null)
        //        return this.Parent.GetMarketPrice(this.Balance);
        //    return base.GetBalanceMarketPrice();
        //}
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
