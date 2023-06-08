using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Ton.TonConnect;

namespace Complex.Ton
{
    public class TonConnectForm : CaptionForm
    {
        public TonConnectForm(Connection connection, ConnectRequest[] requests)
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(400, 500);

            this.switchContainer = this.Container as SwitchContainer;
            this.connection = connection;
            this.connection.Error += (s, e) =>
            {
                this.controller.ErrorLang(e.Message, null);
            };

            this.requests = requests;
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
        private ConnectRequest[] requests;
        private MainPanel mainPanel;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;
        private PasswordPanel passwordPanel;

        private void Setpasscode()
        {
            this.passwordPanel = new PasswordPanel(false, null, CloseCheck);
            this.passwordPanel.DescriptionID = "enterWalletPassword";
            this.passwordPanel.Complete += (s) =>
            {
                controller.Wait("pleaseWait", null, null, CloseCheck);
                this.connection.wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                {
                    Timer.Delay(300, () =>
                    {
                        if (e == null)
                            this.Connect(this.passwordPanel.Passcode);
                        else
                            this.controller.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                    });
                });

            };
            this.switchContainer.Current = this.passwordPanel;
        }

        private void Connect(string passcode)
        {
            this.controller.Wait("pleaseWait", null, null, Cancel);
            this.connection.Connect(passcode, requests, (e) =>
            {
                Timer.Delay(300, () =>
                {
                    if (e != null)
                        this.controller.ErrorLang(e, null);
                    else
                    {
                        if (this.connection.IsConnected)
                        {
                            this.connection.wallet.Connections.Add(this.connection);
                            this.controller.DoneClose("connected", "");
                        }
                        else
                        {
                            this.controller.Error("dappDisconnect");
                        }
                    }
                });
            });
        }

        private void Decline()
        {
            this.controller.Wait("pleaseWait", null, null, CloseCheck);
            this.connection.Decline((e) =>
            {
                Timer.Delay(300, () =>
                {
                    if (e != null)
                        this.controller.ErrorLang(e, null);
                    else
                        this.switchContainer.Current = new InfoPanel(this.connection.wallet.ThemeColor, "connectionRejected", "UserDeclinedTheConnection", "close", null, CloseCheck, CloseCheck);
                    this.connection.Dispose();
                });
            });
        }

        private void Cancel()
        {
            this.CloseCheck();
            this.connection.Decline((s) => this.connection.Dispose());
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(TonConnectForm form)
                :base("connectToDApp", form.Cancel)
            {
                this.form = form;

                DAppInfoContainer container = new DAppInfoContainer(form.connection);
                container.Dock = DockStyle.Top;
                this.Add(container);


                ColorButton button = new ColorButton("connect");
                button.Dock = DockStyle.Bottom;
                button.MinHeight = 40;
                button.BoxColor = Theme.green0;
                button.Executed += (s) => form.Setpasscode();
                this.Add(button);

                this.Add(new Separator(DockStyle.Bottom, 20));

                button = new ColorButton("decline");
                button.Dock = DockStyle.Bottom;
                button.MinHeight = 40;
                button.BoxColor = Theme.red0;
                button.Executed += (s) => form.Decline();
                this.Add(button);

            }

            private TonConnectForm form;
        }
    }
}
