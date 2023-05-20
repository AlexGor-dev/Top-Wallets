using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class TransactionWaitPanel : WaitMessagePanel
    {
        public TransactionWaitPanel(string captionID, string captionAppendText, string descriptonTextID, EmptyHandler closeHandler, string continueTextID, int continueColor, EmptyHandler continueHandler)
            :base(captionID, captionAppendText, descriptonTextID, closeHandler, continueTextID, continueColor, continueHandler)
        {

        }

        public void AddTransaction(string name, Wallet wallet, ITransactionBase transaction)
        {
            Application.Invoke(() =>
            {
                if (transaction is ITransactionGroup g)
                    this.AddMessages(new GroupItem(name, wallet, transaction, g.Details.ToArray()));
                else if (transaction is ITransactionDetail tr)
                    this.AddMessages(new GroupItem(name, wallet, transaction, tr));
            });
        }

        public void AddTransactions(params TransactionsInfo[] infos)
        {
            Application.Invoke(() =>
            {
                Component[] components = new Component[infos.Length];
                for (int i = 0; i < components.Length; i++)
                {
                    TransactionsInfo info = infos[i];
                    if (info.transaction is ITransactionGroup g)
                        components[i] = new GroupItem(info.name, info.wallet, info.transaction, g.Details.ToArray());
                    else if (info.transaction is ITransactionDetail tr)
                        components[i] = new GroupItem(info.name, info.wallet, info.transaction, tr);

                }
                this.AddMessages(components);
            });
        }

        private class GroupItem : Container, IFocusedComponent
        {
            public GroupItem(string name, Wallet wallet, ITransactionBase transaction, params ITransactionDetail[] details)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    this.Padding.Set(4);
                    Caption caption = new Caption(name);
                    caption.Padding.Set(30, 0, 30, 0);
                    caption.Dock = DockStyle.Top;
                    this.Add(caption);
                }

                Container container = new Container();
                container.Dock = DockStyle.Fill;

                foreach (ITransactionDetail detail in details)
                {
                    Component component = wallet.CreateTransactionDetailItem(detail, null);
                    component.Dock = DockStyle.Top;
                    container.Add(component);
                }

                this.Add(container);

                Container bot = new Container();
                bot.Inflate.width = 10;
                bot.Padding.Set(6, 0, 0, 0);
                bot.Dock = DockStyle.Bottom;


                CurrencyLabel label = new CurrencyLabel(transaction.Fee > 0 ? Language.Current["fee"] + ": " + transaction.Fee.GetTextSharps(8) : null, transaction.Fee > 0 ? transaction.Fee.Symbol : null);
                label.Alignment = ContentAlignment.Right;
                label.Dock = DockStyle.Right;
                label.ValueTextComponent.Style = Theme.Get<ForeTheme>();
                bot.Add(label);

                TextComponent text = new TextComponent("ID: " + transaction.ID);
                text.MaxWidth = 300;
                text.MaxHeight = 20;
                text.Padding.Set(4, 2, 4, 2);
                text.RoundBack = true;
                text.Style = Theme.Get<RoundLabelTheme>();
                text.Font = Theme.font10;
                text.Dock = DockStyle.Left;
                bot.Add(text);

                this.Add(bot);
            }

            void IFocusedComponent.OnFocusedChanged()
            {
            }

            private readonly Rect dispRect = new Rect();

            protected override void OnSizeChanged()
            {
                GetDisplayRectangle(dispRect);
                base.OnSizeChanged();
            }

            protected override void OnDrawBack(Graphics g)
            {
                g.Smoosh(() =>
                {
                    int color = Theme.unselectedItemBackColor;
                    if (this.Selected)
                        color = Theme.selectedItemBackColor;
                    g.FillRoundRect(dispRect, 10, color);
                });

            }

        }
    }

    public class TransactionsInfo
    {
        public TransactionsInfo(Wallet wallet, string name, ITransactionBase transaction)
        {
            this.wallet = wallet;
            this.name = name;
            this.transaction = transaction;
        }

        public readonly Wallet wallet;
        public readonly string name;
        public readonly ITransactionBase transaction;
    }
}
