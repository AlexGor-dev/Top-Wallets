using System;
using Complex.Collections;

namespace Complex.Wallets
{
    [Serializable]
    public class CoinGeckoSettings
    {
        private bool useBarsUrl;
        public bool UseBarsUrl => useBarsUrl;

        private string barsUrl;
        public string BarsUrl => barsUrl;

        private CoinGeckoInfo[] coinsInfos;

        [field: NonSerialized]
        private UniqueCollection<CoinGeckoInfo> coinsInfosu; 
        public UniqueCollection<CoinGeckoInfo> CoinsInfos
        {
            get
            {
                if (this.coinsInfosu == null)
                {
                    this.coinsInfosu = new UniqueCollection<CoinGeckoInfo>();
                    foreach (CoinGeckoInfo info in coinsInfos)
                        this.coinsInfosu.Add(info);
                }
                return this.coinsInfosu;
            }
        }

    }
}
