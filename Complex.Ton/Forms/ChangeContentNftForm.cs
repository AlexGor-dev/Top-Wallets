using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class ChangeContentNftForm : ChangeContentForm
    {
        public ChangeContentNftForm(NftItem wallet)
            : base(wallet)
        {
        }

        private NftInfoPanel infoPanel;
        private NftItem Wallet => base.wallet as NftItem;
        private decimal ammount;

        protected override Component CreateMainPanel()
        {
            NftSingleEnterInfoPanel panel = new NftSingleEnterInfoPanel(this.wallet.Parent, true, "changeContent", () => switchContainer.Current = mainPanel, CloseCheck, wallet.ThemeColor, () => controller.Wait(null, "pleaseWait"), (info, ammount, e) =>
            {
                if (e != null)
                {
                    this.controller.ErrorLang( e, null, ()=> this.switchContainer.Current = this.mainPanel);
                }
                else
                {
                    this.ammount = ammount;
                    if (this.infoPanel == null)
                        this.infoPanel = new NftInfoPanel(this, info);
                    this.infoPanel.Update(info);
                    this.switchContainer.Next = this.infoPanel;
                }
            });
            panel.Update(this.Wallet.Info);
            return panel;
        }

        private class NftInfoPanel : CaptionPanel
        {
            public NftInfoPanel(ChangeContentNftForm form, NftInfo info)
                : base("tokenInformation", () => form.switchContainer.Current = form.mainPanel, form.CloseCheck, "continue", form.wallet.ThemeColor, () => { })
            {
                this.form = form;

                infoContainer = new NftInfoContainer(this.form.wallet, info, false);
                infoContainer.Dock = DockStyle.Top;
                this.Add(infoContainer);
            }

            private ChangeContentNftForm form;
            private NftInfoContainer infoContainer;
            private INftInfo info;

            public void Update(NftInfo info)
            {
                this.info = info;
                this.infoContainer.Update(info);

                this.ClearMeasured();
                this.RelayoutAll();
            }

            protected override void Continue()
            {
                this.form.ChangeContentConfirmation(this.info);
            }
        }
    }
}
