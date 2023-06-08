using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class ChangeOwnerForm : CaptionForm
    {
        public ChangeOwnerForm(TokenWallet wallet)
            : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.parent = wallet.Parent;
            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 550);

            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);

            this.mainPanel = new MainPanel(this);
            this.switchContainer.Current = this.mainPanel;
            this.wallet.TransactionComplete += Wallet_TransactionComplete;

        }

        protected override void OnDisposed()
        {
            this.wallet.TransactionComplete -= Wallet_TransactionComplete;
            this.controller.Dispose();
            base.OnDisposed();
        }

        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            this.transactionWaitPanel.AddTransaction(sender == this.wallet ? this.wallet.Version : "owner", sender as Wallet, transaction);
            if (object.Equals(this.queryId, value))
            {
                this.transactionWaitPanel.StopWait();
                this.transactionWaitPanel.ContinueEnabled(true);
                this.wallet.Adapter.Client.RemoveJetton(this.wallet.Address);
            }
        }

        private TokenWallet wallet;
        private TonWallet parent;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;
        private Balance fee = new Gram(JettonController.MessageTransferFee);
        private Balance fees = null;
        private TransactionWaitPanel transactionWaitPanel;
        private PasswordPanel passwordPanel;
        private object ownerMessageHash;
        private SendInfoPanel sendInfoPanel;
        private long queryId;

        private MainPanel mainPanel;
        private string newOwner;

        private void CompleteChangeOwner()
        {
            WalletsData.Wallets.Remove(this.wallet);
            this.parent.Wallets.Remove(this.wallet.ID);

            Wallet nwallet = WalletsData.GetWallet(this.wallet.AdapterID, this.newOwner, true);
            if (nwallet is TonWallet tonWallet)
            {
                tonWallet.Wallets.Add(this.wallet.ID, this.wallet);
                WalletsData.Wallets.Add(this.wallet);
                Controller.ShowMainWallet(this.wallet);
            }

            this.controller.Done("changingOwnerCompleted", Language.Current["changingOwnerSuccessfull"], "close", wallet.ThemeColor, this.CloseCheck);
        }

        private void SignSend()
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.sendInfoPanel, CloseCheck);
                this.passwordPanel.Complete += (s) =>
                {
                    this.controller.Wait("checkPasscode", null, "waitCheckPasscode", CloseCheck);
                    wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                ChangeOwnerSend();
                            else
                                this.controller.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                        });
                    });
                };
            }
            this.passwordPanel.DescriptionID = "enterPasswordOwner";
            this.passwordPanel.ClearPasscode();
            this.switchContainer.Next = this.passwordPanel;
        }

        private void ChangeOwnerSend()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("changingOwner", null, "walletSendingGramsInfo", this.CloseCheck, "continue", this.wallet.ThemeColor, this.CompleteChangeOwner);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();

            this.queryId = Utils.Random(int.MaxValue);
            this.wallet.ChangeOwner(this.passwordPanel.Passcode, this.queryId, this.newOwner, JettonController.ChangeOwnerFee, (h, e) =>
            {
                if (h != null)
                {
                    this.ownerMessageHash = h;
                    this.wallet.WaitTransactions.Add(this.queryId);
                    this.parent.WaitTransactions.Add(this.ownerMessageHash);
                }
                else
                {
                    this.transactionWaitPanel.StopWait();
                    this.controller.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", ChangeOwnerSend);
                }
            });
            this.switchContainer.Next = this.transactionWaitPanel;
        }

        private void ChangeOwnerConfirmation(string newOwner)
        {
            this.newOwner = newOwner;

            if (this.sendInfoPanel == null)
                this.sendInfoPanel = new SendInfoPanel(() => switchContainer.Current = this.mainPanel, CloseCheck, this.wallet.ThemeColor, SignSend);
            this.fees = this.parent.Balance.Clone(JettonController.ChangeOwnerFee);
            this.sendInfoPanel.UpdateFee(0, this.parent.Balance, this.fees, this.wallet.Symbol);
            this.sendInfoPanel.Update(this.wallet.Address, "changeOwnerConfirmation", "changeOwner2");
            this.sendInfoPanel.UpdateError("attentionChangeOwner");

            wallet.ChangeOwnerCalcFee(0, newOwner, JettonController.ChangeOwnerFee, (fee, error) =>
            {
                if (error == null)
                {
                    this.fees = fee;
                    this.fees.Update(this.fees.Value + this.fee.Value);
                    this.sendInfoPanel.UpdateFee(0, this.parent.Balance, this.fees, this.wallet.Symbol);
                }
                else
                {

                }
            });
            this.switchContainer.Current = this.sendInfoPanel;
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(ChangeOwnerForm form)
                : base("changeOwner", null, "attentionChangeOwner", () => form.switchContainer.Current = form.mainPanel, form.CloseCheck, "continue", form.wallet.ThemeColor, () => { })
            {
                this.form = form;
                this.wallet = form.parent;

                this.UseTab = true;

                this.descriptionComponent.ForeColor = Theme.red2;

                TextComponent text = new TextLocalizeComponent("ownerTokenAddress");
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                addressBox = new TextBox();
                addressBox.ErrorMode = true;
                addressBox.TabStop = true;
                addressBox.TabStopSelected = true;
                addressBox.ApplyOnLostFocus = true;
                addressBox.MaxHeight = 32;
                addressBox.HintTextID = "enterWalletAddress";
                addressBox.Dock = DockStyle.Top;
                addressBox.TextChanged += (s) => this.CheckEnabled();
                this.Add(addressBox);


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
                    if (e.Key == Complex.Key.Up || e.Key == Complex.Key.Down)
                    {
                        addressesView.OnKeyEvent(e);
                        e.Handled = true;
                    }
                };
                addressesView.SelectedComponentChanged += (s) =>
                {
                    AddressItem item = addressesView.SelectedComponent as AddressItem;
                    if (item != null)
                        this.addressBox.Text = item.address;
                };

                foreach (Wallet wallet in WalletsData.Wallets)
                    if (form.wallet.CheckNewOwner(wallet))
                        addressesView.Add(new AddressItem(form.wallet, wallet.Address, 0, null));

                this.Add(addressesView);

                if (addressesView.Components.Count > 0)
                {
                    //this.Add(new Separator(DockStyle.Top, 20));

                    TextComponent textComponent = new TextLocalizeComponent("possibleOwnerOptions");
                    textComponent.Dock = DockStyle.Top;
                    this.Add(textComponent);

                }

                //this.Add(new Separator(DockStyle.Bottom, 20));

                //Balance b = this.wallet.Balance.Clone(JettonController.ChangeOwnerFee);
                //text = new TextComponent(Language.Current["minSendChanjeOwner", b.GetTextSharps(8) + " " + b.Symbol + " coins"]);
                //text.MultilineLenght = 50;
                //text.RoundBack = true;
                //text.RoundBackRadius = 10;
                //text.Style = Theme.Get<RoundLabelTheme>();
                //text.Font = Theme.font10;
                //text.MinHeight = 40;
                //text.Dock = DockStyle.Bottom;
                //this.Add(text);


                //Container ct = new Container();
                //ct.Dock = DockStyle.Bottom;

                //text = new TextLocalizeComponent("amount");
                //text.Alignment = ContentAlignment.Left;
                //text.Dock = DockStyle.Left;
                //ct.Add(text);

                //if (wallet.IsSupportMarket)
                //{
                //    currencyLabel = new CurrencyLabel("", MainSettings.Current.General.Currency.ID);
                //    currencyLabel.ValueTextComponent.AppendLeftText = "≈";
                //    currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                //    currencyLabel.Dock = DockStyle.Fill;
                //    currencyLabel.Alignment = ContentAlignment.Right;
                //    ct.Add(currencyLabel);
                //}
                //this.Add(ct);

                //ammountBox = new NumberEditBoxEx();
                //ammountBox.HintTextID = Language.Current["amount"] + " " + wallet.Symbol + " coins";
                //ammountBox.TabStop = true;
                //ammountBox.SignCount = 10;
                //ammountBox.ApplyOnLostFocus = true;
                //ammountBox.Maximum = 1000000000;
                //ammountBox.MinHeight = 32;
                //ammountBox.Dock = DockStyle.Bottom;
                //ammountBox.Value = (decimal)b;
                //ammountBox.ValueChanged += (s) => this.CheckEnabled();
                //this.Add(ammountBox);

                //ct = new Container();
                //ct.Dock = DockStyle.Bottom;

                //text = new TextLocalizeComponent("balance");
                //text.AppendRightText = ":";
                //text.Alignment = ContentAlignment.Left;
                //text.Dock = DockStyle.Left;
                //ct.Add(text);

                //balance = new CurrencyLabel(null, wallet.Symbol);
                //balance.ValueTextComponent.ForeColor = wallet.ThemeColor;
                //balance.ValueTextComponent.Text = wallet.Balance.GetTextSharps(8);
                //balance.Dock = DockStyle.Left;
                //ct.Add(balance);

                //this.Add(ct);


                this.continueButton.BringToFront();
                this.continueButton.Enabled = false;

                addressBox.Text = form.wallet.OwnerAddress;
            }

            private ChangeOwnerForm form;
            private TonWallet wallet;
            private TextBox addressBox;
            private AnyViewAnimation addressesView;
            //private NumberEditBoxEx ammountBox;
            //private CurrencyLabel currencyLabel;
            //private CurrencyLabel balance;

            private void CheckEnabled()
            {
                string address = addressBox.Text.Trim();
                addressBox.ErrorMode = !string.IsNullOrEmpty(address) && !form.wallet.Adapter.IsValidAddress(address) || address == form.wallet.OwnerAddress;
                this.continueButton.Enabled = !addressBox.ErrorMode;
                //if (ammountBox != null)
                //{
                //    ammountBox.ErrorMode = this.wallet.Balance.FromDecimal(ammountBox.Value) < JettonController.ChangeOwnerFee;
                //    if (this.continueButton.Enabled)
                //        this.continueButton.Enabled = !ammountBox.ErrorMode;
                //}
                //if (currencyLabel != null)
                //{
                //    currencyLabel.ValueTextComponent.Text = ((decimal)ammountBox.Value * wallet.Market.LastPrice).GetTextSharps(2);
                //    currencyLabel.Parent.ClearMeasured();
                //    currencyLabel.Parent.RelayoutAll();
                //}

            }

            protected override void Continue()
            {
                form.ChangeOwnerConfirmation(addressBox.Text.Trim());
            }
        }
    }
}
