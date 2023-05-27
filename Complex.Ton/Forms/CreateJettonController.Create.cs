using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Navigation;
using Complex.Themes;
using Complex.Wallets;
using Complex.Collections;

namespace Complex.Ton
{
    public partial class CreateJettonController
    {
        private JettonEnterInfoPanel enterInfoPanel;
        private JettonInfoPanel infoPanel;
        private PasswordPanel walletPasswordPanel;
        private JettonDeployData data;
        private object messageHash;
        private long queryId;
        private TransactionWaitPanel transactionWaitPanel;
        private Array<TransactionsInfo> transactionsInfos = new Array<TransactionsInfo>();

        private JettonMinter jettonMinter;
        private JettonWallet jettonWallet;

        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            if (this.messageHash != null)
            {
                this.transactionsInfos.Add(new TransactionsInfo(sender as Wallet, wallet.Name, transaction));
                if (this.transactionsInfos.Count == 2)
                {
                    this.transactionWaitPanel.StopWait();
                    this.transactionWaitPanel.ContinueEnabled(true);
                    this.transactionsInfos.Sort((a, b) => -a.transaction.CompareTo(b.transaction));
                    this.transactionWaitPanel.AddTransactions(this.transactionsInfos.ToArray());
                }
            }
        }


        private void CreateJetton()
        {
            if (this.wallet.Balance < (decimal)0.25)
                this.Error("enoughBalance", null);
            else
            {
                this.enterInfoPanel = new JettonEnterInfoPanel("createToken", true, () => switchContainer.Current = mainPanel, closeHandler, wallet.ThemeColor, () => Wait(null, "pleaseWait"), (info, img) =>
                   {
                       this.queryId = Utils.Random(int.MaxValue);
                       JettonDeployData data = JettonController.CreateJetton(queryId, wallet.Address, info, null);
                       Timer.Delay(300, () => ShowInfoPanel(data, img));

                   });
                //this.enterInfoPanel.Update("New Jetton", "NJ", "New Jetton project token.", 9, 100000000, "https://complex-soft.com/images/new64.png", "");
                this.switchContainer.Next = this.enterInfoPanel;
            }
        }


        private void ShowInfoPanel(JettonDeployData data, IImage img)
        {
            if (this.infoPanel == null)
                this.infoPanel = new JettonInfoPanel(this);
            this.infoPanel.Update(data, img);
            this.switchContainer.Next = this.infoPanel;
        }

        private void CreateJettonMinter(ParamHandler<JettonMinter, string> resultHandler)
        {
            this.wallet.Adapter.CreateAccountState(data.JettonMinterAddress, (s, e) =>
            {
                if (s != null)
                {
                    JettonInfo info = JettonController.GetJettonInfo(s);
                    if (info != null)
                    {
                        JettonMinter wallet = new JettonMinter(this.wallet.AdapterID, data.JettonMinterAddress, info, this.wallet);
                        wallet.Update(s);
                        s.Dispose();
                        resultHandler(wallet, null);
                    }
                    else
                    {
                        resultHandler(null, "notFoundJettonInfo");
                    }
                }
                else
                {
                    resultHandler(null, e);
                }
            });
        }

        private void CreateJettonWallet(ParamHandler<JettonWallet, string> resultHandler)
        {
            this.wallet.Adapter.CreateAccountState(data.JettonWalletAddress, (s, e) =>
            {
                if (s != null)
                {
                    JettonWalletInfo info = JettonController.GetJettonWalletInfo(s);
                    if (info != null)
                    {
                        JettonWallet wallet = new JettonWallet(this.wallet.AdapterID, data.JettonWalletAddress, info, this.wallet);
                        wallet.Update(s);
                        s.Dispose();
                        resultHandler(wallet, null);
                    }
                    else
                    {
                        resultHandler(null, "notFoundJettonWalletInfo");
                    }
                }
                else
                {
                    resultHandler(null, e);
                }
            });
        }

        private void CreateJettonCompete()
        {
            this.wallet.Wallets.Add(this.jettonMinter.ID, this.jettonMinter);
            this.wallet.Wallets.Add(this.jettonWallet.ID, this.jettonWallet);

            WalletsData.Wallets.Add(this.jettonMinter);
            WalletsData.Wallets.Add(this.jettonWallet);

            this.switchContainer.Current = new DoneWalletPanel(jettonMinter, "jettonAttached", "Jetton " + jettonMinter.Symbol, () =>
            {
                this.jettonWallet.Name = UniqueHelper.NextName("Wallet " + this.jettonWallet.Symbol, WalletsData.Wallets);
                Controller.ShowMainWallet(jettonMinter);
                this.Close();
            });
        }

