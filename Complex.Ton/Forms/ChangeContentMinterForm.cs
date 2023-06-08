using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class ChangeContentMinterForm : ChangeContentForm
    {
        public ChangeContentMinterForm(JettonMinter wallet)
            : base(wallet)
        {
        }

        private JettonInfoPanel infoPanel;
        private JettonMinter Wallet => base.wallet as JettonMinter;

        protected override Component CreateMainPanel()
        {
            JettonEnterInfoPanel panel = new JettonEnterInfoPanel("changeContent", false, () => switchContainer.Current = mainPanel, CloseCheck, wallet.ThemeColor, () => controller.Wait(null, "pleaseWait"), (info, img) =>
            {
                if (this.infoPanel == null)
                    this.infoPanel = new JettonInfoPanel(this);
                this.infoPanel.Update(info, img);
                this.switchContainer.Next = this.infoPanel;
            });
            panel.Update(this.Wallet.JettonInfo);
            return panel;
        }

        private class JettonInfoPanel : CaptionPanel
        {
            public JettonInfoPanel(ChangeContentMinterForm form)
                : base("tokenInformation", () => form.switchContainer.Current = form.mainPanel, form.CloseCheck, "continue", form.wallet.ThemeColor, () => { })
            {
                this.form = form;

                infoContainer = new JettonInfoContainer(this.form.wallet, false);
                infoContainer.Dock = DockStyle.Top;
                this.Add(infoContainer);
            }

            private ChangeContentMinterForm form;
            private JettonInfoContainer infoContainer;
            private JettonInfo info;
            private IImage img;

            public void Update(JettonInfo info, IImage img)
            {
                this.info = info;
                if (this.img != null)
                    this.img.Dispose();
                this.img = img;

                this.infoContainer.Update(info, this.form.Wallet.JettonInfo, img);

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
