using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class ReceiveForm : CaptionForm
    {
        public ReceiveForm(Wallet wallet)
            : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 550);

            this.mainPanel = CreateReceiveMainPanel();
            this.switchContainer.Current = this.mainPanel;
        }

        protected readonly Wallet wallet;
        private ReceiveMainPanel mainPanel;
        private InvoicePanel invoicePanel;
        private InvoiceQrPanel invoiceQrPanel;
        private SwitchContainer switchContainer;
        private string invoiceUrl;
        private decimal invoiceAmmount;
        private string invoiceComment;

        protected string GetSymbolCoinsText()
        {
            return wallet.Symbol + " coins";
        }

        protected void ShowInvoicePanel()
        {
            if (this.invoicePanel == null)
                this.invoicePanel = new InvoicePanel(this);
            this.switchContainer.Current = this.invoicePanel;
        }

        protected virtual ReceiveMainPanel CreateReceiveMainPanel()
        {
            return new ReceiveMainPanel(wallet, "yourAddress", Language.Current["walletShareInfo", GetSymbolCoinsText()], CloseCheck, "copyWalletAddress", ShowInvoicePanel);
        }

        private void ShowInvoiceQrPanel(string url, decimal ammount, string comment)
        {
            this.invoiceUrl = url;
            this.invoiceAmmount = ammount;
            this.invoiceComment = comment;
            if (this.invoiceQrPanel == null)
                this.invoiceQrPanel = new InvoiceQrPanel(this);
            this.invoiceQrPanel.Update();
            this.switchContainer.Current = this.invoiceQrPanel;
        }

        private class InvoiceQrPanel : CaptionPanel
        {
            public InvoiceQrPanel(ReceiveForm form)
                : base("invoiceQr", " " + form.GetSymbolCoinsText() + " " + form.wallet.Adapter.NetName, () => form.switchContainer.Current = form.invoicePanel, form.CloseCheck, "copyQr", form.wallet.ThemeColor, () => { })
            {
                this.form = form;

                top = new Container();
                top.MinHeight = 30;
                top.Dock = DockStyle.Top;

                ammountLabel = new NameValueLabel("amount");
                ammountLabel.NameComponent.AppendRightText = ": ";
                ammountLabel.Dock = DockStyle.Left;
                top.Add(ammountLabel);

                if (form.wallet.IsSupportSendText)
                {
                    commentLabel = new NameValueLabel("comment");
                    commentLabel.NameComponent.AppendRightText = ": ";
                    commentLabel.Dock = DockStyle.Right;
                    top.Add(commentLabel);
                }

                this.Add(top);

                urlComponent = new TextComponent();
                urlComponent.RoundBackRadius = 10;
                urlComponent.Font = Theme.font10;
                urlComponent.Padding.Set(6);
                urlComponent.MultilineLenght = 44;
                urlComponent.RoundBack = true;
                urlComponent.Style = Theme.Get<RoundLabelTheme>();
                urlComponent.Dock = DockStyle.Top;
                this.Add(urlComponent);


                qRComponent = new QRComponent();
                qRComponent.Dock = DockStyle.Fill;
                this.Add(qRComponent);

            }

            private ReceiveForm form;
            private Container top;
            private NameValueLabel ammountLabel;
            private NameValueLabel commentLabel;
            private TextComponent urlComponent;
            private QRComponent qRComponent;

            public void Update()
            {
                top.Measured = false;
                ammountLabel.Visible = form.invoiceAmmount > 0;
                ammountLabel.ValueTextID = form.invoiceAmmount.GetTextSharps(8) + " " + form.wallet.Symbol;

                if (form.wallet.IsSupportSendText)
                {
                    commentLabel.Visible = !string.IsNullOrEmpty(form.invoiceComment);
                    commentLabel.ValueTextID = form.invoiceComment;
                }

                top.Visible = ammountLabel.Visible || commentLabel != null && commentLabel.Visible;
                if (top.Visible)
                    top.Layout();

                urlComponent.Text = form.invoiceUrl;
                qRComponent.Bitmap = form.wallet.Adapter.GenerateQRCode(form.invoiceUrl, 250, 250);
                this.Layout();
            }

            protected override void Continue()
            {
                Clipboard.SetBitmap(qRComponent.Bitmap);
                MessageView.Show(Language.Current["invoiceQr"] + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            }

        }


        private class InvoicePanel : CaptionPanel
        {
            public InvoicePanel(ReceiveForm form)
                :base("createInvoice", " " + form.GetSymbolCoinsText() + " " + form.wallet.Adapter.NetName, ()=> form.switchContainer.Current = form.mainPanel, form.CloseCheck, "copyInvoiceUrl", form.wallet.ThemeColor, ()=> { })
            {
                this.form = form;

                this.UseTab = true;

                TextLocalizeComponent text = new TextLocalizeComponent("amount");
                text.AppendRightText = " " + form.wallet.Symbol;
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Top;
                this.Add(text);

                ammountBox = new NumberEditBoxEx();
                ammountBox.TabStop = true;
                ammountBox.TabStopSelected = true;
                ammountBox.ApplyOnLostFocus = true;
                ammountBox.MaxHeight = 30;
                ammountBox.HintTextID = Language.Current["amount"] + " " + form.GetSymbolCoinsText();
                ammountBox.TabStop = true;
                ammountBox.ApplyOnLostFocus = true;
                ammountBox.MinHeight = 32;
                ammountBox.Dock = DockStyle.Top;
                ammountBox.Maximum = form.wallet.Balance.Maximum;
                ammountBox.ValueChanged += (s) =>
                {
                    urlComponent.Text = GetUrl();
                    this.Layout();
                };
                this.Add(ammountBox);

                if (form.wallet.IsSupportSendText)
                {
                    text = new TextLocalizeComponent("commentOptional");
                    text.Alignment = ContentAlignment.Left;
                    text.Dock = DockStyle.Top;
                    this.Add(text);

                    commentBox = new TextEditor();
                    commentBox.Multiline = true;
                    commentBox.ScrollVisible = false;
                    commentBox.TabStop = true;
                    //commentBox.ApplyOnLostFocus = true;
                    commentBox.MaxHeight = 30;
                    commentBox.TabStop = true;
                    commentBox.ToolTipInfo = new ToolTipInfo("commentOptional");
                    commentBox.MinHeight = 32;
                    commentBox.HintTextID = "commentOptional";
                    commentBox.Dock = DockStyle.Top;
                    commentBox.TextChanged += (s) =>
                    {
                        urlComponent.Text = GetUrl();
                        this.Layout();
                    };
                    this.Add(commentBox);
                }

                text = new TextLocalizeComponent("walletInvoiceInfo");
                text.Alignment = ContentAlignment.Center;
                text.MultilineLenght = 60;
                text.Dock = DockStyle.Top;
                this.Add(text);

                urlComponent = new TextComponent(GetUrl());
                urlComponent.Font = Theme.font10;
                urlComponent.Alignment = ContentAlignment.Middle;
                urlComponent.MultilineLenght = 44;
                urlComponent.RoundBack = true;
                urlComponent.RoundBackRadius = 10;
                urlComponent.Style = Theme.Get<RoundLabelTheme>();
                urlComponent.Dock = DockStyle.Fill;
                this.Add(urlComponent);

                TextComponent text2 = new TextLocalizeComponent(Language.Current["walletShareInvoiceUrlInfo", form.GetSymbolCoinsText()]);
                text2.Alignment = ContentAlignment.Center;
                text2.Dock = DockStyle.Bottom;
                this.Add(text2);

                TextButton textButton = new TextButton("generateQrCode");
                textButton.ClickEffect.EffectMode = ClickEffectMode.Quad;
                textButton.Font = Theme.font11Bold;
                textButton.Dock = DockStyle.Bottom;
                textButton.MinHeight = 40;
                textButton.DrawBorder = true;
                textButton.Executed += (s) =>
                {
                    form.ShowInvoiceQrPanel(GetUrl(), this.ammountBox.Value, this.commentBox != null ? this.commentBox.Text : null);
                };
                this.Add(textButton);

                this.continueButton.BringToFront();

            }

            private ReceiveForm form;
            private NumberEditBoxEx ammountBox;
            private TextEditor commentBox;

            private TextComponent urlComponent;

            private string GetUrl()
            {
                return form.wallet.GetInvoiceUrl(form.wallet.Address, ammountBox.Value, this.commentBox != null ? this.commentBox.Text : null);
            }

            protected override void Continue()
            {
                Uri uri = new Uri(GetUrl());
                Clipboard.SetText(uri.AbsoluteUri);
                MessageView.Show(Language.Current["invoiceUrl"] + " " + uri.AbsoluteUri + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            }
        }

    }
}
