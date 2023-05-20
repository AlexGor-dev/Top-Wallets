using System;
using Complex.Controls;
using Complex.Animations;
using Complex.Themes;
using Complex.Drawing;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public partial class TotalPanel
    {
        private class SupportContainer : Container
        {
            public SupportContainer(UniqueCollection<TopSupport> topSupports, UniqueCollection<TopSupport> lastMessages, BalloonComponent balloon)
            {
                this.topSupports = topSupports;
                this.lastMessages = lastMessages;
                this.balloon = balloon;
                this.Padding.Set(10, 0, 10, 0);
                this.MinWidth = 200;
                this.supportButton = new TextButton("supportProject");

#if DEBUG
                this.debug = true;
#endif


                if (MainSettings.Current.Remote.Support.Enabled || this.debug)
                {
                    foreach (CryptoCoinInfo info in MainSettings.Current.Remote.CryptoCoins)
                    {
                        if (this.debug)
                            this.coinInfos.Add(new CryptoCoinInfo(info.AdapterID + " Test", info.Symbol, info.SupportTestAddress, info.SupportActionAmount));
                        else if (info.SupportEnabled)
                            this.coinInfos.Add(info);
                    }

                    if (System.Diagnostics.Debugger.IsAttached)
                    {
                        foreach (TopSupport support in this.topSupports.ToArray())
                        {
                            CryptoCoinInfo coinInfo = this.coinInfos[support.Symbol];
                            if (coinInfo != null && coinInfo.AdapterID != support.AdapterID)
                                this.topSupports.Remove(support);
                        }

                        foreach (TopSupport support in this.lastMessages.ToArray())
                        {
                            CryptoCoinInfo coinInfo = this.coinInfos[support.Symbol];
                            if (coinInfo != null && coinInfo.AdapterID != support.AdapterID)
                                this.lastMessages.Remove(support);
                        }
                    }

                    this.topContainer = new TopSupportContainer(this.topSupports, this.lastMessages);
                    this.topContainer.Dock = DockStyle.Top;
                    this.topContainer.Visible = this.topSupports.Count > 0;
                    this.topContainer.Update(this.topSupports.First);
                    this.Add(this.topContainer);

                    this.supportButton.Padding.Set(4, 2, 4, 2);
                    this.supportButton.Dock = DockStyle.BottomCenter;
                    this.supportButton.Executed += (s) => new SupportForm().Show(this.supportButton, MenuAlignment.Bottom);
                    this.supportButton.Visible = this.coinInfos.Count > 0;
                    this.Add(this.supportButton);


                    this.timer = new ThreadTimer(5000);
                    this.timer.Tick += (s ,p) =>
                    {
                        this.timer.Stop();
                        TopSupport support;
                        lock (this.pendingSupports)
                        {
                            support = this.pendingSupports[0];
                            this.pendingSupports.RemoveAt(0);
                        }
                        if (MainSettings.Current.General.ShowProjectSupportMessages)
                        {
                            if (support != null)
                            {
                                Application.Invoke(() =>
                                {
                                    if (this.balloon == null)
                                    {
                                        this.balloon = new BalloonComponent();
                                        this.balloon.TailHeight = 50;
                                        MainContainer.Current.Add(this.balloon);
                                    }
                                    this.balloon.BringToFront();
                                    this.balloon.Show(this.Left + this.Width / 2, this.Parent.Bottom, MenuAlignment.Bottom, new SupportItem(support, true));
                                    this.delayShowTimer.Start();
                                    MainSettings.Current.PlaySound("projectSupportMessages");
                                });
                            }
                        }
                    };

                    this.delayShowTimer = new ThreadTimer(4000);
                    this.delayShowTimer.Tick += (s, p) =>
                    {
                        this.delayShowTimer.Stop();
                        Application.Invoke(() => this.balloon.Hide());
                        if (MainSettings.Current.General.ShowProjectSupportMessages)
                            if (this.pendingSupports.Count > 0)
                                this.timer.Start();
                    };
                    Controller.Adapters.Added += Adapters_Added;
                    Controller.Adapters.Removed += Adapters_Removed;
                }
            }

            protected override void OnDisposed()
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    Controller.Adapters.Added += Adapters_Added;
                    Controller.Adapters.Removed += Adapters_Removed;
                }
                if (this.delayShowTimer != null)
                    this.delayShowTimer.Dispose();
                base.OnDisposed();
            }


            private ThreadTimer timer;
            private ThreadTimer delayShowTimer;
            private TextButton supportButton;
            private TopSupportContainer topContainer;
            private bool firtsStart = true;

            private BalloonComponent balloon;
            public BalloonComponent Balloon => balloon;

            private readonly Rect clientRect = new Rect();

            private bool debug = false;

            private Font font = Theme.font12Bold;

            private UniqueCollection<CryptoCoinInfo> coinInfos = new UniqueCollection<CryptoCoinInfo>();

            private UniqueCollection<Wallet> wallets = new UniqueCollection<Wallet>();

            private Array<TopSupport> pendingSupports = new Array<TopSupport>();
            private Hashtable<string, ITransactionBase> lastViewTransactions = new Hashtable<string, ITransactionBase>();

            private UniqueCollection<TopSupport> topSupports;
            public UniqueCollection<TopSupport> TopSupports => topSupports;

            private UniqueCollection<TopSupport> lastMessages;
            public UniqueCollection<TopSupport> LastMessages => lastMessages;

            protected override void OnSizeChanged()
            {
                GetClientRectangle(clientRect);
                base.OnSizeChanged();
            }

            public void Update()
            {
                foreach (Wallet w in this.wallets)
                {
                    w.Changed -= W_Changed;
                    w.TransactionsNew -= W_TransactionsNew;
                }
                this.wallets.Clear(true);
                if (Controller.Adapters.Count == 0 || !WalletsData.IsCurrentExist)
                {
                    this.supportButton.Visible = false;
                    if (this.topContainer != null)
                        this.topContainer.Visible = false;
                }
                else
                {
                    this.supportButton.Visible = this.coinInfos.Count > 0;
                    if (this.topContainer != null)
                        this.topContainer.Visible = this.topSupports.Count > 0;
                    foreach (WalletAdapter adapter in Controller.Adapters)
                        Adapters_Added(null, adapter);
                }
            }

            private void Adapters_Added(object sender, WalletAdapter adapter)
            {
                if (adapter.IsConnected)
                    Adapter_Connected(adapter);
                else
                    adapter.Connected += Adapter_Connected;
            }

            private void Adapters_Removed(object sender, WalletAdapter adapter)
            {
                adapter.Connected -= Adapter_Connected;
            }

            private void Adapter_Connected(object sender)
            {
                WalletAdapter adapter = sender as WalletAdapter;
                foreach (CryptoCoinInfo coinInfo in this.coinInfos)
                {
                    if (coinInfo.AdapterID == adapter.ID)
                    {
                        string id = Wallet.GetID(adapter.ID, coinInfo.SupportAddress, false);
                        Wallet wallet = this.wallets[id];
                        if (wallet == null)
                        {
                            adapter.GetWallet(coinInfo.SupportAddress, (w, e) =>
                            {
                                if (w != null)
                                {
                                    w.Tag = coinInfo;
                                    lock (this.wallets)
                                        this.wallets.Add(w);
                                    w.Changed += W_Changed;
                                    w.TransactionsNew += W_TransactionsNew;
                                    ThreadTimer.Delay(3000, () => w.GetTransactions(null, 16,(ts, e2)=> { }));
                                }
                            });
                        }
                    }
                }
            }

            private void W_TransactionsNew(object sender, ITransactionBase[] transactions)
            {
                bool first = this.firtsStart;
                this.firtsStart = false;
                Wallet wallet = sender as Wallet;
                CryptoCoinInfo coinInfo = wallet.Tag as CryptoCoinInfo;

                foreach (TopSupport support in this.topSupports.ToArray())
                {
                    if ((DateTime.UtcNow - support.Time).TotalDays > MainSettings.Current.Remote.Support.MaxDays)
                        this.topSupports.Remove(support);
                    if (support.AdapterID == wallet.AdapterID)
                        support.Volume = support.Amount * wallet.Market.LastPrice;
                    else
                    {
                        WalletAdapter adapter = Controller.GetAdapter(support.AdapterID);
                        support.Volume = support.Amount * adapter.Market.LastPrice;
                    }
                }
                ITransactionBase lastView = this.lastViewTransactions[wallet.AdapterID];
                TopSupport last = this.topSupports.Last;
                for (int i = transactions.Length - 1; i >= 0; i--)
                {
                    ITransaction transaction = transactions[i] as ITransaction;
                    if (transaction != null && (DateTime.UtcNow - transaction.Time).TotalDays <= MainSettings.Current.Remote.Support.MaxDays)
                    {
                        if ((decimal)transaction.Amount >= coinInfo.SupportActionAmount || this.debug)
                        {
                            if (!this.lastMessages.Contains(wallet.AdapterID + transaction.ID))
                            {
                                var (name, message) = ParseMessage(transaction.Message);
                                TopSupport support = new TopSupport(wallet.AdapterID, transaction.ID, name, message, transaction.Amount, transaction.Amount * wallet.Market.LastPrice, wallet.Symbol, transaction.Time, wallet.Balance.DefaultSignCount);

                                this.lastMessages.Add(support);
                                if (lastView == null || lastView.CompareTo(transaction) < 0)
                                {
                                    lastView = transaction;
                                    this.lastViewTransactions[wallet.AdapterID] = lastView;
                                    if (!first && MainSettings.Current.General.ShowProjectSupportMessages)
                                    {
                                        lock (this.pendingSupports)
                                        {
                                            this.pendingSupports.Add(support);
                                            if (this.pendingSupports.Count == 1)
                                                this.timer.Start();
                                        }
                                    }
                                }
                            }
                            if (!this.topSupports.Contains(wallet.AdapterID + transaction.ID))
                            {
                                bool add = last == null;
                                if (last != null)
                                {
                                    if (last.AdapterID == wallet.AdapterID)
                                    {
                                        if (transaction.Amount > last.Amount || transaction.Amount == last.Amount && transaction.Time > last.Time)
                                            add = true;
                                    }
                                    else
                                    {
                                        WalletAdapter adapter = Controller.GetAdapter(last.AdapterID);
                                        decimal tvolume = transaction.Amount * wallet.Market.LastPrice;
                                        last.Volume = last.Amount * wallet.Market.LastPrice;
                                        if (tvolume > last.Volume || tvolume == last.Volume && transaction.Time > last.Time)
                                            add = true;
                                    }
                                }
                                if (add)
                                {
                                    var (name, message) = ParseMessage(transaction.Message);
                                    last = new TopSupport(wallet.AdapterID, transaction.ID, name, message, transaction.Amount, transaction.Amount * wallet.Market.LastPrice, wallet.Symbol, transaction.Time, wallet.Balance.DefaultSignCount);
                                    this.topSupports.Add(last);
                                }
                            }
                        }
                    }
                }

                this.lastMessages.Sort((a, b) => -a.Time.CompareTo(b.Time));
                while (this.lastMessages.Count > MainSettings.Current.Remote.Support.MaxLasts)
                    this.lastMessages.RemoveAt(this.lastMessages.Count - 1);

                this.topSupports.Sort((a, b) =>
                {
                    int res = a.Volume.CompareTo(b.Volume);
                    if (res == 0)
                        res = a.Time.CompareTo(b.Time);
                    return -res;
                });
                while (this.topSupports.Count > MainSettings.Current.Remote.Support.MaxTops)
                    this.topSupports.RemoveAt(this.topSupports.Count - 1);
                if (this.topSupports.Count > 0)
                {
                    this.topContainer.Update(this.topSupports.First);
                    this.topContainer.Visible = true;
                }
                this.Invalidate();
            }

            private void W_Changed(object sender)
            {
                Wallet wallet = sender as Wallet;
                Application.Run(() => wallet.GetTransactions(null, 16,(ts,e)=> { }));
            }

            protected override void OnDrawBack(Graphics g)
            {
                base.OnDrawBack(g);
                if (this.topSupports.Count == 0 || !WalletsData.IsCurrentExist)
                    g.DrawTextExclude(Language.Current["multiWalletCryptocurrency"], font, clientRect.x, clientRect.y, clientRect.width, clientRect.height - this.supportButton.Height, ParentBackColor, -25, 25, ContentAlignment.Center);
            }

            private static (string name, string message) ParseMessage(string msg)
            {
                string name = null;
                string message = null;
                if (!string.IsNullOrEmpty(msg))
                {
                    int index = msg.IndexOf("\"");
                    if (index != -1)
                    {
                        if (index < msg.Length - 1)
                        {
                            int endIndex = msg.IndexOf("\"", index + 1);
                            if (endIndex == -1)
                                endIndex = msg.Length - 1;
                            message = msg.Substring(index + 1, endIndex - index - 1);
                            if (message.Length > MainSettings.Current.Remote.Support.MaxMessageLenght)
                                message = message.Substring(0, MainSettings.Current.Remote.Support.MaxMessageLenght);
                        }
                        name = msg.Substring(0, index);
                    }
                    else
                    {
                        name = msg;
                    }
                }
                if (!string.IsNullOrEmpty(name))
                {
                    if (name.Length > MainSettings.Current.Remote.Support.MaxNameLenght)
                        name = name.Substring(0, MainSettings.Current.Remote.Support.MaxNameLenght);
                }
                return (name, message);
            }

            private class TopSupportContainer : Container
            {
                public TopSupportContainer(UniqueCollection<TopSupport> topSupports, UniqueCollection<TopSupport> lastSupports)
                {
                    this.Padding.Set(10, 0, 10, 0);

                    this.nameComponent = new UrlTextComponent();
                    this.nameComponent.LinkExecuted += (s, l) => WinApi.ShellExecute(IntPtr.Zero, l);
                    this.nameComponent.Style = Theme.Get<CaptionForeTheme>();
                    this.nameComponent.Font = Theme.font10Bold;
                    this.nameComponent.Dock = DockStyle.TopCenter;
                    this.Add(this.nameComponent);

                    container = new Container();
                    container.Dock = DockStyle.TopCenter;
                    container.Inflate.width = 6;

                    MenuButton menuButton = CreateMenuButton("", "Top", "topProjectSupport", topSupports);
                    menuButton.Dock = DockStyle.Left;
                    container.Add(menuButton);

                    this.coinCurrencyLabel = new CurrencyLabel(null, null);
                    this.coinCurrencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                    this.coinCurrencyLabel.Dock = DockStyle.Right;
                    container.Add(this.coinCurrencyLabel);

                    this.currencyLabel = new CurrencyLabel(null, null);
                    this.currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                    this.currencyLabel.Dock = DockStyle.Right;
                    container.Add(this.currencyLabel);

                    menuButton = CreateMenuButton("", "Last", "lastMessages", lastSupports);
                    menuButton.Dock = DockStyle.Right;
                    container.Add(menuButton);

                    this.Add(container);

                    MainSettings.Current.General.CurrencyChanged += OnCurrencyChanged;
                }

                protected override void OnDisposed()
                {
                    MainSettings.Current.General.CurrencyChanged -= OnCurrencyChanged;
                    base.OnDisposed();
                }

                private void OnCurrencyChanged(object sender)
                {
                    if (this.support != null)
                    {
                        Application.Invoke(() =>
                        {
                            this.currencyLabel.ValueTextComponent.Text = (support.Amount * adapter.Market.LastPrice).GetTextSharps(2);
                            this.currencyLabel.CurrencyTextComponent.Text = MainSettings.Current.General.Currency.ID;
                            this.currencyLabel.Visible = support.Amount * adapter.Market.LastPrice > 0;
                            this.ClearMeasured();
                            this.Layout();
                        });
                    }
                }

                private Container container;
                private UrlTextComponent nameComponent;
                private CurrencyLabel coinCurrencyLabel;
                private CurrencyLabel currencyLabel;

                private TopSupport support;

                private WalletAdapter adapter;
                private WalletAdapter Adapter
                {
                    get => this.adapter;
                    set
                    {
                        if (this.adapter == value) return;
                        if (this.adapter != null)
                        {
                            this.adapter.Market.CoinChanged -= OnCurrencyChanged;
                            this.adapter.Market.LastPriceChanged -= OnCurrencyChanged;
                        }
                        this.adapter = value;
                        if (this.adapter != null)
                        {
                            this.adapter.Market.CoinChanged += OnCurrencyChanged;
                            this.adapter.Market.LastPriceChanged += OnCurrencyChanged;
                        }
                    }
                }

                private static MenuButton CreateMenuButton(string imageID, string textID, string toolTipID, UniqueCollection<TopSupport> supports)
                {
                    MenuButton menuButton = new MenuButton(imageID, textID);
                    menuButton.ToolTipInfo = new ToolTipInfo(menuButton.ImageID, toolTipID, null);
                    menuButton.TwoStrip = false;
                    menuButton.MenuAnimationMode = true;
                    menuButton.CreateContainer += () => new Container();
                    menuButton.InitMenu += (s, c) =>
                    {
                        c.Clear();
                        c.MaxSize.Set(500, 500);

                        Caption caption = new Caption(toolTipID);
                        caption.Dock = DockStyle.Top;
                        caption.MinHeight = 32;

                        ImageButton button = new ImageButton("close.svg");
                        button.MaxSize.Set(20, 20);
                        button.Dock = DockStyle.Right;
                        button.Executed += (s2) => (c.Form as Menu).Hide();
                        caption.Add(button);


                        c.Add(caption);

                        AnyView anyView = new AnyView();
                        anyView.Style = Theme.Get<DockViewTheme>();
                        anyView.Alignment = ContentAlignment.None;
                        anyView.Dock = DockStyle.Fill;
                        anyView.Padding.Set(10);
                        anyView.Inflate.height = 10;
                        anyView.VScrollStep = 20;
                        anyView.ScrollVisible = true;

                        foreach (TopSupport support in supports)
                            anyView.Add(new SupportItem(support, false));

                        c.Add(anyView);
                    };
                    return menuButton;
                }

                public void Update(TopSupport support)
                {
                    if (this.support == support) return;
                    this.support = support;
                    if (this.support != null)
                    {
                        this.Adapter = Controller.GetAdapter(this.support.AdapterID);
                        decimal volume = support.Amount * adapter.Market.LastPrice;
                        this.nameComponent.Text = string.IsNullOrEmpty(support.Name) ? Language.Current["incognito"] : support.Name;
                        this.coinCurrencyLabel.ValueTextComponent.Text = support.Amount.GetTextSharps(support.SignCount);
                        this.coinCurrencyLabel.CurrencyTextComponent.Text = support.Symbol;
                        this.currencyLabel.ValueTextComponent.Text = volume.GetTextSharps(2);
                        this.currencyLabel.CurrencyTextComponent.Text = MainSettings.Current.General.Currency.ID;
                        this.currencyLabel.Visible = volume > 0;
                    }
                    else
                    {
                        this.Adapter = null;
                        this.nameComponent.Text = null;
                        this.coinCurrencyLabel.ValueTextComponent.Text = null;
                        this.coinCurrencyLabel.CurrencyTextComponent.Text = null;
                        this.currencyLabel.ValueTextComponent.Text = null;
                        this.currencyLabel.CurrencyTextComponent.Text = null;
                    }
                    this.ClearMeasured();
                    this.RelayoutAll();
                }

                protected override void OnDrawBack(Graphics g)
                {
                    this.coinCurrencyLabel.ValueTextComponent.ForeColor = this.adapter.ThemeColor;
                    base.OnDrawBack(g);
                }

            }


        }

    }
}
