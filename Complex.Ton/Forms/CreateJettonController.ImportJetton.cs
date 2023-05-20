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
        private EnterJettonAddressPanel enterJettonAddressPanel;
        private JettonImportInfoPanel jettonImportInfoPanel;
        private JettonInfo info;

        private void ImportJetton()
        {
            this.enterJettonAddressPanel = new EnterJettonAddressPanel(this);
            this.switchContainer.Next = this.enterJettonAddressPanel;
        }


        public void ShowJettonInfo(JettonInfo info)
        {
            this.info = info;
            if (this.jettonImportInfoPanel == null)
                this.jettonImportInfoPanel = new JettonImportInfoPanel(this);
            this.jettonImportInfoPanel.HideGoback(this.mainPanel == null);
            this.jettonImportInfoPanel.Update(info);
            this.switchContainer.Next = this.jettonImportInfoPanel;
        }

        private void CompleteImportJetton()
        {
            this.wallet.Adapter.CreateAccountState(info.JettonAddress, (s, e) =>
            {
                if (s != null)
                {
                    JettonMinter wallet = new JettonMinter(this.wallet.AdapterID, info.JettonAddress, info, this.wallet);
                    wallet.Update(s);
                    s.Dispose();
                    this.wallet.Wallets.Add(wallet.ID, wallet);
                    WalletsData.Wallets.Add(wallet);
                    this.switchContainer.Current = new DoneWalletPanel(wallet, "jettonAttached", "Jetton " + wallet.Symbol, () =>
                    {
                        Controller.ShowMainWallet(wallet);
                        this.Close();
                    });
                }
                else
                {
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CompleteImportJetton);
                }
            });
        }

        private class EnterJettonAddressPanel : StartImportPanel
        {
            public EnterJettonAddressPanel(CreateJettonController controller)
                : base(controller, "enterJettonAddress", null)
            {
            }

            protected override void Import()
            {
                JettonInfo info = null;
                if (this.controller.wallet.Adapter.IsValidAddress(addressBox.Text))
                    info = this.controller.wallet.Adapter.Client.GetJettonInfo(addressBox.Text);
                Timer.Delay(300, () =>
                {
                    if (info == null)
                        this.controller.Error(Language.Current["addressNotJetton", addressBox.Text]);
                    else
                    {
                        if (WalletsData.Wallets.Contains(Wallet.GetID(this.controller.wallet.Adapter, info.JettonAddress, true)))
                            this.controller.Error(Language.Current["walletAlreadyExist", info.JettonAddress]);
                        else
                            this.controller.ShowJettonInfo(info);
                    }
                });
            }
        }
        private class JettonImportInfoPanel : CaptionPanel
        {
            public JettonImportInfoPanel(CreateJettonController controller)
                : base("tokenInformation", null, "", () => controller.switchContainer.Current = controller.mainPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, ()=> { })
            {
                this.controller = controller;

                infoContainer = new JettonInfoContainer(this.controller.wallet, false);
                infoContainer.Dock = DockStyle.Top;
                this.Add(infoContainer);
            }

            private CreateJettonController controller;
            private JettonInfoContainer infoContainer;
            private bool validOwnerAddress;
            private JettonInfo info;

            public void Update(JettonInfo info)
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
                    this.controller.CompleteImportJetton();
                else
                    this.controller.closeHandler();
            }
        }

    }
}