using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Drawing;
using Complex.Themes;
using Complex.Navigation;

namespace Complex.Wallets
{
    public class TokensListPanel : WalletBasePanel
    {
        protected TokensListPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.listView = data["listView"] as NavigationListView;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["listView"] = this.listView;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TokensListPanel(Wallet wallet)
            : base(wallet)
        {
            this.listView = new NavigationListView();
            this.listView.Dock = DockStyle.Fill;


            this.Init();
        }

        private void Init()
        {
            this.listView.MinRowHeight = 46;
            this.listView.ImageSize = 40;
            this.listView.BeginLoad += (s, a) =>
            {
                this.waitEffect.Start();
            };
            this.listView.EndLoad += (s) =>
            {
                this.waitEffect.Stop();
            };
            this.Add(this.listView);

            this.waitEffect = new GridWaitEffect(this);
            this.adapter = new TokensAdapter(this.Wallet, this.waitEffect);

        }


        protected override void OnDisposed()
        {
            this.adapter.Dispose();
            base.OnDisposed();
        }

        private NavigationListView listView;
        private TokensAdapter adapter;
        private GridWaitEffect waitEffect;

        protected override void OnCreated()
        {

            if(this.Wallet.Adapter.IsConnected)
                this.listView.Load(this.adapter, false);
            base.OnCreated();
        }

        protected override void OnAdapterEndUpdated()
        {
            if (this.Wallet.Adapter.IsConnected)
            {
                Wallet.GetTokens((ts, e) =>
                {
                    if (e != null)
                        MessageView.Show(e);
                    if (ts != null && ts.Length > 0)
                    {
                        //UniqueCollection<ITokenInfo> items = new UniqueCollection<ITokenInfo>();
                        foreach (ITokenInfo token in ts)
                        {
                            TokenItem old = this.adapter.items[token.ID];
                            if (old == null)
                                this.adapter.Add(token);
                            else
                            {
                                ITokenInfo info = old.token;
                                if (info.Name != token.Name || info.Balance.Numerator != token.Balance.Numerator || info.Balance.Denominator != token.Balance.Denominator || info.Balance.Symbol != token.Balance.Symbol)
                                {
                                    old.token = token;
                                    this.adapter.Change(token);
                                }
                            }
                        }
                    }
                });
            }
            base.OnAdapterEndUpdated();
        }

        protected override void OnConnectionChanged()
        {
            if (this.Wallet.Adapter.IsConnected)
                this.listView.Load(this.adapter, false);
            base.OnConnectionChanged();
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (this.waitEffect.IsRunning)
                this.waitEffect.Draw(g);
        }

        private class TokensAdapter : NavigationAdapter
        {
            public TokensAdapter(Wallet wallet, GridWaitEffect waitEffect)
                : base(null, wallet.ID)
            {
                this.wallet = wallet;
                this.waitEffect = waitEffect;
                this.ColumnsID = "Tokens";
            }

            private Wallet wallet;
            private GridWaitEffect waitEffect;
            public readonly UniqueCollection<TokenItem> items = new UniqueCollection<TokenItem>();

            public override ColumnInfoCollection CreateColumns()
            {
                ColumnInfoCollection columns = new ColumnInfoCollection();
                columns.Add("Jetton", 250);
                columns.Add("address", 300);
                columns.Add("balance", 200);
                columns.SortedColumn = "Jetton";
                columns.SortDirection = SortDirection.Ascending;
                return columns;
            }

            public override int Compare(string column, INavigationItem a, INavigationItem b)
            {
                ITokenInfo ad = (a as TokenItem).token;
                ITokenInfo bd = (b as TokenItem).token;
                int res = 0;
                switch (column)
                {
                    case "Jetton":
                        res = StringHelper.CompareNumbers(ad.Name, bd.Name);
                        if (res == 0)
                            res = ad.Balance.CompareTo(bd.Balance);
                        if (res == 0)
                            goto default;
                        break;
                    case "balance":
                        res = ad.Balance.CompareTo(bd.Balance);
                        if (res == 0)
                            goto default;
                        break;
                    default:
                        res = StringHelper.CompareNumbers(ad.Address, bd.Address);
                        break;
                }
                return res;
            }

            public override void LoadItems(INavigationView view, ItemsLoadHandler handler, RetParamHandler<bool> cancelHanler)
            {
                Application.Run(() =>
                {
                    wallet.GetTokens((ts, e) =>
                    {
                        if (e != null)
                            MessageView.Show(e);
                        Array<INavigationItem> items = new Array<INavigationItem>();
                        if (ts != null)
                        {
                            foreach (ITokenInfo token in ts)
                            {
                                TokenItem item = new TokenItem(this.wallet, token, this.waitEffect);
                                items.Add(item);
                                this.items.Add(item);
                            }
                        }
                        handler(this, items);

                    });
                });

            }

            public override void Subscribe()
            {
            }

            public override void Unsubscribe()
            {
            }

            public void Add(ITokenInfo token)
            {
                TokenItem item = new TokenItem(this.wallet, token, this.waitEffect);
                this.OnAdded(item);
                this.items.Add(item);
            }

