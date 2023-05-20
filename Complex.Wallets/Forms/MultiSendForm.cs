using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class MultiSendForm : CaptionForm
    {
        public MultiSendForm(WalletAdapter adapter, ITranserParams transerParams)
            : base(new SwitchContainer(false))
        {
            this.adapter = adapter;
            this.transerParams = transerParams;
            this.MinimumSize.Set(500, 550);
            this.MaximumSize.Set(500, 550);
            this.controller = this.CreateMultiSendController();
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private MultiSendController controller;
        protected readonly WalletAdapter adapter;
        protected readonly ITranserParams transerParams;

        protected virtual MultiSendController CreateMultiSendController()
        {
            return new MultiSendController(this.Component as SwitchContainer, CloseCheck, null, adapter, transerParams);
        }
    }
}
