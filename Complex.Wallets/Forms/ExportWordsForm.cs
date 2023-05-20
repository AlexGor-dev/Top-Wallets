using System;
using Complex.Controls;
using Complex.Themes;

namespace Complex.Wallets
{
    public class ExportWordsForm : WalletForm
    {
        public ExportWordsForm(Wallet wallet)
            : base(wallet)
        {
        }

        protected override WalletController CreateController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
        {
            return new ExportWordsController(wallet, switchContainer, passcode, CloseCheck, CloseCheck);
        }
    }
}
