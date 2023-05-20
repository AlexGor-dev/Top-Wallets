using System;
using Complex.Collections;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public abstract class WalletAdapterExtension : Disposable, IUnique
    {
        public WalletAdapterExtension(IData data)
        {

        }

        protected override void Load(IData data)
        {
            this.testnet = (bool)data["testnet"];
        }

        protected override void Save(IData data)
        {
            data["testnet"] = this.testnet;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletAdapterExtension(bool testnet)
        {
            this.testnet = testnet;
            this.Init();
        }

        private void Init()
        {
            Controller.AddCoinImage(BannerImageID, 96, this.ThemeColor, this.ImageID);
            Controller.AddCoinImage(SmallImageID, 48, this.ThemeColor, this.ImageID);
        }


        public virtual bool SupportWallets => false;

        public virtual bool SupportExplorer => false;

        public abstract string ImageID { get; }

        private bool testnet;
        public bool IsTestnet => testnet;

        public virtual string SmallImageID => "Small_" + Symbol + (IsTestnet ? "_Test" : "");

        public virtual string BannerImageID => "Banner_" + Symbol + (IsTestnet ? "_Test" : "");

        public abstract string Symbol { get; }
        public abstract string Name { get; }

        public string ID => GetID(Symbol, testnet);

        public virtual string NetName => IsTestnet ? "(testnet)" : "(mainnet)";

        private string fullName;
        public virtual string FullName
        {
            get
            {
                if (fullName == null)
                {
                    if (IsTestnet)
                        fullName = Symbol + " Test " + NetName;
                    else
                        fullName = Symbol + " " + NetName;
                }
                return fullName;
            }
        }

        public virtual int TestColor => Theme.gray1;
        public virtual int MainColor => Theme.gray1;

        public virtual ThemeColor TestThemeColor => new ThemeColor(TestColor, TestColor, TestColor, TestColor);
        public virtual ThemeColor MainThemeColor => new ThemeColor(MainColor, MainColor, MainColor, MainColor);

        private ThemeColor color;
        public virtual ThemeColor ThemeColor
        {
            get
            {
                if (color == null)
                {
                    color = IsTestnet ? TestThemeColor : MainThemeColor;
                    Theme.Add(color);
                }
                return color;
            }
        }


        public abstract WalletAdapter CreateAdapter();


        public static string GetID(string symbol, bool testnet)
        {
            return symbol + (testnet ? " Test" : "");
        }

        public override string ToString()
        {
            return base.ToString() + " " + ID;
        }
    }
}
