using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Themes;
using Complex.Drawing;
using Complex.Trader;

namespace Complex.Wallets
{
    public class SupportSendController : SendController
    {
        public SupportSendController(SwitchContainer switchContainer, EmptyHandler closeHandler, EmptyHandler doneHandler)
            : base(null, switchContainer, closeHandler, doneHandler)
        {
            int count = 0;
            Hashtable<object, Array<Wallet>> wallets = new Hashtable<object, Array<Wallet>>();

            foreach (CryptoCoinInfo coin in MainSettings.Current.Remote.CryptoCoins)
            {

#if DEBUG
                CryptoCoinInfo c = new CryptoCoinInfo(coin.AdapterID, coin.Symbol, coin.SupportTestAddress, coin.SupportActionAmount);
                Array<Wallet> arr = new Array<Wallet>();
                foreach (Wallet wallet in WalletsData.Wallets)
                {
                    if (wallet.IsMain && wallet.IsSupportSupport && wallet.Balance > 0 && wallet.AdapterID == coin.Symbol + " Test" && wallet.Symbol == c.Symbol)
                    {
                        if (wallet.Address != c.SupportAddress)
                        {
                            arr.Add(wallet);
                            count++;
                        }
                    }
                }
                if (arr.Count > 0)
                    wallets.Add(c, arr);
#else
                if (coin.SupportEnabled && !string.IsNullOrEmpty(coin.SupportAddress))
                {
                    Array<Wallet> arr = new Array<Wallet>();
                    foreach (Wallet wallet in WalletsData.Wallets)
                    {
                        if (wallet.IsMain && wallet.Balance > 0 && !wallet.Adapter.IsTestnet && wallet.Adapter.Symbol == coin.Symbol &&  wallet.Symbol == coin.Symbol)
                        {
                            if (wallet.Address != coin.SupportAddress)
                            {
                                arr.Add(wallet);
                                count++;
                            }
                        }
                    }
                    if (arr.Count > 0)
                        wallets.Add(coin, arr);
                }
#endif
            }
            if (count == 0)
            {
                this.Error("noWalletsAvailable");
            }
            else if (count == 1)
            {
                this.coinInfo = wallets.GetKey(0) as CryptoCoinInfo;
                this.SetWallet(wallets[0][0]);
                this.sendPanel = new SendSupportPanel(this, null);
                this.sendPanel.SetWallet(this.Wallet);
                this.SetMainPanel(this.sendPanel);
                this.switchContainer.Current = this.sendPanel;
            }
            else
            {
                this.selectPanel = new MultiWalletsSelectPanel("supportProject", wallets, closeHandler, (coinInfo, wallet) =>
                {
                    this.coinInfo = coinInfo as CryptoCoinInfo;
                    this.SetWallet(wallet);
                    if (this.sendPanel == null)
                        this.sendPanel = new SendSupportPanel(this, () => this.switchContainer.Current = this.selectPanel);
                    this.sendPanel.SetWallet(wallet);
                    this.SetMainPanel(this.sendPanel);
                    this.switchContainer.Current = this.sendPanel;
                });
                this.switchContainer.Current = this.selectPanel;
            }

        }

        private MultiWalletsSelectPanel selectPanel;
        private SendSupportPanel sendPanel;
        private CryptoCoinInfo coinInfo;

        protected override void OnWalletAdded(Wallet wallet)
        {
            wallet.Adapter.Connected += Adapter_Connected;
            wallet.Adapter.Disconnected += Adapter_Disconnected;
        }
        protected override void OnWalletRemoved(Wallet wallet)
        {
            wallet.Adapter.Connected -= Adapter_Connected;
            wallet.Adapter.Disconnected -= Adapter_Disconnected;
        }

        private void Adapter_Disconnected(object sender)
        {
            if(this.sendPanel != null)
                this.sendPanel.CheckEnabledSend();
        }

        private void Adapter_Connected(object sender)
        {
            if (this.sendPanel != null)
                this.sendPanel.CheckEnabledSend();
        }

