using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Navigation;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public class JettonEnterInfoPanel : CaptionPanel
    {
        public JettonEnterInfoPanel(string captionID, bool useTotalSupply, EmptyHandler goback, EmptyHandler closeHandler, int continueButtonColor, EmptyHandler waitHandler, ParamHandler<JettonInfo,IImage> resultHandler)
            : base(captionID, goback, closeHandler, "continue", continueButtonColor, ()=> { })
        {
            this.waitHandler = waitHandler;
            this.resultHandler = resultHandler;

            this.UseTab = true;

            TextComponent text = new TextLocalizeComponent("projectUnabbreviatedName");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            nameBox = new TextBox();
            nameBox.ErrorMode = true;
            nameBox.TabStop = true;
            nameBox.TabStopSelected = true;
            nameBox.ApplyOnLostFocus = true;
            nameBox.MaxHeight = 32;
            nameBox.HintTextID = "enterTokenName";
            nameBox.Dock = DockStyle.Top;
            nameBox.TextChanged += (s) => this.CheckEnabledSend();
            this.Add(nameBox);

            text = new TextLocalizeComponent("tokenSymbolInfo");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            Container container = new Container();
            container.Dock = DockStyle.Top;
            container.Inflate.width = 6;

            symbolBox = new TextBox();
            symbolBox.ErrorMode = true;
            symbolBox.TabStop = true;
            symbolBox.ApplyOnLostFocus = true;
            symbolBox.MaxHeight = 32;
            symbolBox.HintTextID = "enterTokenSymbol";
            symbolBox.Dock = DockStyle.Fill;
            symbolBox.TextChanged += (s) => this.CheckEnabledSend();
            container.Add(symbolBox);

            colorPickerButton = new GradientAndColorPickerButton("color", "jettonColor");
            colorPickerButton.MaxHeight = 28;
            colorPickerButton.Dock = DockStyle.Right;
            colorPickerButton.Container = this;
            colorPickerButton.ColorChanged += ColorPickerButton_ColorChanged;
            container.Add(colorPickerButton);

            this.Add(container);

            text = new TextLocalizeComponent("decimalPrecisionInfo");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            decimalBox = new NumberEditBoxEx();
            decimalBox.HintTextID = "9";
            decimalBox.TabStop = true;
            decimalBox.SignCount = 0;
            decimalBox.ApplyOnLostFocus = true;
            decimalBox.Maximum = 28;
            decimalBox.MinHeight = 32;
            decimalBox.Dock = DockStyle.Top;
            this.Add(decimalBox);

            text = new TextLocalizeComponent("numberTokensInfo");
            text.Visible = useTotalSupply;
            text.MultilineLenght = 50;
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            totalBox = new NumberEditBoxEx();
            totalBox.Visible = useTotalSupply;
            totalBox.HintTextID = "tokensMintHint";
            totalBox.TabStop = true;
            totalBox.SignCount = 28;
            totalBox.ApplyOnLostFocus = true;
            totalBox.Maximum = decimal.MaxValue;
            totalBox.MinHeight = 32;
            totalBox.Dock = DockStyle.Top;
            this.Add(totalBox);


            text = new TextLocalizeComponent("descriptionTokenInfo");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            descBox = new TextEditor();
            descBox.ScrollVisible = false;
            descBox.Multiline = true;
            descBox.TabStop = true;
            descBox.ToolTipInfo = new ToolTipInfo("enterTokenDescription");
            descBox.MinHeight = 40;
            descBox.HintTextID = "enterTokenDescription";
            descBox.Dock = DockStyle.Top;
            descBox.TextChanged += (s) => this.CheckEnabledSend();
            this.Add(descBox);

            text = new TextLocalizeComponent("enterTokenImageUrlOrSelectImageFile");
            text.Alignment = ContentAlignment.Left;
            text.Dock = DockStyle.Top;
            this.Add(text);

            imageBox = new FileDialogEditor(Language.Current["PNGImage"] + "|*.png|"
                                            + Language.Current["BMPImage"] + "|*.bmp|"
                                            + Language.Current["GIFImage"] + "|*.gif|"
                                            + Language.Current["JPEGImage"] + "|*.jpeg", Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures));
            imageBox.TabStop = true;
            imageBox.ApplyOnLostFocus = true;
            imageBox.MaxHeight = 32;
            imageBox.HintTextID = "enterTokenImageUrl";
            imageBox.Dock = DockStyle.Top;
            imageBox.TextChanged += (s) => this.CheckEnabledSend();
            this.Add(imageBox);

            this.Add(new Separator(DockStyle.Bottom, 20));
        }

        private void ColorPickerButton_ColorChanged(object sender)
        {
            this.colorSet = true;
        }

        private EmptyHandler waitHandler;
        private ParamHandler<JettonInfo, IImage> resultHandler;
        private TextBox nameBox;
        private TextBox symbolBox;
        private TextEditor descBox;
        private FileDialogEditor imageBox;
        private NumberEditBox decimalBox;
        private NumberEditBox totalBox;
        private GradientAndColorPickerButton colorPickerButton;
        private bool colorSet = false;

        private void CheckEnabledSend()
        {
            this.nameBox.ErrorMode = string.IsNullOrEmpty(this.nameBox.Text);
            this.symbolBox.ErrorMode = string.IsNullOrEmpty(this.symbolBox.Text);
            this.continueButton.Enabled = !this.nameBox.ErrorMode && !this.symbolBox.ErrorMode;

            if (!this.colorSet && !this.symbolBox.ErrorMode)
            {
                this.colorPickerButton.ColorValue = Complex.Drawing.Color.From(this.symbolBox.Text, 50);
                this.colorPickerButton.Invalidate();
            }

        }

        public void Update(string name, string symbol, string description, int decimals, decimal total, string image, string color)
        {
            this.nameBox.Text = name;
            this.symbolBox.Text = symbol;
            this.descBox.Text = description;
            this.decimalBox.Value = decimals;
            this.totalBox.Value = total;
            this.imageBox.Text = image;

            int c = Color.Parse(color);
            if (c != 0)
                this.colorSet = true;
            if (c == 0 && !string.IsNullOrEmpty(symbol))
                c = Complex.Drawing.Color.From(symbol, 55);
            if(c != 0)
                this.colorPickerButton.ColorValue = c;
        }

        public void Update(JettonInfo info)
        {
            this.Update(info.Name, info.Symbol, info.Description, info.Decimals, info.TotalSupply, info.ImageData, info.Color);
        }

        protected override void Continue()
        {
            this.waitHandler();
            Application.Run(() =>
            {
                string image = this.imageBox.Text;
                IImage img = null;
                if (!string.IsNullOrEmpty(image))
                {
                    try
                    {
                        if (System.IO.File.Exists(image))
                        {
                            img = Bitmap.FromFile(image);
                        }
                        else
                        {
                            if(Uri.TryCreate(image, UriKind.Absolute, out Uri uri))
                            {
                                img = ImageLoader.Load(image);
                            }
                            else
                            {
                                byte[] data = Convert.FromBase64String(image);
                                img = Bitmap.FromData(data);
                            }
                        }
                        if (img is Bitmap bmp)
                        {
                            Bitmap scaleimg = bmp.OvalImage(96, 16);
                            img.Dispose();
                            img = scaleimg;
                        }

                    }
                    catch (Exception e)
                    {
                    }
                }
                int dec = this.decimalBox.Value == 0 ? 9 : (int)this.decimalBox.Value;
                UInt128 value = (UInt128)(this.totalBox.Value * (decimal)Math.Pow(10, dec));
                JettonInfo info = new JettonInfo(this.nameBox.Text, this.descBox.Text, this.symbolBox.Text, image, value, dec, null, null, "#" + Color.A(this.colorPickerButton.ColorValue, this.colorPickerButton.ColorAlpha).ToString("X"), "{\"name\":\"Top-Wallets\",\"url\":\"https://complex-soft.com/top_wallets.html\"}");
                this.resultHandler(info, img);
            });
        }

    }
}
