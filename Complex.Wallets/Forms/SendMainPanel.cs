using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Drawing;
using Complex.Collections;

namespace Complex.Wallets
{
    public class SendMainPanel : CaptionPanel
    {
        public SendMainPanel(SendController controller, string address, decimal amount, string comment, EmptyHandler goBack)
            :base("send", " " + controller.Wallet.Symbol + " coins " + controller.Wallet.Adapter.NetName, goBack, controller.Close, Language.Current["send"] + " " + controller.GetSymbolCoinsText(), controller.Wallet.ThemeColor, ()=> { })
        {
            this.controller = controller;

            this.UseTab = true;

            Container container = new Container();
            container.Dock = DockStyle.Top;

            TextLocalizeComponent text = new TextLocalizeComponent(controller.GetRecipientTextID());
            text.AppendRightText = " " + controller.Wallet.Symbol + " coins";
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Left;
            container.Add(text);

            knownAddress = new TextComponent();
            knownAddress.Dock = DockStyle.Fill;
            knownAddress.Alignment = ContentAlignment.Right;
            knownAddress.Style = Theme.Get<CaptionStyle>();
            knownAddress.Font = Theme.font11Bold;
            container.Add(knownAddress);

            this.Add(container);

            addressBox = new TextBox();
            addressBox.ErrorMode = true;
            addressBox.TabStop = true;
            addressBox.TabStopSelected = true;
            addressBox.ApplyOnLostFocus = true;
            addressBox.MaxHeight = 32;
            addressBox.HintTextID = "enterWalletAddress";
            addressBox.Dock = DockStyle.Top;
            addressBox.TextChanged += (s) => this.CheckEnabledSend();
            this.Add(addressBox);

            text = new TextLocalizeComponent("lastTransactions");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            addressesView = new AnyViewAnimation();
            addressesView.TabStop = false;
            addressesView.ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
            addressesView.Dock = DockStyle.Fill;
            addressesView.ScrollVisible = true;
            addressesView.Inflate.height = 2;
            addressesView.Style = Theme.Get<DockViewTheme>();
            addressesView.VScrollStep = 10;
            addressesView.PreKeyEvent += (s, e) =>
            {
                if (e.Key == Key.Up || e.Key == Key.Down)
                {
                    addressesView.OnKeyEvent(e);
                    e.Handled = true;
                }
            };
            addressesView.SelectedComponentChanged += (s) =>
            {
                AddressItem item = addressesView.SelectedComponent as AddressItem;
                if (item != null)
                {
                    this.addressBox.Text = item.address;
                    if(item.amount != 0)
                        this.ammountBox.Value = item.amount;
                    if(!string.IsNullOrEmpty(item.message))
                        this.commentBox.Text = item.message;
                }
            };
            this.Add(addressesView);


            this.Add(new Separator(DockStyle.Bottom, 10));

            Container ct = new Container();
            ct.Dock = DockStyle.Bottom;

            text = new TextLocalizeComponent("amount");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Left;
            ct.Add(text);

            if (this.controller.Wallet.IsSupportMarket)
            {
                currencyLabel = new CurrencyLabel("", MainSettings.Current.General.Currency.ID);
                currencyLabel.ValueTextComponent.AppendLeftText = "≈";
                currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                currencyLabel.Dock = DockStyle.Fill;
                currencyLabel.Alignment = ContentAlignment.Right;
                ct.Add(currencyLabel);
            }
            this.Add(ct);

            ammountBox = new NumberEditBoxEx();
            ammountBox.ErrorMode = true;
            ammountBox.HintTextID = Language.Current["amount"] + " " + this.controller.GetSymbolCoinsText();
            ammountBox.TabStop = true;
            ammountBox.SignCount = 10;
            ammountBox.ApplyOnLostFocus = true;
            ammountBox.Maximum = 1000000000;
            ammountBox.MinHeight = 32;
            ammountBox.Dock = DockStyle.Bottom;
            ammountBox.ValueChanged += (s) => this.CheckEnabledSend();
            this.Add(ammountBox);



            container = new Container();
            container.Dock = DockStyle.Bottom;

            text = new TextLocalizeComponent("balance");
            text.AppendRightText = ":";
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Left;
            container.Add(text);

            balance = new CurrencyLabel(null, controller.Wallet.Symbol);
            balance.ValueTextComponent.ForeColor = controller.Wallet.ThemeColor;
            balance.Dock = DockStyle.Left;
            container.Add(balance);

            this.Add(container);

            if (controller.Wallet.IsSupportSendText)
            {
                this.Add(new Separator(DockStyle.Bottom, 10));

                text = new TextLocalizeComponent("commentOptional");
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Bottom;
                this.Add(text);

                commentBox = new TextEditor();
                commentBox.ScrollVisible = false;
                commentBox.Multiline = true;
                commentBox.TabStop = true;
                commentBox.ToolTipInfo = new ToolTipInfo("commentOptional");
                //commentBox.ApplyOnLostFocus = true;
                commentBox.MinHeight = 40;
                commentBox.HintTextID = "commentOptional";
                commentBox.Dock = DockStyle.Bottom;
                this.Add(commentBox);
            }

            this.Add(new Separator(DockStyle.Bottom, 10));


            this.continueButton.BringToFront();

            this.SetWallet(this.controller.Wallet);

            addressBox.Text = address;
            ammountBox.Value = amount;
            if(commentBox != null)
                commentBox.Text = comment;

        }

