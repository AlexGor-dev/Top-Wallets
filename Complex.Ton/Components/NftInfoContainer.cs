using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Wallets;
using Complex.Animations;

namespace Complex.Ton
{
    public class NftInfoContainer : ButtonsPanel, IEndAnimation
    {
        public NftInfoContainer(TonUnknownWallet wallet, NftInfo info, bool activeButton)
        {
            this.wallet = wallet;
            this.info = info;
            this.activeButton = activeButton;
            this.Padding.Set(10);
            this.BackRadius = 10;
            this.animator = new Animator(this, 1, 500);

            this.waitDna = new WaitDna();
            this.waitDna.Padding.Set(150);
            this.waitDna.Dock = DockStyle.Fill;
            this.waitDna.Alpha = 0;
            this.Add(this.waitDna);

            main = new Container();
            main.Dock = DockStyle.Fill;

            main.Inflate.height = 10;


            caption = new Caption("Nft");
            caption.MinHeight = 24;
            caption.Padding.Set(30, 0, 30, 0);
            caption.Dock = DockStyle.Top;
            main.Add(caption);

            if (!this.activeButton && !string.IsNullOrEmpty(info.Type))
            {
                TextComponent text = new TextComponent(info.Type);
                text.Dock = DockStyle.Top;
                main.Add(text);
            }

            imageComponent = new ImageComponent("nft_token.svg");
            //imageComponent.MinSize.Set(256, 256);
            imageComponent.Dock = DockStyle.Fill;
            main.Add(imageComponent);

            errorComponent = new TextComponent();
            errorComponent.Visible = false;
            errorComponent.ForeColor = Theme.red0;
            errorComponent.Dock = DockStyle.Bottom;
            main.Add(errorComponent);

            descriptionComponent = new TextComponent("Description");
            //descriptionComponent.MinHeight = 70;
            descriptionComponent.Font = Theme.font10;
            descriptionComponent.MultilineLenght = 60;
            descriptionComponent.Padding.Set(16, 6, 16, 6);
            descriptionComponent.Alignment = ContentAlignment.Center;
            descriptionComponent.RoundBack = true;
            descriptionComponent.RoundBackRadius = 10;
            descriptionComponent.Dock = DockStyle.Bottom;
            descriptionComponent.Style = Theme.Get<RoundLabelTheme>();
            main.Add(descriptionComponent);


            Container container = null;

            if (!string.IsNullOrEmpty(info.OwnerAddress))
            {
                main.Add(new Separator(DockStyle.Bottom, 20));

                container = new Container();
                container.Dock = DockStyle.Bottom;

                TextComponent text = new TextLocalizeComponent("ownerAddress");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                TextButton addressButton = new TextButton(info.OwnerAddress);
                addressButton.Alignment = ContentAlignment.Left;
                addressButton.MaxWidth = 250;
                addressButton.Dock = DockStyle.Fill;
                addressButton.Executed += (s) =>
                {
                    if (this.activeButton)
                    {
                        Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Symbol, info.OwnerAddress);
                        this.Form.Hide();
                    }
                };
                container.Add(addressButton);

                ImageButton button = new ImageButton("copyAddress.svg");
                button.MaxHeight = 20;
                button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
                button.Dock = DockStyle.Right;
                button.Executed += (s) =>
                {
                    Clipboard.SetText(info.OwnerAddress);
                    MessageView.Show(Language.Current["address"] + " " + info.OwnerAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                };
                container.Add(button);
                main.Add(container);
            }

            if (!string.IsNullOrEmpty(info.CollectionAddress))
            {
                main.Add(new Separator(DockStyle.Bottom, 20));

                container = new Container();
                container.Dock = DockStyle.Bottom;

                TextComponent text = new TextLocalizeComponent("collectionAddress");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                TextButton addressButton = new TextButton(info.CollectionAddress);
                addressButton.Alignment = ContentAlignment.Left;
                addressButton.MaxWidth = 250;
                addressButton.Dock = DockStyle.Fill;
                addressButton.Executed += (s) =>
                {
                    if (this.activeButton)
                    {
                        Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Symbol, info.CollectionAddress);
                        this.Form.Hide();
                    }
                };
                container.Add(addressButton);

                ImageButton button = new ImageButton("copyAddress.svg");
                button.MaxHeight = 20;
                button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
                button.Dock = DockStyle.Right;
                button.Executed += (s) =>
                {
                    Clipboard.SetText(info.CollectionAddress);
                    MessageView.Show(Language.Current["address"] + " " + info.CollectionAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                };
                container.Add(button);
                main.Add(container);
            }

            if (info is NftSingleInfo sinfo)
            {
                main.Add(new Separator(DockStyle.Bottom, 20));

                container = new Container();
                container.Dock = DockStyle.Bottom;

                TextComponent text = new TextLocalizeComponent("editorAddress");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                TextButton addressButton = new TextButton(sinfo.EditorAddress);
                addressButton.Alignment = ContentAlignment.Left;
                addressButton.MaxWidth = 250;
                addressButton.Dock = DockStyle.Fill;
                addressButton.Executed += (s) =>
                {
                    if (this.activeButton)
                    {
                        Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Symbol, sinfo.EditorAddress);
                        this.Form.Hide();
                    }
                };
                container.Add(addressButton);

                ImageButton button = new ImageButton("copyAddress.svg");
                button.MaxHeight = 20;
                button.ToolTipInfo = new ToolTipInfo(button.Image, "copyAddress", null);
                button.Dock = DockStyle.Right;
                button.Executed += (s) =>
                {
                    Clipboard.SetText(info.CollectionAddress);
                    MessageView.Show(Language.Current["address"] + " " + sinfo.EditorAddress + " " + Language.Current["copiedToClipboard"] + ".", MessageViewType.Message);
                };
                container.Add(button);
                main.Add(container);

            }
            this.Add(main);


