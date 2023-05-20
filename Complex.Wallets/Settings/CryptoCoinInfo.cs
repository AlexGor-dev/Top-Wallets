using System;
using Complex.Collections;
using Complex.Drawing;
namespace Complex.Wallets
{
    [Serializable]
    public class CryptoCoinInfo : IUnique
    {
        public CryptoCoinInfo(string adapterID, string symbol, string supportAddress, decimal supportActionAmount)
        {
            this.adapterID = adapterID;
            this.symbol = symbol;
            this.supportAddress = supportAddress;
            this.supportActionAmount = (double)supportActionAmount;
        }

        string IUnique.ID => symbol;

        private string adapterID;
        public string AdapterID => adapterID;

        private string symbol;
        public string Symbol => symbol;

        private string name;
        public string Name => name;

        private BuySellActor[] buySellActors;
        public BuySellActor[] BuySellActors => buySellActors;

        private bool supportEnabled;
        public bool SupportEnabled => supportEnabled;

        private string supportAddress;
        public string SupportAddress => supportAddress;

        private string supportTestAddress;
        public string SupportTestAddress => supportTestAddress;

        private double supportActionAmount;
        public decimal SupportActionAmount => (decimal)supportActionAmount;

        private bool enabled;
        public bool Enabled => enabled;

        private string color;
        public int Color => Complex.Drawing.Color.Parse(color);

        private string geckoSymbol;
        public string GeckoSymbol => geckoSymbol;

        private string geckoID;
        public string GeckoID => geckoID;

        private decimal tickSize;
        public decimal TickSize => tickSize;

        [field:NonSerialized]
        private bool imagesLoaded = false;

        public void LoadImages(EmptyHandler loadedHandler)
        {
            if (!this.imagesLoaded)
            {
                this.imagesLoaded = true;
                Util.Run(() =>
                {
                    foreach (BuySellActor buySellActor in this.buySellActors)
                    {
                        IImage image = buySellActor.Image;
                    }
                    if (loadedHandler != null)
                        loadedHandler();
                });
            }
            else
            {
                if (loadedHandler != null)
                    loadedHandler();
            }
        }
        public override string ToString()
        {
            return symbol + " " + base.ToString();
        }
    }
}