        public readonly TextBox addressBox;
        private NumberEditBoxEx ammountBox;
        private TextEditor commentBox;
        private TextComponent knownAddress;
        private CurrencyLabel currencyLabel;
        private CurrencyLabel balance;
        public readonly AnyViewAnimation addressesView;

        private Wallet wallet;

        private SendController controller;

        private void InitTransactions()
        {
            addressesView.Clear();
            Hashtable<string, AddressItem> hash = new Hashtable<string, AddressItem>();
            foreach (ITransactionBase transaction in wallet.Transactions)
            {
                if (transaction is ITransaction tr)
                {
                    if (tr.IsOut)
                        if (!hash.ContainsKey(tr.Address))
                            hash.Add(tr.Address, new AddressItem(this.wallet, tr.Address, (decimal)tr.Amount, tr.Message));
                }
            }

            foreach (Wallet wallet in WalletsData.Wallets)
                if (!hash.ContainsKey(wallet.Address) && this.wallet.CheckSendWallet(wallet))
                    hash.Add(wallet.Address, new AddressItem(this.wallet, wallet.Address, 0, null));

            foreach(AddressItem item in hash)
                addressesView.Add(item);

            addressesView.Relayout();
        }

        public void SetWallet(Wallet wallet)
        {
            this.wallet = wallet;
            balance.ValueTextComponent.Text = wallet.Balance.GetTextSharps(8);
            this.continueButton.BoxColor = wallet.ThemeColor;
            this.CheckEnabledSend();
            this.InitTransactions();
        }

        protected override void Continue()
        {
            this.controller.Send(addressBox.Text, ammountBox.Value, commentBox != null ? commentBox.Text : null);
        }

        private void CheckEnabledSend()
        {
            knownAddress.Text = wallet.Adapter.GetKnownAddress(addressBox.Text.Trim());
            //knownAddress.Invalidate();
            addressBox.ErrorMode = !wallet.Adapter.IsValidAddress(addressBox.Text.Trim()) || addressBox.Text.Trim() == wallet.Address;
            ammountBox.ErrorMode = (decimal)ammountBox.Value >= wallet.Balance || ammountBox.Value == 0;

            if (currencyLabel != null)
            {
                currencyLabel.ValueTextComponent.Text = ((decimal)ammountBox.Value * wallet.Market.LastPrice).GetTextSharps(2);
                currencyLabel.Parent.ClearMeasured();
                currencyLabel.Parent.RelayoutAll();
            }

            this.continueButton.Enabled = !addressBox.ErrorMode && !ammountBox.ErrorMode && ammountBox.Value > 0;
        }

    }
}
