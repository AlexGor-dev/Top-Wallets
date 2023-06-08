using System;
using Complex.Collections;
using Complex.Controls;
using Complex.Wallets;

namespace Complex.Ton
{
    public class JettonMultiSendForm : MultiSendForm
    {
        public JettonMultiSendForm(TonAdapter adapter, TonUrl tonUrl)
            :base(adapter, tonUrl)
        {

        }

        protected override MultiSendController CreateMultiSendController()
        {
            return new JettonMultiSendController(this.Component as SwitchContainer, CloseCheck, null, adapter, transerParams);
        }


        private class JettonMultiSendController : MultiSendController
        {
            public JettonMultiSendController(SwitchContainer switchContainer, EmptyHandler closeHandler, EmptyHandler doneHandler, WalletAdapter adapter, ITranserParams transerParams)
                :base(switchContainer, closeHandler, doneHandler, adapter, transerParams)
            {

            }

            public override string GetRecipientTextID()
            {
                return "recipientOwnerWalletAddress";
            }

            protected override Hashtable<object, Array<Wallet>> GetSupportWallets()
            {
                TonUrl tonUrl = this.transerParams as TonUrl;
                Hashtable<object, Array<Wallet>> wallets = new Hashtable<object, Array<Wallet>>();
                Array<Wallet> arr = new Array<Wallet>();
                foreach (Wallet wallet in WalletsData.Wallets)
                {
                    if (wallet.IsMain && wallet is JettonWallet jw && jw.WalletInfo.JettonInfo.JettonAddress == tonUrl.Jetton && jw.WalletInfo.OwnerAddress != tonUrl.Address)
                    {
                        arr.Add(wallet);
                    }
                }
                if (arr.Count > 0)
                    wallets.Add(new object(), arr);
                return wallets;
            }
        }
    }
}
