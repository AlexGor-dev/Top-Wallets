using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Navigation;
using Complex.Files;

namespace Complex.Wallets
{
    public class DeleteWalletForm : WalletForm
    {
        public DeleteWalletForm(Wallet wallet)
        : base(wallet)
        {
        }

        protected override WalletController CreateController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
        {
            return new DeleteWalletController(wallet, switchContainer, passcode, CloseCheck, CloseCheck);
        }
    }
}
