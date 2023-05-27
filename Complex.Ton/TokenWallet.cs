using System;
using Complex.Themes;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;


namespace Complex.Ton
{
    public  abstract class TokenWallet : TonUnknownWallet, IToken
    {
        protected TokenWallet(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.parent = data["parent"] as TonWallet;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["parent"] = this.parent;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();

        }
        public TokenWallet(string adapterID, string address, TonWallet parent)
            : base(adapterID, address)
        {
            this.parent = parent;
            this.Init();
        }

        private void Init()
        {
            if (this.IsMain)
                this.parent.TransactionComplete += Parent_TransactionComplete;
        }

        private TonWallet parent;
        public TonWallet Parent
        {
            get => parent;
            internal set
            {
                if (this.parent == value) return;
                if (this.parent != null)
                    this.parent.TransactionComplete -= Parent_TransactionComplete;
                this.parent = value;
                if (this.parent != null)
                    this.parent.TransactionComplete += Parent_TransactionComplete;
            }
        }



        public override bool IsMain => parent != null && parent.IsMain;
        public override bool IsSupportMarket => false;
        public override bool IsSupportExport => false;

        int ITokenInfo.Color => this.ThemeColor;

        Wallet IToken.Parent => this.parent;

        public abstract string Owner { get; }

        void ITokenInfo.LoadImage(ParamHandler<IImage> resultHandler)
        {
            resultHandler(Images.Get(this.SmallImageID));
        }

        public override string GetBalanceMarketPrice()
        {
            return base.Balance.GetTextSharps(8) + " " + base.Balance.Symbol;
        }

        public override string GetMarketPrice(decimal balance)
        {
            return null;
        }

        private void Parent_TransactionComplete(object sender, ITransactionBase t1, object t2)
        {
            this.OnTransactionComplete(sender, t1, t2);
        }

        public virtual bool CheckNewOwner(Wallet wallet)
        {
            return wallet is TonWallet tonWallet && tonWallet.Address != this.parent.Address;
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
            this.parent.SendMessage(passcode, data, (h, e) =>
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

        public override Component CreateWalletItem()
        {
            return new JettonWalletItem(this);
        }

        public abstract void LoadImage(ParamHandler<IImage> paramHandler);
    }
}
