using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Trader;
using Complex.Animations;

namespace Complex.Wallets
{
    public class TransactionGroupItem : Container, IFocusedComponent, IEndAnimation
    {
        public TransactionGroupItem(Wallet wallet, GridWaitEffect waitEffect, ITransactionBase transaction, params ITransactionDetail[] details)
        {
            this.waitEffect = waitEffect;
            this.wallet = wallet;
            this.transaction = transaction;

            this.Padding.Set(8, 4, 8, 4);

            this.expandedPanel = new TransactionExpandedPanel(wallet, transaction);
            this.expandedPanel.Dock = DockStyle.Fill;
            this.Add(this.expandedPanel);

            main = new Container();
            main.Dock = DockStyle.Top;

            if (!string.IsNullOrEmpty(transaction.Name))
            {
                Caption nameCaption = new Caption(transaction.Name);
                nameCaption.Padding.Set(30, 0, 30, 0);
                nameCaption.Dock = DockStyle.Top;
                main.Add(nameCaption);

                main.Add(new Dummy(DockStyle.Top, 0, 4));
            }

            foreach (ITransactionDetail detail in details)
                main.Add(wallet.CreateTransactionDetailItem(detail, waitEffect));

            bot = new Container();
            bot.Inflate.width = 10;
            bot.Padding.Set(6, 0, 0, 0);
            bot.Dock = DockStyle.Top;

            ExpandButton button = new ExpandButton(false);
            button.MaxSize.Set(26, 26);
            button.Dock = DockStyle.Left;
            button.CheckedChanged += (s) =>
            {
                this.Expanded = (s as ExpandButton).Checked;
            };
            bot.Add(button);

            timeComponent = new TextComponent(GetTimeText());
            timeComponent.TextChanged += (s) =>
            {
                (s as TextComponent).Measured = false;
                bot.Layout();
            };
            timeComponent.Padding.Set(4, 0, 4, 0);
            timeComponent.Dock = DockStyle.Left;
            timeComponent.Alignment = ContentAlignment.Left;
            timeComponent.MaxHeight = 20;
            bot.Add(timeComponent);

            CurrencyLabel label = new CurrencyLabel(transaction.Fee > 0 ? Language.Current["fee"] + ": " + transaction.Fee.GetTextSharps(8) : null, transaction.Fee > 0 ? transaction.Fee.Symbol : null);
            label.Alignment = ContentAlignment.Right;
            label.Dock = DockStyle.Right;
            label.ValueTextComponent.Style = Theme.Get<ForeTheme>();
            bot.Add(label);




            main.Add(bot);

            this.Add(main);

            expandAnimator = new Animator(this, 1, 255);
        }

        private GridWaitEffect waitEffect;
        protected readonly Wallet wallet;
        private ITransactionBase transaction;
        protected readonly Container main;
        protected readonly Container bot;
        private TextComponent timeComponent;
        private Animator expandAnimator;
        private TransactionExpandedPanel expandedPanel;

        public InsertedAnyView ListView => this.Parent as InsertedAnyView;

        private readonly Rect expandRect = new Rect();
        private bool expanded = false;
        public bool Expanded
        {
            get => expanded;
            set
            {
                if (expanded == value) return;
                expanded = value;
                expandAnimator.Start(value ? 1 : -1);
            }
        }

        protected override void OnKeyDown(KeyEvent e)
        {
            switch (e.Key)
            {
                case Complex.Key.Enter:
                    this.Expanded = !this.Expanded;
                    break;
            }

            base.OnKeyDown(e);
        }
        protected override void OnMeasure(float widthMeasure, float heightMeasure)
        {
            base.OnMeasure(widthMeasure, heightMeasure);
            expandedPanel.Measure();
            SetMeasured(0, Padding.vertical + main.MeasuredHeight + expandAnimator.GetValue(expandedPanel.MeasuredHeight));
        }

        protected override void OnSizeChanged()
        {
            expandRect.Set(expandedPanel.Left + expandedPanel.Padding.left, expandedPanel.Top
                , expandedPanel.Width - expandedPanel.Padding.horizontal, expandedPanel.Height);
            base.OnSizeChanged();
        }

        void IAnimation.OnAnimation(Animator animator, float value)
        {
            this.expandedPanel.Alpha = animator.GetValue(255);
            Measured = false;
            this.ListView.Relayout();
            this.ListView.Parent.Invalidate();
        }

        void IEndAnimation.OnEndAnimation(Animator animator, float value)
        {
            this.ListView.EnsureVisibleAnimation(this);
        }

        private string GetTimeText()
        {
            if (Complex.Wallets.MainSettings.Current.General.RelativeTimeTransactions)
                return (DateTime.UtcNow - transaction.Time).ToYMD() + " " + Language.Current["ago"];
            return transaction.Time.ToLocalLongDateTimeString();
        }


        protected override void OnDrawBack(Graphics g)
        {
            timeComponent.Text = GetTimeText();

            g.Smoosh(() =>
            {
                int color = Theme.unselectedItemBackColor;
                int back2 = Theme.selectedItemBackColor;
                if (this.Selected)
                {
                    color = Theme.selectedItemBackColor;
                    back2 = Theme.unselectedItemBackColor;
                }
                g.FillRoundRect(0, 0, main.Width + Padding.horizontal, main.Height + Padding.vertical, 10, color);
                g.FillRoundRect(main.Left + bot.Left + timeComponent.Left, main.Top + bot.Top + timeComponent.Top, timeComponent.Width, timeComponent.Height, timeComponent.Height / 2, back2);
                //if (messgComponent != null)
                //    g.FillRoundRect(main.Left + bot.Left + messgComponent.Left, main.Top + bot.Top + messgComponent.Top, messgComponent.Width, messgComponent.Height, 6, back2);
            });
        }

        void IFocusedComponent.OnFocusedChanged()
        {
        }
    }
}
