using System;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;

namespace Complex.Ton
{
    public class JettonWallet : JettonMinter
    {
        protected JettonWallet(IData data) : base(data)
        {
        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.walletInfo = data["walletInfo"] as JettonWalletInfo;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["walletInfo"] = this.walletInfo;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }
        public JettonWallet(string adapterID, string address, JettonWalletInfo walletInfo, TonWallet parent)
            : base(adapterID, address, walletInfo.JettonInfo, parent)
        {
            this.walletInfo = walletInfo;
            this.Init();
        }

        private void Init()
        {
            this.WaitTransactions.Added += WaitTransactions_Added;
        }

        private void WaitTransactions_Added(object sender, object value)
        {
            if (this.Parent != null)
                this.Parent.WaitTransactions.Add(value);
        }

        private JettonWalletInfo walletInfo;
        public JettonWalletInfo WalletInfo => walletInfo;

        public override Balance Balance => walletInfo.Balance;

        public override string OwnerAddress => walletInfo.OwnerAddress;
        public override bool IsSupportInvoiceUrl => true;

        protected override JettonInfo GetJettonInfo(AccountState state)
        {
            JettonWalletInfo info = JettonController.GetJettonWalletInfo(state);
            this.walletInfo = info == null ? this.walletInfo : info;
            if (this.walletInfo != null)
                return this.walletInfo.JettonInfo;
            return null;
        }

        public override bool Update(AccountState state)
        {
            bool res = base.Update(state);
            if (!res)
            {
                JettonWalletInfo info = JettonController.GetJettonWalletInfo(state);
                if (info != null && !CompareFields(this.walletInfo.JettonInfo, info.JettonInfo))
                {
                    this.walletInfo = info;
                    this.JettonInfo = this.walletInfo.JettonInfo;
                    return true;
                }
            }
            return res;
        }

        public void BurnCoins(string passcode, long queryId, decimal amount, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, JettonController.CreateBurnData(queryId, this.Address, this.Parent.Address, this.Balance.FromDecimal(amount)), resultHanler);
        }

        public override void SendAmount(string passcode, string destAddress, decimal amount, string message, ParamHandler<object, string> resultHanler)
        {
            long queryId = Utils.Random(int.MaxValue);
            this.SendMessage(passcode, JettonController.CreateTransferData(queryId, this.Address, destAddress, this.Parent.Address, this.Balance.FromDecimal(amount)), (h,e)=>
            {
                if (h != null)
                    this.WaitTransactions.Add(queryId);
                resultHanler(h, e);
            });
        }

        public override void CalcFees(string destAddress, decimal amount, string message, ParamHandler<Balance, string> resultHanler)
        {
            resultHanler(new Gram(JettonController.MessageTransferFee), null);
        }

        protected override void OnTransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            if (sender == this.Parent)
            {
                if (value is byte[])
                    this.WaitTransactions.Remove(value);
            }
            base.OnTransactionComplete(sender, transaction, value);
        }

        public override ColorButton CreateMainLeftButton()
        {
            ColorButton button = new ColorButton("send");
            button.Padding.Set(6);
            button.MinWidth = 120;
            button.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
            button.Radius = 6;
            button.Executed += (s) => new JettonSendForm(this).Show(s as Component, MenuAlignment.Bottom);
            return button;
        }

        public override ColorButton CreateMainRightButton()
        {
            ColorButton button = new ColorButton("receve");
            button.Padding.Set(6);
            button.MinWidth = 120;
            button.Radius = 6;
            button.Executed += (s) => new JettonReceiveForm(this).Show(s as Component, MenuAlignment.Bottom);
            return button;
        }

        public override string GetInvoiceUrl(string address, decimal amount, string message)
        {
            if (address != this.Address)
                return base.GetInvoiceUrl(address, amount, message);
            string url = "ton://transfer/" + this.walletInfo.OwnerAddress + "?jetton=" + this.walletInfo.JettonInfo.JettonAddress;

            if (amount > 0)
            {
                url += "&amount=" + this.Balance.FromDecimal(amount);
                if (!string.IsNullOrEmpty(message))
                    url += "&";
            }
            return url;
        }
    }
}
