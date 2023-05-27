using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;
using Complex.Drawing;
using Complex.Collections;

namespace Complex.Ton
{
    public class CreateNftForm : CaptionForm
    {
        public CreateNftForm(TonWallet wallet)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(500, 550);
            this.controller = new CreateNftController(wallet, this.Component as SwitchContainer, CloseCheck);
            this.controller.Start();
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private CreateNftController controller;
    }

    public class AttachNftForm : CaptionForm
    {
        public AttachNftForm(TonWallet wallet, NftInfo info)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(500, 550);
            this.controller = new CreateNftController(wallet, this.Component as SwitchContainer, CloseCheck);
            this.controller.ShowNftInfo(info);
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private CreateNftController controller;
    }

    public partial class CreateNftController : SwitchFormController
    {
        public CreateNftController(TonWallet wallet, SwitchContainer switchContainer, EmptyHandler closeHandler)
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
        private object messageHash;
        private long queryId;
        private TransactionWaitPanel transactionWaitPanel;
        private Array<TransactionsInfo> transactionsInfos = new Array<TransactionsInfo>();

        public override void Start()
        {
            this.mainPanel = new MainPanel(this);
            this.switchContainer.Current = this.mainPanel;
        }

        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object value)
        {
            if (this.messageHash != null)
            {
                this.transactionsInfos.Add(new TransactionsInfo(sender as Wallet, wallet.Name, transaction));
                if (this.transactionsInfos.Count == 2)
                {
                    this.transactionWaitPanel.StopWait();
                    this.transactionWaitPanel.ContinueEnabled(true);
                    this.transactionsInfos.Sort((a, b) => -a.transaction.CompareTo(b.transaction));
                    this.transactionWaitPanel.AddTransactions(this.transactionsInfos.ToArray());
                }
            }
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(CreateNftController controller)
                : base("mintYourNft", null, "mintYourNftInfo", null, controller.closeHandler)
            {
                this.controller = controller;

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


                importNftCollectionButton = new ColorButton("attachNft");
                importNftCollectionButton.Enabled = controller.wallet.Adapter.IsConnected;
                importNftCollectionButton.BoxColor = controller.wallet.ThemeColor;
                importNftCollectionButton.Dock = DockStyle.Bottom;
                importNftCollectionButton.MinHeight = 40;
                importNftCollectionButton.Executed += (s) =>
                {
                    controller.Import();
                };
                this.Add(importNftCollectionButton);
                this.Add(new Separator(DockStyle.Bottom, 20));

                createNftSingleButton = new ColorButton("createNftSingle");
                createNftSingleButton.BoxColor = controller.wallet.ThemeColor;
                createNftSingleButton.Enabled = controller.wallet.Adapter.IsConnected;
                createNftSingleButton.Dock = DockStyle.Bottom;
                createNftSingleButton.MinHeight = 40;
                createNftSingleButton.Executed += (s) =>
                {
                    controller.CreateSingle();
                };
                this.Add(createNftSingleButton);

                this.Add(new Separator(DockStyle.Bottom, 20));

                createNftCollectionButton = new ColorButton("createNftCollection");
                createNftCollectionButton.BoxColor = controller.wallet.ThemeColor;
                createNftCollectionButton.Enabled = controller.wallet.Adapter.IsConnected;
                createNftCollectionButton.Dock = DockStyle.Bottom;
                createNftCollectionButton.MinHeight = 40;
                createNftCollectionButton.Executed += (s) =>
                {
                    controller.CreateCollection();
                };
                this.Add(createNftCollectionButton);


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
                createNftCollectionButton.BoxColor = controller.wallet.ThemeColor;
                importNftCollectionButton.BoxColor = controller.wallet.ThemeColor;
                createNftSingleButton.BoxColor = controller.wallet.ThemeColor;
                createNftCollectionButton.Enabled = controller.wallet.Adapter.IsConnected;
                importNftCollectionButton.Enabled = controller.wallet.Adapter.IsConnected;
                createNftSingleButton.Enabled = controller.wallet.Adapter.IsConnected;
            }

            private CreateNftController controller;
            private ColorButton createNftCollectionButton;
            private ColorButton createNftSingleButton;
            private ColorButton importNftCollectionButton;
        }

    }
}
