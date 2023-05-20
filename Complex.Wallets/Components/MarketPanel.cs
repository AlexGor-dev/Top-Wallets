using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;

namespace Complex.Wallets
{
    public class MarketPanel : Component
    {
        public MarketPanel(Wallet wallet)
        {
            this.wallet = wallet;
            this.adapter = wallet.Adapter;
            this.MinHeight = 40;
            this.Padding.Set(20, 0, 20, 0);
            this.instrument = this.wallet.Market.Coin;
            this.wallet.Market.CoinChanged += Wallet_CoinChanged;
            this.wallet.Market.QuoteChanged += Wallet_CoinQuoteChanged;
        }

        private void Wallet_CoinQuoteChanged(object sender)
        {
            this.Invalidate();
        }

        private void Wallet_CoinChanged(object sender)
        {
            this.instrument = this.wallet.Market.Coin;
            this.Invalidate();
        }

        protected override void OnDisposed()
        {
            this.wallet.Market.CoinChanged -= Wallet_CoinChanged;
            this.wallet.Market.QuoteChanged -= Wallet_CoinQuoteChanged;
            base.OnDisposed();
        }

        private Wallet wallet;
        private WalletAdapter adapter;
        private Instrument instrument;

        private readonly Rect clientRect = new Rect();
        private BuySellTheme buySell = Theme.Get<BuySellTheme>();
        private decimal prevPrice;
        private decimal lastPrice;

        protected override Type GetDefaultTheme()
        {
            return typeof(MapBackTheme);
        }


        protected override void OnSizeChanged()
        {
            GetClientRectangle(clientRect);
            base.OnSizeChanged();
        }

        protected override void OnDraw(Graphics g)
        {
            if (instrument.LastPrice != lastPrice)
            {
                prevPrice = lastPrice;
                lastPrice = instrument.LastPrice;
            }
            int s = g.Save();
            g.SetClip(clientRect);

            MapBackTheme style = GetStyle<MapBackTheme>();


            int signCount = 2;

            float x = clientRect.x;
            float y = clientRect.y + 2;
            float vtop = y + style.font.Height + 4;

            int priceColor = prevPrice < lastPrice ? buySell.buyColor : buySell.sellColor;

            g.DrawText(Language.Current["lastPrice"], style.font, x, y, style.foreColor);
            g.DrawText(Currency.GetPrice(instrument.Currency, lastPrice.GetTextSharps(signCount)), style.valueFont, x, y + vtop, priceColor);

            string text = (instrument.NetChange > 0 ? "+" : "") + instrument.NetChange.GetTextSharps(1) + "%";
            x = clientRect.x + (clientRect.width / 2 - Math.Max(style.valueFont.GetWidth(text), style.font.GetWidth(Language.Current["netChange24"]))) / 2;
            g.DrawText(Language.Current["netChange24"], style.font, x, y, style.foreColor);
            g.DrawText(text, style.valueFont, x, vtop, instrument.NetChange > 0 ? buySell.buyColor : buySell.sellColor);

            text = Currency.GetPrice(instrument.Currency, instrument.High.GetTextSharps(signCount));
            x = clientRect.x + (clientRect.width - Math.Max(style.valueFont.GetWidth(text), style.font.GetWidth(Language.Current["high24"]))) / 2;
            g.DrawText(Language.Current["high24"], style.font, x, y, style.foreColor);
            g.DrawText(text, style.valueFont, x, vtop, style.valueForeColor);

            text = Currency.GetPrice(instrument.Currency, instrument.Low.GetTextSharps(signCount));
            x = clientRect.x + clientRect.width / 2 + (clientRect.width / 2 - Math.Max(style.valueFont.GetWidth(text), style.font.GetWidth(Language.Current["low24"]))) / 2;
            g.DrawText(Language.Current["low24"], style.font, x, y, style.foreColor);
            g.DrawText(text, style.valueFont, x, vtop, style.valueForeColor);

            text = instrument.Volume.GetText(0) + " " + wallet.Symbol;
            x = clientRect.right - style.valueFont.GetWidth(text);
            g.DrawText(Language.Current["volume24"], style.font, x, y, style.foreColor);
            g.DrawText(text, style.valueFont, x, vtop, style.valueForeColor);

            g.Restore(s);
        }
    }
}
