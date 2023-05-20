using System;
using Complex.Wallets;
using Complex.Controls;

namespace Complex.Ton
{
    public class JettonSendForm : SendForm
    {
        public JettonSendForm(JettonWallet wallet)
            :base(wallet, null, 0, null)
        {

        }

        protected override SendController CreateController()
        {
            return new JettonSendController(this.wallet, this.switchContainer, this.CloseCheck, null);
        }

        private class JettonSendController : SendController
        {
            public JettonSendController(Wallet wallet, SwitchContainer switchContainer, EmptyHandler closeHandler, EmptyHandler doneHandler)
                :base(wallet, switchContainer, closeHandler, doneHandler)
            {

            }

            public override string GetRecipientTextID()
            {
                return "recipientOwnerWalletAddress";
            }
        }
    }
}
