using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public partial class ChangeForm : CaptionForm
    {
        public ChangeForm(JettonMinter wallet)
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
            this.transactionWaitPanel.AddTransaction(sender == this.wallet ? "Jetton minter" : "owner", sender as Wallet, transaction);
            if (this.ownerMessageHash == value)
            {
                this.transactionWaitPanel.StopWait();
                this.transactionWaitPanel.ContinueEnabled(true);
                this.wallet.Adapter.Client.RemoveJetton(this.wallet.Address);
            }
        }

        private JettonMinter wallet;
        private TonWallet parent;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;
        private MainPanel mainPanel;
        private Balance fee = new Gram(JettonController.MessageTransferFee);
        private Balance fees = null;
        private TransactionWaitPanel transactionWaitPanel;
        private JettonEnterInfoPanel enterInfoPanel;
        private JettonInfoPanel infoPanel;
        private PasswordPanel passwordPanel;
        private object ownerMessageHash;
        private JettonInfo info;
        private SendInfoPanel sendInfoPanel;

        private void CompleteChangeContent()
        {
            this.controller.Done("changingContentCompleted", Language.Current["changingContentSuccessfull"], "close", wallet.ThemeColor, this.CloseCheck);
        }

        private void ChangeContent()
        {
            if (this.enterInfoPanel == null)
            {
                this.enterInfoPanel = new JettonEnterInfoPanel("changeContent", false, () => switchContainer.Current = mainPanel, CloseCheck, wallet.ThemeColor, () => controller.Wait(null, "pleaseWait"), (info, img) =>
                {
                    if (this.infoPanel == null)
                        this.infoPanel = new JettonInfoPanel(this);
                    this.infoPanel.Update(info, img);
                    this.switchContainer.Next = this.infoPanel;
                });
                this.enterInfoPanel.Update(this.wallet.JettonInfo);
            }
            this.switchContainer.Next = this.enterInfoPanel;
        }

        private void InitTransactionWaitPanel(string captionID, EmptyHandler completeHandler)
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("changingContent", null, "walletSendingGramsInfo", this.CloseCheck, "continue", this.wallet.ThemeColor, completeHandler);
            this.transactionWaitPanel.SetContinue(completeHandler);
            this.transactionWaitPanel.SetCaption(captionID);
        }

        private void ChangeContentSend()
        {
            this.InitTransactionWaitPanel("changingContent", this.CompleteChangeContent);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
            long qid = Utils.Random(int.MaxValue);
            this.wallet.ChangeContent(this.passwordPanel.Passcode, qid, this.info, null, (h, e) =>
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

        private void InitSendInfoPanel(EmptyHandler goback, EmptyHandler continueHandler)
        {
            if (this.sendInfoPanel == null)
                this.sendInfoPanel = new SendInfoPanel(()=> switchContainer.Current = this.mainPanel, CloseCheck, this.wallet.ThemeColor, continueHandler);
            this.sendInfoPanel.SetGoback(goback);
            this.sendInfoPanel.SetContinue(continueHandler);
        }

        private void ChangeContentConfirmation()
        {
            this.InitSendInfoPanel(() => switchContainer.Current = this.infoPanel, () => SignSend(ChangeContentSend));
            this.fees = Balance.Empty;
            this.sendInfoPanel.UpdateFee(0, this.wallet.Balance, this.fees, this.wallet.Symbol);
            this.sendInfoPanel.Update(this.wallet.Address, "changeContentConfirmation", "changeContent2");
            this.sendInfoPanel.UpdateError(null);
            wallet.ChangeContentCalcFee(0, this.info, null, (fee, error) =>
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

        private void SignSend(EmptyHandler completeHandler)
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.sendInfoPanel, CloseCheck);
                this.passwordPanel.Complete += (s) =>
                {
                    wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                completeHandler();
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
            public MainPanel(ChangeForm form)
                : base("changeContract", null, "youCanChangeContent", null, form.CloseCheck, "changeOwner", form.wallet.ThemeColor, form.ChangeOwner)
            {
                this.form = form;
                changeContractButton = new ColorButton("changeContent");
                changeContractButton.Enabled = form.wallet.Adapter.IsConnected;
                changeContractButton.BoxColor = form.wallet.ThemeColor;
                changeContractButton.Dock = DockStyle.Bottom;
                changeContractButton.MinHeight = 40;
                changeContractButton.Executed += (s) =>
                {
                    form.ChangeContent();
                };
                this.Add(changeContractButton);
                this.Add(new Separator(DockStyle.Bottom, 20));
                this.continueButton.BringToFront();
            }

            private ChangeForm form;
            private ColorButton changeContractButton;
        }

        private class JettonInfoPanel : CaptionPanel
        {
            public JettonInfoPanel(ChangeForm form)
                : base("tokenInformation", () => form.switchContainer.Current = form.enterInfoPanel, form.CloseCheck, "continue", form.wallet.ThemeColor, () => { })
            {
                this.form = form;

                infoContainer = new JettonInfoContainer(this.form.wallet, false);
                infoContainer.Dock = DockStyle.Top;
                this.Add(infoContainer);
            }

            private ChangeForm form;
            private JettonInfoContainer infoContainer;
            private JettonInfo info;
            private IImage img;

            public void Update(JettonInfo info, IImage img)
            {
                this.info = info;
                if (this.img != null)
                    this.img.Dispose();
                this.img = img;

                this.infoContainer.Update(info, this.form.wallet.JettonInfo, img);

                this.ClearMeasured();
                this.RelayoutAll();
            }

            protected override void Continue()
            {
                this.form.info = this.info;
                this.form.ChangeContentConfirmation();
            }
        }
    }
}
