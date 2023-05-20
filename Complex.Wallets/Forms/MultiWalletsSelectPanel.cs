using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class MultiWalletsSelectPanel : CaptionPanel
    {
        public MultiWalletsSelectPanel(string captionTextID, Hashtable<object, Array<Wallet>> wallets, EmptyHandler closeHandler, ParamHandler<object, Wallet> completeHandler)
            : base(captionTextID, null, "selectWalletToSend", null, closeHandler, "continue", Theme.gray1, () =>{ })
        {
            this.completeHandler = completeHandler;
            this.wallets = wallets;

            this.anyView = new AnyView();
            this.anyView.VScrollStep = 30;
            this.anyView.Dock = DockStyle.Fill;
            this.anyView.Inflate.Set(4, 4);

            foreach (KeyValue<object, Array<Wallet>> kv in wallets.EnumKeyValue())
            {
                foreach (Wallet wallet in kv.Value)
                {
                    WalletItem item = new WalletItem(wallet);
                    item.Tag = kv.Key;
                    item.MinHeight = 90;
                    this.anyView.Add(item);
                }
            }

            this.anyView.SelectedComponentChanged += (s) =>
            {
                WalletItem item = this.anyView.SelectedComponent as WalletItem;
                if (item != null)
                {
                    this.buttonsPanel.Remove(typeof(WalletItem), true);
                    this.selectedWallet = item.Wallet;
                    this.selectedObject = item.Tag;
                    this.continueButton.Enabled = true;
                    WalletItem wi = new WalletItem(this.selectedWallet);
                    wi.Dock = DockStyle.Fill;
                    wi.MinHeight = 90;
                    this.buttonsPanel.Add(wi);
                }
                else
                {
                    this.buttonsPanel.Remove(typeof(WalletItem), true);
                    this.selectedWallet = null;
                    this.selectedObject = null;
                    this.continueButton.Enabled = false;
                }
            };
            this.Add(this.anyView);

            this.Add(new Separator(DockStyle.Bottom, 20));

            this.buttonsPanel = new ButtonsPanel();
            this.buttonsPanel.Padding.Set(4);
            this.buttonsPanel.BackRadius = 10;
            this.buttonsPanel.Dock = DockStyle.Bottom;
            this.buttonsPanel.MinHeight = 120;

            TextLocalizeComponent text = new TextLocalizeComponent("selectedWallet");
            text.Dock = DockStyle.Top;
            text.Alignment = ContentAlignment.Center;
            text.Font = Theme.font10Bold;
            this.buttonsPanel.Add(text);


            this.Add(this.buttonsPanel);

            this.continueButton.BringToFront();
            this.continueButton.Enabled = false;
        }

        private Hashtable<object, Array<Wallet>> wallets;
        private AnyView anyView;
        private ParamHandler<object, Wallet> completeHandler;
        private Wallet selectedWallet;
        private object selectedObject;
        private ButtonsPanel buttonsPanel;

        protected override void Continue()
        {
            completeHandler(this.selectedObject, this.selectedWallet);
        }
    }
}
