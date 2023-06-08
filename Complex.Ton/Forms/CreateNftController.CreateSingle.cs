using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public partial class CreateNftController
    {
        private NftSingleEnterInfoPanel enterSingleInfoPanel;
        private NftSingleInfoPanel singleInfoPanel;
        private NftSingleInfo singleInfo;
        private SendInfoPanel confirmCreateSinglePanel;
        private NftSingleDeployData singleDeployData;
        private PasswordPanel passwordPanel;
        private decimal ammount;

        private void CreateSingle()
        {
            if(enterSingleInfoPanel == null)
                enterSingleInfoPanel = new NftSingleEnterInfoPanel(this.wallet, false, "createNftSingle", () => switchContainer.Current = mainPanel, closeHandler, wallet.ThemeColor, () => Wait(null, "pleaseWait"), this.CreateSingleResult);
            this.switchContainer.Current = this.enterSingleInfoPanel;
        }

        private void CreateSingleResult(NftInfo info, decimal ammount, string error)
        {
            this.ammount = ammount;
            Timer.Delay(300, () =>
            {
                if (error != null)
                    this.Error(this.wallet.ThemeColor, error, null, false, () => switchContainer.Current = enterSingleInfoPanel);
                else
                {
                    this.singleInfo = info as NftSingleInfo;
                    if (this.singleInfoPanel != null)
                        this.singleInfoPanel.Dispose();
                    this.singleInfoPanel = new NftSingleInfoPanel(this);
                    this.switchContainer.Current = this.singleInfoPanel;
                }
            });
        }

        private void SingleCheckExist()
        {
            Wait(null, "pleaseWait");
            NftSingleDeployData data = NftController.CreateNftSingle(this.singleInfo, this.wallet.Balance.FromDecimal(this.ammount));
            this.wallet.Adapter.CreateAccountState(data.SingleAddress, (s, e) =>
            {
                Timer.Delay(300, () =>
                {
                    if (e != null)
                        this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", SingleCheckExist);
                    else
                    {
                        if (s.State == ContractState.Active)
                        {
                            this.Error(this.wallet.ThemeColor, Language.Current["walletAlreadyDeployed", data.SingleAddress], null, false, () => switchContainer.Current = enterSingleInfoPanel);
                            data.Dispose();
                        }
                        else
                        {
                            this.ConfirmCreateSingle(data);
                        }
                        s.Dispose();
                    }
                });
            });
        }

        private void CreateSingleCompete(NftItem wallet)
        {
            this.wallet.Wallets.Add(wallet.ID, wallet);
            WalletsData.Wallets.Add(wallet);
            this.switchContainer.Current = new DoneWalletPanel(wallet, "nftSingleAttached", UniqueHelper.NextName("Nft-single 1", WalletsData.Wallets), () =>
            {
                Controller.ShowMainWallet(wallet);
                this.Close();
            });
        }

        private void CreateNftSingle()
        {
            this.wallet.Adapter.CreateAccountState(singleDeployData.SingleAddress, (s, e) =>
            {
                if (s != null)
                {
                    NftInfo info = this.wallet.Adapter.GetNftInfo(s);
                    if (info != null)
                    {
                        NftItem wallet = new NftItem(this.wallet.AdapterID, singleDeployData.SingleAddress, info, this.wallet);
                        wallet.Update(s);
                        s.Dispose();
                        CreateSingleCompete(wallet);
                    }
                    else
                    {
                        this.Error(this.wallet.ThemeColor, "errorOccurred", "notFoundJettonInfo", "repeat", CreateNftSingle);
                    }
                }
                else
                {
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CreateNftSingle);
                }
            });
        }

        private void CreateSingleWait()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("creatingNftSingle", null, "walletSendingGramsInfo", this.closeHandler, "continue", this.wallet.ThemeColor, this.CreateNftSingle);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
        }

        private void CreateSingleSend()
        {
            this.wallet.CreateNftSingle(this.passwordPanel.Passcode, singleDeployData, (hash, e) =>
            {
                if (e != null)
                    this.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", CreateSingleSend);
                else
                {
                    this.messageHash = hash;
                    this.wallet.WaitTransactions.Add(this.messageHash);
                    this.CreateSingleWait();
                }
            });
        }

        private void SignSend()
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.confirmCreateSinglePanel, closeHandler);
                this.passwordPanel.Complete += (s) =>
                {
                    this.Wait("pleaseWait", null, null, closeHandler);
                    wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                this.CreateSingleSend();
                            else
                                this.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                        });
                    });
                };
            }
            this.passwordPanel.DescriptionID = "enterWalletPassword";
            this.passwordPanel.ClearPasscode();
            this.switchContainer.Next = this.passwordPanel;

        }

        private void ConfirmCreateSingle(NftSingleDeployData data)
        {
            if (this.singleDeployData != null)
                this.singleDeployData.Dispose();
            this.singleDeployData = data;

            if(this.confirmCreateSinglePanel == null)
                this.confirmCreateSinglePanel = new SendInfoPanel(() => switchContainer.Current = this.enterSingleInfoPanel, closeHandler, this.wallet.ThemeColor, SignSend);
            this.confirmCreateSinglePanel.UpdateFee(ammount, this.wallet.Balance, this.wallet.Balance.Clone(NftController.deployGas), this.wallet.Symbol);
            this.confirmCreateSinglePanel.Update(null, "confirmCreateNftSingle", Language.Current["createNftSingleSend", ammount.GetTextSharps(8) + " " + this.wallet.Symbol + " coins"]);
            this.switchContainer.Next = this.confirmCreateSinglePanel;

        }

        private class NftSingleInfoPanel : CaptionPanel
        {
            public NftSingleInfoPanel(CreateNftController controller)
                    : base("NftSingleInformation", () => controller.switchContainer.Current = controller.enterSingleInfoPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, controller.SingleCheckExist)
            {
                this.controller = controller;
                this.info = this.controller.singleInfo;

                infoContainer = new NftInfoContainer(this.controller.wallet, info, false);
                infoContainer.Dock = DockStyle.Fill;
                this.Add(infoContainer);

                Container container = null;
                TextComponent text = null;
                TextButton addressButton = null;

                if (!string.IsNullOrEmpty(info.EditorAddress))
                {
                    infoContainer.MainContainer.Add(new Separator(DockStyle.Bottom, 20));

                    container = new Container();
                    container.Dock = DockStyle.Bottom;

                    text = new TextLocalizeComponent("editorAddress");
                    text.MinWidth = 200;
                    text.Alignment = ContentAlignment.Left;
                    text.AppendRightText = ":";
                    text.Dock = DockStyle.Left;
                    text.Style = Theme.Get<CaptionStyle>();
                    container.Add(text);

                    addressButton = new TextButton(info.EditorAddress);
                    addressButton.Alignment = ContentAlignment.Left;
                    addressButton.MaxWidth = 250;
                    addressButton.Dock = DockStyle.Fill;
                    container.Add(addressButton);

                    ImageButton button = new ImageButton("copyAddress.svg");
                    button.MaxHeight = 20;
                    button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
                    button.Dock = DockStyle.Right;
                    button.Executed += (s) =>
                    {
                        Clipboard.SetText(info.OwnerAddress);
                        MessageView.Show(Language.Current["address"] + " " + info.EditorAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                    };
                    container.Add(button);
                    infoContainer.MainContainer.Add(container);
                }

                infoContainer.MainContainer.Add(new Separator(DockStyle.Bottom, 20));

                container = new Container();
                container.Dock = DockStyle.Bottom;

                text = new TextLocalizeComponent("royaltyAddress");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                addressButton = new TextButton(string.IsNullOrEmpty(info.RoyaltyParams.destination) ? "notSet" : info.RoyaltyParams.destination);
                addressButton.Alignment = ContentAlignment.Left;
                addressButton.MaxWidth = 250;
                addressButton.Dock = DockStyle.Fill;
                container.Add(addressButton);

                if (!string.IsNullOrEmpty(info.RoyaltyParams.destination))
                {

                    ImageButton button = new ImageButton("copyAddress.svg");
                    button.MaxHeight = 20;
                    button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
                    button.Dock = DockStyle.Right;
                    button.Executed += (s) =>
                    {
                        Clipboard.SetText(info.OwnerAddress);
                        MessageView.Show(Language.Current["address"] + " " + info.RoyaltyParams.destination + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                    };
                    container.Add(button);
                    infoContainer.MainContainer.Add(container);

                    container = new Container();
                    container.Dock = DockStyle.Bottom;

                    text = new TextLocalizeComponent("royaltyProcent");
                    text.MinWidth = 200;
                    text.Alignment = ContentAlignment.Left;
                    text.AppendRightText = ":";
                    text.Dock = DockStyle.Left;
                    text.Style = Theme.Get<CaptionStyle>();
                    container.Add(text);

                    text = new TextComponent(info.RoyaltyParams.Procent.GetTextSharps(8));
                    text.Alignment = ContentAlignment.Left;
                    text.AppendRightText = " %";
                    text.MaxWidth = 250;
                    text.Dock = DockStyle.Fill;
                    container.Add(text);

                }
                infoContainer.MainContainer.Add(container);
            }

            private CreateNftController controller;
            private NftSingleInfo info;
            private NftInfoContainer infoContainer;
        }
    }
}
