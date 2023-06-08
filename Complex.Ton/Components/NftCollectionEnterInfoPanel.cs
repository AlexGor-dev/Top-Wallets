using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class NftCollectionEnterInfoPanel : CaptionPanel
    {
        public NftCollectionEnterInfoPanel(TonWallet wallet, string captionID, EmptyHandler goback, EmptyHandler closeHandler, int continueButtonColor, EmptyHandler waitHandler, ParamHandler<NftSingleInfo> resultHandler)
            : base(captionID, goback, closeHandler, "continue", continueButtonColor, () => { })
        {
            this.wallet = wallet;
            this.waitHandler = waitHandler;
            this.resultHandler = resultHandler;
            this.UseTab = true;

            TextComponent text = new TextLocalizeComponent("royaltyAddress");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            royaltyAddressBox = new TextBox();
            royaltyAddressBox.Text = wallet.Address;
            royaltyAddressBox.TabStop = true;
            royaltyAddressBox.ApplyOnLostFocus = true;
            royaltyAddressBox.MaxHeight = 32;
            royaltyAddressBox.HintTextID = "enterRoyaltyAddress";
            royaltyAddressBox.Dock = DockStyle.Top;
            royaltyAddressBox.TextChanged += (s) => this.CheckEnabledSend();
            this.Add(royaltyAddressBox);


            royaltyProcentBox = new NumberEditBoxEx();
            royaltyProcentBox.LeftTextID = "royaltyProcent";
            royaltyProcentBox.RightTextID = "%";
            royaltyProcentBox.Dock = DockStyle.Top;
            royaltyProcentBox.Maximum = 100;
            royaltyProcentBox.SignCount = 8;
            royaltyProcentBox.Value = 1;
            this.Add(royaltyProcentBox);

            this.Add(new Separator(DockStyle.Top, 20));

            text = new TextLocalizeComponent("contentUrl");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            contentBox = new TextBox();
            contentBox.ErrorMode = true;
            contentBox.TabStop = true;
            contentBox.TabStopSelected = true;
            contentBox.ApplyOnLostFocus = true;
            contentBox.MaxHeight = 32;
            contentBox.HintTextID = "enterContentUrl";
            contentBox.Dock = DockStyle.Top;
            contentBox.TextChanged += (s) => this.CheckEnabledSend();
            this.Add(contentBox);

            this.continueButton.Enabled = false;
        }

        private TonWallet wallet;
        private EmptyHandler waitHandler;
        private ParamHandler<NftSingleInfo> resultHandler;
        private TextBox royaltyAddressBox;
        private NumberEditBoxEx royaltyProcentBox;
        private TextBox contentBox;

        private void CheckEnabledSend()
        {
            this.royaltyAddressBox.ErrorMode = !string.IsNullOrEmpty(this.royaltyAddressBox.Text) && !this.wallet.Adapter.IsValidAddress(this.royaltyAddressBox.Text);
            this.contentBox.ErrorMode = string.IsNullOrEmpty(this.contentBox.Text) || !Uri.TryCreate(this.contentBox.Text, UriKind.Absolute, out Uri uri);
            this.continueButton.Enabled = !this.royaltyAddressBox.ErrorMode && !this.contentBox.ErrorMode;
        }

        public void Update(NftSingleInfo info)
        {
            this.royaltyAddressBox.Text = info.RoyaltyParams.destination;
            this.royaltyProcentBox.Value = info.RoyaltyParams.Procent;
            this.contentBox.Text = info.Content;
        }

        protected override void Continue()
        {
            this.waitHandler();
        }

    }
}
