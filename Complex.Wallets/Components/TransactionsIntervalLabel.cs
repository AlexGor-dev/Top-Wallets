using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Drawing;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public class TransactionsIntervalLabel : Container
    {
        public TransactionsIntervalLabel(Wallet wallet)
        {
            this.wallet = wallet;
            this.Padding.Set(4, 0, 4, 0);
            this.MaxHeight = 24;
            this.Init();
        }

        private void Init()
        {
            this.switchText = new SwitchTextComponent();
            this.switchText.Dock = DockStyle.Fill;
            this.switchText.Style = Theme.Get<CaptionStyle>();
            this.switchText.TextChanged += (s) =>
            {
                this.Measured = false;
            };
            this.Add(this.switchText);

            this.menuButton = new MenuButton("time_interval.svg", null);
            this.menuButton.ToolTipInfo = new ToolTipInfo(this.menuButton.ImageID, "transactionsIntervals", null);
            this.menuButton.MenuAnimationMode = true;
            this.menuButton.MaxHeight = 20;
            this.menuButton.TwoStrip = false;
            this.menuButton.Dock = DockStyle.Right;
            this.menuButton.InitMenu += delegate (object s, Container menu)
            {
                this.UpdateIntervals();
                menu.Padding.Set(10);
                menu.Inflate.Set(0, 4);
                menu.Add(new MenuStripLabelLocalize("transactionsIntervals"));
                MenuStripRadioButton button = null;
                foreach (Interval interval in this.intervals)
                {
                    button = new MenuStripRadioButton(Language.Current["for"] + " " + interval.ToLang(), this.interval == interval);
                    button.Tag = interval;
                    button.Executed += (s1) => { this.Interval = (Interval)(s1 as MenuStripRadioButton).Tag; };
                    menu.Add(button);
                }
                button = new MenuStripRadioButton("allLoadedTransactions", this.interval == Interval.Max);
                button.Tag = Interval.Max;
                button.Executed += (s1) => { this.Interval = (Interval)(s1 as MenuStripRadioButton).Tag; };
                menu.Add(button);
            };
            this.Add(menuButton);
        }

        public event Handler IntervalChanged;

        private Wallet wallet;

        private SwitchTextComponent switchText;
        private MenuButton menuButton;
        private RoundCaptionLabelTheme style = Theme.Get<RoundCaptionLabelTheme>();
        private readonly Rect dispRect = new Rect();

        private Array<Interval> intervals = new Array<Interval>();

        private Interval interval = Interval.Max;
        public Interval Interval
        {
            get => interval;
            set
            {
                if (this.interval == value) return;
                this.interval = value;
                this.OnIntervalChanged();
            }
        }

        private void OnIntervalChanged()
        {
            Events.Invoke(this.IntervalChanged, this);
        }

        private void UpdateIntervals()
        {
            this.intervals.Clear();
            DateTime now = DateTime.UtcNow;
            foreach (ITransactionBase transaction in this.wallet.Transactions)
            {
                TimeSpan span = now - transaction.Time;
                if ((int)span.TotalMinutes <= 1)
                    intervals.AddUnique(Interval.M1);
                else if ((int)span.TotalMinutes <= 30)
                    intervals.AddUnique(Interval.M30);
                else if ((int)span.TotalHours <= 1)
                    intervals.AddUnique(Interval.H1);
                else if ((int)span.TotalHours <= 3)
                    intervals.AddUnique(Interval.H3);
                else if ((int)span.TotalHours <= 6)
                    intervals.AddUnique(Interval.H6);
                else if ((int)span.TotalHours <= 12)
                    intervals.AddUnique(Interval.H12);
                else if ((int)span.TotalDays <= 1)
                    intervals.AddUnique(Interval.D1);
                else if ((int)span.TotalDays <= 3)
                    intervals.AddUnique(Interval.D3);
                else if (span.TotalWeeks() <= 1)
                    intervals.AddUnique(Interval.W1);
                else if (span.TotalWeeks() <= 2)
                    intervals.AddUnique(Interval.W2);
                else if (span.TotalMonth() <= 1)
                    intervals.AddUnique(Interval.Mon1);
                else if (span.TotalMonth() <= 3)
                    intervals.AddUnique(Interval.Mon3);
                else if (span.TotalMonth() <= 6)
                    intervals.AddUnique(Interval.Mon6);
                else if (span.TotalYears() <= 1)
                    intervals.AddUnique(Interval.Y1);
                else if (span.TotalYears() <= 3)
                    intervals.AddUnique(Interval.Y3);
                else if (span.TotalYears() <= 5)
                    intervals.AddUnique(Interval.Y5);
            }
            intervals.Sort((a, b) => { return a.CompareTo(b); });
        }

        public bool Update()
        {
            string text = this.switchText.Text;
            if (this.interval != Interval.Max)
            {
                this.switchText.TextID = Language.Current["for"] + " " + this.interval.ToLang();
            }
            else
            {
                ITransactionBase last = this.wallet.Transactions.Last;
                if (last != null)
                    this.switchText.Text = Language.Current["for"] + " " + (DateTime.UtcNow - last.Time).ToYMD();
                else
                    this.switchText.TextID = "noTransaction";
            }
            return text != this.switchText.Text;
        }

        protected override void OnSizeChanged()
        {
            GetDisplayRectangle(dispRect);
            base.OnSizeChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            g.Smoosh(() =>
            {
                float radius = dispRect.height / 2;
                g.ShadowRoundRect(dispRect, 4, radius, radius, Theme.back3, Color.Offset(Theme.back3, -10), 8);
                g.FillRoundRect(dispRect, radius, style.backColor);
            });
        }

    }
}
