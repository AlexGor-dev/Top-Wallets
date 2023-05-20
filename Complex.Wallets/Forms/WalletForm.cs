using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public abstract class WalletForm : CaptionForm
    {
        public WalletForm(Wallet wallet)
            : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 620);

            this.passwordPanel = new PasswordPanel(false, null, CloseCheck);
            this.passwordPanel.DescriptionID = "enterWalletPassword";
            this.passwordPanel.Complete += (s) =>
            {
                this.wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                {
                    this.controller = CreateController(wallet, switchContainer, this.passwordPanel.Passcode, CloseCheck, CloseCheck);
                    Application.Invoke(() =>
                    {
                        if (e == null)
                            this.controller.Start();
                        else
                            this.controller.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                    });
                });

            };
            this.switchContainer.Current = this.passwordPanel;
        }

        protected override void OnDisposed()
        {
            if(this.controller != null)
                this.controller.Dispose();
            base.OnDisposed();
        }

        private Wallet wallet;
        private SwitchContainer switchContainer;
        private PasswordPanel passwordPanel;
        private WalletController controller;

        protected abstract WalletController CreateController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler);

    }
}
