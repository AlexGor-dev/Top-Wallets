using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public partial class CreateNftController
    {
        private EnterNftCollectionAddressPanel enterJettonAddressPanel;
        private NftImportInfoPanel nftImportInfoPanel;

        private NftInfo info;


        private void Import()
        {
            if (enterJettonAddressPanel == null)
                enterJettonAddressPanel = new EnterNftCollectionAddressPanel(this);
            this.switchContainer.Next = enterJettonAddressPanel;
        }

        public void ShowNftInfo(NftInfo info)
        {
            this.info = info;
            if (this.nftImportInfoPanel == null)
                this.nftImportInfoPanel = new NftImportInfoPanel(this);
            this.nftImportInfoPanel.HideGoback(this.mainPanel == null);
            this.nftImportInfoPanel.Update(info);
            this.switchContainer.Next = this.nftImportInfoPanel;

        }

        private void CompleteImportNft()
        {
            this.wallet.Adapter.CreateAccountState(info.Address, (s, e) =>
            {
                if (s != null)
                {
                    NftWallet wallet = null;
                    switch (s.Type)
                    {
                        case WalletType.NftCollection:
                            wallet = new NftCollection(this.wallet.AdapterID, info.Address, info, this.wallet);
                            break;
                        case WalletType.NftItem:
                        case WalletType.NftSingle:
                            wallet = new NftItem(this.wallet.AdapterID, info.Address, info, this.wallet);
                            break;
                    }
                    wallet.Update(s);
                    s.Dispose();
                    this.wallet.Wallets.Add(wallet.ID, wallet);
                    WalletsData.Wallets.Add(wallet);
                    this.switchContainer.Current = new DoneWalletPanel(wallet, "NftAttached", info.Type, () =>
                    {
                        Controller.ShowMainWallet(wallet);
                        this.Close();
                    });
                }
                else
                {
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CompleteImportNft);
                }
            });

        }

        private class EnterNftCollectionAddressPanel : StartImportPanel
        {
            public EnterNftCollectionAddressPanel(CreateNftController controller)
                : base(controller, "enterNftAddress", null)
            {
            }

            protected override void Import()
            {
                if (!this.controller.wallet.Adapter.IsValidAddress(addressBox.Text))
                    this.controller.Error(Language.Current["invalidAddress", addressBox.Text]);
                else
                {
                    this.controller.wallet.Adapter.GetNftInfo(addressBox.Text, (info, e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (info == null)
                                this.controller.Error(Language.Current["addressNotNft", addressBox.Text]);
                            else
                            {
                                if (WalletsData.Wallets.Contains(Wallet.GetID(this.controller.wallet.Adapter, info.Address, true)))
                                    this.controller.Error(Language.Current["walletAlreadyExist", info.Address]);
                                else
                                    this.controller.ShowNftInfo(info);
                            }
                        });
                    });
                }
            }
        }

        private class NftImportInfoPanel : CaptionPanel
        {
            public NftImportInfoPanel(CreateNftController controller)
                : base("nftInformation", null, "", () => controller.switchContainer.Current = controller.mainPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, () => { })
            {
                this.controller = controller;

                infoContainer = new NftInfoContainer(this.controller.wallet, controller.info, false);
                infoContainer.Dock = DockStyle.Fill;
                this.Add(infoContainer);
            }

            private CreateNftController controller;
            private NftInfoContainer infoContainer;
            private bool validOwnerAddress;
            private NftInfo info;

            public void Update(NftInfo info)
            {
                this.info = info;
                infoContainer.Update(info);
                this.validOwnerAddress = info.Owner == this.controller.wallet.Address;
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
                    this.controller.CompleteImportNft();
                else
                    this.controller.closeHandler();
            }
        }

        private class StartImportPanel : CaptionPanel
        {
            public StartImportPanel(CreateNftController controller, string topTextID, string hintTextID)
                : base("enterNftAddress", () => controller.switchContainer.Current = controller.mainPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, () => { })
            {
                this.controller = controller;

                TextComponent text = new TextLocalizeComponent(topTextID);
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                addressBox = new TextBox();
                addressBox.TabStop = true;
                addressBox.TabStopSelected = true;
                addressBox.ApplyOnLostFocus = true;
                addressBox.MaxHeight = 32;
                addressBox.HintTextID = hintTextID;
                addressBox.Dock = DockStyle.Top;
                addressBox.TextChanged += (s) => this.CheckEnabledSend();
                this.Add(addressBox);

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
                continueButton.Enabled = controller.wallet.Adapter.IsConnected && !addressBox.ErrorMode;
            }

            protected readonly CreateNftController controller;
            protected readonly TextBox addressBox;

            private void CheckEnabledSend()
            {
                addressBox.ErrorMode = !this.controller.wallet.Adapter.IsValidAddress(addressBox.Text);
                continueButton.Enabled = controller.wallet.Adapter.IsConnected && !addressBox.ErrorMode;

            }

            protected virtual void Import()
            {

            }

            protected override void Continue()
            {
                controller.Wait(null, "pleaseWait");
                SingleThread.Run(this.Import);
            }
        }

    }
}
