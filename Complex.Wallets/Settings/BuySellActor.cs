using System;
using System.IO;
using System.Threading.Tasks;
using Complex.Trader;
using Complex.Drawing;

namespace Complex.Wallets
{
    [Serializable]
    public class BuySellActor
    {
        private string name;
        public string Name => name;

        private string image;
        //public string ImageUrl => image;

        [field:NonSerialized]
        private IImage imageu;
        public IImage Image
        {
            get
            {
                if (imageu == null && !string.IsNullOrEmpty(this.image))
                {
                    imageu = ImageLoader.Load(this.image);
                    imageu.Disposable = false;
                }
                return imageu;
            }
        }

        private string action;
        public OrderAction Action
        {
            get
            {
                OrderAction action = this.action.IndexOf("buy") != -1 ? OrderAction.Buy : OrderAction.Sell;
                if (this.action.IndexOf("sell") != -1)
                    action |= OrderAction.Sell;
                return action;
            }
        }

        private string urlBuy;
        public string UrlBuy => urlBuy;

        private string urlSell;
        public string UrlSell => urlSell;

        private string enBuy;
        private string ruBuy;

        public string BuyText
        {
            get
            {
                if (Language.Current.IsRus)
                    return ruBuy;
                return enBuy;
            }
        }

        private string enSell;
        private string ruSell;

        public string SellText
        {
            get
            {
                if (Language.Current.IsRus)
                    return ruSell;
                return enSell;
            }
        }


        public override string ToString()
        {
            return this.name + " " + this.action + " " + base.ToString();
        }

    }
}
