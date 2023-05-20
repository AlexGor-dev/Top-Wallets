using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Themes;

namespace Complex.Wallets
{
    public class SupportForm : CaptionForm
    {
        public SupportForm()
            : base(new SwitchContainer(false))
        {
            this.MinimumSize.Set(500, 550);
            this.MaximumSize.Set(500, 550);
            this.controller = new SupportSendController(this.Component as SwitchContainer, CloseCheck, null);
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private SupportSendController controller;
    }
}
