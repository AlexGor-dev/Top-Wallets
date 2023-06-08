using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public abstract class ChangeContentForm : CaptionForm
    {
        public ChangeContentForm(TokenWallet wallet)
            : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.parent = wallet.Parent;
            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 550);

            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);

            this.mainPanel = this.CreateMainPanel();
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
            if (this.ownerMessageHash == value)
            {
                this.transactionWaitPanel.StopWait();
                this.transactionWaitPanel.ContinueEnabled(true);
                this.wallet.Adapter.Client.RemoveJetton(this.wallet.Address);
            }
        }

        protected readonly TokenWallet wallet;
        protected readonly TonWallet parent;
        protected readonly SwitchContainer switchContainer;
        protected readonly SwitchFormController controller;
        private Balance fee = new Gram(JettonController.MessageTransferFee);
        private Balance fees = null;
        private TransactionWaitPanel transactionWaitPanel;
        protected readonly Component mainPanel;
        private PasswordPanel passwordPanel;
        private object ownerMessageHash;
        private object info;
        private SendInfoPanel sendInfoPanel;
        private long queryId;

        protected abstract Component CreateMainPanel();

        private void CompleteChangeContent()
        {
            this.controller.Done("changingContentCompleted", Language.Current["changingContentSuccessfull"], "close", wallet.ThemeColor, this.CloseCheck);
        }

        private void ChangeContentSend()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("changingContent", null, "walletSendingGramsInfo", this.CloseCheck, "continue", this.wallet.ThemeColor, this.CompleteChangeContent);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
            this.queryId = Utils.Random(int.MaxValue);
            this.wallet.ChangeContent(this.passwordPanel.Passcode, this.queryId, this.info, (h, e) =>
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
                    this.controller.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", ChangeContentSend);
                }
            });
        }

        internal void ChangeContentConfirmation(object info)
        {
            this.info = info;

            if (this.sendInfoPanel == null)
                this.sendInfoPanel = new SendInfoPanel(()=> switchContainer.Current = this.mainPanel, CloseCheck, this.wallet.ThemeColor, SignSend);

            this.fees = Balance.Empty;
            this.sendInfoPanel.UpdateFee(0, this.wallet.Balance, this.fees, this.wallet.Symbol);
            this.sendInfoPanel.Update(this.wallet.Address, "changeContentConfirmation", "changeContent2");
            this.sendInfoPanel.UpdateError(null);
            wallet.ChangeContentCalcFee(0, this.info, (fee, error) =>
            {
                if (error == null)
                {
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
                                ChangeContentSend();
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

    }
}
