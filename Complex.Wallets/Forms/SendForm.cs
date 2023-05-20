using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class SendForm : CaptionForm
    {
        public SendForm(Wallet wallet, string address, decimal amount, string comment)
        : base(new SwitchContainer(false))
        {
            this.wallet = wallet;
            this.switchContainer = this.Container as SwitchContainer;
            this.MinimumSize.Set(500, 550);
            this.MaximumSize.Set(500, 550);
            this.controller = CreateController();
            SendMainPanel sendMain = new SendMainPanel(this.controller, address, amount, comment, null);
            //sendMain.InitTransactions();
            this.switchContainer.Current = sendMain;
            this.controller.SetMainPanel(sendMain);
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        protected readonly Wallet wallet;
        protected readonly SwitchContainer switchContainer;
        private SendController controller;

        protected virtual SendController CreateController()
        {
            return new SendController(this.wallet, this.switchContainer, this.CloseCheck, null);
        }
    }
}
