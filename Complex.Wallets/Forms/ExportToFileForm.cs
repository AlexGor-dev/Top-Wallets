using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Navigation;
using Complex.Files;

namespace Complex.Wallets
{
    public class ExportToFileForm : WalletForm
    {
        public ExportToFileForm(Wallet wallet)
            : base(wallet)
        {
        }

        protected override WalletController CreateController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
        {
            return new ExportToFileController(wallet, switchContainer, passcode, CloseCheck, CloseCheck);
        }
    }
}
