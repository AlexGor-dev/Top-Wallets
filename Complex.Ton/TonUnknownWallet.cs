using System;
using System.Threading;
using Complex.Wallets;
using Complex.Collections;
using Complex.Controls;

namespace Complex.Ton
{
    public class TonUnknownWallet : Wallet
    {
        protected TonUnknownWallet(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.address = data["address"] as string;
            this.lastTransactionId = (long)data["lastTransactionId"];
            this.version = data["version"] as string;
            this.state = (WalletState)data["state"];
            this.lastActivityTime = (DateTime)data["lastActivityTime"];
            this.lastTransactionHash = data["lastTransactionHash"] as byte[];
            this.type = (WalletType)data["type", this.type];
            Util.Try(() => this.balance = data["balance"] as Gram);
            if(this.balance == null)
                this.balance = new Gram(0);
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["address"] = this.address.ToString();
            data["balance"] = this.balance;
            data["lastTransactionId"] = this.lastTransactionId;
            data["version"] = this.version;
            data["state"] = this.state;
            data["lastActivityTime"] = this.lastActivityTime;
            data["lastTransactionHash"] = this.lastTransactionHash;
            data["type"] = this.type;
        }

        public TonUnknownWallet(string adapterID, string address)
            :base(adapterID)
        {
            this.address = address;
            this.balance = new Gram(0);
        }

        private Gram balance;

        private long lastTransactionId;
        private byte[] lastTransactionHash;

        private string address;

        public override string Address => address;
        public override Balance Balance => balance;

        public new TonAdapter Adapter => base.Adapter as TonAdapter;

        private string version;
        public override string Version => version;

        private WalletState state;
        public override WalletState State => state;

        private WalletType type = WalletType.Empty;
        public WalletType Type => type;

        private DateTime lastActivityTime;
        public override DateTime LastActivityTime => lastActivityTime;

        public override bool IsEmpty => this.lastTransactionId == 0;

        public override bool CanTokens => true;
        public override bool CanNfts => true;

        public virtual bool Update(AccountState state)
        {
            if (state != null)
            {
                if (this.type == WalletType.Empty)
                    this.type = state.Type;
                if (this.lastTransactionId != state.Transaction || state.Balance > 0 && this.balance.Value != state.Balance)
                {
                    this.lastTransactionId = state.Transaction;
                    this.lastTransactionHash = state.TransactionHash;
                    this.version = state.Version;
                    this.lastActivityTime = state.Time;
                    this.type = state.Type;
                    WalletState oldState = this.state;
                    this.state = state.GetWalletState();
                    bool isnew = this.lastTransactionId == 0;
                    this.balance.Update((UInt128)Math.Max(0, state.Balance));
                    if (this.IsMain && this.state != oldState && this.state == WalletState.Active)
                        this.OnCreated();
                    return true;
                }
            }
            return false;
        }

        protected override void UpdateCore(ParamHandler<bool, string> paramHandler)
        {
            var (s, e) = this.Adapter.CreateAccountState(this.address);
            bool res = Update(s);
            if(s != null)
                s.Dispose();
            paramHandler(res, e);
        }

        private void GetHttpTransactions(ITonTransaction last, int count, ParamHandler<ITransactionBase[], string> resultHanler)
        {
            long utime = 0;
            if (last != null)
                utime = last.UTime;
            this.Adapter.GetTransactions(this, utime, count, resultHanler);
        }

        protected override void GetTransactionsCore(ITransactionBase last, int count, ParamHandler<ITransactionBase[], string> resultHanler)
        {
            if (!Application.IsExited)
            {
                ITonTransaction tlast = last as ITonTransaction;
                long lt = 0;
                string hash = null;
                if (tlast != null)
                {
                    lt = tlast.Lt;
                    hash = tlast.Hash;
                }
                else
                {
                    lt = this.lastTransactionId;
                    hash = Convert.ToBase64String(this.lastTransactionHash);
                }
                this.Adapter.GetTransactions(this.type, this.address, lt, hash, count, (ts, e) =>
                {
                    if (ts != null && e != null)
                        GetHttpTransactions(last as ITonTransaction, count, resultHanler);
                    else
                        resultHanler(ts, e);
                });
            }
        }

