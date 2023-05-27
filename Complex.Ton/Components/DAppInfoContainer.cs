using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Ton.TonConnect;

namespace Complex.Ton
{
    public class DAppInfoContainer : Container
    {
        public DAppInfoContainer(Connection connection)
        {
            this.Padding.Set(4);
            this.Inflate.height = 10;

            Caption caption = new Caption(connection.wallet.Name);
            caption.Padding.Set(30, 0, 30, 0);
            caption.Dock = DockStyle.Top;
            this.Add(caption);

            TextComponent descriptionComponent = new TextComponent(Language.Current["connectionEstablishingUsing", connection.whiteWallet.name]);
            descriptionComponent.Font = Theme.font10;
            descriptionComponent.MultilineLenght = 60;
            descriptionComponent.Padding.Set(16, 6, 16, 6);
            descriptionComponent.Alignment = ContentAlignment.Center;
            descriptionComponent.RoundBack = true;
            descriptionComponent.RoundBackRadius = 10;
            descriptionComponent.Dock = DockStyle.Top;
            descriptionComponent.Style = Theme.Get<RoundLabelTheme>();
            this.Add(descriptionComponent);

            this.dappLabel = new ImageNameLabel(connection.dapp.Name, "dapp.svg");
            this.dappLabel.Inflate.height = 4;
            this.dappLabel.MaxHeight = 150;
            this.dappLabel.MinHeight = 150;
            this.dappLabel.Dock = DockStyle.Top;
            this.dappLabel.textComponent.Font = Theme.font10Bold;
            //this.dappLabel.textComponent.Dock = DockStyle.Top;
            this.Add(this.dappLabel);

            this.Add(new Separator(DockStyle.Top, 20));

            if (!string.IsNullOrEmpty(connection.dapp.TermsOfUseUrl))
            {
                Container container = new Container();
                container.Dock = DockStyle.Top;

                TextComponent text = new TextLocalizeComponent("termsOfUse");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                UrlTextComponent utext = new UrlTextComponent(connection.dapp.TermsOfUseUrl);
                utext.LinkExecuted += (s, url) => WinApi.ShellExecute(url);
                utext.MaxWidth = 200;
                utext.Dock = DockStyle.Fill;
                container.Add(utext);
                this.Add(container);
            }

            if (!string.IsNullOrEmpty(connection.dapp.PrivacyPolicyUrl))
            {
                Container container = new Container();
                container.Dock = DockStyle.Top;

                TextComponent text = new TextLocalizeComponent("privacyPolicy");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                UrlTextComponent utext = new UrlTextComponent(connection.dapp.PrivacyPolicyUrl);
                utext.LinkExecuted += (s, url) => WinApi.ShellExecute(url);
                utext.MaxWidth = 200;
                utext.Dock = DockStyle.Fill;
                container.Add(utext);
                this.Add(container);
            }

            connection.dapp.LoadImage((image) => { this.dappLabel.Image = image; this.dappLabel.Invalidate(); });
        }

        private ImageNameLabel dappLabel;

        protected override void OnDrawBack(Graphics g)
        {
            g.Smoosh(() => g.FillRoundRect(0, 0, Width, Height, 10, Theme.unselectedItemBackColor));
        }

    }
}
