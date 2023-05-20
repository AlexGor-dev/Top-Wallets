using System;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public class CoinMarket : Disposable, IMarketDataHandler, IUnique
    {
        public CoinMarket(string symbol)
        {
            this.symbol = symbol;
            this.Coin = MainSettings.Current.GetCoin(symbol);
            MainSettings.Current.General.CurrencyChanged += General_CurrencyChanged;

        }

        protected override void OnDisposed()
        {
            MainSettings.Current.General.CurrencyChanged -= General_CurrencyChanged;
            this.Coin = null;
            base.OnDisposed();
        }

        public event Handler LastPriceChanged;
        public event Handler QuoteChanged;
        public event Handler CoinChanged;

        private void General_CurrencyChanged(object sender)
        {
            this.Coin = MainSettings.Current.GetCoin(symbol);
        }

        private string symbol;
        public string ID => symbol;

        private decimal lastPrice;
        public decimal LastPrice => lastPrice;

        private Coin coin;
        public Coin Coin
        {
            get => this.coin;
            private set
            {
                if (this.coin == value) return;
                if (this.coin != null)
                    this.coin.Unsubscribe(this, MarketDataType.Quote);
                this.coin = value;
                this.lastPrice = 0;
                if (this.coin != null)
                {
                    this.coin.Subscribe(this, MarketDataType.Quote);
                    this.lastPrice = this.coin.LastPrice;
                }
                Events.Invoke(this.CoinChanged, this);
            }
        }

        public decimal GetVolume(Balance balance)
        {
            return (decimal)balance * lastPrice;
        }

        void IMarketDataHandler.OnMarketData(Instrument instrument, MarketData md)
        {
            switch (md.type)
            {
                case MarketDataType.LastPrice:
                    if (this.lastPrice != md.price)
                    {
                        this.lastPrice = md.price;
                        Events.Invoke(this.LastPriceChanged, this);
                    }
                    break;
                case MarketDataType.Quote:
                    Events.Invoke(this.QuoteChanged, this);
                    break;
            }
        }
    }
}
