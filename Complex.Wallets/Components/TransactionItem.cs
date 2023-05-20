//using System;
//using Complex.Controls;
//using Complex.Drawing;
//using Complex.Themes;
//using Complex.Animations;
//using Complex.Trader;

//namespace Complex.Wallets
//{
//    public class TransactionItem : Container, IFocusedComponent, IEndAnimation
//    {
//        public TransactionItem(Wallet wallet, ITransaction transaction, GridWaitEffect waitEffect)
//        {
//            this.wallet = wallet;
//            this.transaction = transaction;
//            this.waitEffect = waitEffect;
//            //this.MinHeight = 80;
//            this.Padding.Set(8, 4, 8, 4);
//            //this.Inflate.Set(10, 0);

//            this.expandedPanel = new TransactionExpandedPanel(this.wallet, this.transaction);
//            this.expandedPanel.Dock = DockStyle.Fill;
//            this.Add(this.expandedPanel);

//            main = new Container();
//            main.Dock = DockStyle.Top;

//            if (!string.IsNullOrEmpty(transaction.Name))
//            {
//                Caption nameCaption = new Caption(transaction.Name);
//                nameCaption.Padding.Set(30, 0, 30, 0);
//                nameCaption.Dock = DockStyle.Top;
//                main.Add(nameCaption);

//                main.Add(new Dummy(DockStyle.Top, 0, 4));
//            }

//            top = new Container();
//            top.Dock = DockStyle.Top;

//            this.address = transaction.Address;
//            Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, this.address);
//            LargeDescButtonLabel dlabel = new LargeDescButtonLabel(transaction.IsOut ? "out_transaction.svg" : "in_transaction.svg",
//                transaction.IsOut ? "sended" : "received",
//                (transaction.IsOut ? Language.Current["to"] : Language.Current["from"]) + ": " + (wt != null ? wt.Name : Controller.GetKnownAddress(wallet.Adapter, address)));
//            dlabel.Execute += (s) =>
//            {
//                if(this.waitEffect != null)
//                    Controller.ShowAnyWallet(wallet.Adapter, wallet.Symbol, this.address, () => this.waitEffect.Start(), () => this.waitEffect.Stop());
//            };
//            dlabel.RightClick += (s) =>
//            {
//                this.wallet.CreateTransactionAddressMenu(s as Component, this.address, (m) =>
//                {
//                    if (m != null)
//                        Application.Invoke(() => m.Show(s as Component, MenuAlignment.Bottom));
//                });
//            };
//            dlabel.Dock = DockStyle.Left;
//            dlabel.MaxWidth = 250;
//            dlabel.MinWidth = 250;
//            dlabel.Inflate.Set(10, 0);
//            //label.ImageComponent.ImageColor = transaction.IsOut ? buySell.sellColor : buySell.buyColor;
//            dlabel.DescComponent.ForeColor = wallet.ThemeColor;
//            top.Add(dlabel);


//            if (!string.IsNullOrEmpty(transaction.Type))
//            {
//                top.Add(new Dummy(DockStyle.Left, 10, 0));

//                typeComponent = new TextLocalizeComponent(transaction.Type);
//                typeComponent.Dock = DockStyle.Left;
//                typeComponent.MaxWidth = 250;
//                typeComponent.MaxHeight = 20;
//                typeComponent.Padding.Set(4, 0, 4, 0);
//                top.Add(typeComponent);
//            }

//            //LargeCurrencyLabel label = new LargeCurrencyLabel((transaction.IsOut ? "-" : "+") + transaction.Amount.GetTextSharps(8), wallet.Symbol, transaction.Fee > 0 ? Language.Current["fee"] + ": " + transaction.Fee.GetTextSharps(8) : null, transaction.Fee > 0 ? wallet.Symbol : null);
//            //label.TextCurrencyLabel.Alignment = ContentAlignment.Right;
//            //label.DescCurrencyLabel.Alignment = ContentAlignment.Right;
//            //label.Dock = DockStyle.Right;
//            //label.TextCurrencyLabel.ValueTextComponent.ForeColor = transaction.IsOut ? buySell.sellColor : buySell.buyColor;
//            //label.DescCurrencyLabel.ValueTextComponent.Style = Theme.Get<ForeTheme>();
//            ////label.DescCurrencyLabel.Enabled = false;
//            //top.Add(label);

//            CurrencyLabel label = new CurrencyLabel((transaction.IsOut ? "-" : "+") + transaction.Amount.GetTextSharps(8), transaction.Amount.Symbol);
//            label.Alignment = ContentAlignment.Right;
//            label.Dock = DockStyle.Right;
//            label.ValueTextComponent.ForeColor = transaction.IsOut ? buySell.sellColor : buySell.buyColor;
//            top.Add(label);

//            main.Add(top);

//            bot = new Container();
//            bot.Inflate.width = 10;
//            bot.Padding.Set(6, 0, 0, 0);
//            bot.Dock = DockStyle.Top;

//            ExpandButton button = new ExpandButton(false);
//            button.MaxSize.Set(26, 26);
//            button.Dock = DockStyle.Left;
//            button.CheckedChanged += (s) =>
//            {
//                this.Expanded = (s as ExpandButton).Checked;
//            };
//            bot.Add(button);


