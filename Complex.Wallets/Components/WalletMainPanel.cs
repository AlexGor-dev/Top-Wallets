using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Trader;

namespace Complex.Wallets
{
    public class WalletMainPanel : Container
    {
        protected WalletMainPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.wallet = data["wallet"] as Wallet;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["wallet"] = this.wallet;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletMainPanel(Wallet wallet)
        {
            this.wallet = wallet;
            this.MinHeight = 318;
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.adapter = wallet.Adapter;

            topContainer = new Container();
            topContainer.MinHeight = 40;
            topContainer.Padding.Set(10, 10, 10, 4);
            topContainer.Inflate.Set(4, 0);

            topContainer.Dock = DockStyle.Top;
            topContainer.Style = Theme.Get<MapBackTheme>();
            CaptionStyle style = Theme.Get<CaptionStyle>();

            TextComponent caption = null;

            caption = new TextComponent("Symbol:");
            caption.Dock = DockStyle.Left;
            caption.Style = style;
            topContainer.Add(caption);

            TextComponent textComponent = new TextComponent(this.wallet.Symbol);
            textComponent.MaxWidth = 100;
            textComponent.Dock = DockStyle.Left;
            topContainer.Add(textComponent);

            topContainer.Add(new Separator(DockStyle.Left, 4));

            if (!string.IsNullOrEmpty(this.wallet.Version))
            {
                caption = new TextComponent("Wallet:");
                caption.Dock = DockStyle.Left;
                caption.Style = style;
                topContainer.Add(caption);


                typeLabel = new TextComponent(this.wallet.Version);
                typeLabel.Dock = DockStyle.Left;
                topContainer.Add(typeLabel);

                topContainer.Add(new Separator(DockStyle.Left, 4));
            }


            caption = new TextComponent("State:");
            caption.Dock = DockStyle.Left;
            caption.Style = style;
            topContainer.Add(caption);

            stateLabel = new TextComponent(this.wallet.State.ToString());
            stateLabel.Dock = DockStyle.Left;
            topContainer.Add(stateLabel);

            topContainer.Add(new Separator(DockStyle.Left, 4));

            caption = new TextComponent("Last:");
            caption.Dock = DockStyle.Left;
            caption.Style = style;
            topContainer.Add(caption);

            string text = null;
            if (wallet.IsEmpty)
                text = Language.Current["noChanges"];
            else
                text = this.wallet.LastActivityTime.ToLocalLongDateTimeString();
            timeComponent = new TextComponent(text);
            timeComponent.Dock = DockStyle.Left;
            topContainer.Add(timeComponent);

            //topContainer.Add(new Separator(DockStyle.Left, 4));

            if (wallet.IsMain && wallet.IsSupportExport)
            {
                ImageNameButton mbutton = new ImageNameButton("words", "secret.svg");
                mbutton.ToolTipInfo = new ToolTipInfo(mbutton.ImageID, "viewWalletWords", null);
                mbutton.Dock = DockStyle.Right;
                mbutton.Executed += (s) =>
                {
                    Timer.Delay(100, () => new ExportWordsForm(this.Wallet).Show(s as ImageNameButton, MenuAlignment.BottomRight));
                };
                topContainer.Add(mbutton);

                mbutton = new ImageNameButton("export", "export.svg");
                mbutton.ToolTipInfo = new ToolTipInfo(mbutton.ImageID, "exportWalletToFile", null);
                mbutton.Dock = DockStyle.Right;
                mbutton.Executed += (s) =>
                {
                    Timer.Delay(100, () => new ExportToFileForm(this.Wallet).Show(s as ImageNameButton, MenuAlignment.BottomRight));
                };
                topContainer.Add(mbutton);
            }

            if (!wallet.IsMain)
            {
                this.multiWallet = this.Wallet as IMultiWallet;
                if (this.multiWallet == null && this.Wallet is IToken t)
                    this.multiWallet = t.Parent as IMultiWallet;

                if (this.multiWallet != null && multiWallet.Wallets.Count > 0)
                {
                    foreach (Wallet w in multiWallet.Wallets)
                    {
                        CoinMarket m = w.Market;
                    }
                    this.menuButton = new MenuButton(this.Wallet.ImageID, this.Wallet.Symbol);
                    this.menuButton.Label.ImageComponent.MaxSize.Set(20, 20);
                    this.menuButton.TwoStrip = false;
                    this.menuButton.Dock = DockStyle.Right;
                    this.menuButton.MenuAlignment = MenuAlignment.BottomRight;
                    this.menuButton.InitMenu += (s, c) =>
                    {
                        c.Clear();
                        FullChessView chessView = new FullChessView();
                        chessView.ColumnsCount = 1 + Math.Max(1, multiWallet.Wallets.Count / 2);
                        chessView.ColumnWidth = 150;
                        chessView.RowHeight = 100;
                        chessView.Vertical = true;
                        chessView.FixedSize = true;
                        chessView.Padding.Set(6);
                        chessView.Inflate.Set(6);
                        this.AddToken(chessView, multiWallet as Wallet);

                        foreach (Wallet w in multiWallet.Wallets)
                            this.AddToken(chessView, w);

                        c.Add(chessView);
                    };
                    topContainer.Add(this.menuButton);
                }
            }

            this.Add(topContainer);

            this.topDummy = new Dummy(DockStyle.Top, 0, 10);
            this.Add(this.topDummy);

            this.adapterLabel = new AdapterLabel(this.adapter, this.wallet);
            this.AdapterLabel.Dock = DockStyle.TopCenter;
            this.Add(this.adapterLabel);

            this.Add(new Dummy(DockStyle.Top, 0, 6));

            this.nameCaption = new Caption(this.wallet.Name);
            this.nameCaption.TextChanged += (s) => { this.nameCaption.Invalidate(); };
            this.nameCaption.DrawShadow = true;
            this.nameCaption.RoundBack = true;
            this.nameCaption.Dock = DockStyle.TopCenter;
            this.nameCaption.MaxWidth = 300;
            this.nameCaption.MinWidth = 100;

            ImageButton button = new ImageButton("copyAddress.svg");
            button.MaxHeight = 20;
            button.ToolTipInfo = new ToolTipInfo(button.Image, "copyWalletAddress", null);
            button.Dock = DockStyle.Right;
            button.Executed += (s) =>
            {
                Clipboard.SetText(Wallet.Address);
                MessageView.Show(Language.Current["address"] + " " + Wallet.Address + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            };
            this.nameCaption.Add(button);

            CheckedImageButton imageButton = new CheckedImageButton("qrcode.svg", false);
            imageButton.MaxHeight = 20;
            imageButton.DrawBorder = false;
            imageButton.ToolTipInfo = new ToolTipInfo(imageButton.Image, "qrcode", null);
            imageButton.Dock = DockStyle.Right;
            imageButton.CheckedChanged += (s) =>
            {
                CheckedImageButton bt = s as CheckedImageButton;
                if (bt.Checked)
                {
                    ReceiveMenu menu = new ReceiveMenu(this.Wallet);
                    menu.ActionComponent = bt;
                    menu.Hided += delegate (object s2)
                    {
                        menu = s2 as ReceiveMenu;
                        bt.Checked = false;
                        menu.Dispose();
                    };
                    menu.Show(bt, MenuAlignment.BottomRight, (p) =>
                    {
                    });
                }
            };
            this.nameCaption.Add(imageButton);

            this.Add(this.nameCaption);

            this.Add(new Dummy(DockStyle.Top, 0, 6));

            this.lastTimeName = new TextLocalizeComponent("lastChanges");
            this.lastTimeName.Font = Theme.font10Bold;
            this.lastTimeName.AppendRightText = ":";
            this.Add(this.lastTimeName);

            this.lastTimeLabel = new LastTimeLabel(this.wallet);
            this.Add(this.lastTimeLabel);

            this.balanceLabel = new BalanceLabel(this.wallet);
            this.balanceLabel.Dock = DockStyle.Top;
            this.Add(this.balanceLabel);

            this.balanceChangedLabel = new BalanceChangedLabel(this.wallet);
            this.Add(this.balanceChangedLabel);

            this.transactionLabel = new TextLocalizeComponent("transactions");
            this.transactionLabel.Font = Theme.font10Bold;
            this.transactionLabel.AppendRightText = ":";
            this.Add(this.transactionLabel);

            this.intervalLabel = new TransactionsIntervalLabel(this.wallet);
            this.intervalLabel.IntervalChanged += (s) =>
            {
                this.transactionsChanged.Update(this.intervalLabel.Interval);
                Adapter_EndUpdated(null);
            };
            this.Add(this.intervalLabel);

            this.transactionsChanged = new TransactionsChangedLabel(this.wallet);
            this.Add(this.transactionsChanged);

            if (wallet.IsMain)
            {
                this.leftButton = wallet.CreateMainLeftButton();
                if (this.leftButton != null)
                    this.Add(this.leftButton);

                this.rightButton = wallet.CreateMainRightButton();
                if (this.rightButton != null)
                    this.Add(this.rightButton);
            }


            this.componentsAnimator = new LayoutComponentsAnimator(this);

            this.wallet.Changed += Wallet_Changed;
            this.wallet.TransactionsLoaded += Wallet_TransactionLoaded;
            this.wallet.Adapter.EndUpdated += Adapter_EndUpdated;
            this.wallet.Adapter.Connected += Adapter_Connected;
            this.wallet.Adapter.Disconnected += Adapter_Disconnected;
        }
        protected override void OnDisposed()
        {
            this.wallet.Adapter.Connected -= Adapter_Connected;
            this.wallet.Adapter.Disconnected -= Adapter_Disconnected;
            this.wallet.Adapter.EndUpdated -= Adapter_EndUpdated;
            this.wallet.TransactionsLoaded -= Wallet_TransactionLoaded;
            this.wallet.Changed -= Wallet_Changed;
            base.OnDisposed();
        }

        private void Adapter_Connected(object sender)
        {
            this.OnConnectionChanged();
        }

        private void Adapter_Disconnected(object sender)
        {
            this.OnConnectionChanged();
        }

        private void Adapter_EndUpdated(object sender)
        {
            Application.Invoke(() =>
            {
                if (this.intervalLabel.Update() || this.lastTimeLabel.Update())
                {
                    this.SetAnimations(false);
                    this.componentsAnimator.End();
                }
            });
        }

        private void Wallet_TransactionLoaded(object sender)
        {
            Application.Invoke(() =>
            {
                if (!this.componentsAnimator.IsRunning)
                    this.SetAnimations(true);
                this.lastTimeLabel.Update();
                this.balanceChangedLabel.Update();
                this.intervalLabel.Update();
                this.transactionsChanged.Update(this.intervalLabel.Interval);

                if (!this.componentsAnimator.IsRunning)
                {
                    this.SetAnimations(false);
                    this.componentsAnimator.Start();
                }
            });
        }

        protected virtual void OnWalletChanged()
        {

        }

        private void Wallet_Changed(object sender)
        {
            Application.Invoke(() =>
            {
                this.OnWalletChanged();
                this.UpdateButtonsState();
                if (!this.componentsAnimator.IsRunning)
                    this.SetAnimations(true);
                if(this.typeLabel != null)
                    this.typeLabel.Text = wallet.Version;
                this.stateLabel.Text = wallet.State.ToString();
                this.timeComponent.Text = this.wallet.LastActivityTime.ToLocalLongDateTimeString();
                this.nameCaption.Text = this.wallet.Name;
                this.lastTimeLabel.Update();
                this.balanceChangedLabel.Update();
                this.transactionsChanged.Update(this.intervalLabel.Interval);
                this.intervalLabel.Update();
                this.balanceLabel.Update();
                this.topContainer.Layout();
                this.topContainer.Invalidate();
                if (!this.componentsAnimator.IsRunning)
                {
                    this.SetAnimations(false);
                    this.componentsAnimator.Start();
                }
                this.Invalidate();
            });
        }

        private Wallet wallet;
        public Wallet Wallet => wallet;

        private WalletAdapter adapter;
        public WalletAdapter Adapter => adapter;

        protected Container topContainer;
        private Dummy topDummy;
        private ColorButton leftButton;
        private ColorButton rightButton;
        private MenuButton menuButton;
        private IMultiWallet multiWallet;

        private TextComponent typeLabel;
        private TextComponent stateLabel;
        private TextComponent timeComponent;

        private Caption nameCaption;
        public Caption NameCaption => nameCaption;

        private AdapterLabel adapterLabel;
        public AdapterLabel AdapterLabel => adapterLabel;

        private BalanceLabel balanceLabel;
        public BalanceLabel BalanceLabel => balanceLabel;

        private BalanceChangedLabel balanceChangedLabel;
        private LastTimeLabel lastTimeLabel;
        private TextLocalizeComponent lastTimeName;

        private TransactionsChangedLabel transactionsChanged;
        private TransactionsIntervalLabel intervalLabel;
        private TextLocalizeComponent transactionLabel;

        private LayoutComponentsAnimator componentsAnimator;

        protected readonly Rect clientRect = new Rect();

        private void UpdateButtonsState()
        {
            if (this.leftButton != null)
                this.leftButton.Enabled = Wallet.Adapter.IsConnected && Wallet.State != WalletState.None;
            if (this.rightButton != null && !this.rightButton.Enabled)
                this.rightButton.Enabled = Wallet.Adapter.IsConnected && Wallet.State != WalletState.None;
        }

        protected virtual void OnConnectionChanged()
        {
            this.UpdateButtonsState();
        }

        protected override void OnCreated()
        {
            this.balanceChangedLabel.Update();
            this.transactionsChanged.Update(this.intervalLabel.Interval);
            this.intervalLabel.Update();
            this.lastTimeLabel.Update();
            base.OnCreated();
        }

        protected override void OnSizeChanged()
        {
            GetClientRectangle(clientRect);
            base.OnSizeChanged();
        }

        protected override void OnLayout()
        {
            base.OnLayout();
            this.SetAnimations(false);
            this.componentsAnimator.End();
            if (this.leftButton != null)
            {
                this.leftButton.Measure();
                this.leftButton.SetBounds(clientRect.x + (this.AdapterLabel.Left - this.leftButton.MeasuredWidth) / 2, this.AdapterLabel.Top - 20 + (this.AdapterLabel.Height - this.leftButton.MeasuredHeight) / 2, this.leftButton.MeasuredWidth, this.leftButton.MeasuredHeight);
            }
            if (this.rightButton != null)
            {
                this.rightButton.Measure();
                this.rightButton.SetBounds(this.AdapterLabel.Right + (clientRect.right - this.AdapterLabel.Right - this.rightButton.MeasuredWidth) / 2, this.AdapterLabel.Top - 20 + (this.AdapterLabel.Height - this.rightButton.MeasuredHeight) / 2, this.rightButton.MeasuredWidth, this.rightButton.MeasuredHeight);
            }

        }

        private void SetAnimations(bool start)
        {
            this.balanceChangedLabel.Measure();
            this.lastTimeName.Measure();
            this.lastTimeLabel.Measure();
            this.transactionsChanged.Measure();
            this.intervalLabel.Measure();

            float width = (this.balanceLabel.Width - this.balanceLabel.MeasuredWidth) / 2;
            float x = this.balanceLabel.Left + (width - this.balanceChangedLabel.MeasuredWidth) / 2;

            Rect rect = this.componentsAnimator.Set(this.balanceChangedLabel, start, x, this.balanceLabel.Bottom - this.balanceChangedLabel.MeasuredHeight,
                this.balanceChangedLabel.MeasuredWidth, this.balanceChangedLabel.MeasuredHeight);

            this.componentsAnimator.Set(this.lastTimeLabel, start, rect.x + (rect.width - this.lastTimeLabel.MeasuredWidth) / 2,
                rect.y - this.lastTimeLabel.MeasuredHeight, this.lastTimeLabel.MeasuredWidth, this.lastTimeLabel.MeasuredHeight);
            this.componentsAnimator.Set(this.lastTimeName, start, rect.x + (rect.width - this.lastTimeName.MeasuredWidth) / 2,
                rect.y - this.lastTimeName.MeasuredHeight - this.lastTimeLabel.MeasuredHeight - 4, this.lastTimeName.MeasuredWidth, this.lastTimeName.MeasuredHeight);



            x = this.balanceLabel.Right - width + (width - this.transactionsChanged.MeasuredWidth) / 2;
            rect = this.componentsAnimator.Set(this.transactionsChanged, start, x, this.balanceLabel.Bottom - this.transactionsChanged.MeasuredHeight,
                                        this.transactionsChanged.MeasuredWidth, this.transactionsChanged.MeasuredHeight);

            this.componentsAnimator.Set(this.intervalLabel, start, rect.x + (rect.width - this.intervalLabel.MeasuredWidth) / 2,
                                    rect.y - this.intervalLabel.MeasuredHeight, this.intervalLabel.MeasuredWidth, this.intervalLabel.MeasuredHeight);
            this.componentsAnimator.Set(this.transactionLabel, start, rect.x + (rect.width - this.transactionLabel.MeasuredWidth) / 2,
                        rect.y - this.intervalLabel.MeasuredHeight - this.transactionLabel.MeasuredHeight - 4, this.transactionLabel.MeasuredWidth, this.transactionLabel.MeasuredHeight);

        }

        protected override void OnDrawBack(Graphics g)
        {
            if (this.leftButton != null)
                this.leftButton.BoxColor = Wallet.ThemeColor;
            if (this.rightButton != null)
                this.rightButton.BoxColor = Wallet.ThemeColor;
            base.OnDrawBack(g);
            g.FillGradient(0, this.topDummy.Top, Width, Height - this.topDummy.Top, Theme.back3, Color.Offset(Theme.back3, -10), ContentAlignment.Top | ContentAlignment.Bottom, 10);

        }

        private void AddToken(FullChessView chessView, Wallet wallet)
        {
            TokenButton button = new TokenButton(wallet, wallet == this.Wallet);
            button.Executed += (s) =>
            {
                TokenButton tb = s as TokenButton;
                Controller.ExploreWallet(tb.wallet);
                //this.Wallet = tb.wallet;
                tb.Form.Close();
            };
            chessView.Add(button);
        }

        private class TokenButton : CheckedButton
        {
            public TokenButton(Wallet wallet, bool isChecked)
                : base(wallet.ImageID, wallet is IToken t ? t.Name : wallet.Symbol, isChecked)
            {
                this.wallet = wallet;
                this.Padding.Set(6);
                this.Inflate.Set(0, 6);
                this.IsFixed = isChecked;
                //this.ImageComponent.MaxSize.Set(26, 26);
                this.ImageComponent.MaxHeight = 26;
                this.TextComponent.Font = Theme.font11Bold;
                this.TextAlignment = Drawing.ContentAlignment.Center;

                this.ImageComponent.Dock = DockStyle.Top;
                this.TextComponent.Dock = DockStyle.Top;

                CurrencyLabel curLabel = new CurrencyLabel(this.wallet.Balance.ToKMB(3), this.wallet.Symbol);
                curLabel.ValueTextComponent.Font = Theme.font9Bold;
                curLabel.Dock = DockStyle.Top;
                curLabel.CurrencyTextComponent.ForeColor = wallet.ThemeColor;
                this.Add(curLabel);

                curLabel = new CurrencyLabel(this.wallet.Volume.GetTextSharps(3), MainSettings.Current.General.Currency.ID);
                curLabel.ValueTextComponent.Font = Theme.font9Bold;
                curLabel.ValueTextComponent.AppendLeftText = "≈";
                curLabel.Dock = DockStyle.Bottom;
                this.Add(curLabel);


            }

            public readonly Wallet wallet;
        }

    }
}
