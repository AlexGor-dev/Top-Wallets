using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public class JettonMenu : Menu
    {
        public JettonMenu(TonUnknownWallet wallet, JettonInfo info, bool activeButton)
            :base(new JettonInfoContainer(wallet, info, activeButton))
        {
            this.AnimationMode = true;
        }
    }
}