        private class SendSupportPanel : CaptionPanel
        {
            public SendSupportPanel(SupportSendController controller, EmptyHandler goback)
                : base("enterYourNameMessage", null, "",
                   goback, controller.closeHandler, "", 0, () => { })
            {
                this.controller = controller;

                this.UseTab = true;

                buttonsPanel = new Complex.Controls.ButtonsPanel();
                buttonsPanel.Padding.Set(4);
                buttonsPanel.BackRadius = 10;
                buttonsPanel.Dock = DockStyle.Top;
                buttonsPanel.MinHeight = 120;

                TextLocalizeComponent text = new TextLocalizeComponent("selectedWallet");
                text.Dock = DockStyle.Top;
                text.Alignment = ContentAlignment.Center;
                text.Font = Theme.font10Bold;
                buttonsPanel.Add(text);


                this.Add(buttonsPanel);

                text = new TextLocalizeComponent("enterYourNameOrLink");
                text.AppendRightText = " (max " + MainSettings.Current.Remote.Support.MaxNameLenght + " chars)";
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                nameTextBox = new TextBox();
                nameTextBox.TabStopSelected = true;
                nameTextBox.MaxTextLenght = MainSettings.Current.Remote.Support.MaxNameLenght;
                nameTextBox.ClearSelectedOnFreeDown = true;
                nameTextBox.TabStop = true;
                nameTextBox.ToolTipInfo = new ToolTipInfo("enterYourNameOptional");
                nameTextBox.ApplyOnLostFocus = true;
                nameTextBox.MinHeight = 32;
                nameTextBox.HintTextID = "enterYourNameOptional";
                nameTextBox.Dock = DockStyle.Top;
                this.Add(nameTextBox);

                //this.Add(new Separator(DockStyle.Top, 10));


                text = new TextLocalizeComponent("enterMessageOrLink");
                text.AppendRightText = " (max " + MainSettings.Current.Remote.Support.MaxMessageLenght + " chars)";
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                messageTextBox = new TextEditor();
                messageTextBox.ScrollVisible = false;
                messageTextBox.MaxTextLenght = MainSettings.Current.Remote.Support.MaxMessageLenght;
                messageTextBox.ClearSelectedOnFreeDown = true;
                messageTextBox.Multiline = true;
                messageTextBox.TabStop = true;
                messageTextBox.ToolTipInfo = new ToolTipInfo("enterMessageOptional");
                //messageTextBox.ApplyOnLostFocus = true;
                //messageTextBox.MinHeight = 64;
                messageTextBox.HintTextID = "enterMessageOptional";
                messageTextBox.Dock = DockStyle.Fill;
                this.Add(messageTextBox);

                //this.Add(new Separator(DockStyle.Bottom, 10));


                Container ct = new Container();
                ct.Dock = DockStyle.Bottom;

                text = new TextLocalizeComponent("amount");
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Left;
                ct.Add(text);

                currencyLabel = new CurrencyLabel("", MainSettings.Current.General.Currency.ID);
                currencyLabel.ValueTextComponent.AppendLeftText = "≈";
                currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                currencyLabel.Dock = DockStyle.Fill;
                currencyLabel.Alignment = ContentAlignment.Right;
                ct.Add(currencyLabel);

                this.Add(ct);

                ammountBox = new NumberEditBoxEx();
                ammountBox.ErrorMode = true;
                ammountBox.TabStop = true;
                ammountBox.SignCount = 10;
                ammountBox.ApplyOnLostFocus = true;
                ammountBox.Maximum = 1000000000;
                ammountBox.MinHeight = 32;
                ammountBox.Dock = DockStyle.Bottom;
                ammountBox.ValueChanged += (s) =>
                {
                    this.CheckEnabledSend();
                };
                this.Add(ammountBox);

                this.continueButton.BringToFront();
                this.continueButton.Enabled = false;

            }

            private NumberEditBoxEx ammountBox;
            private SupportSendController controller;
            private TextBox nameTextBox;
            private TextEditor messageTextBox;
            private CurrencyLabel currencyLabel;
            private Wallet wallet;
            Complex.Controls.ButtonsPanel buttonsPanel;

            public void CheckEnabledSend()
            {
                if (this.currencyLabel != null)
                {
                    this.currencyLabel.ValueTextComponent.Text = ((decimal)ammountBox.Value * wallet.Market.LastPrice).GetTextSharps(2);
                    this.currencyLabel.Parent.ClearMeasured();
                    this.currencyLabel.Parent.RelayoutAll();
                }
                this.ammountBox.ErrorMode = (decimal)ammountBox.Value >= this.wallet.Balance;
                this.continueButton.Enabled = !ammountBox.ErrorMode && ammountBox.Value > 0 && this.controller.Wallet.Adapter.IsConnected;
            }

            protected override void Continue()
            {
                const string separ = "\"";
                string message = null;
                if (!string.IsNullOrEmpty(nameTextBox.Text))
                    message = nameTextBox.Text.Replace(separ, "");
                if (!string.IsNullOrEmpty(messageTextBox.Text))
                    message += (message != null ? " " : "") + separ + messageTextBox.Text.Replace(separ, "") + separ;
                this.controller.Send(controller.coinInfo.SupportAddress, ammountBox.Value, message);
            }

            public void SetWallet(Wallet wallet)
            {
                if (this.wallet != wallet)
                {
                    this.wallet = wallet;
                    this.continueButton.Text = Language.Current["send"] + " " + controller.GetSymbolCoinsText();
                    this.continueButton.BoxColor = this.wallet.ThemeColor;

                    this.descriptionComponent.Text = Language.Current["enterYourNameMessageDesc", controller.coinInfo.SupportActionAmount + " " + controller.coinInfo.Symbol + " coins"];

                    buttonsPanel.Remove(typeof(WalletItem), true);
                    WalletItem wi = new WalletItem(this.wallet);
                    wi.Dock = DockStyle.Top;
                    wi.MinHeight = 90;
                    buttonsPanel.Add(wi);

                    this.ammountBox.HintTextID = Language.Current["amount"] + " " + this.controller.GetSymbolCoinsText();

                    this.CheckEnabledSend();

                    this.Layout();
                }
            }
        }

        protected override void InitDonePanel(DonePanel donePanel)
        {
            Caption caption = new Caption("thankYouSupport");
            caption.Dock = DockStyle.Top;
            caption.Font = Theme.font12Bold;
            caption.TextComponent.ForeColor = Theme.green1;
            caption.RoundBack = true;
            caption.RoundRadius = 4;
            donePanel.Add(caption);

            if (this.Amount >= this.coinInfo.SupportActionAmount && !string.IsNullOrEmpty(this.Comment))
            {
                TextLocalizeComponent descriptionComponent = new TextLocalizeComponent("showYourMessage");
                descriptionComponent.Font = Theme.font10Bold;
                descriptionComponent.ForeColor = Theme.up;
                descriptionComponent.MultilineLenght = 50;
                descriptionComponent.Padding.Set(16, 6, 16, 6);
                descriptionComponent.Alignment = ContentAlignment.Center;
                descriptionComponent.RoundBack = true;
                descriptionComponent.RoundBackRadius = 10;
                descriptionComponent.Dock = DockStyle.Top;
                descriptionComponent.Style = Theme.Get<RoundLabelTheme>();
                donePanel.Add(descriptionComponent);

            }
        }

    }
}
