using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;
using Complex.Animations;

namespace Complex.Wallets
{
    public class WalletPanel : WalletBasePanel, IEndAnimation
    {
        protected WalletPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.pager = data["pager"] as Pager;
            this.transactionsChartPage = data["transactionsChartPage"] as Page;
            this.coinChartPage = data["coinChartPage"] as Page;
            this.chartPanel = data["chartPanel"] as ChartPanel;
            this.transactionsChartPanel = data["transactionsChartPanel"] as Component;
            this.loadTransactionsButton = data["loadTransactionsButton"] as ImageButton;
            this.expandButton = data["expandButton"] as ExpandButton;
            this.tokensPage = data["tokensPage"] as Page;
            this.nftstPage = data["nftstPage"] as Page;
            this.tokensPanel = data["tokensPanel"] as Component;
            this.nftsPanel = data["nftsPanel"] as Component;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["pager"] = this.pager;
            data["transactionsChartPage"] = this.transactionsChartPage;
            data["coinChartPage"] = this.coinChartPage;
            data["chartPanel"] = this.chartPanel;
            data["transactionsChartPanel"] = this.transactionsChartPanel;
            data["loadTransactionsButton"] = this.loadTransactionsButton;
            data["expandButton"] = this.expandButton;
            data["tokensPage"] = this.tokensPage;
            data["nftstPage"] = this.nftstPage;
            data["tokensPanel"] = this.tokensPanel;
            data["nftsPanel"] = this.nftsPanel;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletPanel(Wallet wallet)
            :base(wallet)
        {
            this.pager = new Pager();
            this.pager.Dock = DockStyle.Fill;
            this.pager.LinePager.Container.Padding.Set(10, 0, 10, 0);
            this.pager.LinePager.Container.MinHeight = 36;

            Page page = new Page("transactions");
            page.Component = this.Wallet.CreateTransactionPanel();
            this.pager.Pages.Add(page);

            transactionsChartPage = new Page("transactionsChart");
            transactionsChartPage.MaxWidth = 200;
            this.pager.Pages.Add(transactionsChartPage);

            if (wallet.IsMain && wallet.IsSupportMarket)
            {
                coinChartPage = new Page(wallet.Symbol + "Chart");
                coinChartPage.MaxWidth = 200;
                this.pager.Pages.Add(coinChartPage);
            }

            if (wallet.CanTokens)
            {
                tokensPage = new Page("Tokens");
                tokensPage.MaxWidth = 200;
                this.pager.Pages.Add(tokensPage);
            }

            if (wallet.CanNfts)
            {
                nftstPage = new Page("Nfts");
                nftstPage.MaxWidth = 200;
                this.pager.Pages.Add(nftstPage);
            }

            loadTransactionsButton = new ImageButton("load_transactions.svg");
            loadTransactionsButton.Dock = DockStyle.Right;
            this.pager.LinePager.Container.Add(loadTransactionsButton);

            this.pager.LinePager.Container.Add(new Dummy(DockStyle.Right, 20, 0));

            expandButton = new ExpandButton(true);
            expandButton.MaxSize.Set(28, 28);
            expandButton.Dock = DockStyle.Right;
            this.pager.LinePager.Container.Add(expandButton);

            this.Init();

            this.pager.CurrentPage = page;

        }

        private void Init()
        {
            this.adapter = this.Wallet.Adapter;
            this.animator = new Animator(this);

            this.mainPanel = this.Wallet.CreateMainPanel();
            this.mainPanel.Dock = DockStyle.Top | DockStyle.Animation;
            this.Add(this.mainPanel);

            if (this.Wallet.IsSupportMarket)
            {
                Component marketPanel = this.Wallet.CreateMarketPanel();
                marketPanel.Dock = DockStyle.Top;
                this.Add(marketPanel);
            }
            if (this.Wallet.IsMain && !this.Wallet.Adapter.IsTestnet)
            {
                Component buySell = this.Wallet.CreateBuySellPanel();
                buySell.Dock = DockStyle.Top;
                this.Add(buySell);
            }

            this.Add(this.pager);


            transactionsChartPage.GetComponent += () =>
            {
                this.transactionsChartPanel = this.Wallet.CreateTransactionsChartPanel();
                return this.transactionsChartPanel;
            };

            if (coinChartPage != null)
            {
                coinChartPage.GetComponent += () =>
                {
                    this.chartPanel = new ChartPanel(Wallet);
                    return this.chartPanel;
                };
            }

            if (tokensPage != null)
            {
                tokensPage.GetComponent += () =>
                {
                    this.tokensPanel = this.Wallet.CreateTokensPanel();
                    return this.tokensPanel;
                };
            }


            if (nftstPage != null)
            {
                nftstPage.GetComponent += () =>
                {
                    this.nftsPanel = this.Wallet.CreateNftsPanel();
                    return this.nftsPanel;
                };
            }


            loadTransactionsButton.ToolTipInfo = new ToolTipInfo(loadTransactionsButton.Image, "loadTransactions", null);
            loadTransactionsButton.Executed += (s) =>
            {
                loadTransactionsButton.Enabled = false;
                Wallet.GetTransactions(Wallet.Transactions.Last, 16,(ts,e)=> { });
                Timer.Delay(1000, () => { loadTransactionsButton.Enabled = true; });
            };


            expandButton.ToolTipInfo = new ToolTipInfo("expand");
            expandButton.CheckedChanged += (s) =>
            {
                if (expandButton.Checked)
                {
                    expandButton.ToolTipInfo = new ToolTipInfo("expand");
                    mainPanel.Visible = true;
                    this.animator.Start(-1);
                }
                else
                {
                    expandButton.ToolTipInfo = new ToolTipInfo("collapse");
                    this.animator.Start(1);
                }
            };

        }

        protected override void OnConnectionChanged()
        {
            if (this.chartPanel != null)
                this.chartPanel.Update();
            this.Invalidate();
        }

        private WalletAdapter adapter;
        private Animator animator;

        private ChartPanel chartPanel;
        private Component transactionsChartPanel;

        private Component mainPanel;
        private Pager pager;
        private Page transactionsChartPage;
        private Page coinChartPage;
        private Page tokensPage;
        private Page nftstPage;
        private ExpandButton expandButton;
        private ImageButton loadTransactionsButton;

        private Component tokensPanel;
        private Component nftsPanel;

        private readonly Rect dispRect = new Rect();

        protected override void OnMeasure(float widthMeasure, float heightMeasure)
        {
            base.OnMeasure(widthMeasure, heightMeasure);
            if (!expandButton.Checked)
            {
                mainPanel.AnimationPos = mainPanel.MeasuredHeight;
                mainPanel.Visible = false;
            }
        }

        protected override void OnSizeChanged()
        {
            GetDisplayRectangle(dispRect);
            base.OnSizeChanged();
        }

        void IAnimation.OnAnimation(Animator animator, float value)
        {
            mainPanel.AnimationPos = Helper.GetValue(0, mainPanel.MeasuredHeight, value);
            this.Layout();
        }

        void IEndAnimation.OnEndAnimation(Animator animator, float value)
        {
            if (animator.Dir > 0)
                mainPanel.Visible = false;
        }

    }
}
