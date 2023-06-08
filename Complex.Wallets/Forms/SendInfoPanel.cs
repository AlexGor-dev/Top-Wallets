using System;
using Complex.Controls;
using Complex.Themes;

namespace Complex.Wallets
{
    public class SendInfoPanel : CaptionPanel
    {
        public SendInfoPanel(EmptyHandler goback, EmptyHandler closeHandler, int continueColor, EmptyHandler continueHandler)
            : base("confirmation", goback, closeHandler, "", continueColor, continueHandler)
        {

            confirmText = new TextComponent();
            confirmText.Dock = DockStyle.Top;
            confirmText.MultilineLenght = 50;
            this.Add(confirmText);

            this.addressText = new TextComponent();
            this.addressText.RoundBack = true;
            this.addressText.Dock = DockStyle.Top;
            this.addressText.MinHeight = 30;
            this.addressText.Style = Theme.Get<RoundLabelTheme>();
            this.Add(this.addressText);

            this.feeText = new TextComponent();
            this.feeText.MinHeight = 20;
            //this.feeText.Enabled = false;
            this.feeText.Dock = DockStyle.Top;
            this.Add(this.feeText);

            this.errText = new TextComponent();
            this.errText.MultilineLenght = 50;
            this.errText.ForeColor = Theme.red0;
            this.errText.Font = Theme.font10Bold;
            this.errText.MinHeight = 40;
            this.errText.Dock = DockStyle.Top;
            this.Add(this.errText);

            this.Add(new Dummy(DockStyle.Top, 0, 20));

            this.Add(new Separator(DockStyle.Top, 20));

            this.messageText = new TextComponent();
            this.messageText.MultilineLenght = 50;
            this.messageText.Dock = DockStyle.Fill;
            this.messageText.Font = Theme.font10Bold;
            this.Add(this.messageText);


            this.errBotText = new TextComponent();
            this.errBotText.MultilineLenght = 50;
            this.errBotText.ForeColor = Theme.red1;
            this.errBotText.Font = Theme.font10Bold;
            this.errBotText.MinHeight = 40;
            this.errBotText.Dock = DockStyle.Bottom;
            this.Add(this.errBotText);

            this.continueButton.BringToFront();
        }

        private TextComponent confirmText;
        private TextComponent addressText;
        private TextComponent feeText;
        private TextComponent errText;
        private TextComponent messageText;
        private TextComponent errBotText;

        public void Update(string destAddress, decimal amount, string comment, string symbolText)
        {
            this.messageText.Text = string.IsNullOrEmpty(comment) ? "" : Language.Current["walletCantEncryptComment", comment];
            this.addressText.Text = destAddress;
            this.confirmText.Text = Language.Current["walletConfirmationText", amount.GetTextSharps(8), symbolText];
            this.continueButton.TextID = Language.Current["send"] + " " + amount.GetTextSharps(8) + " " + symbolText;
            this.continueButton.Enabled = false;
            this.errText.Visible = false;
            this.Layout();
        }

        public void Update(string destAddress, string confirmTextID, string continueText)
        {
            this.addressText.Text = destAddress;
            this.confirmText.Text = Language.Current[confirmTextID];
            this.continueButton.TextID = continueText;
            this.Layout();
        }
        public void UpdateFee(decimal amount, Balance walletBalance, Balance fee, string symbolText)
        {
            bool invalidAmount = fee + amount >= walletBalance;
            this.errText.Text = invalidAmount ? Language.Current["insufficientFunds", symbolText] : "";
            this.errText.Visible = invalidAmount;
            this.feeText.Text = fee > 0 ? Language.Current["fee"] + ": ≈" + fee.GetTextSharps(8) + " " + fee.Symbol : "";
            this.continueButton.Enabled = !invalidAmount;
            this.Invalidate();
        }

        public void UpdateDestState(WalletInfo destWallet, string symbolText)
        {
            if (destWallet != null && destWallet.state != WalletState.Active)
                this.errBotText.Text = Language.Current["walletSendWarningText", symbolText];
            else
                this.errBotText.Text = null;
        }

        public void UpdateError(string errorTextID)
        {
            this.errBotText.Text = Language.Current[errorTextID];
        }
    }
}
