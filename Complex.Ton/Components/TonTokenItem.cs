using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Animations;
using Complex.Wallets;

namespace Complex.Ton
{
    public class TonTokenItem : TokenItem
    {
        public TonTokenItem(TonUnknownWallet wallet, JettonWalletInfo token, GridWaitEffect waitEffect)
            :base(wallet, token, waitEffect)
        {
            this.wallet = wallet;
            this.token = token;
        }

        private TonUnknownWallet wallet;
        private JettonWalletInfo token;
        protected override void OnNameButtonClicked(CheckedButton button)
        {
            JettonMenu menu = new JettonMenu(this.wallet as TonUnknownWallet, this.token.JettonInfo, true);
            menu.Hided += (s2) => button.Checked = false;
            menu.Show(button, MenuAlignment.TopLeft);
        }
    }
}
