using System;
using System.Net;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;
using Complex.Remote;

namespace Complex.Ton
{
    public class NftSingleEnterInfoPanel : CaptionPanel
    {
        public NftSingleEnterInfoPanel(TonWallet wallet, bool editMode, string captionID, EmptyHandler goback, EmptyHandler closeHandler, int continueButtonColor, EmptyHandler waitHandler, ParamHandler<NftInfo, decimal, string> resultHandler)
            : base(captionID, goback, closeHandler, "continue", continueButtonColor, () => { })
        {
            this.wallet = wallet;
            this.editMode = editMode;
            this.waitHandler = waitHandler;
            this.resultHandler = resultHandler;

            this.UseTab = true;
            TextComponent text = null;

            if (!editMode)
            {
                text = new TextLocalizeComponent("owner");
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                //this.Add(text);

                ownerAddressBox = new TextBox();
                ownerAddressBox.Enabled = false;
                ownerAddressBox.Text = wallet.Address;
                ownerAddressBox.MaxHeight = 32;
                ownerAddressBox.Dock = DockStyle.Top;
                //this.Add(ownerAddressBox);

                //this.Add(new Separator(DockStyle.Top, 20));

                text = new TextLocalizeComponent("editorAddress");
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                editorAddressBox = new TextBox();
                editorAddressBox.Text = wallet.Address;
                editorAddressBox.TabStop = true;
                editorAddressBox.ApplyOnLostFocus = true;
                editorAddressBox.MaxHeight = 32;
                editorAddressBox.HintTextID = "enterEditorAddress";
                editorAddressBox.Dock = DockStyle.Top;
                editorAddressBox.TextChanged += (s) => this.CheckEnabled();
                this.Add(editorAddressBox);

                this.Add(new Separator(DockStyle.Top, 20));

                text = new TextLocalizeComponent("royaltyAddress");
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
                royaltyAddressBox.TextChanged += (s) => this.CheckEnabled();
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
            }

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
            contentBox.TextChanged += (s) => this.CheckEnabled();
            this.Add(contentBox);


            Container ct = new Container();
            ct.Dock = DockStyle.Bottom;

            text = new TextLocalizeComponent("amount");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Left;
            ct.Add(text);

            if (wallet.IsSupportMarket)
            {
                currencyLabel = new CurrencyLabel("", MainSettings.Current.General.Currency.ID);
                currencyLabel.ValueTextComponent.AppendLeftText = "≈";
                currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                currencyLabel.Dock = DockStyle.Fill;
                currencyLabel.Alignment = ContentAlignment.Right;
                ct.Add(currencyLabel);
            }
            this.Add(ct);

            ammountBox = new NumberEditBoxEx();
            ammountBox.ErrorMode = true;
            ammountBox.HintTextID = Language.Current["amount"] + " " + wallet.Symbol + " coins";
            ammountBox.TabStop = true;
            ammountBox.SignCount = 10;
            ammountBox.ApplyOnLostFocus = true;
            ammountBox.Maximum = 1000000000;
            ammountBox.MinHeight = 32;
            ammountBox.Dock = DockStyle.Bottom;
            ammountBox.ValueChanged += (s) => this.CheckEnabled();
            this.Add(ammountBox);

            ct = new Container();
            ct.Dock = DockStyle.Bottom;

            text = new TextLocalizeComponent("balance");
            text.AppendRightText = ":";
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Left;
            ct.Add(text);

            balance = new CurrencyLabel(null, wallet.Symbol);
            balance.ValueTextComponent.ForeColor = wallet.ThemeColor;
            balance.ValueTextComponent.Text = wallet.Balance.GetTextSharps(8);
            balance.Dock = DockStyle.Left;
            ct.Add(balance);

            this.Add(ct);


            this.continueButton.Enabled = false;
            this.continueButton.BringToFront();
        }

        private bool editMode;
        private TonWallet wallet;
        private EmptyHandler waitHandler;
        private ParamHandler<NftInfo, decimal, string> resultHandler;

        private TextBox editorAddressBox;
        private TextBox royaltyAddressBox;
        private NumberEditBoxEx royaltyProcentBox;
        private TextBox ownerAddressBox;
        private TextBox contentBox;
        private NumberEditBoxEx ammountBox;
        private CurrencyLabel currencyLabel;
        private CurrencyLabel balance;
        private NftInfo info;

        private void CheckEnabled()
        {
            if (!this.Updating)
            {
                if (!this.editMode)
                {
                    this.ownerAddressBox.ErrorMode = !string.IsNullOrEmpty(this.ownerAddressBox.Text) && !this.wallet.Adapter.IsValidAddress(this.ownerAddressBox.Text);
                    this.editorAddressBox.ErrorMode = !string.IsNullOrEmpty(this.editorAddressBox.Text) && !this.wallet.Adapter.IsValidAddress(this.editorAddressBox.Text);
                    this.royaltyAddressBox.ErrorMode = !string.IsNullOrEmpty(this.royaltyAddressBox.Text) && !this.wallet.Adapter.IsValidAddress(this.royaltyAddressBox.Text);
                    this.continueButton.Enabled = !this.editorAddressBox.ErrorMode && !this.royaltyAddressBox.ErrorMode && !this.contentBox.ErrorMode && !this.ownerAddressBox.ErrorMode && !this.ammountBox.ErrorMode;
                }
                else
                {
                    this.contentBox.ErrorMode = string.IsNullOrEmpty(this.contentBox.Text) || !Uri.TryCreate(this.contentBox.Text, UriKind.Absolute, out Uri uri) || string.Compare(this.info.Content, this.contentBox.Text.Trim(), true) == 0;
                    this.ammountBox.ErrorMode = (decimal)this.ammountBox.Value >= wallet.Balance || this.ammountBox.Value == 0;
                    this.continueButton.Enabled = !this.contentBox.ErrorMode && !this.ammountBox.ErrorMode;
                }

                if (currencyLabel != null)
                {
                    currencyLabel.ValueTextComponent.Text = ((decimal)ammountBox.Value * wallet.Market.LastPrice).GetTextSharps(2);
                    currencyLabel.Parent.ClearMeasured();
                    currencyLabel.Parent.RelayoutAll();
                }
            }
        }

        public void Update(NftInfo info)
        {
            this.info = info;
            this.contentBox.Text = info.Content;
            this.CheckEnabled();
        }

        protected override void Continue()
        {
            this.waitHandler();
            Util.Run(() =>
            {
                Uri uri = null;
                try
                {
                    if (!Uri.TryCreate(this.contentBox.Text, UriKind.Absolute, out uri))
                        throw new Exception("invalidUrl");
                    string data = Http.Get(this.contentBox.Text);
                    JsonArray array = Json.Parse(data) as JsonArray;
                    if (array == null)
                        throw new Exception("invalidJson");
                    if (this.editMode)
                        info = new NftInfo(this.info.Address, this.info.Index, this.info.OwnerAddress, this.info.CollectionName, this.info.CollectionAddress, this.contentBox.Text);
                    else
                        info = new NftSingleInfo(null, this.ownerAddressBox.Text, this.editorAddressBox.Text, this.contentBox.Text, new RoyaltyParams(this.royaltyProcentBox.Value, this.royaltyAddressBox.Text));
                    resultHandler(info, this.ammountBox.Value, null);
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.SecureChannelFailure)
                        resultHandler(null, 0, Language.Current["hostNotSupportCurrentWindowsVersion", uri.Host]);
                    else
                        resultHandler(null, 0, e.Message);
                }
                catch (Exception e)
                {
                    resultHandler(null, 0, e.Message);
                }
            });
        }

    }
}
