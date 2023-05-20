using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public partial class TotalPanel
    {
        private class SupportItem : Container
        {
            public SupportItem(TopSupport support, bool popupMode)
            {
                this.support = support;
                WalletAdapter adapter = Controller.GetAdapter(support.AdapterID);
                this.Padding.Set(10);
                this.Inflate.height = 10;
                if (popupMode)
                    this.MaxWidth = 480;
                nameCaption = new UrlTextComponent(string.IsNullOrEmpty(support.Name) ? Language.Current["incognito"] : support.Name);
                nameCaption.LinkExecuted += (s, l) =>
                {
                    WinApi.ShellExecute(IntPtr.Zero, l);
                    this.Form.Hide();
                };
                nameCaption.Alignment = ContentAlignment.Left;
                nameCaption.Style = Theme.Get<CaptionForeTheme>();
                nameCaption.Dock = DockStyle.Top;
                nameCaption.Font = Theme.font11Bold;
                this.Add(nameCaption);

                if (!string.IsNullOrEmpty(support.Message))
                {
                    UrlTextComponent text = new UrlTextComponent(support.Message);
                    text.LinkExecuted += (s, l) =>
                    {
                        WinApi.ShellExecute(IntPtr.Zero, l);
                        this.Form.Hide();
                    };
                    text.Alignment = ContentAlignment.Left;
                    text.Dock = DockStyle.Fill;
                    text.MultilineLenght = 50;
                    this.Add(text);
                }

                Container container = new Container();
                container.Dock = DockStyle.Bottom;
                container.Inflate.width = 6;


                if (!popupMode)
                {
                    ImageButton button = new ImageButton("copyAddress.svg");
                    button.MaxHeight = 18;
                    button.ToolTipInfo = new ToolTipInfo(button.Image, "copyMessage", null);
                    button.Dock = DockStyle.Left;
                    button.Executed += (s) =>
                    {
                        string message = nameCaption.Text + Environment.NewLine + support.Message + Environment.NewLine + "(" + support.Time.ToLocalLongDateTimeString() + ") (" + support.Amount.GetTextSharps(support.SignCount) + support.Symbol + ")";
                        Clipboard.SetText(message);
                        MessageView.Show(Language.Current["message"] + " " + message + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                    };
                    container.Add(button);
                }

                CurrencyLabel currencyLabel = new CurrencyLabel(support.Amount.GetTextSharps(support.SignCount), support.Symbol);
                //currencyLabel.ValueTextComponent.Font = Theme.font10Bold;
                currencyLabel.ValueTextComponent.ForeColor = adapter.ThemeColor;
                currencyLabel.Dock = DockStyle.Left;
                container.Add(currencyLabel);

                currencyLabel = new CurrencyLabel((support.Amount * adapter.Market.LastPrice).GetTextSharps(2), MainSettings.Current.General.Currency.ID);
                currencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                currencyLabel.ValueTextComponent.AppendLeftText = "≈";
                currencyLabel.Dock = DockStyle.Left;
                container.Add(currencyLabel);


                TextComponent timeComponent = new TextComponent((DateTime.UtcNow - support.Time).ToYMD() + " " + Language.Current["ago"]);
                //TextComponent timeComponent = new TextComponent(support.Time.ToLocalLongDateTimeString());
                timeComponent.Padding.Set(4, 0, 4, 0);
                timeComponent.Dock = DockStyle.Right;
                timeComponent.Font = Theme.font9;
                container.Add(timeComponent);

                this.Add(container);
            }

            private TopSupport support;
            private UrlTextComponent nameCaption;

            private readonly Rect dispRect = new Rect();

            private ColorSetTheme colorSet = Theme.Get<ColorSetTheme>();

            protected override void OnSizeChanged()
            {
                GetDisplayRectangle(dispRect);
                base.OnSizeChanged();
            }

            protected override void OnDrawBack(Graphics g)
            {
                if (!string.IsNullOrEmpty(support.Name))
                {
                    char ch = support.Name.FindFirtsChar();
                    if (ch != 0)
                        nameCaption.ForeColor = colorSet.colors[ch % colorSet.colors.Length];
                }
                g.Smoosh(() => g.FillRoundRect(dispRect, 10, Theme.topBackColor));
            }


        }

    }
}
