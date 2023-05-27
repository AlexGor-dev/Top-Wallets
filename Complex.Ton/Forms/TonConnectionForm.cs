using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Ton.TonConnect;

namespace Complex.Ton
{
    public class TonConnectionForm : CaptionForm
    {
        public TonConnectionForm(Connection connection)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(350, 400);

            this.switchContainer = this.Container as SwitchContainer;
            this.connection = connection;
            this.mainPanel = new MainPanel(this);
            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);
            this.switchContainer.Current = this.mainPanel;
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private Connection connection;
        private MainPanel mainPanel;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;


        private void Disconnect()
        {
            this.controller.Wait("pleaseWait", null, null, CloseCheck);
            Util.Run(() => this.connection.Dispose());
            Timer.Delay(300, () => this.switchContainer.Current = new InfoPanel(this.connection.wallet.ThemeColor, "connectionTerminated", "UserDisconnectTheConnection", "close", null, CloseCheck, CloseCheck));
        }

        private class ConnectionContainer : Container
        {
            public ConnectionContainer(Connection connection)
            {
                this.Padding.Set(4);
                this.Inflate.height = 10;

                Caption caption = new Caption(connection.wallet.Name);
                caption.Padding.Set(30, 0, 30, 0);
                caption.Dock = DockStyle.Top;
                this.Add(caption);

                TextComponent descriptionComponent = new TextComponent(Language.Current["connectionEstablishedUsing", connection.whiteWallet.name]);
                descriptionComponent.Font = Theme.font10;
                descriptionComponent.MultilineLenght = 60;
                descriptionComponent.Padding.Set(16, 6, 16, 6);
                descriptionComponent.Alignment = ContentAlignment.Center;
                descriptionComponent.RoundBack = true;
                descriptionComponent.RoundBackRadius = 10;
                descriptionComponent.Dock = DockStyle.Top;
                descriptionComponent.Style = Theme.Get<RoundLabelTheme>();
                this.Add(descriptionComponent);

                ImageNameLabel label = new ImageNameLabel(Resources.Product, "top_wallets_48.png");
                label.Inflate.height = 4;
                label.MaxHeight = 70;
                label.Dock = DockStyle.Top;
                label.textComponent.Font = Theme.font10Bold;
                label.textComponent.Dock = DockStyle.Top;
                this.Add(label);

                waitConnection = new WaitConnection();
                waitConnection.Dock = DockStyle.Fill;
                this.Add(waitConnection);

                this.dappLabel = new ImageNameLabel(connection.dapp.Name, "dapp.svg");
                this.dappLabel.Inflate.height = 4;
                this.dappLabel.MaxHeight = 70;
                this.dappLabel.Dock = DockStyle.Bottom;
                this.dappLabel.textComponent.Font = Theme.font10Bold;
                this.dappLabel.textComponent.Dock = DockStyle.Bottom;
                this.Add(this.dappLabel);

                connection.dapp.LoadImage((image) => { this.dappLabel.Image = image; this.dappLabel.Invalidate(); });

            }

            private ImageNameLabel dappLabel;
            private WaitConnection waitConnection;

            protected override void OnCreated()
            {
                waitConnection.Start();
                base.OnCreated();
            }

            protected override void OnDrawBack(Graphics g)
            {
                g.Smoosh(() => g.FillRoundRect(0, 0, Width, Height, 10, Theme.unselectedItemBackColor));
            }
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(TonConnectionForm form)
                :base("connection", null, null, form.CloseCheck, "disconnect", Theme.red0, form.Disconnect)
            {
                ConnectionContainer container = new ConnectionContainer(form.connection);
                container.Dock = DockStyle.Fill;
                this.Add(container);
            }
        }
    }
}
