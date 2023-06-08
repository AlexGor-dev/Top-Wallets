using System;
using Complex.Themes;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;

namespace Complex.Ton
{
    public class JettonMinter : TokenWallet
    {
        protected JettonMinter(IData data) 
            : base(data)
        {
        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.jettonInfo = data["jettonInfo"] as JettonInfo;
        }
        protected override void Save(IData data)
        {
            base.Save(data);
            data["jettonInfo"] = this.jettonInfo;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public JettonMinter(string adapterID, string address, JettonInfo jettonInfo, TonWallet parent) 
            : base(adapterID, address, parent)
        {
            this.jettonInfo = jettonInfo;
            this.Init();
        }

        private void Init()
        {
            Controller.AddCoinImage(BannerImageID, 96, this.ThemeColor, this.ImageID);
            Controller.AddCoinImage(SmallImageID, 48, this.ThemeColor, this.ImageID);
            jettonInfo.LoadImage((image) => InitImages(image));
        }

        private void InitImages(IImage image)
        {
            if (image != null && image != JettonInfo.JettonImage)
            {
                Images.Add(BannerImageID, image);
                Images.Add(SmallImageID, image);
            }
        }


        private JettonInfo jettonInfo;
        public JettonInfo JettonInfo
        {
            get => this.jettonInfo;
            protected set
            {
                if (value == null) return;
                JettonInfo prev = this.jettonInfo;
                this.jettonInfo = value;
                this.themeColor = null;
                if (prev != null && this.jettonInfo != null)
                {
                    if (prev.ImageData != this.jettonInfo.ImageData)
                    {
                        Images.Remove(prev.ImageID);
                        this.jettonInfo.LoadImage((image) => InitImages(image));
                    }
                    if (prev.Symbol != this.jettonInfo.Symbol)
                    {
                        foreach (ITransactionBase transaction in this.Transactions)
                        {
                            if (transaction is ITransactionGroup g)
                            {
                                foreach (TransactionDetail detail in g.Details)
                                    if (detail is IJettonSource js && js.Jetton != null && js.Jetton.JettonAddress == prev.JettonAddress)
                                    {
                                        js.Jetton = this.jettonInfo;
                                        detail.Amount.Symbol = this.Symbol;
                                    }
                            }
                            else if (transaction is ITransactionDetail detail && detail is IJettonSource js && js.Jetton != null && js.Jetton.Symbol == prev.JettonAddress)
                            {
                                js.Jetton = this.jettonInfo;
                                detail.Amount.Symbol = this.Symbol;
                            }
                        }
                    }
                }
            }
        }



        public override string ImageID => "jetton.svg";
        public override string SmallImageID => "Small_" + Symbol + (this.Adapter.IsTestnet ? "_Test" : "");
        public override string BannerImageID => "Banner_" + Symbol + (this.Adapter.IsTestnet ? "_Test" : "");

        public override string Symbol => jettonInfo.Symbol.First(8);

        public override string OwnerAddress => jettonInfo.OwnerAddress;

        public override Balance Balance => jettonInfo.TotalSupply;

        public Balance TonBalance => base.Balance;

        protected virtual int JettonColor => this.jettonInfo.ThremeColor;

        private ThemeColor themeColor;
        public override ThemeColor ThemeColor
        {
            get
            {
                if (themeColor == null)
                {
                    int c = this.JettonColor;
                    themeColor = new ThemeColor(c, c, c, c);
                    Theme.Add(themeColor);
                }
                return themeColor;
            }
        }


        private ColorButton changeOwnerButton;
        private ColorButton changeContentButton;

        protected virtual JettonInfo GetJettonInfo(AccountState state)
        {
            return this.Adapter.Client.GetJettonInfo(state);
        }

        public override string GetBalanceMarketPrice()
        {
            return base.Balance.GetTextSharps(8) + " " + base.Balance.Symbol;
        }

        public override bool Update(AccountState state)
        {
            bool firstUpdate = this.State == WalletState.None;
            if (base.Update(state))
            {
                this.themeColor = null;
                if (!firstUpdate)
                {
                    JettonInfo info = GetJettonInfo(state);
                    this.JettonInfo = info;
                    if (this.Parent != null && this.jettonInfo != null && this.jettonInfo.OwnerAddress != this.Parent.Address)
                    {
                        if (this.changeOwnerButton != null)
                        {
                            this.changeOwnerButton.Parent.Remove(this.changeOwnerButton);
                            this.changeOwnerButton.Dispose();
                            this.changeOwnerButton = null;
                        }

                        if (this.changeContentButton != null)
                        {
                            this.changeContentButton.Parent.Remove(this.changeContentButton);
                            this.changeContentButton.Dispose();
                            this.changeContentButton = null;
                        }

                    }
                }
                return true;
            }
            return false;
        }



        public void MintCoins(string passcode, long queryId, decimal amount, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateMintData(queryId, this.Address, this.Parent.Address, this.Balance.FromDecimal(amount)), resultHanler);
        }

        public override void ChangeOwner(string passcode, long queryId, string newOwner, UInt128 forwardAmount, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateChangeOwner(queryId, this.Address, newOwner, forwardAmount), resultHanler);
        }
        public override void ChangeOwnerCalcFee(long queryId, string newOwner, UInt128 forwardAmount, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(JettonController.CreateChangeOwner(queryId, this.Address, newOwner, forwardAmount), resultHanler);
        }

        public void ChangeContent(string passcode, long queryId, JettonInfo info, string offchainUri, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateChangeContent(queryId, this.Address, info, offchainUri), resultHanler);
        }

        public void ChangeContentCalcFee(long queryId, JettonInfo info, string offchainUri, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(JettonController.CreateChangeContent(queryId, this.Address, info, offchainUri), resultHanler);
        }

        public override void ChangeContent(string passcode, long queryId, object content, ParamHandler<object, string> resultHanler)
        {
            this.ChangeContent(passcode, queryId, content as JettonInfo, null, resultHanler);
        }

        public override void ChangeContentCalcFee(long queryId, object content, ParamHandler<Balance, string> resultHanler)
        {
            this.ChangeContentCalcFee(queryId, content as JettonInfo, null, resultHanler);
        }

        public override ColorButton CreateMainLeftButton()
        {
            if (this.jettonInfo != null && this.OwnerAddress == this.Parent.Address)
            {
                changeOwnerButton = new ColorButton("changeOwner");
                changeOwnerButton.Padding.Set(6);
                changeOwnerButton.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                changeOwnerButton.Radius = 6;
                changeOwnerButton.Executed += (s) => new ChangeOwnerForm(this).Show(s as Component, MenuAlignment.Bottom);
                return changeOwnerButton;
            }
            return null;
        }

        public override ColorButton CreateMainRightButton()
        {
            if (this.jettonInfo != null && this.jettonInfo.OwnerAddress == this.Parent.Address)
            {
                changeContentButton = new ColorButton("changeContent");
                changeContentButton.Padding.Set(6);
                changeContentButton.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                changeContentButton.Radius = 6;
                changeContentButton.Executed += (s) => new ChangeContentMinterForm(this).Show(s as Component, MenuAlignment.Bottom);
                return changeContentButton;
            }
            return null;
        }



        public override void LoadImage(ParamHandler<IImage> paramHandler)
        {
            this.jettonInfo.LoadImage(paramHandler);
        }
    }
}
