using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class WalletController : SwitchFormController
    {
        public WalletController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
            :base(switchContainer, closeHandler, doneHandler)
        {
            this.wallet = wallet;
            this.passcode = passcode;
        }

        protected readonly Wallet wallet;
        protected readonly string passcode;

        public void Error(string caption, string description, bool hidegoback, EmptyHandler back)
        {
            this.Error(wallet.ThemeColor, caption, description, hidegoback, back);
        }

        public void Wait(WalletAdapter adapter, string captionTextID, string descriptonTextID)
        {
            this.Wait(captionTextID, adapter == null ? "" : " " + wallet.Symbol + " coins", descriptonTextID, null);
        }
    }
}
