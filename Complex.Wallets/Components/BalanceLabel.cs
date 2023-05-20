using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Trader;

namespace Complex.Wallets
{
    public class BalanceLabel : Component
    {
        public BalanceLabel(Wallet wallet)
        {
            this.wallet = wallet;
            this.Padding.Set(4);
            this.Init();
        }

        private void Init()
        {
            if (this.wallet.IsSupportMarket)
            {
                this.wallet.Market.LastPriceChanged += Wallet_CoinLastPriceChanged;
                this.wallet.Market.CoinChanged += Market_CoinChanged;
            }
        }


        protected override void OnDisposed()
        {
            this.wallet.Market.CoinChanged -= Market_CoinChanged;
            this.wallet.Market.LastPriceChanged -= Wallet_CoinLastPriceChanged;
            base.OnDisposed();
        }

        private void Market_CoinChanged(object sender)
        {
            this.Invalidate();
        }

        private void Wallet_CoinLastPriceChanged(object sender)
        {
            this.Invalidate();
        }

        private Wallet wallet;

        private NumberAnimationEffect animationEffect;

        private readonly Rect clientRect = new Rect();
        public readonly static Font font30Bold = Font.Create(30, FontStyle.Bold);
        public readonly static Font font20 = Font.Create(20, FontStyle.Bold);

        protected override void OnCreated()
        {
            this.animationEffect = new NumberAnimationEffect(this.Parent, 500);
            this.animationEffect.SetValue(this.wallet.Balance);
            base.OnCreated();
        }
        public void Update()
        {
            this.Measured = false;
            this.Measure();
            this.animationEffect.SetValue(this.wallet.Balance);
        }

        protected override void OnSizeChanged()
        {
            GetClientRectangle(clientRect);
            base.OnSizeChanged();
        }

        protected override void OnMeasure(float widthMeasure, float heightMeasure)
        {
            float width = Math.Max(font30Bold.GetWidth(this.wallet.Balance.GetTextSharps(8)) + 30 + Theme.font13Bold.GetWidth(wallet.Symbol)
                , font20.GetWidth(this.wallet.GetBalanceMarketPrice()));
            float height = font30Bold.Height + font20.Height;
            this.SetMeasured(Padding.horizontal + width, Padding.vertical + height);
        }

        protected override void OnDraw(Graphics g)
        {
            float x = clientRect.x;
            float y = clientRect.y;
            float width = clientRect.width;

            float right = 0;
            if (this.wallet.Balance >= 1000)
                right = this.animationEffect.Draw(g, font30Bold, Theme.font13Bold, x, y, width - Theme.font13Bold.GetWidth(wallet.Symbol), font30Bold.Height, this.wallet.ThemeColor, this.wallet.ThemeColor);
            else
                right = this.animationEffect.Draw(g, font30Bold, x, y, width - Theme.font13Bold.GetWidth(wallet.Symbol), font30Bold.Height, this.wallet.ThemeColor);

            g.DrawText(wallet.Symbol, Theme.font13Bold, right + 10, y + 0.7f * (font30Bold.Height - Theme.font13Bold.Height), Theme.fore2);

            string text = this.wallet.GetBalanceMarketPrice();
            if (text != null)
            {
                y += font30Bold.Height;
                float twidth = font20.GetWidth(text);
                g.DrawText(text, font20, x + (width - twidth) / 2, y, Theme.appFore);
            }
        }
    }
}