            public void Remove(ITokenInfo token)
            {
                this.OnRemoved(token.ID);
                this.items.Remove(token.ID);
            }

            public void Change(ITokenInfo token)
            {
                this.OnChanged(token.Address);
            }

            private void Tokens_Cleared(object sender)
            {
                this.OnCleared();
            }

            public override void ShowMenu(INavigationView view, float x, float y)
            {
                //INavigationItem[] selItems = view.SelectedItems;
                //if (selItems.Length == 0 && view.FocusedItem != null)
                //    selItems = new INavigationItem[] { view.FocusedItem };
                //if (selItems.Length > 0)
                //{
                //    MenuStrip menu = new MenuStrip();
                //    MenuStripButton button = new MenuStripButton("copyOperation.svg", "copyMessage");
                //    button.Executed += (s) =>
                //    {
                //        string text = "";
                //        foreach (MessageItem item in selItems)
                //            text += Language.Current[item.data.Message] + Environment.NewLine;
                //        Clipboard.SetText(text);
                //    };
                //    menu.Add(button);
                //    menu.Add(ViewCopyMode.Columns).Executed += (object s) => view.Copy(selItems, (ViewCopyMode)(s as MenuStripButton).Tag);

                //    menu.Add(new MenuStripSeparator());

                //    button = new MenuStripButton("deleteOperation.svg", "deleteCmd");
                //    button.Executed += (s) =>
                //    {
                //        foreach (MessageItem item in selItems)
                //            MessageView.Messages.Remove(item.data);
                //    };
                //    menu.Add(button);

                //    button = new MenuStripButton("clearAll");
                //    button.Executed += (s) =>
                //    {
                //        MessageView.Messages.Clear();
                //    };
                //    menu.Add(button);

                //    menu.Show(x, y);
                //}
            }
        }

        private class TokenItem : NavigationItem
        {
            public TokenItem(Wallet wallet, ITokenInfo token, GridWaitEffect waitEffect)
                : base(token.ID)
            {
                this.wallet = wallet;
                this.token = token;
                this.waitEffect = waitEffect;
                token.LoadImage((img) => { });
            }

            private Wallet wallet;
            public ITokenInfo token;
            private GridWaitEffect waitEffect;

            protected override IImage GetImage()
            {
                return token.LoadImage((img) => this.Image = img);
            }

            public override Component GetSubitem(string name)
            {
                switch (name)
                {
                    case "Jetton":
                        Container container = new Container();
                        CheckedTextButton tokenButton = new CheckedTextButton(this.token.Name, false);
                        tokenButton.Dock = DockStyle.Left;
                        tokenButton.DrawBorder = false;
                        tokenButton.Padding.Set(4);
                        tokenButton.Font = Theme.font10Bold;
                        tokenButton.MaxHeight = 28;
                        tokenButton.CheckedChanged += (s) =>
                        {
                            CheckedTextButton bt = s as CheckedTextButton;
                            if (bt.Checked)
                            {
                                this.wallet.CreateTokenInfoMenu(this.token, (menu) =>
                                {
                                    if (menu != null)
                                    {
                                        menu.Hided += (s2) => bt.Checked = false;
                                        menu.Show(bt, MenuAlignment.TopLeft);
                                    }
                                    else
                                    {
                                        bt.Checked = false;
                                    }
                                });
                            }
                        };
                        tokenButton.RightClick += (s) =>
                        {
                            this.wallet.CreateTokenInfoAddressMenu(token, (m) =>
                            {
                                if (m != null)
                                    Application.Invoke(() => m.Show(s as Component, MenuAlignment.Bottom));
                            });
                        };
                        container.Add(tokenButton);
                        return container;
                    case "address":
                        Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, token.Address);
                        TextButton addressButton = new TextButton(wt != null ? wt.Name : Controller.GetKnownAddress(wallet.Adapter, token.Address));
                        addressButton.DrawBorder = false;
                        addressButton.Padding.Set(4);
                        addressButton.Font = Theme.font10Bold;
                        addressButton.MaxHeight = 28;
                        addressButton.Executed += (s) =>
                        {
                            if (this.waitEffect != null)
                                Controller.ShowAnyWallet(wallet.Adapter, wallet.Symbol, token.Address, () => this.waitEffect.Start(), () => this.waitEffect.Stop());
                        };
                        addressButton.RightClick += (s) =>
                        {
                            this.wallet.CreateTransactionAddressMenu(token.Address, (m) =>
                            {
                                if (m != null)
                                    Application.Invoke(() => m.Show(s as Component, MenuAlignment.Bottom));
                            });

                        };
                        return addressButton;
                    case "balance":
                        CurrencyLabel currencyLabel = new CurrencyLabel(this.token.Balance.GetTextSharps(9), this.token.Balance.Symbol.First(8));
                        currencyLabel.ValueTextComponent.ForeColor = token.Color;
                        currencyLabel.CurrencyTextComponent.MaxWidth = 60;
                        currencyLabel.CurrencyTextComponent.MinWidth = 60;
                        currencyLabel.Alignment = ContentAlignment.Right;
                        return currencyLabel;
                }
                return null;
            }
        }

    }
}
