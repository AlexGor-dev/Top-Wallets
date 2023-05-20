using System;
using Complex.Themes;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;

namespace Complex.Ton
{
    public class JettonMinter : TonUnknownWallet, IToken
    {
        protected JettonMinter(IData data) : base(data)
        {
        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.jettonInfo = data["jettonInfo"] as JettonInfo;
            this.parent = data["parent"] as TonWallet;
        }
        protected override void Save(IData data)
        {
            base.Save(data);
            data["jettonInfo"] = this.jettonInfo;
            data["parent"] = this.parent;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public JettonMinter(string adapterID, string address, JettonInfo jettonInfo, TonWallet parent) 
            : base(adapterID, address)
        {
            this.jettonInfo = jettonInfo;
            this.parent = parent;
            this.Init();
        }

        private void Init()
        {
            Controller.AddCoinImage(BannerImageID, 96, this.ThemeColor, this.ImageID);
            Controller.AddCoinImage(SmallImageID, 48, this.ThemeColor, this.ImageID);
            InitImages(jettonInfo.LoadImage((image) => InitImages(image)));
            if (this.IsMain && this.parent != null)
                this.parent.TransactionComplete += Parent_TransactionComplete;
        }

        private void InitImages(IImage image)
        {
            if (image != null && image != JettonInfo.JettonImage)
            {
                Images.Add(BannerImageID, image);
                Images.Add(SmallImageID, image);
            }
        }

        private void Parent_TransactionComplete(object sender, ITransactionBase t1, object t2)
        {
            this.OnTransactionComplete(sender, t1, t2);
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
                        InitImages(this.jettonInfo.LoadImage((image) => InitImages(image)));
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

        private TonWallet parent;
        public TonWallet Parent
        {
            get => parent;
            internal set
            {
                if (this.parent == value) return;
                if(this.parent != null)
                    this.parent.TransactionComplete -= Parent_TransactionComplete;
                this.parent = value;
                if(this.parent != null)
                    this.parent.TransactionComplete += Parent_TransactionComplete;

            }
        }

        public override bool IsMain => parent != null && parent.IsMain;

        public override string SmallImageID => "Small_" + Symbol + (this.Adapter.IsTestnet ? "_Test" : "");
        public override string BannerImageID => "Banner_" + Symbol + (this.Adapter.IsTestnet ? "_Test" : "");

        public override string ImageID => "jetton.svg";

        public override string Symbol => jettonInfo.Symbol.First(8);

        public virtual string Owner => jettonInfo.OwnerAddress;

        public override Balance Balance => jettonInfo.TotalSupply;

        public Balance TonBalance => base.Balance;

        public override bool IsSupportMarket => false;
        public override bool IsSupportExport => false;

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

        int ITokenInfo.Color => this.ThemeColor;

        Wallet IToken.Parent => this.parent;

        private ColorButton changeContractButton;
        private ColorButton mintCoinsButton;

        IImage ITokenInfo.LoadImage(ParamHandler<IImage> resultHandler)
        {
            IImage image = Images.Get(this.SmallImageID);
            resultHandler(image);
            return image;
        }

        public override string GetBalanceMarketPrice()
        {
            return base.Balance.GetTextSharps(8) + " " + base.Balance.Symbol;
        }

        public override string GetMarketPrice(decimal balance)
        {
            return null;
        }

        protected virtual JettonInfo GetJettonInfo(AccountState state)
        {
            return this.Adapter.Client.GetJettonInfo(state);
        }

        public virtual bool CheckNewOwner(Wallet wallet)
        {
            return wallet is TonWallet tonWallet && tonWallet.Address != this.parent.Address;
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
                    if (this.parent != null && this.jettonInfo != null && this.jettonInfo.OwnerAddress != this.parent.Address)
                    {
                        if (this.changeContractButton != null)
                        {
                            this.changeContractButton.Parent.Remove(this.changeContractButton);
                            this.changeContractButton.Dispose();
                            this.changeContractButton = null;
                        }

                        if (this.mintCoinsButton != null)
                        {
                            this.mintCoinsButton.Parent.Remove(this.mintCoinsButton);
                            this.mintCoinsButton.Dispose();
                            this.mintCoinsButton = null;
                        }

                    }
                }
                return true;
            }
            return false;
        }


