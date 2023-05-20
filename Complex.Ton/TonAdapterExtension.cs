using System;
using Complex.Ton;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class TonAdapterExtension : WalletAdapterExtension
    {
        public TonAdapterExtension(IData data)
                : base(data)
        {
        }

        public TonAdapterExtension(bool testnet = false)
                : base(testnet)
        {
        }

        public override bool SupportWallets => true;
        public override bool SupportExplorer => true;

        public override string ImageID => "ton.svg";

        public override string Symbol => "TON";
        public override string Name => "Toncoin";

        public override ThemeColor TestThemeColor => new ThemeColor(Color.Argb(52, 131, 171), Color.Argb(52, 131, 171), Color.Argb(52, 131, 171), Color.Argb(52, 131, 171));
        public override ThemeColor MainThemeColor => new ThemeColor(Color.Argb(33, 144, 199), Color.Argb(70, 129, 187), Color.Argb(33, 144, 199), Color.Argb(70, 129, 187));


        public override WalletAdapter CreateAdapter()
        {
            return new TonAdapter(this);
        }

    }
}
