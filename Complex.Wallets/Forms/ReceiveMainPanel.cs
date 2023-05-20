using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class ReceiveMainPanel : CaptionPanel
    {
        public ReceiveMainPanel(Wallet wallet, string captionID, string mainText, EmptyHandler closeHandler, string continueTextID, EmptyHandler nextHandler)
            :base(captionID, " " + wallet.NameOrCoins + " " + wallet.Adapter.NetName, null, closeHandler, continueTextID, wallet.ThemeColor, ()=> { })
        {
            this.wallet = wallet;

            TextComponent textComponent = new TextComponent(mainText);
            textComponent.Dock = DockStyle.Top;
            textComponent.MinHeight = 50;
            this.Add(textComponent);

            TextComponent addressComponent = new TextComponent(GetReceiveAddress());
            addressComponent.Padding.Set(10, 0, 10, 0);
            addressComponent.RoundBack = true;
            addressComponent.Dock = DockStyle.Top;
            addressComponent.MinHeight = 30;
            addressComponent.Style = Theme.Get<RoundLabelTheme>();
            this.Add(addressComponent);

            QRComponent qRComponent = new QRComponent();
            qRComponent.Bitmap = wallet.Adapter.GenerateQRCode(GetReceiveAddress(), 250, 250);
            qRComponent.Dock = DockStyle.Fill;
            this.Add(qRComponent);

            if (wallet.IsSupportInvoiceUrl)
            {
                TextButton textButton = new TextButton("createInvoice");
                textButton.ClickEffect.EffectMode = ClickEffectMode.Quad;
                textButton.Font = Theme.font11Bold;
                textButton.Dock = DockStyle.Bottom;
                textButton.MinHeight = 40;
                textButton.DrawBorder = true;
                textButton.Executed += (s) =>
                {
                    nextHandler();
                };
                this.Add(textButton);
            }

            this.continueButton.BringToFront();
        }

        protected readonly Wallet wallet;

        protected virtual string GetReceiveAddress()
        {
            return wallet.Address;
        }

        protected override void Continue()
        {
            Clipboard.SetText(wallet.Address);
            MessageView.Show(Language.Current["address"] + " " + GetReceiveAddress() + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
            closeHandler();

        }

    }
}