        private void CreateJettonWallet()
        {
            this.CreateJettonWallet((wallet, e) =>
            {
                if (wallet != null)
                {
                    this.jettonWallet = wallet;
                    this.CreateJettonCompete();
                }
                else
                {
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CreateJettonWallet);
                }
            });
        }

        private void CreateJettonMinter()
        {
            this.Wait(null, "pleaseWait");
            this.CreateJettonMinter((minter, e) =>
            {
                if (minter != null)
                {
                    this.jettonMinter = minter;
                    this.CreateJettonWallet();
                }
                else
                {
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CreateJettonMinter);
                }
            });
        }

        private void CreateJettonWait()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("creatingJetton", " " + data.info.Symbol, "walletSendingGramsInfo", this.closeHandler, "continue", this.wallet.ThemeColor, this.CreateJettonMinter);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
        }

        private void CreateJettonSend()
        {
            this.wallet.CreateJetton(this.walletPasswordPanel.Passcode, data, (hash, e) =>
            {
                if (e != null)
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CreateJettonSend);
                else
                {
                    this.messageHash = hash;
                    this.wallet.WaitTransactions.Add(queryId);
                    this.wallet.WaitTransactions.Add(this.messageHash);
                    //this.wallet.WaitTransactions.Add(this.data.deployParams.internalHash);
                    this.CreateJettonWait();
                }
                data.Dispose();
            });
        }
        private void EnterWalletPassword(JettonDeployData data)
        {
            this.data = data;
            if (this.walletPasswordPanel == null)
            {
                this.walletPasswordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.infoPanel, closeHandler);
                this.walletPasswordPanel.Complete += (s) =>
                {
                    this.Wait("pleaseWait", null, null, closeHandler);
                    wallet.CheckPassword(this.walletPasswordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                CreateJettonSend();
                            else
                                this.ErrorLang("error", e, () => this.switchContainer.Current = this.walletPasswordPanel);
                        });
                    });
                };
            }
            this.walletPasswordPanel.ClearPasscode();
            this.walletPasswordPanel.DescriptionID = "enterWalletPassword";
            this.switchContainer.Next = this.walletPasswordPanel;
        }

        private class JettonInfoPanel : CaptionPanel
        {
            public JettonInfoPanel(CreateJettonController controller)
                : base("tokenInformation", () => controller.switchContainer.Current = controller.enterInfoPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, () => { })
            {
                this.controller = controller;

                infoContainer = new JettonInfoContainer(this.controller.wallet, false);
                infoContainer.Dock = DockStyle.Top;
                this.Add(infoContainer);

                controller.wallet.Adapter.Connected += Adapter_Changed;
                controller.wallet.Adapter.Disconnected += Adapter_Changed;
            }

            protected override void OnDisposed()
            {
                controller.wallet.Adapter.Connected -= Adapter_Changed;
                controller.wallet.Adapter.Disconnected -= Adapter_Changed;
                base.OnDisposed();
            }

            private void Adapter_Changed(object sender)
            {
                continueButton.BoxColor = controller.wallet.ThemeColor;
                continueButton.Enabled = controller.wallet.Adapter.IsConnected;
                if (this.img != null)
                    this.img.Dispose();
            }

            private CreateJettonController controller;
            private JettonInfoContainer infoContainer;
            private JettonDeployData data;
            private IImage img;

            public void Update(JettonDeployData data, IImage img)
            {
                if (this.data != null)
                    this.data.Dispose();
                this.data = data;
                if (this.img != null)
                    this.img.Dispose();
                this.img = img;

                this.infoContainer.Update(data, img);

                this.ClearMeasured();
                this.RelayoutAll();
            }

            protected override void Continue()
            {
                controller.Wait(null, "pleaseWait");
                SingleThread.Run(() =>
                {
                    this.controller.wallet.Adapter.CreateAccountState(data.JettonMinterAddress, (s, e) =>
                    {
                        if (s != null)
                        {
                            ContractState cs = s.State;
                            s.Dispose();
                            Application.Invoke(() =>
                            {
                                if (cs == ContractState.None)
                                    controller.EnterWalletPassword(this.data);
                                else
                                    this.controller.Error(Language.Current["jettonMinterIsCreated", data.JettonMinterAddress]);

                            });
                        }
                        else
                            this.controller.Error(null, e);
                    });
                });
            }
        }
    }
}
