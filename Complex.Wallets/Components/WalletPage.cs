using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Trader;

namespace Complex.Wallets
{
    public class WalletPage : ModePage, IWalletSource
    {
        protected WalletPage(IData data)
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

        public WalletPage(ModePager pager, Wallet wallet)
            :base(pager, wallet.CreateWalletInfoPanel())
        {
            this.wallet = wallet;
            this.Closable = true;

            this.Init();
        }

        private void Init()
        {
            this.instrument = this.wallet.Market.Coin;
            this.wallet.Changed += Wallet_Changed;
        }

        protected override void OnDisposed()
        {
            this.wallet.Changed -= Wallet_Changed;
            addressFont.Dispose();
            base.OnDisposed();
        }

        private void Wallet_Changed(object sender)
        {
            this.Invalidate();
        }


        private Wallet wallet;
        public Wallet Wallet => wallet;

        private Instrument instrument;

        private readonly Font addressFont = Font.Create(10, FontStyle.Bold);
        private const string addressText = "WWWWWWWWW";

        public override float GetFullWidth()
        {
            return Theme.font11Bold.GetWidth(addressText) + base.GetFullWidth();
        }

        protected override void OnMeasure(float widthMeasure, float heightMeasure)
        {
            base.OnMeasure(widthMeasure, heightMeasure);
        }

        protected override void OnBoundsChanged(bool locationChanged, bool sizeChinged)
        {
            addressFont.Size = Helper.GetValue(9f, 11f, GetAnimationValue());
            base.OnBoundsChanged(locationChanged, sizeChinged);
        }

        public override void BeginComponentLayout()
        {
            this.Component.BeginSizeChanged();
            base.BeginComponentLayout();
        }

        public override void EndComponentLayout()
        {
            this.Component.EndSizeChanged();
            base.EndComponentLayout();
        }

        protected override void OnClosed()
        {
            WalletsData.Wallets.Remove(this.wallet);
            base.OnClosed();
        }

        protected override void OnDrawContent(Graphics g, float x, float y, float fullWidth)
        {
            MultiPageTheme style = this.GetStyle<MultiPageTheme>();

            float width = Math.Max(clientRect.width, fullWidth);

            float animationValue = GetAnimationValue();


            y += Helper.GetValue(0f, 2f, animationValue);
            float twidth = addressFont.GetWidth(addressText);
            string text = wallet.Name;
            //string known = wallet.Adapter.GetKnownAddress(text);
            //if (known != null)
            //    text = known;
            twidth = Math.Min(twidth, addressFont.GetWidth(text));
            g.DrawText(text, addressFont, x + (width - twidth) / 2, y, twidth, addressFont.Height,  this.stateManager.GetColor(style.foreStyle));
            y += addressFont.Height;

            y += Helper.GetValue(0f, 4f, animationValue);


            Font balanceFont = Theme.font9Bold;
            text = this.wallet.Balance.GetTextSharps(this.wallet.Balance.DefaultSignCount) + " " + wallet.Symbol;
            g.DrawText(text, balanceFont, x + (width - balanceFont.GetWidth(text)) / 2, y, wallet.ThemeColor);
            y += balanceFont.Height;

            text = this.wallet.GetBalanceMarketPrice();
            if (text != null)
            {
                Font curFont = Theme.font8Bold;
                //text = "≈" + instrument.GetCurrencyPrice(wallet.Balance, 2);
                int bscolor = Color.A(Theme.appFore, GetAnimationValueInvert(255));
                g.DrawText(text, curFont, x + (width - curFont.GetWidth(text)) / 2, y, bscolor);
            }

        }
    }
}