            this.info.LoadIContent(this.OnContentLoaded);
        }

        private TonUnknownWallet wallet;
        private NftInfo info;
        private bool activeButton;
        private Animator animator;

        private Caption caption;
        private ImageComponent imageComponent;
        public ImageComponent ImageComponent => imageComponent;

        private TextComponent descriptionComponent;
        private TextComponent errorComponent;

        private Container main;
        public Container MainContainer => main;

        private WaitDna waitDna;

        private NftContent content;

        public void Update(NftInfo data)
        {

        }

        protected override void OnDraw(Graphics g)
        {
            base.OnDraw(g);
        }
        protected virtual void OnContentLoaded(NftContent content, string error)
        {
            if (error != null)
            {
                descriptionComponent.Text = error;
                descriptionComponent.ForeColor = Theme.red0;
                main.Measured = false; 
                this.Layout();
            }
            else
            {
                this.content = content;
                this.waitDna.Start();
                this.animator.Start(-1);
            }
        }

        void IAnimation.OnAnimation(Animator animator, float value)
        {
            main.Alpha = animator.GetValue(255);
            this.waitDna.Alpha = animator.GetValueInvert(255);
            this.Invalidate();
        }

        void IEndAnimation.OnEndAnimation(Animator animator, float value)
        {
            if (this.animator.Dir < 0)
            {
                this.waitDna.Start();
                caption.Text = content.Name;
                descriptionComponent.Text = content.Description;
                content.LoadImage(256, 16, (img, e) =>
                {
                    if (this.IsDisposed)
                    {
                        if (img != null && img.Disposable)
                            img.Dispose();
                    }
                    else
                    {
                        if (imageComponent.Image != img)
                        {
                            if (img != null)
                                imageComponent.Image = img;
                            if (e != null)
                            {
                                errorComponent.Text = e;
                                errorComponent.Visible = true;
                            }

                            main.Measured = false;
                            this.Layout();
                            this.animator.Start(1);
                        }
                    }
                });
            }
            else
            {
                this.waitDna.Stop();
            }
        }
    }
}