        public override void GetTokens(ParamHandler<ITokenInfo[], string> resultHanler)
        {
            this.Adapter.GetTokens(this.address, resultHanler);
        }

        public override void GetNfts(int offset, int count, ParamHandler<INftInfo[], string> resultHanler)
        {
            this.Adapter.GetNfts(this.address, offset, count, resultHanler);
        }

        public override Component CreateMainPanel()
        {
            return new TonWalletMainPanel(this);
        }

        public override Component CreateTransactionDetailItem(ITransactionDetail detail, GridWaitEffect waitEffect)
        {
            if (detail is IJettonSource js && js.Jetton != null)
                return new TonTransactionDetailItem(this, detail, js);
            return base.CreateTransactionDetailItem(detail, waitEffect);
        }

        private bool CheckQueryIdTransaction(ITransactionBase transaction, TonTransactionMessage detail)
        {
            if (detail != null && detail.QueryId > 0)
            {
                foreach (object value in this.WaitTransactions.ToArray())
                {
                    if (value is long qid && qid == detail.QueryId)
                    {
                        this.WaitTransactions.Remove(qid);
                        Util.Run(() => { this.OnTransactionComplete(transaction, qid); });
                        return true;
                    }
                }
            }
            return false;
        }
        protected void CheckQueryIdTransaction(ITransactionBase transaction)
        {
            if (transaction is TonTransaction tr)
            {
                this.CheckQueryIdTransaction(transaction, tr.Detail as TonTransactionMessage);
            }
            else if (transaction is TonTransactionGroup g)
            {
                foreach (TransactionDetail detail in g.Details)
                    this.CheckQueryIdTransaction(transaction, detail as TonTransactionMessage);
            }

        }

        public override string GetInvoiceUrl(string address, decimal amount, string message)
        {
            string url = "ton://transfer/" + address;
            if (amount > 0)
            {
                url += "?amount=" + this.Balance.FromDecimal(amount);
                if (!string.IsNullOrEmpty(message))
                    url += "&";
            }
            if (!string.IsNullOrEmpty(message))
            {
                if (amount <= 0)
                    url += "?";
                url += "text=" + message;
            }
            return url;
        }

        public override Component CreateTockenItem(ITokenInfo token, GridWaitEffect waitEffect)
        {
            if(token is JettonWalletInfo jwi)
                return new TonTokenItem(this, jwi, waitEffect);
            return base.CreateTockenItem(token, waitEffect);
        }
        public override void CreateTokenInfoMenu(ITokenInfo token, ParamHandler<Menu> paramHandler)
        {
            if (token is JettonWalletInfo jwi)
                paramHandler(new JettonMenu(this, jwi.JettonInfo, true));
            else
                base.CreateTokenInfoMenu(token, paramHandler);
        }

        public override Component CreateNftItem(INftInfo nft, GridWaitEffect waitEffect)
        {
            if (nft is NftInfo ni)
                return new TonNftItem(this, ni, waitEffect);
            return base.CreateNftItem(nft, waitEffect);
        }

        public override void CreateTokenInfoAddressMenu(ITokenInfo token, ParamHandler<MenuStrip> paramHandler)
        {
            if (token is JettonWalletInfo jwi)
            {
                MenuStrip menu = new MenuStrip();
                menu.MinimumSize.width = 200;
                menu.Add("copyAddress.svg", "copyJettonAddress", true).Executed += (s) =>
                {
                    Clipboard.SetText(address);
                    MessageView.Show(Language.Current["address"] + " " + jwi.JettonInfo.JettonAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                };
                paramHandler(menu);

            }
            else
            {
                base.CreateTokenInfoAddressMenu(token, paramHandler);
            }
        }

        public void CreateAccountState(ParamHandler<AccountState, string> paramHandler)
        {
            this.Adapter.CreateAccountState(this.address, paramHandler);
        }
    }
}
