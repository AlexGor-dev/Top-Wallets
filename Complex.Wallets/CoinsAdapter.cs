using System;
using Complex.Trader;

namespace Complex.Wallets
{
    public abstract class CoinsAdapter : Adapter
    {
        protected CoinsAdapter(IData data)
            : base(data)
        {

        }

        public CoinsAdapter()
            :base(new AdapterExtension(typeof(CoinsAdapter)))
        {

        }

        public abstract Coin GetCoin(string symbol, string currency);
    }
}
