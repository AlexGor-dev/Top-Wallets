using System;
using Complex.Controls;
using Complex.Trader;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class TransactionsChartPanel : Container
    {
        protected TransactionsChartPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.wallet = data["wallet"] as Wallet;
            this.barViewMode = (BarViewMode)data["barViewMode"];
            this.crossVisible = (bool)data["crossVisible"];
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["wallet"] = this.wallet;
            data["barViewMode"] = this.liteChart.BarViewMode;
            data["crossVisible"] = this.liteChart.CrossVisible;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TransactionsChartPanel(Wallet wallet)
        {
            this.wallet = wallet;
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.chartData = new TransactionsChartData(this.wallet);

            this.liteChart = new LiteChartMetric(this.chartData);
            this.liteChart.TimeAxis.FixedBarsTime = 4;
            this.liteChart.Indicator.Style = new CustomColorTrendStyle(wallet.ThemeColor, this.barViewMode);
            this.liteChart.Indicator.Step = (decimal)(1 / Math.Pow(10, this.wallet.Balance.DefaultSignCount));
            this.liteChart.Dock = DockStyle.Fill;
            this.Add(this.liteChart);

            top = new Container();
            top.Padding.Set(10, 4, 10, 4);
            top.Inflate.Set(6, 0);
            top.MaxHeight = 30;
            top.Dock = DockStyle.Top;
            top.Style = Themes.Theme.Get<MarketDataPanelTheme>();

            //ImageButton button = new ImageButton("load_transactions.svg");
            //button.ToolTipInfo = new ToolTipInfo(button.Image, "loadTransactions", null);
            //button.Dock = DockStyle.Left;
            //button.Executed += (s) =>
            //{
            //    ImageButton b = s as ImageButton;
            //    b.Enabled = false;
            //    wallet.GetTransactions(wallet.Transactions.Last, 16);
            //    Timer.Delay(1000, () => { b.Enabled = true; });
            //};
            //top.Add(button);



            intervalText = new SwitchTextComponent();
            intervalText.Dock = DockStyle.Left;
            intervalText.Style = Theme.Get<CaptionStyle>();
            top.Add(intervalText);


            top.Add(new Dummy(DockStyle.Left, 10, 0));

            upLabel = new CurrencyLabel("", wallet.Symbol);
            upLabel.ValueTextComponent.Font = Theme.font12Bold;
            upLabel.Dock = DockStyle.Left;
            top.Add(upLabel);

            top.Add(new Dummy(DockStyle.Left, 10, 0));

            downLabel = new CurrencyLabel("", wallet.Symbol);
            downLabel.ValueTextComponent.Font = Theme.font12Bold;
            downLabel.Dock = DockStyle.Left;
            top.Add(downLabel);


            CheckedImageButton barViewModeButton = new CheckedImageButton();
            barViewModeButton.ToolTipInfo = new ToolTipInfo("chartViewMode");
            barViewModeButton.ImageID = this.liteChart.BarViewMode.GetImage();
            barViewModeButton.Dock = DockStyle.Right;
            barViewModeButton.CheckedChanged += delegate (object s)
            {
                if (barViewModeButton.Checked)
                {
                    barViewModeButton.IsFixed = true;
                    BarViewModeMenu menu = new BarViewModeMenu();
                    menu.Mode = this.liteChart.BarViewMode;
                    menu.ActionComponent = barViewModeButton;
                    menu.Hided += delegate (object s2)
                    {
                        menu = s2 as BarViewModeMenu;
                        this.liteChart.BarViewMode = menu.Mode;
                        barViewModeButton.ImageID = this.liteChart.BarViewMode.GetImage();
                        barViewModeButton.Tag = null;
                        barViewModeButton.IsFixed = false;
                        barViewModeButton.Checked = false;
                        menu.Dispose();
                    };
                    menu.Show(barViewModeButton, MenuAlignment.Bottom);
                }
            };
            top.Add(barViewModeButton);

            CheckedImageButton crossButton = new CheckedImageButton("cross.svg", crossVisible);
            crossButton.ToolTipInfo = new ToolTipInfo("showCross");
            crossButton.Dock = DockStyle.Right;
            crossButton.DrawBorder = true;
            crossButton.CheckedChanged += (s) =>
            {
                liteChart.CrossVisible = crossButton.Checked;
            };
            top.Add(crossButton);

            this.Add(top);

            this.Update();
            wallet.TransactionsLoaded += Wallet_TransactionsLoaded;
            wallet.Adapter.EndUpdated += Adapter_EndUpdated;
            wallet.Adapter.Connected += Adapter_Connected;
            wallet.Adapter.Disconnected += Adapter_Disconnected;
        }


        protected override void OnDisposed()
        {
            wallet.Adapter.Connected -= Adapter_Connected;
            wallet.Adapter.Disconnected -= Adapter_Disconnected;
            wallet.Adapter.EndUpdated -= Adapter_EndUpdated;
            wallet.TransactionsLoaded -= Wallet_TransactionsLoaded;
            base.OnDisposed();
        }

        private void Adapter_Disconnected(object sender)
        {
            this.liteChart.Invalidate();
        }

        private void Adapter_Connected(object sender)
        {
            this.liteChart.Invalidate();
        }

        private void Adapter_EndUpdated(object sender)
        {
            Application.Invoke(() =>
            {
                string text = this.intervalText.Text;
                ITransactionBase last = this.wallet.Transactions.Last;
                if (last != null)
                    this.intervalText.Text = Language.Current["for"] + " " + (DateTime.UtcNow - last.Time).ToYMD();
                else
                    this.intervalText.TextID = "noTransaction";
                if (text != this.intervalText.Text)
                    top.Layout();
            });
        }


        private void Wallet_TransactionsLoaded(object sender)
        {
            this.Update();
        }

        private Wallet wallet;
        private TransactionsChartData chartData;

        private LiteChartMetric liteChart;
        private Container top;

        private SwitchTextComponent intervalText;
        private CurrencyLabel upLabel;
        private CurrencyLabel downLabel;

        private BuySellTheme buySell = Theme.Get<BuySellTheme>();

        private BarViewMode barViewMode = BarViewMode.Candlestick;
        private bool crossVisible = true;

        private void Update()
        {

            decimal up = 0;
            decimal down = 0;

            foreach (ITransactionBase transaction in this.wallet.Transactions)
            {
                decimal amount = transaction.GetAmount(this.wallet.Symbol);
                if (amount < 0)
                    down -= amount;
                else
                    up += amount;
            }

            Application.Invoke(() =>
            {
                ITransactionBase last = this.wallet.Transactions.Last;
                if (last != null)
                    this.intervalText.Text = Language.Current["for"] + " " + (DateTime.UtcNow - last.Time).ToYMD();
                else
                    this.intervalText.TextID = "noTransaction";
                upLabel.ValueTextComponent.ForeColor = buySell.buyColor;
                downLabel.ValueTextComponent.ForeColor = buySell.sellColor;

                upLabel.ValueTextComponent.Text = up.ToKMBPlus(3);
                downLabel.ValueTextComponent.Text = (-down).ToKMBPlus(3);
                top.Layout();
            });
        }

        protected override void OnDrawBack(Graphics g)
        {
            (this.liteChart.Indicator.Style as CustomColorTrendStyle).Update(wallet.Adapter.ThemeColor);
            base.OnDrawBack(g);
        }

    }
}
