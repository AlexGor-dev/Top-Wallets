using System;
using Complex.Collections;

namespace Complex.Wallets
{
    [Serializable]
    public class CurrencyInfo : IUnique
    {
        private string id;
        public string ID => id;

        private string ch;

        private bool alignLeft;

        private string en;
        private string ru;

        public string Name
        {
            get
            {
                if (Language.Current.IsRus)
                    return ru;
                return en;
            }
        }

        public override string ToString()
        {
            return this.id + " " + this.ch + " " + en;
        }
    }
}
