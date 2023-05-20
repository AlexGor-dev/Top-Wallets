using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class TransactionExpandedPanel : Container
    {
        public TransactionExpandedPanel(Wallet wallet, ITransactionBase transaction)
        {
            this.wallet = wallet;
            this.transaction = transaction;
            this.tr = this.transaction as ITransaction;
            this.Padding.Set(10, 0, 10, 0);
            this.Alpha = 0;

            this.hash = transaction.Hash;
            Container container = null;
            TextComponent text = null;
            ImageButton button = null;


            if (tr != null)
            {
                this.address = tr.Address;

                if (!string.IsNullOrEmpty(tr.Message))
                {
                    container = new Container();
                    container.Padding.Set(20, 20, 10, 10);
                    container.Dock = DockStyle.Top;

                    text = new TextLocalizeComponent("message");
                    text.MinWidth = 200;
                    text.Alignment = ContentAlignment.Right;
                    text.AppendRightText = ":";
                    text.Dock = DockStyle.Left;
                    text.Style = Theme.Get<CaptionStyle>();
                    container.Add(text);



                    button = new ImageButton("copyAddress.svg");
                    button.MaxHeight = 20;
                    button.ToolTipInfo = new ToolTipInfo(button.Image, Language.Current["copy"] + " " + text.Text, null);
                    button.Dock = DockStyle.Left;
                    button.Executed += (s) =>
                    {
                        Clipboard.SetText(tr.Message);
                        MessageView.Show(Language.Current["message"] + " " + tr.Message + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                    };
                    container.Add(button);

                    UrlTextComponent uriText = new UrlTextComponent(tr.Message);
                    uriText.LinkExecuted += (s, l) => WinApi.ShellExecute(IntPtr.Zero, l);
                    uriText.Font = Theme.font10;
                    uriText.MultilineLenght = 40;
                    uriText.Padding.Set(16, 6, 16, 6);
                    uriText.Alignment = ContentAlignment.Center;
                    uriText.RoundBack = true;
                    uriText.RoundBackRadius = 10;
                    uriText.Dock = DockStyle.Left;
                    uriText.Style = Theme.Get<RoundLabelTheme>();
                    container.Add(uriText);

                    this.Add(container);
                }

                container = new Container();
                container.Padding.Set(20, 10, 10, 10);
                container.Dock = DockStyle.Top;


                text = new TextLocalizeComponent(tr.IsOut ? "addressRecipient" : "sendersAddress");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Right;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);




                button = new ImageButton("copyAddress.svg");
                button.MaxHeight = 20;
                button.ToolTipInfo = new ToolTipInfo(button.Image, "copyWalletAddress", null);
                button.Dock = DockStyle.Left;
                button.Executed += (s) =>
                {
                    Clipboard.SetText(this.address);
                    MessageView.Show(Language.Current["address"] + " " + this.address + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                };
                container.Add(button);

                text = new TextComponent(this.address);
                text.MaxWidth = 300;
                text.Dock = DockStyle.Left;
                container.Add(text);


                this.Add(container);
            }



            if (!string.IsNullOrEmpty(this.hash))
            {
                container = new Container();
                if(tr == null)
                    container.Padding.Set(20, 20, 10, 0);
                else
                    container.Padding.Set(20, 0, 10, 0);
                container.Dock = DockStyle.Top;

                text = new TextLocalizeComponent("hash");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Right;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);


                button = new ImageButton("copyAddress.svg");
                button.MaxHeight = 20;
                button.ToolTipInfo = new ToolTipInfo(button.Image, "copyHash", null);
                button.Dock = DockStyle.Left;
                button.Executed += (s) =>
                {
                    Clipboard.SetText(this.hash);
                    MessageView.Show(Language.Current["hash"] + " " + this.hash + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                };
                container.Add(button);

                text = new TextComponent(this.hash);
                text.MaxWidth = 300;
                text.Dock = DockStyle.Left;
                container.Add(text);

                this.Add(container);

            }




            //this.Add(new Separator(DockStyle.Top, 50));

            Container bot = new Container();
            bot.Padding.Set(20, 0, 20, 10);
            //bot.MinHeight = 54;
            bot.Dock = DockStyle.Top;

            if (tr != null && wallet.IsMain && tr.IsOut)
            {
                TextButton textButton = new TextButton("resendCoins");
                textButton.LangParam = this.wallet.Symbol + " coins";
                textButton.DrawBorder = true;
                textButton.Padding.Set(10, 0, 10, 0); textButton.Font = Theme.font10Bold;
                //textButton.BoxColor = this.wallet.Adapter.Color;
                textButton.Dock = DockStyle.CenterHorizontal;
                textButton.Executed += (s) =>
                {
                    Controller.ShowSendForm(this.wallet, s as TextButton, MenuAlignment.Top, this.address, (decimal)tr.Amount, tr.Message);
                };
                bot.Add(textButton);
            }



            timeComponent = new TextComponent(transaction.Time.ToLocalLongDateTimeString());
            timeComponent.Padding.Set(4, 2, 4, 2);
            timeComponent.MaxHeight = 20;
            timeComponent.RoundBack = true;
            timeComponent.Style = Theme.Get<RoundLabelTheme>();
            timeComponent.Font = Theme.font10;
            timeComponent.Dock = DockStyle.Right;
            bot.Add(timeComponent);

            text = new TextComponent("ID: " + transaction.ID);
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

        private Wallet wallet;
        private TextComponent timeComponent;
        private ITransactionBase transaction;
        private ITransaction tr;
        private string address;
        private string hash;

        private readonly Rect clientRect = new Rect();

        protected override void OnSizeChanged()
        {
            GetClientRectangle(clientRect);
            clientRect.Inflate(-4);
            base.OnSizeChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            if (this.Alpha > 0)
            {
                timeComponent.Text = transaction.Time.ToLocalLongDateTimeString();
                g.Smoosh(() =>
                {
                    int color = Theme.unselectedItemBackColor;
                    if (this.Parent.Selected)
                        color = Theme.selectedItemBackColor;
                    g.ShadowRoundRect(clientRect, 0, 10, Color.Offset(color, -8), color, 20);
                    g.DrawRoundRect(clientRect, 0, 10, color, 1);
                });
            }
        }
    }
}
