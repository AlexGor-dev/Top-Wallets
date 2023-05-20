using System;
using Complex.Controls;
using Complex.Themes;

namespace Complex.Wallets
{
    public class ReceiveMenu : Menu
    {
        public ReceiveMenu(Wallet wallet)
        {
            this.AnimationMode = true;
            ReceiveOrherPanel mainPanel = new ReceiveOrherPanel(wallet, this.Hide);
            mainPanel.Dock = DockStyle.Fill;
            this.Container.Add(mainPanel);
        }

        public class ReceiveOrherPanel : CaptionPanel
        {
            public ReceiveOrherPanel(Wallet wallet, EmptyHandler closeHandler)
                : base("walletAddress", " " + wallet.NameOrCoins + " " + wallet.Adapter.NetName, null, closeHandler, "copyQr", wallet.ThemeColor, () => { })
            {
                this.wallet = wallet;

                TextComponent addressComponent = new TextComponent(wallet.Address);
                addressComponent.Padding.Set(10, 0, 10, 0);
                addressComponent.RoundBack = true;
                addressComponent.Dock = DockStyle.Top;
                addressComponent.MinHeight = 30;
                addressComponent.Style = Theme.Get<RoundLabelTheme>();
                this.Add(addressComponent);

                qRComponent = new QRComponent();
                qRComponent.Bitmap = wallet.Adapter.GenerateQRCode(wallet.Address, 150, 150);
                qRComponent.Dock = DockStyle.Fill;
                this.Add(qRComponent);

                this.continueButton.BringToFront();
            }

            private Wallet wallet;
            private QRComponent qRComponent;

            protected override void Continue()
            {
                Clipboard.SetBitmap(qRComponent.Bitmap);
                MessageView.Show(Language.Current["invoiceQr"] + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                closeHandler();

            }

        }

    }
}
