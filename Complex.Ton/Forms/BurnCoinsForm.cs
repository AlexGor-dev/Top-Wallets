using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;
using Complex.Collections;

namespace Complex.Ton
{
    public class BurnCoinsForm : CaptionForm
    {
        public BurnCoinsForm(JettonWallet wallet)
            : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.minter = WalletsData.GetWallet(this.wallet.AdapterID, this.wallet.WalletInfo.JettonInfo.JettonAddress, true) as JettonMinter;

            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 550);

            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);

            this.mainPanel = new MainPanel(this);
            this.switchContainer.Current = this.mainPanel;
            this.wallet.TransactionComplete += Wallet_TransactionComplete;
            if(this.minter != null)
                this.minter.TransactionComplete += Minter_TransactionComplete;
        }


        protected override void OnDisposed()
        {
            this.wallet.TransactionComplete -= Wallet_TransactionComplete;
            if (this.minter != null)
                this.minter.TransactionComplete -= Minter_TransactionComplete;

            this.controller.Dispose();
            base.OnDisposed();
        }

        private void EndTransactions()
        {
            this.transactionWaitPanel.StopWait();
            this.transactionWaitPanel.ContinueEnabled(true);
            this.transactionsInfos.Sort((a, b) => -a.transaction.CompareTo(b.transaction));
            this.transactionWaitPanel.AddTransactions(this.transactionsInfos.ToArray());
        }

        private void Minter_TransactionComplete(object sender, ITransactionBase transaction, object t2)
        {
            if (sender == this.minter)
            {
                this.transactionsInfos.Add(new TransactionsInfo(this.minter, "Jetton minter", transaction));
                if (this.transactionsInfos.Count == 4)
                    this.EndTransactions();
            }
        }


        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            this.transactionsInfos.Add(new TransactionsInfo(sender as Wallet, sender == this.wallet ? "Jetton wallet" : "owner", transaction));
            if (this.minter == null && this.transactionsInfos.Count == 3 || this.transactionsInfos.Count == 4)
                this.EndTransactions();
        }


        private JettonWallet wallet;
        private JettonMinter minter;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;
        private MainPanel mainPanel;
        private Gram fee = new Gram(JettonController.BurnFee);
        private SendInfoPanel infoPanel;
        private TransactionWaitPanel transactionWaitPanel;
        private Array<TransactionsInfo> transactionsInfos = new Array<TransactionsInfo>();

        private decimal amount;
        private PasswordPanel passwordPanel;
        private object ownerMessageHash;

        public string GetSymbolCoinsText()
        {
            return wallet.Symbol + " coins";
        }

        private void BurnConfirmation(decimal amount)
        {
            this.amount = amount;
            if (this.infoPanel == null)
                this.infoPanel = new SendInfoPanel(() => switchContainer.Current = this.mainPanel, CloseCheck, this.wallet.ThemeColor, SignSend);
            this.infoPanel.UpdateFee(0, this.wallet.Balance, this.fee, this.wallet.Symbol);
            this.infoPanel.Update(this.wallet.Address, Language.Current["burnConfirmationText", amount.GetTextSharps(8), this.GetSymbolCoinsText()], Language.Current["burn2"] + " " + amount.GetTextSharps(8) + " " + this.GetSymbolCoinsText());
            this.switchContainer.Current = this.infoPanel;
        }

        private void Complete()
        {
            this.controller.Done("burningCompleted", Language.Current["burningSuccessfull", amount, GetSymbolCoinsText()], "close", wallet.ThemeColor, this.CloseCheck);
        }

        private void BurnSend()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("burning", " " + wallet.Symbol + " coins " + wallet.Adapter.NetName, "walletSendingGramsInfo", this.CloseCheck, "continue", this.wallet.ThemeColor, this.Complete);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
            long qid = Utils.Random(int.MaxValue);
            this.wallet.BurnCoins(this.passwordPanel.Passcode, this.amount, qid, (h, e) =>
            {
                if (h != null)
                {
                    this.ownerMessageHash = h;
                    this.wallet.WaitTransactions.Add(qid);
                    this.wallet.WaitTransactions.Add(this.ownerMessageHash);
                    if (this.minter != null)
                        this.minter.WaitTransactions.Add(qid);
                }
                else
                {
                    this.transactionWaitPanel.StopWait();
                    this.controller.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", BurnSend);
                }

            });
        }

        private void SignSend()
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.infoPanel, CloseCheck);
                this.passwordPanel.Complete += (s) =>
                {
                    wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                this.BurnSend();
                            else
                                this.controller.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                        });
                    });
                };
            }
            this.passwordPanel.DescriptionID = "enterPasswordOwnerJetton";
            this.passwordPanel.ClearPasscode();
            this.switchContainer.Next = this.passwordPanel;
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(BurnCoinsForm form)
                : base(Language.Current["burnCoins"] + " " + form.GetSymbolCoinsText() + " " + form.wallet.Adapter.NetName, null, null, null, form.CloseCheck, "continue", form.wallet.ThemeColor, () => { })

            {
                this.form = form;

                CaptionStyle captionStyle = Theme.Get<CaptionStyle>();

                TextComponent text = new TextLocalizeComponent("amount");
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                ammountBox = new NumberEditBoxEx();
                ammountBox.ErrorMode = true;
                ammountBox.HintTextID = Language.Current["amount"] + " " + form.GetSymbolCoinsText();
                ammountBox.TabStop = true;
                ammountBox.TabStopSelected = true;
                ammountBox.SignCount = form.wallet.WalletInfo.JettonInfo.Decimals;
                ammountBox.ApplyOnLostFocus = true;
                ammountBox.Maximum = decimal.MaxValue;
                ammountBox.MinHeight = 32;
                ammountBox.Dock = DockStyle.Top;
                ammountBox.ValueChanged += (s) => this.CheckEnabledSend();
                this.Add(ammountBox);

                Container container = new Container();
                container.Dock = DockStyle.Top;

                text = new TextLocalizeComponent("balance");
                text.AppendRightText = ":";
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Left;
                container.Add(text);

                CurrencyLabel currencyLabel = new CurrencyLabel(form.wallet.Balance.GetTextSharps(form.wallet.WalletInfo.JettonInfo.Decimals), form.wallet.Symbol);
                currencyLabel.ValueTextComponent.ForeColor = form.wallet.ThemeColor;
                currencyLabel.Dock = DockStyle.Left;
                container.Add(currencyLabel);

                this.Add(container);

                this.Add(new Separator(DockStyle.Top, 20));

                container = new Container();
                container.Dock = DockStyle.Top;

                text = new TextLocalizeComponent("owner");
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = captionStyle;
                container.Add(text);

                text = new TextComponent(form.wallet.Parent.Name);
                text.Dock = DockStyle.Left;
                container.Add(text);
                this.Add(container);


                container = new Container();
                container.Dock = DockStyle.Top;

                text = new TextLocalizeComponent("balance");
                text.AppendRightText = ":";
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Left;
                container.Add(text);

                currencyLabel = new CurrencyLabel(form.wallet.Parent.Balance.GetTextSharps(8), form.wallet.Parent.Symbol);
                currencyLabel.ValueTextComponent.Font = Theme.font10;
                currencyLabel.ValueTextComponent.ForeColor = form.wallet.Parent.ThemeColor;
                currencyLabel.Dock = DockStyle.Left;
                container.Add(currencyLabel);

                text = new TextComponent();
                text.MinHeight = 20;
                text.Dock = DockStyle.Right;
                text.Text = Language.Current["fee"] + ": ≈";
                container.Add(text);

                currencyLabel = new CurrencyLabel(form.fee.GetTextSharps(6), form.fee.Symbol);
                currencyLabel.ValueTextComponent.Font = Theme.font10;
                currencyLabel.Dock = DockStyle.Right;
                container.Add(currencyLabel);

                this.Add(container);

                if (this.form.wallet.Parent.Balance < form.fee)
                {
                    this.Add(new Separator(DockStyle.Top, 20));

                    text = new TextLocalizeComponent("enoughFunds");
                    text.Dock = DockStyle.Top;
                    text.ForeColor = Theme.red2;
                    this.Add(text);

                }

                this.continueButton.BringToFront();
                this.continueButton.Enabled = false;
            }

            private BurnCoinsForm form;
            private NumberEditBoxEx ammountBox;

            private void CheckEnabledSend()
            {
                ammountBox.ErrorMode = (decimal)ammountBox.Value >= this.form.wallet.Balance || ammountBox.Value == 0;
                this.continueButton.Enabled = !ammountBox.ErrorMode && ammountBox.Value > 0 && this.form.wallet.Parent.Balance >= form.fee;
            }

            protected override void Continue()
            {
                this.form.BurnConfirmation(this.ammountBox.Value);
            }
        }

    }
}
