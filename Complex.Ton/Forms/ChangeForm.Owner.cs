using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public partial class ChangeForm
    {
        private ChangeOwnerPanel changeOwnerPanel;
        private string newOwner;

        private void ChangeOwner()
        {
            if (this.changeOwnerPanel == null)
                this.changeOwnerPanel = new ChangeOwnerPanel(this);
            this.switchContainer.Next = this.changeOwnerPanel;
        }

        private void CompleteChangeOwner()
        {
            this.parent.Wallets.Remove(this.wallet.ID);
            WalletsData.Wallets.Remove(this.wallet);

            Wallet nwallet = WalletsData.GetWallet(this.wallet.AdapterID, this.newOwner, true);
            if (nwallet is TonWallet tonWallet)
            {
                tonWallet.Wallets.Add(this.wallet.ID, this.wallet);
                WalletsData.Wallets.Add(this.wallet);
                Controller.ShowMainWallet(this.wallet);
            }

            this.controller.Done("changingOwnerCompleted", Language.Current["changingOwnerSuccessfull"], "close", wallet.ThemeColor, this.CloseCheck);
        }

        private void ChangeOwnerSend()
        {
            this.InitTransactionWaitPanel("changingOwner", this.CompleteChangeOwner);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
            long qid = Utils.Random(int.MaxValue);
            this.wallet.ChangeOwner(this.passwordPanel.Passcode, this.newOwner, qid, (h, e) =>
            {
                if (h != null)
                {
                    this.ownerMessageHash = h;
                    this.wallet.WaitTransactions.Add(qid);
                    this.wallet.Parent.WaitTransactions.Add(this.ownerMessageHash);
                }
                else
                {
                    this.transactionWaitPanel.StopWait();
                    this.controller.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", ChangeContentSend);
                }
            });
        }

        private void ChangeOwnerConfirmation(string newOwner)
        {
            this.InitSendInfoPanel(() => switchContainer.Current = this.changeOwnerPanel, () => SignSend(ChangeOwnerSend));
            this.fees = Balance.Empty;
            this.sendInfoPanel.UpdateFee(0, this.wallet.Balance, this.fees, this.wallet.Symbol);
            this.sendInfoPanel.Update(this.wallet.Address, "changeOwnerConfirmation", "changeOwner2");
            this.sendInfoPanel.UpdateError("attentionChangeOwner");
            wallet.ChangeOwnerCalcFee(newOwner, 0, (fee, error) =>
            {
                if (error == null)
                {
                    this.newOwner = newOwner;
                    this.fees = fee;
                    this.fees.Update(this.fees.Value + this.fee.Value);
                    this.sendInfoPanel.UpdateFee(0, this.wallet.Balance, this.fees, this.wallet.Symbol);
                }
                else
                {

                }
            });
            this.switchContainer.Current = this.sendInfoPanel;
        }

        private class ChangeOwnerPanel : CaptionPanel
        {
            public ChangeOwnerPanel(ChangeForm form)
                : base("changeOwner", null, "attentionChangeOwner", () => form.switchContainer.Current = form.mainPanel, form.CloseCheck, "continue", form.wallet.ThemeColor, ()=>{ })
            {
                this.form = form;

                this.UseTab = true;

                this.descriptionComponent.ForeColor = Theme.red2;

                TextLocalizeComponent text = new TextLocalizeComponent("ownerJettonAddress");
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
                addressBox.TextChanged += (s) => this.CheckEnabledSend();

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
                    this.Add(new Separator(DockStyle.Top, 20));

                    TextComponent textComponent = new TextLocalizeComponent("possibleOwnerOptions");
                    textComponent.Dock = DockStyle.Top;
                    this.Add(textComponent);

                }

                this.continueButton.BringToFront();
                this.continueButton.Enabled = false;

                addressBox.Text = form.wallet.JettonInfo.OwnerAddress;
            }

            private ChangeForm form;
            private TextBox addressBox;
            private AnyViewAnimation addressesView;

            private void CheckEnabledSend()
            {
                string address = addressBox.Text.Trim();
                addressBox.ErrorMode = !string.IsNullOrEmpty(address) && !form.wallet.Adapter.IsValidAddress(address) || address == form.wallet.JettonInfo.OwnerAddress;

                this.continueButton.Enabled = !addressBox.ErrorMode;
            }

            protected override void Continue()
            {
                form.ChangeOwnerConfirmation(addressBox.Text.Trim());
            }
        }
    }
}
