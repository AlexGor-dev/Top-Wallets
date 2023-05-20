using System;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class CustomWallet : Wallet
    {
        public CustomWallet(IData data) 
            : base(data)
        {
        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.address = data["address"] as string;
            this.symbol = data["symbol"] as string;
            this.imageID = data["imageID"] as string;
            this.balance = data["balance"] as Balance;
            this.version = data["version"] as string;
            this.state = (WalletState)data["state"];
            this.lastActivityTime = (DateTime)data["lastActivityTime"];
            this.color = (int)data["color"];
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["address"] = this.address;
            data["symbol"] = this.symbol;
            data["imageID"] = this.imageID;
            data["balance"] = this.balance;
            data["version"] = this.version;
            data["state"] = this.state;
            data["lastActivityTime"] = this.lastActivityTime;
            data["color"] = this.color;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public CustomWallet(string adapterID, string address, string symbol, string imageID, int color, UInt128 balanceDiv, int signCount)
            : this(adapterID, address, symbol, imageID, color, new Balance(symbol, 0, balanceDiv, signCount))
        {
        }

        public CustomWallet(string adapterID, string address, string symbol, string imageID, int color, Balance balance)
        : base(adapterID)
        {
            this.address = address;
            this.symbol = symbol;
            this.imageID = imageID;
            this.color = color;
            this.balance = balance;
            this.Init();
        }

        private void Init()
        {
            Controller.AddCoinImage(BannerImageID, 96, this.ThemeColor, this.ImageID);
            Controller.AddCoinImage(SmallImageID, 48, this.ThemeColor, this.ImageID);
        }

        public override string SmallImageID => "Small_" + Symbol + (this.Adapter.IsTestnet ? "_Test" : "");
        public override string BannerImageID => "Banner_" + Symbol + (this.Adapter.IsTestnet ? "_Test" : "");

        private CoinMarket market;
        public override CoinMarket Market
        {
            get
            {
                if (market == null)
                    market = Controller.GetCoinMarket(this.Symbol);
                return market;
            }
        }

        private string address;
        public override string Address => address;

        private string symbol;
        public override string Symbol => symbol;

        private string imageID;
        public override string ImageID => imageID;

        private Balance balance;

        private int color;

        private ThemeColor themeColor;
        public override ThemeColor ThemeColor
        {
            get
            {
                if (themeColor == null)
                {
                    int c = this.color;
                    if (this.Adapter.IsTestnet)
                        c = Color.Offset(c, -10);
                    themeColor = new ThemeColor(c, c, c, c);
                    Theme.Add(themeColor);
                }
                return themeColor;
            }
        }

        public override Balance Balance => balance;

        protected string version;

        public override string Version => version;

        protected WalletState state = WalletState.None;

        public override WalletState State => state;

        protected DateTime lastActivityTime;

        public override DateTime LastActivityTime => lastActivityTime;

        public virtual bool Update(UInt128 balance)
        {
            if (this.balance.Value != balance)
            {
                this.balance.Update(balance);
                return true;
            }
            return false;
        }
    }
}
