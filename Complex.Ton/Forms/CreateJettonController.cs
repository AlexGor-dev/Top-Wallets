using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Ton
{
    public class CreateJettonForm : CaptionForm
    {
        public CreateJettonForm(TonWallet wallet)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(500, 550);
            this.controller = new CreateJettonController(wallet, this.Component as SwitchContainer, CloseCheck);
            this.controller.Start();
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private CreateJettonController controller;
    }

    public class AttachJettonForm : CaptionForm
    {
        public AttachJettonForm(TonWallet wallet, JettonInfo info)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(500, 550);
            this.controller = new CreateJettonController(wallet, this.Component as SwitchContainer, CloseCheck);
            this.controller.ShowJettonInfo(info);
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private CreateJettonController controller;
    }

    public class AttachJettonWalletForm : CaptionForm
    {
        public AttachJettonWalletForm(TonWallet wallet, JettonWalletInfo info)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(500, 550);
            this.controller = new CreateJettonController(wallet, this.Component as SwitchContainer, CloseCheck);
            this.controller.ShowJettonWalletInfo(info);
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private CreateJettonController controller;
    }

    public partial class CreateJettonController : SwitchFormController
    {
        public CreateJettonController(TonWallet wallet, SwitchContainer switchContainer, EmptyHandler closeHandler)
            :base(switchContainer, closeHandler, null)
        {
            this.wallet = wallet;
            this.wallet.TransactionComplete += Wallet_TransactionComplete;
        }

        protected override void OnDisposed()
        {
            this.wallet.TransactionComplete -= Wallet_TransactionComplete;
            base.OnDisposed();
        }
        private TonWallet wallet;
        private MainPanel mainPanel;

        public override void Start()
        {
            this.mainPanel = new MainPanel(this);
            this.switchContainer.Current = this.mainPanel;
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(CreateJettonController controller)
                : base("mintYourToken", null, "mintYourTokenInfo", null, controller.closeHandler)
            {
                this.controller = controller;

                this.Add(new Dummy(DockStyle.Top, 0, 10));

                //AdapterLabel adapterLabel = new AdapterLabel(form.wallet.Adapter, form.wallet.BannerImageID);
                //adapterLabel.Dock = DockStyle.Top;
                ////adapterLabel.MinHeight = 150;
                //this.Add(adapterLabel);


                CaptionStyle style = Theme.Get<CaptionStyle>();


                TextComponent caption = new TextLocalizeComponent("owner");
                caption.Dock = DockStyle.Top;
                caption.Style = style;
                this.Add(caption);

                TextComponent textComponent = new TextComponent(controller.wallet.Address);
                textComponent.Dock = DockStyle.Top;
                this.Add(textComponent);

                WalletItem item = new WalletItem(this.controller.wallet);
                item.Dock = DockStyle.Top;
                this.Add(item);

                //this.Add(new Separator(DockStyle.Bottom, 20));



                importJettonButton = new ColorButton("attachJetton");
                importJettonButton.Enabled = controller.wallet.Adapter.IsConnected;
                importJettonButton.BoxColor = controller.wallet.ThemeColor;
                importJettonButton.Dock = DockStyle.Bottom;
                importJettonButton.MinHeight = 40;
                importJettonButton.Executed += (s) =>
                {
                    controller.ImportJetton();
                };
                this.Add(importJettonButton);
                this.Add(new Separator(DockStyle.Bottom, 20));

                importJettonWalletButton = new ColorButton("attachJettonWallet");
                importJettonWalletButton.Enabled = controller.wallet.Adapter.IsConnected;
                importJettonWalletButton.BoxColor = controller.wallet.ThemeColor;
                importJettonWalletButton.Dock = DockStyle.Bottom;
                importJettonWalletButton.MinHeight = 40;
                importJettonWalletButton.Executed += (s) =>
                {
                    controller.ImportJettonWallet();
                };
                this.Add(importJettonWalletButton);
                this.Add(new Separator(DockStyle.Bottom, 20));


                createJettonButton = new ColorButton("createJetton");
                createJettonButton.BoxColor = controller.wallet.ThemeColor;
                createJettonButton.Enabled = controller.wallet.Adapter.IsConnected;
                createJettonButton.Dock = DockStyle.Bottom;
                createJettonButton.MinHeight = 40;
                createJettonButton.Executed += (s) =>
                {
                    controller.CreateJetton();
                };
                this.Add(createJettonButton);


                controller.wallet.Adapter.Connected += Adapter_Changed;
                controller.wallet.Adapter.Disconnected += Adapter_Changed;

            }

            protected override void OnDisposed()
            {
                controller.wallet.Adapter.Connected -= Adapter_Changed;
                controller.wallet.Adapter.Disconnected -= Adapter_Changed;
                base.OnDisposed();
            }

            private void Adapter_Changed(object sender)
            {
                createJettonButton.BoxColor = controller.wallet.ThemeColor;
                importJettonButton.BoxColor = controller.wallet.ThemeColor;
                createJettonButton.Enabled = controller.wallet.Adapter.IsConnected;
                importJettonButton.Enabled = controller.wallet.Adapter.IsConnected;
            }

            private CreateJettonController controller;
            private ColorButton createJettonButton;
            private ColorButton importJettonButton;
            private ColorButton importJettonWalletButton;
        }

    }
}
