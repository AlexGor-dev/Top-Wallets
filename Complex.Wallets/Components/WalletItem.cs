using System;
using System.Collections.Generic;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;
using Complex.Animations;


namespace Complex.Wallets
{
    public class WalletItem : WalletLiteItem, IEndAnimation
    {
        protected WalletItem(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.expanded = (bool)data["expanded", this.expanded];
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["expanded"] = this.expanded;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletItem(Wallet wallet)
            :base(wallet)
        {
            this.Padding.Set(4);
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.multiWallet = this.Wallet as IMultiWallet;
            this.MinHeight = 150;

            quoteLabel = new LargeLabel(null, null, null);
            quoteLabel.TextComponent.Font = Theme.font10;
            quoteLabel.TextComponent.Alignment = ContentAlignment.Right;
            quoteLabel.DescComponent.Alignment = ContentAlignment.Right;
            quoteLabel.Dock = DockStyle.Right;
            quoteLabel.MinWidth = 70;
            topContainer.Add(quoteLabel);


            this.liteChart = new LiteChart(new InstrumentData(null, Interval.H8));
            this.liteChart.IndicatorStyle = new CustomColorTrendStyle(Wallet.ThemeColor);
            this.liteChart.PriceVisible = true;
            this.liteChart.Dock = DockStyle.Fill;
            this.Add(this.liteChart);

            this.Wallet.Market.CoinChanged += Wallet_CoinChanged;
            this.Wallet.Market.QuoteChanged += Wallet_CoinQuoteChanged;

            this.instrument = this.Wallet.Market.Coin;
        }


        protected override void OnDisposed()
        {
            if (this.multiWallet != null && this.Parent is AnyView)
            {
                this.Parent.Components.Added += Wallets_Added;
                this.Parent.Components.Removed += Wallets_Removed;
            }
            this.Wallet.Market.CoinChanged -= Wallet_CoinChanged;
            this.Wallet.Market.QuoteChanged -= Wallet_CoinQuoteChanged;
            base.OnDisposed();
        }

        private void Wallets_Added(object sender, Component value)
        {
            if(value is IWalletSource ws && ws.Wallet is IToken token && token.Parent == this.Wallet)
                this.CheckExpandButtonVisible();
        }

        private void Wallets_Removed(object sender, Component value)
        {
            if (value is IWalletSource ws && ws.Wallet is IToken token && token.Parent == this.Wallet)
                this.CheckExpandButtonVisible();
        }


        private void Wallet_CoinQuoteChanged(object sender)
        {
            Application.Invoke(this.UpdateQuote);
        }

        private void Wallet_CoinChanged(object sender)
        {
            this.instrument = this.Wallet.Market.Coin;
            this.liteChart.InstrumentData.Instrument = instrument;
            this.UpdateQuote();
            this.RelayoutAll();
        }

        protected override void OnWalletChanged()
        {
            quoteLabel.TextComponent.Text = "≈" + instrument.GetCurrencyPrice(Wallet.Balance, 3);
            quoteLabel.Measured = false;
            base.OnWalletChanged();
        }

        private void UpdateQuote()
        {
            quoteLabel.TextComponent.Text = "≈" + instrument.GetCurrencyPrice(Wallet.Balance, 3);
            quoteLabel.DescComponent.Text = (instrument.NetChange > 0 ? "+" : "") + ((decimal)instrument.NetChange).GetTextSharps(1) + "% 24h";
            if (instrument.NetChange > 0)
                quoteLabel.DescComponent.ForeColor = buySell.buyColor;
            else if (instrument.NetChange < 0)
                quoteLabel.DescComponent.ForeColor = buySell.sellColor;
            else
                quoteLabel.DescComponent.ForeColor = 0;
            quoteLabel.Measured = false;
            topContainer.Layout();
        }


        private Instrument instrument;

        private LargeLabel quoteLabel;
        private LiteChart liteChart;
        private ExpandButton expandButton;
        private Animator expandAnimator;

        private BuySellTheme buySell = Theme.Get<BuySellTheme>();

        private List<Component> childs;
        private IMultiWallet multiWallet;

        private bool expanded = true;
        private bool Expanded
        {
            get => this.expanded;
            set
            {
                if (this.expanded == value) return;
                this.expanded = value;
                this.expandButton.Checked = this.expanded;
                this.childs = new List<Component>();
                foreach (Component component in this.EnumChilds())
                    this.childs.Add(component);
                this.expandAnimator.Start(this.expanded ? 1 : -1);
            }
        }

        private IEnumerable<Component> EnumChilds()
        {
            if (this.multiWallet != null && this.multiWallet.Wallets.Count > 0)
                foreach (Component component in this.Parent.Components)
                    if (component is IWalletSource s && s.Wallet is IToken token && token.Parent == this.Wallet)
                        yield return component;
        }

        private void CheckExpandButtonVisible()
        {
            bool visible = false;
            foreach (Component component in this.EnumChilds())
            {
                visible = true;
                break;
            }
            if (visible)
            {
                this.nameCaption.Padding.left = 0;
                if (this.expandButton != null)
                    this.expandButton.Visible = true;
                else
                {
                    this.expandAnimator = new Animator(this);
                    this.nameCaption.Padding.left = 0;
                    this.expandButton = new ExpandButton(this.expanded);
                    this.expandButton.ToolTipInfo = new ToolTipInfo("showHideChildWallets", null);
                    this.expandButton.MaxSize.Set(26, 26);
                    this.expandButton.Dock = DockStyle.Left;
                    this.expandButton.CheckedChanged += (s) => this.Expanded = !this.expanded;
                    this.nameCaption.Add(this.expandButton);
                }
            }
            else
            {
                this.nameCaption.Padding.left = 30;
                if (this.expandButton != null)
                    this.expandButton.Visible = false;
            }
        }

        protected override void OnCreated()
        {
            (this.liteChart.IndicatorStyle as CustomColorTrendStyle).Update(Wallet.ThemeColor);
            this.liteChart.InstrumentData.Instrument = instrument;
            this.CheckExpandButtonVisible();
            if (this.multiWallet != null && this.Parent is AnyView)
            {
                this.Parent.Components.Added += Wallets_Added;
                this.Parent.Components.Removed += Wallets_Removed;
            }
            this.UpdateQuote();
            base.OnCreated();
        }

        protected override void OnDrawBack(Graphics g)
        {
            (this.liteChart.IndicatorStyle as CustomColorTrendStyle).Update(Wallet.ThemeColor);
            base.OnDrawBack(g);
        }

        void IAnimation.OnAnimation(Animator animator, float value)
        {
            foreach (Component component in this.childs)
            {
                if (animator.Dir > 0)
                    component.Visible = true;
                component.Alpha = animator.GetValue(255);
                component.Invalidate();
            }
            this.Parent.Layout();
        }

        void IEndAnimation.OnEndAnimation(Animator animator, float value)
        {
            if (animator.Dir < 0)
            {
                foreach (Component component in this.childs)
                    component.Visible = false;
            }
            if (this.Parent is AnyView anyView)
                anyView.Relayout();
        }

    }
}
