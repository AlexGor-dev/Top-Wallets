using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Navigation;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public partial class CreateJettonController
    {
        private EnterJettonWalletAddressPanel enterJettonWalletAddressPanel;
        private JettonWalletImportInfoPanel jettonWalletImportInfoPanel;
        private JettonWalletInfo walletInfo;

        private void ImportJettonWallet()
        {
            this.enterJettonWalletAddressPanel = new EnterJettonWalletAddressPanel(this);
            this.switchContainer.Next = this.enterJettonWalletAddressPanel;
        }

        public void ShowJettonWalletInfo(JettonWalletInfo info)
        {
            this.walletInfo = info;
            if (this.jettonWalletImportInfoPanel == null)
                this.jettonWalletImportInfoPanel = new JettonWalletImportInfoPanel(this);
            this.jettonWalletImportInfoPanel.HideGoback(this.mainPanel == null);
            this.jettonWalletImportInfoPanel.Update(info);
            this.switchContainer.Next = this.jettonWalletImportInfoPanel;
        }

        private void CompleteImportJettonWallet()
        {
            this.wallet.Adapter.CreateAccountState(walletInfo.Address, (s, e) =>
            {
                if (s != null)
                {
                    JettonWallet wallet = new JettonWallet(this.wallet.AdapterID, walletInfo.Address, walletInfo, this.wallet);
                    wallet.Update(s);
                    s.Dispose();
                    this.wallet.Wallets.Add(wallet.ID, wallet);
                    WalletsData.Wallets.Add(wallet);
                    this.switchContainer.Current = new DoneWalletPanel(wallet, "jettonWalletAttached", UniqueHelper.NextName("Wallet " + wallet.Symbol, WalletsData.Wallets), () =>
                    {
                        Controller.ShowMainWallet(wallet);
                        this.Close();
                    });
                }
                else
                {
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CompleteImportJettonWallet);
                }
            });
        }

        private class EnterJettonWalletAddressPanel : StartImportPanel
        {
            public EnterJettonWalletAddressPanel(CreateJettonController controller)
                : base(controller, "enterJettonWalletAddress", null)
            {
                //this.addressBox.Text = "EQBKxqyQ2tzAf7O4QINBkV0GoTEeWWL-hfbYq1MhAwOuMlvw";
            }

            protected override void Import()
            {
                JettonWalletInfo info = null;
                if (this.controller.wallet.Adapter.IsValidAddress(addressBox.Text))
                    info = this.controller.wallet.Adapter.Client.GetJettonWalletInfo(addressBox.Text);
                Timer.Delay(300, () =>
                {
                    if (info == null)
                        this.controller.Error(Language.Current["addressNotJettonWallet", addressBox.Text]);
                    else
                    {
                        if (WalletsData.Wallets.Contains(Wallet.GetID(this.controller.wallet.Adapter, info.Address, true)))
                            this.controller.Error(Language.Current["walletAlreadyExist", info.Address]);
                        else
                            this.controller.ShowJettonWalletInfo(info);
                    }
                });

            }
        }

        private class JettonWalletImportInfoPanel : CaptionPanel
        {
            public JettonWalletImportInfoPanel(CreateJettonController controller)
                : base("tokenInformation", null, "", () => controller.switchContainer.Current = controller.mainPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, () => { })
            {
                this.controller = controller;

                infoContainer = new JettonWalletInfoContainer(this.controller.wallet, false);
                infoContainer.Dock = DockStyle.Top;
                this.Add(infoContainer);

            }

            private CreateJettonController controller;
            private JettonWalletInfoContainer infoContainer;
            private bool validOwnerAddress;
            private JettonWalletInfo info;

            public void Update(JettonWalletInfo info)
            {
                this.info = info;
                infoContainer.Update(info);

                this.validOwnerAddress = info.OwnerAddress == this.controller.wallet.Address;
                if (this.validOwnerAddress)
                {
                    descriptionComponent.TextID = "addressOwnerValid";
                    descriptionComponent.ForeColor = Theme.green2;
                    continueButton.TextID = "continue";
                }
                else
                {
                    descriptionComponent.TextID = "addressOwnerInvalid";
                    descriptionComponent.ForeColor = Theme.red2;
                    continueButton.TextID = "close";
                }

                this.ClearMeasured();
                this.RelayoutAll();
            }

            protected override void Continue()
            {
                if (this.validOwnerAddress)
                    this.controller.CompleteImportJettonWallet();
                else
                    this.controller.closeHandler();
            }

        }

    }
}
