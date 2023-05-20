using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Navigation;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public class JettonWalletInfoContainer : ButtonsPanel
    {
        public JettonWalletInfoContainer(TonUnknownWallet wallet, bool activeButton)
        {
            this.wallet = wallet;
            this.activeButton = activeButton;

            this.Inflate.Set(0, 10);
            this.Padding.Set(10);
            this.BackRadius = 10;

            Caption caption = new Caption("Jetton Wallet");
            caption.Dock = DockStyle.Top;
            this.Add(caption);

            Container container = new Container();
            container.Dock = DockStyle.Top;

            TextComponent text = new TextLocalizeComponent("ownerAddress");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            ownerAddressText = new TextButton("");
            ownerAddressText.MaxWidth = 250;
            ownerAddressText.Dock = DockStyle.Left;
            ownerAddressText.Executed += (s) =>
            {
                if (this.activeButton && !string.IsNullOrEmpty(ownertAddress))
                {
                    Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Symbol, ownertAddress);
                    this.Form.Hide();
                }
            };
            container.Add(ownerAddressText);

            ImageButton button = new ImageButton("copyAddress.svg");
            button.MaxHeight = 20;
            button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
            button.Dock = DockStyle.Left;
            button.Executed += (s) =>
            {
                Clipboard.SetText(ownertAddress);
                MessageView.Show(Language.Current["address"] + " " + ownertAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            };
            container.Add(button);
            this.Add(container);



            container = new Container();
            container.Dock = DockStyle.Top;

            text = new TextLocalizeComponent("jettonWalletAddress");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            jettonWalletAddressText = new TextButton("");
            jettonWalletAddressText.MaxWidth = 250;
            jettonWalletAddressText.Dock = DockStyle.Left;
            jettonWalletAddressText.Executed += (s) =>
            {
                if (this.activeButton)
                {
                    Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Symbol, jettonWalletAddress);
                    this.Form.Hide();
                }
            };
            container.Add(jettonWalletAddressText);

            button = new ImageButton("copyAddress.svg");
            button.MaxHeight = 20;
            button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
            button.Dock = DockStyle.Left;
            button.Executed += (s) =>
            {
                Clipboard.SetText(jettonWalletAddress);
                MessageView.Show(Language.Current["address"] + " " + jettonWalletAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            };
            container.Add(button);

            this.Add(container);

            container = new Container();
            container.Dock = DockStyle.Top;

            text = new TextLocalizeComponent("balance");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            currencyLabel = new CurrencyLabel(null, null);
            currencyLabel.Dock = DockStyle.Left;
            container.Add(currencyLabel);
            this.Add(container);

            container = new Container();
            container.Dock = DockStyle.Top;

            text = new TextComponent("Jetton");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            jettonInfoButton = new CheckedTextButton(null, false);
            jettonInfoButton.Dock = DockStyle.Left;
            jettonInfoButton.MaxWidth = 250;
            jettonInfoButton.CheckedChanged += (s) =>
            {
                CheckedTextButton cb = s as CheckedTextButton;
                if (cb.Checked)
                {
                    JettonMenu menu = new JettonMenu(this.wallet, this.walletInfo.JettonInfo, false);
                    menu.Hided += (s2) => cb.Checked = false;
                    menu.Show(s as Component, MenuAlignment.BottomLeft);
                }
            };
            container.Add(jettonInfoButton);
            this.Add(container);

        }

        private TonUnknownWallet wallet;
        private bool activeButton;

        private TextButton ownerAddressText;
        private TextButton jettonWalletAddressText;
        private CurrencyLabel currencyLabel;
        private CheckedTextButton jettonInfoButton;
        private JettonWalletInfo walletInfo;
        private string jettonWalletAddress;
        private string ownertAddress;

        public void Update(JettonWalletInfo walletInfo)
        {
            this.walletInfo = walletInfo;

            jettonWalletAddress = walletInfo.Owner;
            ownertAddress = walletInfo.Address;

            ownerAddressText.Text = walletInfo.Owner;
            jettonWalletAddressText.Text = walletInfo.Address;

            currencyLabel.ValueTextComponent.Text = walletInfo.Balance.GetTextSharps(walletInfo.JettonInfo.Decimals);
            currencyLabel.ValueTextComponent.ForeColor = walletInfo.JettonInfo.ThremeColor;
            currencyLabel.CurrencyTextComponent.Text = walletInfo.JettonInfo.Symbol;

            jettonInfoButton.Text = walletInfo.JettonInfo.Name;

        }
    }
}
