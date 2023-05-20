using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class JettonInfoContainer : ButtonsPanel
    {
        public JettonInfoContainer(TonUnknownWallet wallet, bool activeButton)
        {
            this.wallet = wallet;
            this.activeButton = activeButton;

            this.Inflate.Set(0, 10);
            this.Padding.Set(10);
            this.BackRadius = 10;

            Caption caption = new Caption("Jetton");
            caption.Dock = DockStyle.Top;
            this.Add(caption);

            infoLabel = new LargeLabelNotLocalize("jetton.svg", null, null, false);
            infoLabel.Inflate.Set(10, 6);
            infoLabel.ImageComponent.MaxSize.Set(64, 64);
            infoLabel.ImageComponent.MinSize.Set(64, 64);
            infoLabel.DescComponent.MultilineLenght = 60;
            infoLabel.DescComponent.MaxHeight = 200;
            infoLabel.Dock = DockStyle.Top;
            this.Add(infoLabel);

            this.Add(new Separator(DockStyle.Top, 20));

            Container container = new Container();
            //container.Padding.Set(20, 10, 10, 10);
            container.Dock = DockStyle.Top;

            TextComponent text = new TextLocalizeComponent("symbol");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            symbolText = new TextComponent("");
            symbolText.MaxWidth = 100;
            symbolText.Dock = DockStyle.Left;
            container.Add(symbolText);
            this.Add(container);

            container = new Container();
            container.Dock = DockStyle.Top;

            text = new TextLocalizeComponent("ownerAddress");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            ownerAddressText = new TextButton("");
            ownerAddressText.MaxWidth = 250;
            ownerAddressText.Dock = DockStyle.Fill;
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
            button.Dock = DockStyle.Right;
            button.Executed += (s) =>
            {
                Clipboard.SetText(ownertAddress);
                MessageView.Show(Language.Current["address"] + " " + ownertAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            };
            container.Add(button);
            this.Add(container);

            container = new Container();
            container.Dock = DockStyle.Top;

            text = new TextLocalizeComponent("jettonMinterAddress");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            jettonMinterAddressText = new TextButton("");
            jettonMinterAddressText.MaxWidth = 250;
            jettonMinterAddressText.Dock = DockStyle.Fill;
            jettonMinterAddressText.Executed += (s) =>
            {
                if (this.activeButton)
                {
                    Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Symbol, jettonAddress);
                    this.Form.Hide();
                }
            };
            container.Add(jettonMinterAddressText);

            button = new ImageButton("copyAddress.svg");
            button.MaxHeight = 20;
            button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
            button.Dock = DockStyle.Right;
            button.Executed += (s) =>
            {
                Clipboard.SetText(jettonAddress);
                MessageView.Show(Language.Current["address"] + " " + jettonAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
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
            jettonWalletAddressText.Dock = DockStyle.Fill;
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
            button.Dock = DockStyle.Right;
            button.Executed += (s) =>
            {
                Clipboard.SetText(jettonWalletAddress);
                MessageView.Show(Language.Current["address"] + " " + jettonWalletAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            };
            container.Add(button);

            this.Add(container);



            container = new Container();
            container.Dock = DockStyle.Top;

            text = new TextLocalizeComponent("decimals");
            text.MinWidth = 200;
            text.Alignment = ContentAlignment.Left;
            text.AppendRightText = ":";
            text.Dock = DockStyle.Left;
            text.Style = Theme.Get<CaptionStyle>();
            container.Add(text);

            decimalText = new TextComponent("");
            decimalText.Dock = DockStyle.Left;
            container.Add(decimalText);
            this.Add(container);

            container = new Container();
            container.Dock = DockStyle.Top;

            currencyText = new TextLocalizeComponent("totalSupply");
            currencyText.MinWidth = 200;
            currencyText.Alignment = ContentAlignment.Left;
            currencyText.AppendRightText = ":";
            currencyText.Dock = DockStyle.Left;
            currencyText.Style = Theme.Get<CaptionStyle>();
            container.Add(currencyText);

            currencyLabel = new CurrencyLabel(null, null);
            currencyLabel.CurrencyTextComponent.MaxWidth = 100;
            currencyLabel.Dock = DockStyle.Left;
            container.Add(currencyLabel);
            this.Add(container);

            container = new Container();
            container.Dock = DockStyle.Top;

            deploerText = new TextLocalizeComponent("createdWith");
            deploerText.Visible = false;
            deploerText.MinWidth = 200;
            deploerText.Alignment = ContentAlignment.Left;
            deploerText.AppendRightText = ":";
            deploerText.Dock = DockStyle.Left;
            deploerText.Style = Theme.Get<CaptionStyle>();
            container.Add(deploerText);

            deploerButton = new TextButton();
            deploerButton.ForeColor = Theme.blue0;
            deploerButton.Visible = false;
            deploerButton.MaxWidth = 250;
            deploerButton.Dock = DockStyle.Left;
            deploerButton.Executed += (s) =>
            {
                WinApi.ShellExecute(deploerButton.Tag as string);
            };
            container.Add(deploerButton);
            this.Add(container);

        }

        public JettonInfoContainer(TonUnknownWallet wallet, JettonInfo info, bool activeButton)
            :this(wallet, activeButton)
        {
            this.Update(info);
        }

        private TonUnknownWallet wallet;
        private bool activeButton;
        private LargeLabelNotLocalize infoLabel;
        private TextComponent symbolText;
        private TextComponent decimalText;
        private TextButton ownerAddressText;
        private TextButton jettonMinterAddressText;
        private TextButton jettonWalletAddressText;
        private CurrencyLabel currencyLabel;
        private TextComponent currencyText;
        private string jettonAddress;
        private string jettonWalletAddress;
        private string ownertAddress;
        private TextLocalizeComponent deploerText;
        private TextButton deploerButton;

        public void Update(JettonDeployData data, IImage img)
        {
            symbolText.Text = data.info.Symbol;
            decimalText.Text = data.info.Decimals.ToString();

            infoLabel.ImageComponent.Image = img != null ? img : Images.Get("jetton.svg");

            infoLabel.TextComponent.Text = data.info.Name;
            infoLabel.DescComponent.Text = data.info.Description;


            jettonAddress = data.jettonMinterAddress;
            jettonWalletAddress = data.jettonWalletAddress;
            ownertAddress = data.deployParams.deployer;

            ownerAddressText.Text = !string.IsNullOrEmpty(ownertAddress) ? ownertAddress : "null";
            jettonMinterAddressText.Text = Controller.GetKnownAddress(wallet.Adapter, jettonAddress);
            jettonWalletAddressText.Text = Controller.GetKnownAddress(wallet.Adapter, jettonWalletAddress);

            currencyText.Visible = data.info.TotalSupply > 0;
            currencyLabel.Visible = data.info.TotalSupply > 0;
            if (currencyLabel.Visible)
            {
                currencyLabel.ValueTextComponent.Text = new Balance(data.info.Symbol, data.info.TotalSupply, data.info.Decimals, 3).GetTextSharps(data.info.Decimals);
                currencyLabel.ValueTextComponent.ForeColor = data.info.ThremeColor;
                currencyLabel.CurrencyTextComponent.Text = data.info.Symbol;
            }
        }

        public void Update(JettonInfo info)
        {
            symbolText.Text = info.Symbol;
            decimalText.Text = info.Decimals.ToString();

            infoLabel.ImageComponent.Image = info.LoadImage((image) => infoLabel.ImageComponent.Image = image);

            infoLabel.TextComponent.Text = info.Name;
            infoLabel.DescComponent.Text = info.Description;

            jettonAddress = info.JettonAddress;
            ownertAddress = info.OwnerAddress;

            ownerAddressText.Text = !string.IsNullOrEmpty(ownertAddress) ? ownertAddress : "null";
            jettonMinterAddressText.Text = Controller.GetKnownAddress(wallet.Adapter, jettonAddress);
            jettonWalletAddressText.Parent.Visible = false;

            currencyText.Visible = info.TotalSupply > 0;
            currencyLabel.Visible = info.TotalSupply > 0;
            if (currencyLabel.Visible)
            {
                currencyLabel.ValueTextComponent.Text = info.TotalSupply.GetTextSharps(info.Decimals);
                currencyLabel.ValueTextComponent.ForeColor = info.ThremeColor;
                currencyLabel.CurrencyTextComponent.Text = info.Symbol;
            }

            if (!string.IsNullOrEmpty(info.Deployer))
            {
                JsonArray array = Json.Parse(info.Deployer) as JsonArray;
                if (array != null && array.Contains("name"))
                {
                    deploerButton.Text = array.GetString("name");
                    deploerButton.Tag = array.GetString("url");
                    deploerText.Visible = true;
                    deploerButton.Visible = true;
                }
            }
            else
            {
                deploerText.Visible = false;
                deploerButton.Visible = false;
            }
        }

        public void Update(JettonDeployInfo info, JettonInfo jettonInfo, IImage img)
        {
            symbolText.Text = info.Symbol;
            decimalText.Text = info.Decimals.ToString();

            infoLabel.ImageComponent.Image = img != null ? img : Images.Get("jetton.svg");

            infoLabel.TextComponent.Text = info.Name;
            infoLabel.DescComponent.Text = info.Description;

            jettonAddress = jettonInfo.JettonAddress;
            ownertAddress = jettonInfo.OwnerAddress;

            ownerAddressText.Text = !string.IsNullOrEmpty(ownertAddress) ? ownertAddress : "null";
            jettonMinterAddressText.Text = Controller.GetKnownAddress(wallet.Adapter, jettonAddress);
            jettonWalletAddressText.Parent.Visible = false;

            currencyText.Visible = jettonInfo.TotalSupply > 0;
            currencyLabel.Visible = jettonInfo.TotalSupply > 0;
            if (currencyLabel.Visible)
            {
                currencyLabel.ValueTextComponent.Text = jettonInfo.TotalSupply.GetTextSharps(info.Decimals);
                currencyLabel.ValueTextComponent.ForeColor = info.ThremeColor;
                currencyLabel.CurrencyTextComponent.Text = info.Symbol;
            }

        }
    }
}
