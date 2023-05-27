using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;
using Complex.Collections;

namespace Complex.Ton
{
    public class MintCoinsForm : CaptionForm
    {
        public MintCoinsForm(JettonMinter wallet)
            : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.jettonWallet = wallet.Parent.GetMainChild(wallet.Symbol, WalletType.JettonWallet) as JettonWallet;

            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 550);

            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);

            this.mainPanel = new MainPanel(this);
            this.switchContainer.Current = this.mainPanel;
            this.wallet.TransactionComplete += Wallet_TransactionComplete;
            if (this.jettonWallet != null)
                this.jettonWallet.TransactionComplete += JettonWallet_TransactionComplete;

        }


        protected override void OnDisposed()
        {
            this.wallet.TransactionComplete -= Wallet_TransactionComplete;
            if (this.jettonWallet != null)
                this.jettonWallet.TransactionComplete -= JettonWallet_TransactionComplete;

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

        private void JettonWallet_TransactionComplete(object sender, ITransactionBase transaction, object t2)
        {
            if (sender == this.jettonWallet)
            {
                this.transactionsInfos.Add(new TransactionsInfo(this.jettonWallet, "Jetton wallet", transaction));
                if (this.transactionsInfos.Count == 4)
                    this.EndTransactions();
            }

        }

        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            this.transactionsInfos.Add(new TransactionsInfo(sender as Wallet, sender == this.wallet ? "Jetton minter" : "owner", transaction));
            if (this.jettonWallet == null && this.transactionsInfos.Count == 3 || this.transactionsInfos.Count == 4)
                this.EndTransactions();
        }


        private JettonMinter wallet;
        private JettonWallet jettonWallet;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;
        private MainPanel mainPanel;
        private Gram fee = new Gram(JettonController.MintFee);
        private SendInfoPanel infoPanel;
        private TransactionWaitPanel transactionWaitPanel;
        private decimal amount;
        private PasswordPanel passwordPanel;
        private object ownerMessageHash;
        private Array<TransactionsInfo> transactionsInfos = new Array<TransactionsInfo>();

        public string GetSymbolCoinsText()
        {
            return wallet.Symbol + " coins";
        }

        private void MintConfirmation(decimal amount)
        {
            this.amount = amount;
            if (this.infoPanel == null)
                this.infoPanel = new SendInfoPanel(() => switchContainer.Current = this.mainPanel, CloseCheck, this.wallet.ThemeColor, SignSend);
            this.infoPanel.UpdateFee(0, this.wallet.Balance, this.fee, this.wallet.Symbol);
            this.infoPanel.Update(this.wallet.Address, Language.Current["mintConfirmationText", amount.GetTextSharps(8), this.GetSymbolCoinsText()], Language.Current["mint2"] + " " + amount.GetTextSharps(8) + " " + this.GetSymbolCoinsText());
            this.switchContainer.Current = this.infoPanel;

            this.switchContainer.Current = this.infoPanel;

        }

        private void Complete()
        {
            this.controller.Done("mintingCompleted", Language.Current["mintingSuccessfull", amount, GetSymbolCoinsText()], "close", wallet.ThemeColor, this.CloseCheck);
        }

        private void MintSend()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("minting", " " + GetSymbolCoinsText() + " " + wallet.Adapter.NetName, "walletSendingGramsInfo", this.CloseCheck, "continue", this.wallet.ThemeColor, this.Complete);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
            long qid = Utils.Random(int.MaxValue);
            this.wallet.MintCoins(this.passwordPanel.Passcode, qid, this.amount, (h, e) =>
            {
                if (h != null)
                {
                    this.ownerMessageHash = h;
                    this.wallet.WaitTransactions.Add(qid);
                    if (this.jettonWallet != null)
                        this.jettonWallet.WaitTransactions.Add(qid);
                    else
                        this.wallet.Parent.WaitTransactions.Add(qid);
                    this.wallet.Parent.WaitTransactions.Add(this.ownerMessageHash);
                }
                else
                {
                    this.transactionWaitPanel.StopWait();
                    this.controller.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", MintSend);
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
                                this.MintSend();
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
            public MainPanel(MintCoinsForm form)
                : base(Language.Current["mintCoins"] + " " + form.GetSymbolCoinsText() + " " + form.wallet.Adapter.NetName, null, null, null, form.CloseCheck, "continue", form.wallet.ThemeColor, () => { })
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
                ammountBox.SignCount = form.wallet.JettonInfo.Decimals;
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

                CurrencyLabel currencyLabel = new CurrencyLabel(form.wallet.Balance.GetTextSharps(form.wallet.JettonInfo.Decimals), form.wallet.Symbol);
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

            private MintCoinsForm form;
            private NumberEditBoxEx ammountBox;

            private void CheckEnabledSend()
            {
                ammountBox.ErrorMode = (decimal)ammountBox.Value >= this.form.wallet.Balance || ammountBox.Value == 0;
                this.continueButton.Enabled = !ammountBox.ErrorMode && ammountBox.Value > 0 && this.form.wallet.Parent.Balance >= form.fee;
            }

            protected override void Continue()
            {
                this.form.MintConfirmation(this.ammountBox.Value);
            }

        }
    }
}
