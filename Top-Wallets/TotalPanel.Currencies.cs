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
        private class CurrenciesPanel : Container, IMarketDataHandler
        {
            public CurrenciesPanel(int startIndex)
            {
                this.MinWidth = 120;
                this.MaxHeight = 36;
                this.topCoinInfo = MainSettings.Current.Remote.CryptoCoins[startIndex];
                this.botCoinInfo = MainSettings.Current.Remote.CryptoCoins[startIndex + 1];

                this.top = new Container();
                this.top.Dock = DockStyle.Top;

                this.topMain = new TextComponent(this.topCoinInfo.Symbol);
                this.topMain.Font = Theme.font10Bold;
                this.topMain.ForeColor = this.topCoinInfo.Color;
                this.topMain.Dock = DockStyle.Left;
                this.top.Add(this.topMain);

                this.topCur = new CurrencyLabel("", "");
                this.topCur.ValueTextComponent.Font = Theme.font9;
                this.topCur.Dock = DockStyle.Right;
                this.topCur.Alignment = ContentAlignment.Right;
                this.top.Add(this.topCur);

                this.Add(this.top);

                this.Add(new Separator(DockStyle.Top, 6));

                this.bot = new Container();
                this.bot.Dock = DockStyle.Top;

                this.botMain = new TextComponent(this.botCoinInfo.Symbol);
                this.botMain.ForeColor = this.botCoinInfo.Color;
                this.botMain.Font = Theme.font10Bold;
                this.botMain.Dock = DockStyle.Left;
                this.bot.Add(this.botMain);

                this.botCur = new CurrencyLabel("", "");
                this.botCur.ValueTextComponent.Font = Theme.font9;
                this.botCur.Dock = DockStyle.Right;
                this.botCur.Alignment = ContentAlignment.Right;
                this.bot.Add(this.botCur);

                this.Add(this.bot);
                MainSettings.Current.General.CurrencyChanged += Instance_CurrencyChanged;

                this.Subscribe();

            }

            protected override void OnDisposed()
            {
                MainSettings.Current.General.CurrencyChanged -= Instance_CurrencyChanged;
                this.Unsubscribe();
                base.OnDisposed();
            }

            private CryptoCoinInfo topCoinInfo;
            private Coin topCoin;
            private Container top;
            private TextComponent topMain;
            private CurrencyLabel topCur;

            private CryptoCoinInfo botCoinInfo;
            private Coin botCoin;
            private Container bot;
            private TextComponent botMain;
            private CurrencyLabel botCur;

            private void Instance_CurrencyChanged(object sender)
            {
                this.Unsubscribe();
                this.Subscribe();
                this.Calculate();
            }

            private void Unsubscribe()
            {
                if (this.topCoin != null)
                    this.topCoin.Unsubscribe(this, MarketDataType.Quote);
                if (this.botCoin != null)
                    this.botCoin.Unsubscribe(this, MarketDataType.Quote);
            }

            private void Subscribe()
            {
                this.topCoin = MainSettings.Current.Adapter.GetCoin(this.topCoinInfo.Symbol, MainSettings.Current.General.Currency.ID);
                this.topCoin.Subscribe(this, MarketDataType.Quote);

                this.botCoin = MainSettings.Current.Adapter.GetCoin(this.botCoinInfo.Symbol, MainSettings.Current.General.Currency.ID);
                this.botCoin.Subscribe(this, MarketDataType.Quote);
            }

            private void Calculate()
            {
                string ttext = this.topCur.ValueTextComponent.Text;
                string btext = this.botCur.ValueTextComponent.Text;

                this.topCur.ValueTextComponent.Text = this.topCoin.LastPrice.GetTextSharps(this.topCoin.SignCount);
                this.topCur.CurrencyTextComponent.Text = MainSettings.Current.General.Currency.ID;

                this.botCur.ValueTextComponent.Text = this.botCoin.LastPrice.GetTextSharps(this.botCoin.SignCount);
                this.botCur.CurrencyTextComponent.Text = MainSettings.Current.General.Currency.ID;

                if (ttext != this.topCur.ValueTextComponent.Text || btext != this.botCur.ValueTextComponent.Text)
                {
                    this.ClearMeasured();
                    Application.Invoke(() => this.RelayoutAll());
                }
            }

            public void Update()
            {
                this.Unsubscribe();

                this.Subscribe();
                if (this.IsCreated)
                    this.Calculate();
            }

            void IMarketDataHandler.OnMarketData(Instrument instrument, MarketData md)
            {
                if (md.type == MarketDataType.LastPrice)
                {
                    this.Calculate();
                }
            }
        }

    }
}
