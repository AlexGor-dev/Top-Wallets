using System;
using Complex.Collections;

namespace Complex.Wallets
{
    [Serializable]
    public class CoinGeckoInfo : IUnique
    {
        public string ID => symbol;

        private string symbol;
        public string Symbol => symbol;

        private string geckoSymbol;
        public string GeckoSymbol => geckoSymbol;

        private string geckoID;
        public string GeckoID => geckoID;

        private double tickSize;
        public double TickSize => tickSize;

        public override string ToString()
        {
            return symbol;
        }
    }
}
