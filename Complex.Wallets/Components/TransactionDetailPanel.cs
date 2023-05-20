using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class TransactionDetailPanel : FloatingPanel
    {
        public TransactionDetailPanel(Wallet wallet)
        {
            this.wallet = wallet;
            MinHeight = 300;
            this.Padding.Set(6);
            this.Inflate.Set(0, 6);

            caption = new Caption("");
            caption.Dock = DockStyle.Top;
            caption.MinHeight = 50;
            caption.TextComponent.Font = Theme.font13Bold;
            caption.Padding.Set(10);
            caption.Inflate.Set(6, 0);
            this.Add(caption);

            this.timeComponent = new TextComponent("");
            this.timeComponent.Dock = DockStyle.Right;
            caption.Add(timeComponent);

            ImageButton closeButton = new ImageButton("dockClose2.svg");
            closeButton.Dock = DockStyle.Right;
            closeButton.DrawBorder = true;
            closeButton.MaxHeight = 32;
            closeButton.Radius = 16;
            closeButton.Executed += (s) =>
            {
                this.Hide();
            };
            caption.Add(closeButton);

            mainPanel = new MainPanel();
            mainPanel.Dock = DockStyle.Fill;

            Container container = new Container();
            container.Dock = DockStyle.Top;
            container.MinHeight = 50;

            nameText = new TextLocalizeComponent("");
            nameText.MinWidth = 200;
            nameText.Dock = DockStyle.Left;
            container.Add(nameText);

            senderInfo = new LargeLabel(null, null, null);
            senderInfo.Dock = DockStyle.Fill;
            container.Add(senderInfo);

            mainPanel.Add(container);

            this.Add(mainPanel);
        }

        private Wallet wallet;
        private Caption caption;
        private TextComponent timeComponent;
        private MainPanel mainPanel;
        private TextLocalizeComponent nameText;
        private LargeLabel senderInfo;

        private ITransaction transaction;
        public ITransaction Transaction
        {
            get => transaction;
            set
            {
                if (transaction == value) return;
                transaction = value;
                if (transaction != null)
                {
                    caption.BeginUpdate();
                    mainPanel.BeginUpdate();

                    DateTime time = transaction.Time.ToLocalTime();
                    timeComponent.Text = time.ToLongDateString() + " " + time.ToShortTimeString();
                    senderInfo.TextComponent.Text = transaction.Address;
                    if (transaction.IsOut)
                    {
                        caption.ImageID = "out_transaction.svg";
                        caption.TextID = "sended";
                        caption.AppendRightText = " " + "-" + transaction.Amount + " " + this.wallet.Symbol;
                        nameText.TextID = "receiver";
                    }
                    else
                    {
                        caption.ImageID = "in_transaction.svg";
                        caption.TextID = "received";
                        caption.AppendRightText = " " +  "+" + transaction.Amount + " " + this.wallet.Symbol;
                        nameText.TextID = "sender";
                    }
                    caption.EndUpdate();
                    mainPanel.EndUpdate();

                    this.BringToFront();
                    this.Show();
                }
                else
                {
                    this.Hide();
                }
            }
        }

        private class MainPanel : Container
        {
            public MainPanel()
            {
                this.Padding.Set(10);
            }

            private readonly Rect clientRect = new Rect();

            protected override void OnSizeChanged()
            {
                GetDisplayRectangle(clientRect);
                base.OnSizeChanged();
            }

            protected override void OnDrawBack(Graphics g)
            {
                g.FillRoundRect(clientRect, 16, Theme.back3);
            }
        }
    }
}
