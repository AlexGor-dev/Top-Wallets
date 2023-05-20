using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class DoneWalletPanel : DonePanel
    {
        public DoneWalletPanel(Wallet wallet, string descriptionID, string defaultName, EmptyHandler buttonHandler)
            : base("walletReady", descriptionID, "walletView", wallet.Adapter.ThemeColor, buttonHandler)
        {
            this.wallet = wallet;
            string name = this.wallet.OriginalName;
            if (string.IsNullOrEmpty(name))
                name = defaultName;
            this.wallet.Name = name;

            int index = this.Components.Count - 2;
            this.Insert(index, new Separator(DockStyle.Bottom, 20));
            index++;

            TextLocalizeComponent text = new TextLocalizeComponent("setWalletName");
            text.Dock = DockStyle.BottomCenter;
            this.Insert(index, text);
            index++;

            textBox = new TextBox();
            textBox.ClearSelectedOnFreeDown = true;
            textBox.Text = this.wallet.Name;
            textBox.ApplyOnLostFocus = true;
            textBox.TextChanged += (s) => { this.wallet.Name = (s as TextBox).Text; };
            textBox.MaxWidth = 200;
            textBox.MinWidth = 200;
            textBox.HintTextID = "setWalletName";
            textBox.Dock = DockStyle.BottomCenter;
            this.Insert(index, textBox);
            index++;

            this.Insert(index, new Separator(DockStyle.Bottom, 20));

            this.continueButton.BringToFront();
        }

        public DoneWalletPanel(Wallet wallet, string defaultName, EmptyHandler buttonHandler)
            :this(wallet, "walletReadyInfo", defaultName, buttonHandler)
        {
        }
        private Wallet wallet;
        private TextBox textBox;

        protected override void OnCreated()
        {
            textBox.Focused = true;
            base.OnCreated();
        }
    }
}
