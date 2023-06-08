using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public class JettonReceiveForm : ReceiveForm
    {
        public JettonReceiveForm(JettonWallet wallet)
            : base(wallet)
        {

        }

        protected override ReceiveMainPanel CreateReceiveMainPanel()
        {
            return new ReceivePanel(this.wallet as JettonWallet, Language.Current["walletShareInfo", GetSymbolCoinsText()], CloseCheck, "copyWalletOwnerAddress", ShowInvoicePanel);
        }

        private class ReceivePanel : ReceiveMainPanel
        {
            public ReceivePanel(JettonWallet wallet, string mainText, EmptyHandler closeHandler, string continueTextID, EmptyHandler nextHandler)
                :base(wallet, "walletOwnerAddress", mainText, closeHandler, continueTextID, nextHandler)
            {
            }

            protected override string GetReceiveAddress()
            {
                return (this.wallet as JettonWallet).WalletInfo.OwnerAddress;
            }
        }
    }
}
