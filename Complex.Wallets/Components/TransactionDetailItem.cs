using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;

namespace Complex.Wallets
{
    public class TransactionDetailItem : Container
    {
        public TransactionDetailItem(Wallet wallet, ITransactionDetail detail, GridWaitEffect waitEffect)
        {
            this.waitEffect = waitEffect;
            this.wallet = wallet;
            this.detail = detail;
            this.Dock = DockStyle.Top;
            this.Padding.Set(4);


            Container container = new Container();
            container.Dock = DockStyle.Fill;
            container.Inflate.Set(10, 2);


            bot = new Container();
            bot.Dock = DockStyle.Bottom;

            if (!string.IsNullOrEmpty(detail.Address))
            {
                this.address = detail.Address;
                Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, this.address);
                string name = null;
                if (wt != null)
                    name = wt.Name;
                else
                    name = Controller.GetKnownAddress(wallet.Adapter, address);

                Container top = new Container();
                top.Dock = DockStyle.Top;

                ImageComponent imageComponent = new ImageComponent(detail.IsOut ? "out_transaction.svg" : "in_transaction.svg");
                imageComponent.Dock = DockStyle.Left;
                this.Add(imageComponent);


                TextComponent textComponent = new TextLocalizeComponent(detail.IsOut ? "sended" : "received");
                textComponent.MinWidth = 150;
                textComponent.Alignment = ContentAlignment.Left;
                textComponent.Dock = DockStyle.Left;
                textComponent.Style = captionForeTheme;
                top.Add(textComponent);

                if (!string.IsNullOrEmpty(detail.Type))
                {
                    typeComponent = new TextLocalizeComponent(detail.Type);
                    typeComponent.Dock = DockStyle.Left;
                    typeComponent.MaxWidth = 250;
                    typeComponent.MaxHeight = 20;
                    typeComponent.Padding.Set(4, 0, 4, 0);
                    top.Add(typeComponent);
                }

                TextButton textButton = new TextButton((detail.IsOut ? Language.Current["to"] : Language.Current["from"]) + ": " + name);
                textButton.MaxWidth = 250;
                textButton.Dock = DockStyle.Left;
                textButton.Alignment = ContentAlignment.Left;
                textButton.Executed += (s) =>
                {
                    if (this.waitEffect != null)
                        Controller.ShowAnyWallet(wallet.Adapter, wallet.Symbol, this.address, () => this.waitEffect.Start(), () => this.waitEffect.Stop());
                };
                textButton.RightClick += (s) =>
                {
                    this.wallet.CreateTransactionAddressMenu(this.address, (m) =>
                    {
                        if (m != null)
                            Application.Invoke(()=> m.Show(s as Component, MenuAlignment.Bottom));
                    });
                };
                bot.Add(textButton);

                if (!string.IsNullOrEmpty(detail.Message))
                {
                    messgComponent = new TextComponent(detail.Message);
                    messgComponent.MaxWidth = 250;
                    messgComponent.MaxHeight = 20;
                    messgComponent.Padding.Set(4, 0, 4, 0);
                    messgComponent.Dock = DockStyle.Left;
                    top.Add(messgComponent);
                }

                container.Add(top);
            }



            currencyLabel = new CurrencyLabel((detail.IsOut ? "-" : "+") + detail.Amount.GetTextSharps(8), detail.Amount.Symbol);
            currencyLabel.MaxHeight = 20;
            currencyLabel.CurrencyTextComponent.MaxWidth = 100;
            currencyLabel.Alignment = ContentAlignment.Right;
            currencyLabel.Dock = DockStyle.Right;
            currencyLabel.ValueTextComponent.ForeColor = detail.IsOut ? buySell.sellColor : buySell.buyColor;
            bot.Add(currencyLabel);

            container.Add(bot);

            this.Add(container);

        }

        protected static readonly BuySellTheme buySell = Theme.Get<BuySellTheme>();

        private GridWaitEffect waitEffect;
        protected readonly Wallet wallet;
        protected readonly ITransactionDetail detail;
        private string address;
        private readonly Point point = new Point();

        private static CaptionForeTheme captionForeTheme = Theme.Get<CaptionForeTheme>();

        private TextComponent messgComponent;
        private TextLocalizeComponent typeComponent;
        protected readonly Container bot;
        protected readonly CurrencyLabel currencyLabel;
        protected override void OnDrawBack(Graphics g)
        {
            if (messgComponent != null || typeComponent != null)
            {
                g.Smoosh(() =>
                {
                    int color = Theme.unselectedItemBackColor;
                    int back2 = Theme.selectedItemBackColor;
                    if (this.Parent.Parent.Selected)
                    {
                        color = Theme.selectedItemBackColor;
                        back2 = Theme.unselectedItemBackColor;
                    }
                    if (messgComponent != null)
                    {
                        messgComponent.GetOffset(this, point);
                        g.FillRoundRect(point.x, point.y, messgComponent.Width, messgComponent.Height, messgComponent.Height / 2, back2);
                    }
                    if (typeComponent != null)
                    {
                        typeComponent.GetOffset(this, point);
                        g.FillRoundRect(point.x, point.y, typeComponent.Width, typeComponent.Height, typeComponent.Height / 2, back2);
                    }

                });
            }
        }

        protected override void OnLayout()
        {
            base.OnLayout();
        }

    }
}
