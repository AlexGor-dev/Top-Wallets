using System;
using Complex.Collections;

namespace Complex.Wallets
{
    [Serializable]
    public class RemoteSettings
    {
        private int version;
        public int Version => version;

        private SupportSetting support;
        public SupportSetting Support => support;

        private string tgCanalRu;
        private string tgCanalEn;
        public string TgCanal
        {
            get
            {
                if (Language.Current.ID == "ru")
                    return tgCanalRu;
                return tgCanalEn;
            }
        }

        private string tgSupportRu;
        private string tgSupportEn;
        public string TgSupport
        {
            get
            {
                if (Language.Current.ID == "ru")
                    return tgSupportRu;
                return tgSupportEn;
            }
        }

        private string site;
        public string Site => site;

        private string supportMail;
        public string SupportMail => supportMail;

        private CoinGeckoSettings coinGecko;
        public CoinGeckoSettings CoinGecko => coinGecko;

        private CryptoCoinInfo[] cryptoCoins;
        [field: NonSerialized]
        private UniqueCollection<CryptoCoinInfo> cryptoCoinsu;
        public UniqueCollection<CryptoCoinInfo> CryptoCoins
        {
            get
            {
                if (this.cryptoCoinsu == null)
                {
                    this.cryptoCoinsu = new UniqueCollection<CryptoCoinInfo>();
                    foreach (CryptoCoinInfo info in this.cryptoCoins)
                        this.cryptoCoinsu.Add(info);
                }
                return this.cryptoCoinsu;
            }
        }
    }
}
