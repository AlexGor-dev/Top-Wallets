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
        private class StartImportPanel : CaptionPanel
        {
            public StartImportPanel(CreateJettonController controller, string topTextID, string hintTextID)
                : base("enterTokenAddress", () => controller.switchContainer.Current = controller.mainPanel, controller.closeHandler, "continue", controller.wallet.ThemeColor, () => { })
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

            protected readonly CreateJettonController controller;
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
