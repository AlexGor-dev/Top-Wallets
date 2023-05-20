using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class TransactionMessageContainer : Container, IMessageDataSource
    {
        public TransactionMessageContainer(Wallet wallet, ITransactionBase transaction, ITransactionDetail detail)
        {
            this.transaction = transaction;
            this.detail = detail;
            //this.Padding.Set(10);

            Caption caption = new Caption(wallet.Name);
            caption.Dock = DockStyle.Top;
            this.Add(caption);

            Container container = new Container();
            container.MaxWidth = 350;
            container.Dock = DockStyle.Fill;
            container.Padding.Set(6);
            container.Inflate.height = 2;

            Label typeCaption = new Label(detail.IsOut ? "outTransaction" : "inTransaction");
            typeCaption.Padding.Set(0);
            typeCaption.TextComponent.Alignment = ContentAlignment.Center;
            typeCaption.TextComponent.Style = Theme.Get<CaptionForeTheme>();
            typeCaption.Dock = DockStyle.Top;
            typeCaption.Font = Theme.font11Bold;
            container.Add(typeCaption);

            string destText = null;
            if (!string.IsNullOrEmpty(detail.Address))
            {
                Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, detail.Address);
                string name = null;
                if (wt != null)
                    name = wt.Name;
                else
                    name = wallet.Adapter.GetKnownAddress(detail.Address);

                LargeLabel label = new LargeLabel(null, detail.IsOut ? "to" : "from", name != null ? name : detail.Address);
                label.Padding.Set(0);
                label.TextComponent.AppendRightText = ":";
                label.DescComponent.MinWidth = 200;
                label.MinHeight = 20;
                label.TextComponent.Dock = DockStyle.Left;
                label.DescComponent.Dock = DockStyle.Fill;
                label.Dock = DockStyle.TopCenter;
                label.Inflate.Set(10, 0);
                label.DescComponent.ForeColor = wallet.ThemeColor;
                container.Add(label);

                destText = label.TextComponent.Text + " " + label.DescComponent.Text;
            }

            string messageText = null;

            if (!string.IsNullOrEmpty(detail.Message))
            {
                messageText = " \"" + detail.Message + "\" ";
                TextComponent text = new TextComponent(detail.Message);
                text.Dock = DockStyle.Fill;
                text.MultilineLenght = 30;
                text.MaxHeight = 100;
                container.Add(text);

                container.Add(new Dummy(DockStyle.Bottom, 0, 10));
            }

            Container bot = new Container();
            bot.Dock = DockStyle.Bottom;
            bot.Inflate.width = 6;


            currencyLabel = new CurrencyLabel(detail.Amount.ToKMBPlus(wallet.Balance.DefaultSignCount), detail.Amount.Symbol);
            currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
            currencyLabel.Dock = DockStyle.Left;
            bot.Add(currencyLabel);

            if (wallet.IsSupportMarket)
            {
                CurrencyLabel curLabel = new CurrencyLabel(((decimal)detail.Amount * wallet.Market.LastPrice).GetTextSharps(3), MainSettings.Current.General.Currency.ID);
                curLabel.ValueTextComponent.Font = Theme.font9Bold;
                curLabel.ValueTextComponent.AppendLeftText = "≈";
                curLabel.Dock = DockStyle.Left;
                bot.Add(curLabel);
            }

            TextComponent timeComponent = new TextComponent(transaction.Time.ToLocalLongDateTimeString());
            timeComponent.Padding.Set(4, 0, 4, 0);
            timeComponent.Dock = DockStyle.Right;
            timeComponent.Font = Theme.font9;
            bot.Add(timeComponent);

            container.Add(bot);

            this.Add(container);

            messageData = new MessageData(wallet.Name, typeCaption.Text + " " + destText + messageText + detail.Amount + " " + detail.Amount.Symbol + " " + timeComponent.Text, MessageViewType.Message, 4000);
        }

        private ITransactionBase transaction;
        private ITransactionDetail detail;
        private CurrencyLabel currencyLabel;

        private MessageData messageData;

        UpDownTheme upDown = Theme.Get<UpDownTheme>();

        public IMessageData MessageData => messageData;

        private readonly Rect dispRect = new Rect();

        protected override void OnSizeChanged()
        {
            GetDisplayRectangle(dispRect);
            base.OnSizeChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            currencyLabel.ValueTextComponent.ForeColor = detail.IsOut ? upDown.downColor : upDown.upColor;
            g.Smoosh(() => g.FillRoundRect(dispRect, 10, Theme.unselectedItemBackColor));
        }

    }
}