//            if (!string.IsNullOrEmpty(transaction.Message))
//            {
//                messgComponent = new TextComponent(transaction.Message);
//                messgComponent.MaxWidth = 250;
//                messgComponent.MaxHeight = 20;
//                messgComponent.Padding.Set(4, 0, 4, 0);
//                messgComponent.Dock = DockStyle.Left;
//                bot.Add(messgComponent);
//            }

//            timeComponent = new TextComponent(GetTimeText());
//            timeComponent.TextChanged += (s) =>
//            {
//                (s as TextComponent).Measured = false;
//                bot.Layout();
//            };

//            timeComponent.Padding.Set(4, 0, 4, 0);
//            timeComponent.Dock = DockStyle.Right;
//            timeComponent.Alignment = ContentAlignment.Right;
//            timeComponent.MaxHeight = 20;
//            bot.Add(timeComponent);

//            label = new CurrencyLabel(transaction.Fee > 0 ? Language.Current["fee"] + ": " + transaction.Fee.GetTextSharps(8) : null, transaction.Fee > 0 ? transaction.Fee.Symbol : null);
//            label.Alignment = ContentAlignment.Right;
//            label.Dock = DockStyle.Right;
//            label.ValueTextComponent.Style = Theme.Get<ForeTheme>();
//            bot.Add(label);

//            main.Add(bot);

//            this.Add(main);



//            expandAnimator = new Animator(this, 1, 255);

//        }

//        private GridWaitEffect waitEffect;

//        private Wallet wallet;
//        public Wallet Wallet => wallet;

//        private ITransaction transaction;
//        public ITransaction Transaction => transaction;

//        private string address;
//        private Animator expandAnimator;

//        private readonly Rect expandRect = new Rect();
//        protected static readonly BuySellTheme buySell = Theme.Get<BuySellTheme>();
//        private TextComponent timeComponent;
//        private TextComponent messgComponent;
//        private TextLocalizeComponent typeComponent;
//        //private Container top;
//        protected readonly Container top;
//        protected readonly Container bot;
//        protected readonly Container main;
//        private TransactionExpandedPanel expandedPanel;

//        public InsertedAnyView ListView => this.Parent as InsertedAnyView;

//        private bool expanded = false;
//        public bool Expanded
//        {
//            get => expanded;
//            set
//            {
//                if (expanded == value) return;
//                expanded = value;
//                expandAnimator.Start(value ? 1 : -1);
//            }
//        }

//        protected override void OnKeyDown(KeyEvent e)
//        {
//            switch (e.Key)
//            {
//                case Key.Enter:
//                    this.Expanded = !this.Expanded;
//                    break;
//            }

//            base.OnKeyDown(e);
//        }
//        protected override void OnMeasure(float widthMeasure, float heightMeasure)
//        {
//            base.OnMeasure(widthMeasure, heightMeasure);
//            expandedPanel.Measure();
//            SetMeasured(0, Padding.vertical + main.MeasuredHeight + expandAnimator.GetValue(expandedPanel.MeasuredHeight));
//        }

//        protected override void OnSizeChanged()
//        {
//            expandRect.Set(expandedPanel.Left + expandedPanel.Padding.left, expandedPanel.Top
//                , expandedPanel.Width - expandedPanel.Padding.horizontal, expandedPanel.Height);
//            base.OnSizeChanged();
//        }

//        private string GetTimeText()
//        {
//            if (MainSettings.Current.General.RelativeTimeTransactions)
//                return (DateTime.UtcNow - transaction.Time).ToYMD() + " " + Language.Current["ago"];
//            return transaction.Time.ToLocalLongDateTimeString();
//        }

//        protected override void OnDrawBack(Graphics g)
//        {
//            timeComponent.Text = GetTimeText();

//            g.Smoosh(() =>
//            {
//                int color = Theme.unselectedItemBackColor;
//                int back2 = Theme.selectedItemBackColor;
//                if (this.Selected)
//                {
//                    color = Theme.selectedItemBackColor;
//                    back2 = Theme.unselectedItemBackColor;
//                }
//                g.FillRoundRect(0, 0, main.Width + Padding.horizontal, main.Height + Padding.vertical, 10, color);
//                g.FillRoundRect(main.Left + bot.Left + timeComponent.Left, main.Top + bot.Top + timeComponent.Top, timeComponent.Width, timeComponent.Height, timeComponent.Height/ 2, back2);
//                if (messgComponent != null)
//                    g.FillRoundRect(main.Left + bot.Left + messgComponent.Left, main.Top + bot.Top + messgComponent.Top, messgComponent.Width, messgComponent.Height, messgComponent.Height / 2, back2);
//                if(typeComponent != null)
//                    g.FillRoundRect(main.Left + top.Left + typeComponent.Left, main.Top + top.Top + typeComponent.Top, typeComponent.Width, typeComponent.Height, typeComponent.Height / 2, back2);
//            });
//        }

//        void IFocusedComponent.OnFocusedChanged()
//        {
//        }

//        void IAnimation.OnAnimation(Animator animator, float value)
//        {
//            this.expandedPanel.Alpha = animator.GetValue(255);
//            Measured = false;
//            this.ListView.Relayout();
//            this.ListView.Parent.Invalidate();
//        }

//        void IEndAnimation.OnEndAnimation(Animator animator, float value)
//        {
//            this.ListView.EnsureVisibleAnimation(this);
//        }

//    }
//}
