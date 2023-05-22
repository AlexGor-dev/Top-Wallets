using System;
using Complex.Controls;
using Complex.Themes;

namespace Complex.Wallets
{
    public class SendController : SwitchFormController
    {
        public SendController(Wallet wallet, SwitchContainer switchContainer, EmptyHandler closeHandler, EmptyHandler doneHandler)
            :base(switchContainer, closeHandler, doneHandler)
        {
            this.SetWallet(wallet);
        }

        protected override void OnDisposed()
        {
            this.SetWallet(null);
            base.OnDisposed();
        }

        private Wallet wallet;
        public Wallet Wallet => wallet;

        private WalletInfo destWallet;
        private string address;

        private decimal amount;
        public decimal Amount => amount;

        private string comment;
        public string Comment => comment;

        private Balance fees;
        private Component mainPanel;
        private PasswordPanel passwordPanel;
        private SendInfoPanel sendInfoPanel;
        private object msgHash;
        private TransactionWaitPanel transactionWaitPanel;

        public virtual string GetRecipientTextID()
        {
            return "recipientWalletAddress";
        }

        protected virtual void OnWalletAdded(Wallet wallet)
        {

        }

        protected virtual void OnWalletRemoved(Wallet wallet)
        {

        }

        public void SetWallet(Wallet wallet)
        {
            if (this.wallet != null)
            {
                this.wallet.TransactionComplete -= Wallet_TransactionComplete;
                this.OnWalletRemoved(this.wallet);
            }
            this.wallet = wallet;
            if (this.wallet != null)
            {
                this.wallet.TransactionComplete += Wallet_TransactionComplete;
                this.OnWalletAdded(this.wallet);
            }
        }

        public void SetMainPanel(Component mainPanel)
        {
            this.mainPanel = mainPanel;
        }

        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            if (this.msgHash != null)
            {
                Wallet w = sender as Wallet;
                this.transactionWaitPanel.AddTransaction(w.Name, w, transaction);
                if (this.msgHash == value)
                {
                    this.transactionWaitPanel.StopWait();
                    this.transactionWaitPanel.ContinueEnabled(true);
                }
            }
        }

        private void Complete()
        {
            this.Done(Language.Current["sendingCompleted", GetSymbolCoinsText() + " " + wallet.Adapter.NetName], Language.Current["walletSendDoneText", amount, GetSymbolCoinsText(), address], "close", wallet.ThemeColor, closeHandler);
        }

        private void SendAmmount()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("sending", " " + wallet.Symbol + " coins " + wallet.Adapter.NetName, "walletSendingGramsInfo", this.closeHandler, "continue", this.wallet.ThemeColor, this.Complete);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;

            this.wallet.SendAmount(this.passwordPanel.Passcode, this.address, this.amount, this.comment, (transaction, error) =>
            {
                if (!this.IsDisposed)
                {
                    if (transaction != null)
                    {
                        this.msgHash = transaction;
                        this.wallet.WaitTransactions.Add(transaction);
                    }
                    else
                    {
                        this.transactionWaitPanel.StopWait();
                        this.Error(this.wallet.ThemeColor, "errorOccurred", error, "repeat", SendAmmount);
                    }
                }
            });
        }

        private void Sign()
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(false, () => { this.switchContainer.Current = sendInfoPanel; }, this.closeHandler);
                Wallet passWallet = this.wallet;
                if (this.wallet is IToken token && token.Parent != null)
                    passWallet = token.Parent;
                this.passwordPanel.DescriptionID = Language.Current["enterPasswordForWallet", passWallet.OriginalName, passWallet.Address];
                this.passwordPanel.Complete += (s) =>
                {
                    this.wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                    {
                        if (e == null)
                            SingleThread.Run(this.SendAmmount);
                        else
                            this.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                    });
                };

            }
            this.switchContainer.Current = this.passwordPanel;
        }

        public string GetSymbolCoinsText()
        {
            return wallet.Symbol + " coins";
        }

        private void CalcFee()
        {
            this.sendInfoPanel.UpdateDestState(this.destWallet, GetSymbolCoinsText());
            wallet.CalcFees(this.address, this.amount, this.comment, (fee, error) =>
            {
                if (error == null)
                    this.fees = fee;
                this.sendInfoPanel.UpdateFee(this.amount, this.wallet.Balance, this.fees, GetSymbolCoinsText());
                this.switchContainer.Next = this.sendInfoPanel;
            });
        }

        public void Send(string address, decimal amount, string comment)
        {
            this.address = address;
            this.amount = amount;
            this.comment = comment;
            if (this.comment == null)
                this.comment = "";
            if (!wallet.Adapter.IsValidAddress(address))
                this.Error("invalidAddress");
            else if (this.address == wallet.Address)
                this.Error(Language.Current["walletSendSameWalletText", wallet.Symbol]);
            else if ((decimal)amount >= wallet.Balance)
                this.Error(Language.Current["insufficientFunds", GetSymbolCoinsText()]);
            else
            {
                this.destWallet = null;
                if (this.sendInfoPanel == null)
                    this.sendInfoPanel = new SendInfoPanel(() => switchContainer.Current = mainPanel, closeHandler, wallet.ThemeColor, Sign);
                this.sendInfoPanel.UpdateDestState(this.destWallet, GetSymbolCoinsText());
                this.fees = Balance.Empty;
                this.sendInfoPanel.Update(this.address, this.amount, this.comment, GetSymbolCoinsText());
                this.sendInfoPanel.UpdateFee(this.amount, this.wallet.Balance, this.fees, GetSymbolCoinsText());
                wallet.Adapter.GetWalletInfo(address, (w, e) =>
                {
                    this.destWallet = w;
                    if (w != null)
                        this.CalcFee();
                    else
                        this.Error("", e);
                });
            }
        }

    }
}