        protected override void DeleteWallet(Component item)
        {
            if (MessageBox.Show(item, MenuAlignment.Center, Language.Current["removingToken"], null, Language.Current["deleteToken", this.Name], MessageBoxButtons.OKCancel))
            {
                if (this.parent != null)
                    this.parent.Wallets.Remove(this.ID);
                WalletsData.Wallets.Remove(this);
            }
        }

        public override void CheckPassword(string passcode, ParamHandler<string> resultHanler)
        {
            this.parent.CheckPassword(passcode, resultHanler);
        }

        protected void SendMessage(string passcode, MessageData data, ParamHandler<object, string> resultHanler)
        {
            this.Parent.SendMessage(passcode, data, (h, e) =>
            {
                data.Dispose();
                resultHanler(h, e);
            });
        }

        protected void CalcFees(MessageData data, ParamHandler<Balance, string> resultHanler)
        {
            this.parent.CalcFees(this.Address, data.amount, "", data.message, null, (fee, e) =>
            {
                data.Dispose();
                resultHanler(fee, e);
            });
        }

        public void MintCoins(string passcode, decimal amount, long queryId, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateMintData(this.Address, queryId, this.Parent.Address, this.Balance.FromDecimal(amount)), resultHanler);
        }

        public void ChangeOwner(string passcode, string newOwner, long queryId, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateChangeOwner(this.Address, queryId, newOwner), resultHanler);
        }

        public void ChangeContent(string passcode, JettonDeployInfo info, string offchainUri, long queryId, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateChangeContent(this.Address, queryId, info, offchainUri), resultHanler);
        }

        public void ChangeOwnerCalcFee(string newOwner, long queryId, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(JettonController.CreateChangeOwner(this.Address, queryId, newOwner), resultHanler);
        }

        public void ChangeContentCalcFee(JettonDeployInfo info, string offchainUri, long queryId, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(JettonController.CreateChangeContent(this.Address, queryId, info, offchainUri), resultHanler);
        }

        public override ColorButton CreateMainLeftButton()
        {
            if (this.jettonInfo != null && this.jettonInfo.OwnerAddress == this.parent.Address)
            {
                changeContractButton = new ColorButton("changeContract");
                changeContractButton.Padding.Set(6);
                changeContractButton.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                changeContractButton.Radius = 6;
                changeContractButton.Executed += (s) =>
                {
                    new ChangeForm(this).Show(s as Component, MenuAlignment.Bottom);
                };
                return changeContractButton;
            }
            return null;
        }

        public override ColorButton CreateMainRightButton()
        {
            if (this.jettonInfo != null && this.jettonInfo.OwnerAddress == this.parent.Address)
            {
                mintCoinsButton = new ColorButton("mintCoins");
                mintCoinsButton.Padding.Set(6);
                mintCoinsButton.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                mintCoinsButton.Radius = 6;
                mintCoinsButton.Executed += (s) =>
                {
                    new MintCoinsForm(this).Show(s as Component, MenuAlignment.Bottom);
                };
                return mintCoinsButton;
            }
            return null;
        }

        public override Component CreateWalletItem()
        {
            return new JettonWalletItem(this);
        }

        protected override void UpdateWaitTransactions(ITransactionBase last, ITransactionBase[] ts)
        {
            if (last == null && ts != null && this.WaitTransactions.Count > 0)
            {
                foreach (ITransactionBase transaction in ts)
                {
                    this.CheckQueryIdTransaction(transaction);
                    if (this.WaitTransactions.Count == 0)
                        break;
                }
            }

        }

    }
}
