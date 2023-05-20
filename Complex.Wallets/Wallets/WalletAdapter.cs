using System;
using Complex.Collections;
using Complex.Trader;
using Complex.Drawing;
using Complex.Controls;
using Complex.Themes;

namespace Complex.Wallets
{
    public abstract class WalletAdapter : UpdateElement, IUnique, IBannerImageSource
    {
        protected WalletAdapter(IData data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.extension = data["extension"] as WalletAdapterExtension;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["extension"] = this.extension;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletAdapter(WalletAdapterExtension extension)
        {
            this.extension = extension;
            this.Init();
        }

        private void Init()
        {
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public event Handler Connected;
        public event Handler Disconnected;

        private WalletAdapterExtension extension;
        public WalletAdapterExtension Extension => extension;

        public virtual DateTime ServerUtcTime => DateTime.UtcNow;
        public virtual ThemeColor ThemeColor => IsConnected ? this.extension.ThemeColor : Theme.gray1;

        public virtual string ImageID => extension.ImageID;

        public virtual string SmallImageID => this.extension.SmallImageID;

        public virtual string BannerImageID => this.extension.BannerImageID;

        public virtual bool SupportConnection => false;

        public virtual bool IsConnected => !SupportConnection;

        public bool IsTestnet => extension.IsTestnet;

        public virtual string Symbol => extension.Symbol;
        public string Name => extension.Name;

        public string ID => extension.ID;

        public string FullName => extension.FullName;

        public string NetName => extension.NetName;

        private CoinMarket market;
        public CoinMarket Market
        {
            get
            {
                if (market == null)
                    market = Controller.GetCoinMarket(Symbol);
                return market;
            }
        }

        public virtual System.Collections.Generic.IEnumerable<Component> TopActionComponents => null;

        protected virtual void OnConnected()
        {
            Events.Invoke(this.Connected, this);
            MainSettings.Current.PlaySound("connected");
            MessageView.Show(Language.Current["MessageViewType.Message"] + ": " + this.FullName, "connected", MessageViewType.Message);
        }

        protected virtual void OnDisconnected()
        {
            Events.Invoke(this.Disconnected, this);
            MainSettings.Current.PlaySound("disconnected");
            MessageView.Show(Language.Current["MessageViewType.Warning"] + ": " + this.FullName, "disconnected", MessageViewType.Warning);
        }

        protected virtual void OnError(string error)
        {
            MessageView.Show(Language.Current["MessageViewType.Error"] + ": " + this.FullName, error, MessageViewType.Error);
        }

        public virtual void Start()
        {

        }

        public virtual void CreateWallet(Component component, ParamHandler<Wallet,string> paramHandler)
        {
        }

        public virtual Bitmap GenerateQRCode(string url, int width, int height)
        {
            return null;
        }

        public virtual string GetKnownAddress(string address)
        {
            return null;
        }

        public virtual void GetWallet(string address, ParamHandler<Wallet, string> paramHandler)
        {
        }

        public virtual void GetWalletInfo(string address, ParamHandler<WalletInfo, string> paramHandler)
        {

        }

        protected virtual bool OnRefresh()
        {
            return true;
        }

        public virtual bool IsValidAddress(string address)
        {
            return true;
        }

        public bool Refresh()
        {
            if (!this.Updating && !this.IsDisposed)
            {
                this.BeginUpdate();
                bool res = this.OnRefresh();
                this.EndUpdate();
                return res;
            }
            return false;
        }

        public virtual void Connect()
        {

        }

        public virtual bool ExecuteCmd(string cmd)
        {
            return false;
        }

        public override string ToString()
        {
            return base.ToString() + " " + ID;
        }
    }
}
