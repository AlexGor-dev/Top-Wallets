using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Wallets;

namespace Complex.Ton
{
    public class NftInfoMenu : Menu
    {
        public NftInfoMenu(NftWallet wallet, NftInfo data)
            : base(new NftInfoContainer(wallet, data, true))
        {
            this.AnimationMode = true;
            this.MinimumSize.Set(500, 500);

        }

    }
}
