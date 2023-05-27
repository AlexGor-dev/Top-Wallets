using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Trader;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public abstract class Wallet : Serializable, IUnique, INameSource, IAdapterSource, IBannerImageSource
    {
        protected Wallet(IData data)
        {

        }

        protected override void Load(IData data)
        {
            this.adapterID = data["adapterID"] as string;
            this.name = data["name"] as string;
            this.lastTransactionID = data["lastTransactionID"] as string;
        }

        protected override void Save(IData data)
        {
            data["adapterID"] = this.adapterID;
            data["name"] = this.name;
            data["lastTransactionID"] = this.lastTransactionID;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public Wallet(string adapterID)
        {
            this.adapterID = adapterID;
            this.Init();
        }

        private void Init()
        {
            transactions = new Array<ITransactionBase>();
            tokens = new Collection<ITokenInfo>();
        }

        public event Handler Changed;
        public event Handler Created;
        public event Handler<ITransactionBase,object> TransactionComplete;
        public event Handler TransactionsLoaded;
        public event Handler<ITransactionBase[]> TransactionsNew;
        public event Handler<ITransactionBase[]> TransactionsOld;

        private string lastTransactionID;

        private string name;
        public string Name
        {
            get
            {
                if (name != null)
                    return name;
                string known = this.Adapter.GetKnownAddress(this.Address);
                if (known != null)
                    return known;
                return this.Address;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) value = null;
                if (name == value) return;
                name = value;
                OnChanged();
            }
        }

        private object tag;
        public object Tag
        {
            get => tag;
            set => tag = value;
        }

        private bool firstStart = true;
        public string OriginalName => name;

        public string NameOrCoins => OriginalName != null ? OriginalName : Symbol + " coins";

        private string adapterID;
        public string AdapterID => adapterID;

        public virtual string ID => GetID(adapterID, Address, this.IsMain);

        public abstract string Address { get; }

        public abstract Balance Balance { get; }

        //public abstract int DefaultBalanceSignCount { get; }

        public abstract string Version { get; }

        public abstract WalletState State { get; }

        public abstract DateTime LastActivityTime { get; }

        public virtual bool IsMain => false;

        public virtual bool CanTokens => false;
        public virtual bool CanNfts => false;

        public bool IsSupport
        {
            get
            {
                CryptoCoinInfo info = MainSettings.Current.Remote.CryptoCoins[this.Symbol];
                if(info != null)
                    return info.SupportAddress == this.Address || info.SupportTestAddress == this.Address;
                return false;
            }
        }

        public virtual bool IsSupportMarket => true;
        public virtual bool IsSupportExport => true;
        public virtual bool IsSupportSendText => false;
        public virtual bool IsSupportInvoiceUrl => false;

        private object loasdTransactionAsync = new object();

        private Collection<object> waitTransactions = new Collection<object>();
        public Collection<object> WaitTransactions => waitTransactions;

        private Array<ITransactionBase> transactions;
        public Array<ITransactionBase> Transactions => transactions;

        private Collection<ITokenInfo> tokens;
        public Collection<ITokenInfo> Tokens => tokens;

        public virtual ITransactionBase LastTransaction => this.transactions.First;

        public virtual bool IsEmpty => true;

        public static string GetID(string adapterID, string address, bool isMain) => (isMain ? "Main" : "Other") + "_" + adapterID + "_" + address;

        public static string GetID(WalletAdapter adapter, string address, bool isMain) => GetID(adapter.ID, address, isMain);

        public virtual string GetMarketPrice(decimal balance)
        {
            if (this.IsSupportMarket)
            {
                decimal lastPrice = Market.LastPrice;
                if (lastPrice != 0)
                    return "≈" + Market.Coin.GetCurrencyPrice(balance, 3);
            }
            return null;
        }

        public virtual string GetBalanceMarketPrice()
        {
            return GetMarketPrice(Balance);
        }


        private WalletAdapter adapter;
        public WalletAdapter Adapter
        {
            get
            {
                if (adapter == null)
                    adapter = Controller.GetAdapter(this.adapterID);
                return adapter;
            }
        }

        public virtual CoinMarket Market => adapter.Market;

        public decimal Volume => this.Market.GetVolume(this.Balance);
        public virtual string Symbol => this.Adapter.Symbol;

        public virtual ThemeColor ThemeColor => this.Adapter.ThemeColor;

        public virtual string ImageID => this.Adapter.ImageID;

        public virtual string SmallImageID => this.Adapter.SmallImageID;

        public virtual string BannerImageID => this.Adapter.BannerImageID;

        protected virtual void UpdateCore(ParamHandler<bool, string> paramHandler)
        {
        }

        public void Update()
        {
            this.UpdateCore((r, e) =>
            {
                if (r)
                    this.OnChanged();
                else if (e != null)
                    MessageView.Show(e);
            });
        }

        protected virtual void OnChanged()
        {
            Events.Invoke(this.Changed, this);
        }

        protected virtual void OnCreated()
        {
            Events.Invoke(this.Created, this);
        }

        protected virtual void OnTransactionsLoaded()
        {
            Events.Invoke(this.TransactionsLoaded, this);
        }

        protected virtual void OnTransactionsNew(ITransactionBase[] transactions)
        {
            if (transactions != null && transactions.Length > 0)
            {
                bool first = this.firstStart;
                this.firstStart = false;
                this.lastTransactionID = transactions.First().ID;
                Events.Invoke(this.TransactionsNew, this, transactions);
                if (!first)
                {
                    if (this.IsMain)
                    {
                        Array<ITransactionBase> arr = new Array<ITransactionBase>();
                        foreach (ITransactionBase transaction in transactions)
                            if (transaction.Time > this.Adapter.ServerUtcTime.AddSeconds(-60))
                                arr.Add(transaction);
                        if (arr.Count > 0)
                        {
                            foreach (ITransactionBase transaction in arr)
                            {
                                decimal amount = transaction.GetAmount(this.Symbol);
                                if (amount != 0)
                                {
                                    if (amount < 0)
                                        MainSettings.Current.PlaySound("outTransaction");
                                    else
                                        MainSettings.Current.PlaySound("inTransaction");
                                    break;
                                }
                            }
                            if (MainSettings.Current.General.ShowTransactionMessages)
                            {
                                foreach (ITransactionBase transaction in arr)
                                {
                                    decimal amount = transaction.GetAmount(this.Symbol);
                                    if (amount != 0)
                                    {
                                        if (transaction is ITransactionGroup g)
                                        {
                                            foreach (ITransactionDetail detail in g.Details)
                                            {
                                                if (detail.Amount.Symbol == this.Symbol)
                                                {
                                                    Component component = this.CreateTransactionMessage(transaction, detail);
                                                    if (component != null)
                                                        MessageView.Show(component);
                                                }
                                            }
                                        }
                                        else if(transaction is ITransactionDetail detail)
                                        {
                                            Component component = this.CreateTransactionMessage(transaction, detail);
                                            if (component != null)
                                                MessageView.Show(component);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnTransactionsOld(ITransactionBase[] transactions)
        {
            if (transactions != null && transactions.Length > 0)
                Events.Invoke(this.TransactionsOld, this, transactions);
        }
        protected virtual void OnTransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            Events.Invoke(this.TransactionComplete, sender, transaction, value);
        }

        protected virtual void OnTransactionComplete(ITransactionBase transaction, object value)
        {
            this.OnTransactionComplete(this, transaction, value);
        }


        public virtual void CheckPassword(string passcode, ParamHandler<string> resultHanler)
        {
            resultHanler("invalidPassword");
        }

        public virtual void SendAmount(string passcode, string destAddress, decimal amount, string message, ParamHandler<object, string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public virtual void CreateWallet(string passcode, ParamHandler<object, string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public virtual void CalcFees(string destAddress, decimal amount, string message, ParamHandler<Balance, string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        protected virtual void UpdateWaitTransactions(ITransactionBase last, ITransactionBase[] ts)
        {
        }

        protected virtual void GetTransactionsCore(ITransactionBase last, int count, ParamHandler<ITransactionBase[], string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public void GetTransactions(ITransactionBase last, int count, ParamHandler<ITransactionBase[], string> resultHanler)
        {
            ITransactionBase[] tra = null;
            bool transLoaded = false;
            lock (loasdTransactionAsync)
            {
                if (this.Transactions.Count > 0)
                {
                    if (last != null && last != this.transactions.Last)
                    {
                        int index = QuickSort.GetIndex(this.Transactions, last, SortDirection.Descending, (a, b) => { return a.CompareTo(b); }) + 1;
                        if (index < this.Transactions.Count)
                        {
                            tra = new ITransactionBase[Math.Min(this.Transactions.Count - index, count)];
                            for (int i = 0; i < tra.Length; i++)
                                tra[i] = this.Transactions[i + index];
                        }
                    }
                }
            }
            if (tra != null)
            {
                resultHanler(tra, null);
            }
            else
            {
                GetTransactionsCore(last, count, (ts, e) =>
                {
                    if (e != null)
                    {
                        resultHanler(ts, e);
                    }
                    else
                    {
                        lock (loasdTransactionAsync)
                        {
                            this.UpdateWaitTransactions(last, ts);
                            if (ts != null)
                            {
                                System.Array.Sort(ts, (a, b) => { return -a.CompareTo(b); });
                                Array<ITransactionBase> arr = new Array<ITransactionBase>();
                                if (last == null)
                                {
                                    ITransactionBase first = this.Transactions.First;
                                    foreach (ITransactionBase transaction in ts)
                                    {
                                        if (first != null && transaction.CompareTo(first) <= 0)
                                            break;
                                        //if (transaction.Amount > 0)
                                        arr.Add(transaction);
                                    }
                                }
                                else
                                {
                                    ITransactionBase lastt = this.Transactions.Last;
                                    foreach (ITransactionBase transaction in ts)
                                    {
                                        if (lastt != null && transaction.CompareTo(lastt) >= 0)
                                            continue;
                                        //if (transaction.Amount > 0)
                                        arr.Add(transaction);
                                    }

                                }
                                ts = arr.ToArray();
                                if (ts.Length > 0)
                                {
                                    lock (this.Transactions)
                                    {
                                        this.Transactions.Add(ts);
                                        this.Transactions.Sort((a, b) => { return -a.CompareTo(b); });
                                    }
                                    transLoaded = true;
                                }
                                tra = ts;
                            }

                        }
                        if (tra != null && tra.Length > 0)
                        {
                            if (transLoaded)
                                this.OnTransactionsLoaded();
                            if (last == null)
                                this.OnTransactionsNew(tra);
                            else
                                this.OnTransactionsOld(tra);
                        }
                        resultHanler(tra, null);
                    }
                });
            }
        }


        public virtual void GetTokens(ParamHandler<ITokenInfo[], string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public virtual void GetNfts(int offset, int count, ParamHandler<INftInfo[], string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public virtual void GetWords(string passcode, ParamHandler<string[], string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public virtual string GetInvoiceUrl(string address, decimal amount, string message)
        {
            return null;
        }

        public virtual void ExportData(string passcode, string dataPassword, ParamHandler<object, string> resultHanler)
        {
            resultHanler(null, "notSupport");
        }

        public virtual void ImportData(string passcode, string dataPassword, object exportData, ParamHandler<string> resultHanler)
        {
            resultHanler("notSupport");
        }

        public virtual void Delete(string passcode, ParamHandler<string> resultHanler)
        {
            resultHanler("notSupport");
        }

        protected virtual void DeleteWallet(Component item)
        {
            new DeleteWalletForm(this).ShowDialog(Application.Form);
        }

        public virtual void CreateMenu(Component item, ParamHandler<Menu> paramHandler)
        {
            MenuStrip menu = new MenuStrip();
            menu.MinimumSize.width = 200;
            MenuStripLabel label = new MenuStripLabel(this.Name);
            label.MaxWidth = 200;
            menu.Add(label);
            menu.Add("setName").Executed += (s) =>
            {
                string name = this.OriginalName;
                if (string.IsNullOrEmpty(name))
                    name = UniqueHelper.NextName("Wallet1", WalletsData.Wallets);
                if (TextDialog.Show(item, MenuAlignment.Center, Language.Current["setName"], "", ref name))
                {
                    this.Name = name.Trim();
                }
            };
            menu.Add("remove").Executed += (s) => { this.DeleteWallet(item); };
            paramHandler(menu);
        }

        public virtual void CreateTokenInfoMenu(ITokenInfo token, ParamHandler<Menu> paramHandler)
        {
            paramHandler(null);
        }

        public virtual void CreateTokenInfoAddressMenu(ITokenInfo token, ParamHandler<MenuStrip> paramHandler)
        {
            paramHandler(null);
        }

        public virtual void CreateTransactionAddressMenu(string address, ParamHandler<MenuStrip> paramHandler)
        {
            MenuStrip menu = new MenuStrip();
            menu.MinimumSize.width = 200;
            menu.Add("copyAddress.svg", "copyAddress", true).Executed += (s) =>
            {
                Clipboard.SetText(address);
                MessageView.Show(Language.Current["address"] + " " + address + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            };
            paramHandler(menu);
        }

        public virtual Component CreateWalletItem()
        {
            if (this.IsSupportMarket)
                return new WalletItem(this);
            return new WalletLiteItem(this);
        }

        public virtual Component CreateWalletPanel()
        {
            return new WalletPanel(this);
        }

        public virtual ColorButton CreateMainLeftButton()
        {
            ColorButton button = new ColorButton("send");
            button.Padding.Set(6);
            button.MinWidth = 120;
            button.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
            button.Radius = 6;
            button.Executed += (s) =>
            {
                Controller.ShowSendForm(this, s as ColorButton, MenuAlignment.Bottom);
            };
            return button;
        }

        public virtual ColorButton CreateMainRightButton()
        {
            ColorButton button = new ColorButton("receve");
            button.Padding.Set(6);
            button.MinWidth = 120;
            button.Radius = 6;
            button.Executed += (s) =>
            {
                Controller.ShowReceiveForm(this, s as ColorButton, MenuAlignment.Bottom);
            };
            return button;
        }

        public virtual Component CreateWalletInfoPanel()
        {
            return new WalletPanel(this);

        }

        public virtual Component CreateTockenItem(ITokenInfo token, GridWaitEffect waitEffect)
        {
            return new TokenItem(this, token, waitEffect);
        }

        public virtual Component CreateNftItem(INftInfo nft, GridWaitEffect waitEffect)
        {
            return new NftInfoItem(this, nft, waitEffect);
        }

        public virtual Component CreateTokensPanel()
        {
            return new TokensListPanel(this);
        }

        public virtual Component CreateNftsPanel()
        {
            return new NftsPanel(this);
        }

        public virtual Component CreateTransactionPanel()
        {
            return new TransactionPanel(this);
        }

        public virtual Component CreateTransactionsChartPanel()
        {
            return new TransactionsChartPanel(this);
        }

        public virtual Component CreateBuySellPanel()
        {
            return new BuySellPanel(this);
        }

        public virtual Component CreateMarketPanel()
        {
            return new MarketPanel(this);
        }

        public virtual Component CreateMainPanel()
        {
            return new WalletMainPanel(this);
        }

        public virtual Component CreateTransactionItem(ITransactionBase transaction, GridWaitEffect waitEffect)
        {
            if (transaction is ITransaction tr)
                return new TransactionGroupItem(this, waitEffect, transaction, tr);
            if (transaction is ITransactionGroup g)
                return new TransactionGroupItem(this, waitEffect, transaction, g.Details.ToArray());
            return null;
        }

        public virtual Component CreateTransactionDetailItem(ITransactionDetail detail, GridWaitEffect waitEffect)
        {
            return new TransactionDetailItem(this, detail, waitEffect);
        }

        public virtual Component CreateTransactionMessage(ITransactionBase transaction, ITransactionDetail detail)
        {
            return new TransactionMessageContainer(this, transaction, detail);
        }

        public virtual bool CheckSendWallet(Wallet wallet)
        {
            return wallet.IsMain && wallet.AdapterID == this.AdapterID && wallet.Address != this.Address;
        }

        public override string ToString()
        {
            return ID + " " + Balance + " " + base.ToString();
        }
    }
}
