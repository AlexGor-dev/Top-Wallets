using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public class TransactionsChangedLabel : CircleArrowDualComponent
    {
        public TransactionsChangedLabel(Wallet wallet)
        {
            this.wallet = wallet;
            this.HoveredAnimation = true;
            this.ArcAlpha = 150;
            if(this.wallet.IsSupportMarket)
                this.wallet.Market.LastPriceChanged += Wallet_CoinLastPriceChanged;
        }
        protected override void OnDisposed()
        {
            this.wallet.Market.LastPriceChanged -= Wallet_CoinLastPriceChanged;
            base.OnDisposed();
        }

        private void Wallet_CoinLastPriceChanged(object sender)
        {
            this.Invalidate();
        }


        private Wallet wallet;

        private decimal upVolume;
        private decimal downVolume;

        private int upTransactions;
        private int downTransactions;

        private BalloonComponent upBallon;
        private BalloonComponent downpBallon;

        private BuySellTheme buySell = Theme.Get<BuySellTheme>();
        private ToolTipStyle style = Theme.Get<ToolTipStyle>();

        private Interval interval;

        public void Update(Interval interval)
        {
            this.interval = interval;
            decimal up = 0;
            decimal down = 0;
            this.upTransactions = 0;
            this.downTransactions = 0;

            if (this.wallet.Transactions.Last == null)
            {
                this.Start(0, 0, "0", this.wallet.Symbol, false);
            }
            else
            {
                DateTime endTime = DateTime.MinValue;
                if (interval != Interval.Max)
                    endTime = DateTime.UtcNow.AddMinutes(-((int)interval * 2 + 1));

                foreach (ITransactionBase transaction in this.wallet.Transactions)
                {
                    if (transaction.Time < endTime)
                        break;
                    decimal amount = transaction.GetAmount(this.wallet.Symbol);
                    if (amount < 0)
                    {
                        down -= amount;
                        this.downTransactions++;
                    }
                    else
                    {
                        up += amount;
                        this.upTransactions++;
                    }
                }
                if (up != this.upVolume || down != this.downVolume || this.BottomText != this.wallet.Symbol)
                {
                    this.upVolume = up;
                    this.downVolume = down;
                    decimal delta = this.upVolume - this.downVolume;
                    this.Start(this.upVolume, this.downVolume, delta.ToKMBPlus(3), this.wallet.Symbol, true);
                }
            }
        }

        protected override void OnUpShowed(float cx, float cy)
        {
            if (this.upBallon == null)
            {
                this.upBallon = new BalloonComponent();
                this.Parent.Add(this.upBallon);
            }
            this.upBallon.Show(Left + cx, Top + cy, MenuAlignment.LeftTop, this.CreateBallonContainer(false));
        }

        protected override void OnDownShowed(float cx, float cy)
        {
            if (this.downpBallon == null)
            {
                this.downpBallon = new BalloonComponent();
                this.Parent.Add(this.downpBallon);
            }
            this.downpBallon.Show(Left + cx, Top + cy, MenuAlignment.RighTop, this.CreateBallonContainer(true));
        }

        protected override void OnUpHided()
        {
            if (this.upBallon != null)
                this.upBallon.Hide();
            base.OnUpHided();
        }

        protected override void OnDownHided()
        {
            if (this.downpBallon != null)
                this.downpBallon.Hide();
            base.OnDownHided();
        }

        private Container CreateBallonContainer(bool isOut)
        {
            Container container = new Container();
            container.Inflate.Set(0, 4);
            container.Padding.Set(0, 0, 0, 6);
            ITransactionBase last = this.wallet.Transactions.Last;

            string interval = this.interval.ToLang();
            if (this.interval == Interval.Max && last != null)
                interval = (DateTime.UtcNow - last.Time).ToYMD();
            Caption caption = new Caption(isOut ? "out_transaction.svg" : "in_transaction.svg", Language.Current[isOut ? "sended" : "received"] + " " + Language.Current["for"] + " " + interval);
            caption.RoundRadius = 10;
            caption.Padding.Set(4);
            caption.Dock = DockStyle.Top;
            caption.ImageComponent.ImageColor = isOut ? buySell.sellColor : buySell.buyColor;
            //caption.Style = style.captionStyle;

            container.Add(caption);

            CurrencyLabel label = new CurrencyLabel(isOut ? (-downVolume).ToKMBPlus(3) : upVolume.ToKMBPlus(3), this.wallet.Symbol);
            label.ValueTextComponent.ForeColor = isOut ? buySell.sellColor : buySell.buyColor;
            label.ValueTextComponent.Font = Theme.font12Bold;
            label.Dock = DockStyle.Top;
            container.Add(label);

            TextComponent text;
            //TextComponent text = new TextComponent((isOut ? (-downVolume).ToKMBPlus(3) : upVolume.ToKMBPlus(3)) + " " + this.wallet.Adapter.Symbol);
            //text.ForeColor = isOut ? buySell.sellColor : buySell.buyColor;
            //text.Font = Theme.font12Bold;
            //text.Dock = DockStyle.Top;
            //container.Add(text);

            string priceText = this.wallet.GetMarketPrice(isOut ? downVolume : upVolume);
            if (priceText != null)
            {
                text = new TextComponent(priceText);
                text.Style = Theme.Get<CaptionForeTheme>();
                text.Font = Theme.font9Bold;
                text.Dock = DockStyle.Top;
                container.Add(text);
            }

            container.Add(new Separator(DockStyle.Top, 40));

            text = new TextComponent(Language.Current["transactions"] + " " + (isOut ? downTransactions : upTransactions));
            text.Font = Theme.font9Bold;
            text.Dock = DockStyle.Top;
            container.Add(text);


            return container;
        }
    }
}
