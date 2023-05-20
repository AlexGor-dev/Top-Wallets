using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;

namespace Complex.Wallets
{
    public class BuySellPanel : Container
    {
        public BuySellPanel(Wallet wallet)
        {
            this.wallet = wallet;
            this.Padding.Set(6);

            buyButton = new ColorCheckedButton("buyCoins", true);
            buyButton.LangParam = wallet.Symbol + " coins";
            buyButton.Enabled = false;
            buyButton.Padding.Set(4);
            buyButton.Dock = DockStyle.CenterHorizontal;
            buyButton.BoxColor = Color.Argb(150, buySell.upColor);
            buyButton.CheckedChanged += (s) =>
            {
                ColorCheckedButton bt = s as ColorCheckedButton;
                if (!bt.Checked)
                {
                    BuySellMenu menu = new BuySellMenu(wallet.Adapter, this.info.BuySellActors, OrderAction.Buy);
                    menu.ActionComponent = bt;
                    menu.Hided += (s2) =>
                    {
                        menu = s2 as BuySellMenu;
                        bt.Checked = true;
                        menu.Dispose();
                    };
                    menu.Show(bt, MenuAlignment.Bottom);
                }
            };
            this.Add(buyButton);

            sellButton = new ColorCheckedButton("sellCoins", true);
            sellButton.LangParam = wallet.Symbol + " coins";
            sellButton.Enabled = false;
            sellButton.Padding.Set(4);
            sellButton.Dock = DockStyle.CenterHorizontal;
            sellButton.BoxColor = Color.Argb(150, buySell.downColor);
            sellButton.CheckedChanged += (s) =>
            {
                ColorCheckedButton bt = s as ColorCheckedButton;
                if (!bt.Checked)
                {
                    BuySellMenu menu = new BuySellMenu(wallet.Adapter, this.info.BuySellActors, OrderAction.Sell);
                    menu.ActionComponent = bt;
                    menu.Hided += (s2) =>
                    {
                        menu = s2 as BuySellMenu;
                        bt.Checked = true;
                        menu.Dispose();
                    };
                    menu.Show(bt, MenuAlignment.Bottom);
                }
            };
            this.Add(sellButton);

            this.info = MainSettings.Current.Remote.CryptoCoins[this.wallet.AdapterID];
            info.LoadImages(() =>
            {
                Application.Invoke(() =>
                {
                    buyButton.Enabled = true;
                    sellButton.Enabled = true;
                });
            });
        }

        private Wallet wallet;

        private UpDownTheme buySell = Theme.Get<UpDownTheme>();
        private ColorCheckedButton buyButton;
        private ColorCheckedButton sellButton;
        private CryptoCoinInfo info;

        protected override Type GetDefaultTheme()
        {
            return typeof(MapBackTheme);
        }

        protected override void OnStyleChanged()
        {
            base.OnStyleChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            buyButton.BoxColor = Color.Argb(150, buySell.upColor);
            sellButton.BoxColor = Color.Argb(150, buySell.downColor);
            base.OnDrawBack(g);
        }
    }
}
