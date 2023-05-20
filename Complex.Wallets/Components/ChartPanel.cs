using System;
using Complex.Controls;
using Complex.Trader;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class ChartPanel : Container, IMarketDataHandler
    {
        protected ChartPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.wallet = data["wallet"] as Wallet;
            this.barViewMode = (BarViewMode)data["barViewMode"];
            this.crossVisible = (bool)data["crossVisible"];
            this.mainInstrument = (bool)data["mainInstrument"];
            this.interval = (Interval)data["interval"];
            this.currency = data["currency"] as string;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["wallet"] = this.wallet;
            data["barViewMode"] = this.liteChart.BarViewMode;
            data["crossVisible"] = this.liteChart.CrossVisible;
            data["mainInstrument"] = this.mainInstrument;
            data["interval"] = this.liteChart.InstrumentData.Interval;
            data["currency"] = this.Instrument.Currency;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public ChartPanel(Wallet wallet)
        {
            this.wallet = wallet;
            this.currency = MainSettings.Current.General.Currency.ID;
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.instrument = MainSettings.Current.GetCoin(this.wallet.Symbol, currency);
            this.instrumentData = new InstrumentData(this.instrument, interval);

            this.liteChart = new LiteChartMetric(instrumentData);
            this.liteChart.Indicator.Style = new CustomColorTrendStyle(wallet.ThemeColor, this.barViewMode);
            this.liteChart.Dock = DockStyle.Fill;
            this.liteChart.TimeAxis.FixedBarsTime = 4;
            this.Add(this.liteChart);

            top = new Container();
            top.Padding.Set(10, 4, 10, 4);
            top.Inflate.Set(6, 0);
            //top.MinHeight = 24;
            top.MaxHeight = 30;
            top.Dock = DockStyle.Top;
            top.Style = Themes.Theme.Get<MarketDataPanelTheme>();

            this.instrumentsButton = new MenuButton(null, instrument.Ticker);
            this.instrumentsButton.Dock = DockStyle.Left;
            this.instrumentsButton.DrawBorder = true;
            this.instrumentsButton.TwoStrip = false;
            this.instrumentsButton.InitMenu += (s, c) =>
            {
                c.Components.Clear(true);
                foreach (Currency currency in MainSettings.Current.General.Currencies)
                {
                    Instrument instr = MainSettings.Current.GetCoin(this.instrument.Symbol, currency.ID);
                    MenuStripRadioButton button = new MenuStripRadioButton(instr.Ticker, instr.Ticker == this.instrumentsButton.TextID);
                    button.Padding.right = 10;
                    button.Tag = instr;
                    button.Executed += (s2) => { this.Instrument = (s2 as Component).Tag as Instrument;};
                    c.Add(button);
                }
            };
            top.Add(instrumentsButton);

            lastPriceComponent = new TextComponent("");
            lastPriceComponent.Dock = DockStyle.Left;
            lastPriceComponent.Font = Theme.font12Bold;
            top.Add(lastPriceComponent);

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

            crossButton = new CheckedImageButton("cross.svg", crossVisible);
            crossButton.ToolTipInfo = new ToolTipInfo("showCross");
            crossButton.Dock = DockStyle.Right;
            crossButton.DrawBorder = true;
            crossButton.CheckedChanged += (s) =>
            {
                liteChart.CrossVisible = crossButton.Checked;
            };
            top.Add(crossButton);

            top.Add(new Separator(DockStyle.Right, 2));

            group = new FixedGroup();

            AddButton("24h", Interval.M30);
            AddButton("7d", Interval.H2);
            AddButton("14d", Interval.H4);
            AddButton("30d", Interval.H8);
            AddButton("90d", Interval.D1);
            AddButton("180d", Interval.D2);
            AddButton("1y", Interval.D4);
            AddButton("Max", Interval.W1);

            this.Add(top);

            lastPrice = instrument.LastPrice;
            this.UpdateLastPrice();

            this.instrument.Subscribe(this, MarketDataType.Quote);
            MainSettings.Current.General.CurrencyChanged += Instance_CurrencyChanged;
            wallet.Adapter.Connected += Adapter_Connected;
            wallet.Adapter.Disconnected += Adapter_Disconnected;
        }

        protected override void OnDisposed()
        {
            wallet.Adapter.Connected -= Adapter_Connected;
            wallet.Adapter.Disconnected -= Adapter_Disconnected;
            MainSettings.Current.General.CurrencyChanged -= Instance_CurrencyChanged;
            base.OnDisposed();
        }

        protected override void OnCreated()
        {
            (this.liteChart.Indicator.Style as CustomColorTrendStyle).Update(wallet.ThemeColor);
            base.OnCreated();
        }

        private void Adapter_Disconnected(object sender)
        {
            this.liteChart.Invalidate();
        }

        private void Adapter_Connected(object sender)
        {
            this.liteChart.Invalidate();
        }

        private void Instance_CurrencyChanged(object sender)
        {
            if(mainInstrument)
                this.Instrument = this.wallet.Market.Coin;
        }


        void IMarketDataHandler.OnMarketData(Instrument instrument, MarketData md)
        {
            if (md.type == MarketDataType.LastPrice)
            {
                this.prevPrice = 0;
                this.UpdateLastPrice();
            }
        }

        private void UpdateLastPrice()
        {
            Application.Invoke(() =>
            {
                if (instrument.LastPrice != lastPrice)
                {
                    prevPrice = lastPrice;
                    lastPrice = instrument.LastPrice;
                };
                lastPriceComponent.ForeColor = prevPrice == 0 ? 0 : (prevPrice < lastPrice ? buySell.buyColor : buySell.sellColor);
                lastPriceComponent.Text = Currency.GetPrice(instrument.Currency, lastPrice.GetTextSharps(2));
                instrumentsButton.TextID = this.instrument.Ticker;
                top.Layout();
            });
        }

        protected override void OnDrawBack(Graphics g)
        {
            (this.liteChart.Indicator.Style as CustomColorTrendStyle).Update(wallet.ThemeColor);
            base.OnDrawBack(g);
        }

        private Wallet wallet;

        private bool mainInstrument = true;
        private BarViewMode barViewMode = BarViewMode.Area;
        private bool crossVisible = true;
        private Interval interval = Interval.H8;

        private string currency;

        private Instrument instrument;
        public Instrument Instrument
        {
            get => instrument;
            set
            {
                if (this.instrument == value) return;
                if (this.instrument != null)
                    this.instrument.Unsubscribe(this, MarketDataType.Quote);
                this.instrument = value;
                this.currency = this.instrument.Currency;
                this.prevPrice = 0;
                this.UpdateLastPrice();
                this.instrument.Subscribe(this, MarketDataType.Quote);
                this.mainInstrument = this.instrument.Currency == MainSettings.Current.General.Currency.ID;
                this.liteChart.InstrumentData.Instrument = instrument;
            }
        }

        private InstrumentData instrumentData;
        private LiteChartMetric liteChart;
        private MenuButton instrumentsButton;
        private Container top;
        private FixedGroup group;
        private BuySellTheme buySell = Theme.Get<BuySellTheme>();
        private decimal prevPrice;
        private decimal lastPrice;
        private CheckedImageButton crossButton;

        private TextComponent lastPriceComponent;

        private void AddButton(string text, Interval interval)
        {
            CheckedTextButton button = new CheckedTextButton(text, interval == instrumentData.Interval);
            button.Tag = interval;
            button.Dock = DockStyle.Right;
            button.Alignment = Drawing.ContentAlignment.Center;
            button.MinWidth = 40;
            button.DrawBorder = true;
            button.CheckedChanged += (s) =>
            {
                CheckedTextButton b = s as CheckedTextButton;
                if (b.Checked)
                {
                    instrumentData.Interval = (Interval)b.Tag;
                }
            };
            group.Add(button);
            top.Add(button);

        }

        public void Update()
        {
            (this.liteChart.Indicator.Style as CustomColorTrendStyle).Update(wallet.ThemeColor);

        }
    }
}
