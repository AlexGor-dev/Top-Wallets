using System;
using System.Reflection;
using Complex.Wallets;
using Complex.Collections;
using Complex.Drawing;

namespace Complex.Ton
{
    [Serializable]
    public class JettonInfo : IUnique
    {
        public static readonly JettonInfo Empty = new JettonInfo();

        public static IImage JettonImage = Images.Get("jetton.svg");
        public JettonInfo(string name, string description, string symbol, string image_data, Balance totalSupply, int decimals, string jettonAddress, string ownerAddress, string color, string deployer)
        {
            this.name = name;
            this.description = description;
            this.symbol = symbol;
            this.image_data = image_data;
            this.totalSupply = totalSupply;
            this.decimals = decimals;
            this.jettonAddress = jettonAddress;
            this.ownerAddress = ownerAddress;
            this.color = color;
            this.deployer = deployer;
        }

        private JettonInfo()
        {

        }

        private string name;
        public string Name => name;

        private string description;
        public string Description => description;

        private string symbol;
        public string Symbol => symbol;

        private Balance totalSupply;
        public Balance TotalSupply => totalSupply;

        private int decimals;
        public int Decimals => decimals;

        private string jettonAddress;
        public string JettonAddress => jettonAddress;

        private string ownerAddress;
        public string OwnerAddress => ownerAddress;

        private string image_data;
        public string ImageData => image_data;

        private string color;
        public string Color => color;

        private string deployer;
        public string Deployer => deployer;

        [field: NonSerialized]
        private int thremeColor;

        public int ThremeColor
        {
            get
            {
                if (this.thremeColor == 0)
                {
                    this.thremeColor = Complex.Drawing.Color.Parse(this.color);
                    if (this.thremeColor == 0)
                        this.thremeColor = Complex.Drawing.Color.From(this.symbol, 75);
                }
                return this.thremeColor;
            }
        }

        string IUnique.ID => jettonAddress;

        public string ImageID => "Jetton_" + this.jettonAddress;

        public IImage LoadImage(ParamHandler<IImage> resultHandler)
        {
            return Images.LoadImage(image_data, ImageID, "jetton.svg", resultHandler);
        }

        public static JettonInfo FromDict(Dict<string> dict, string jtonAddress, string owner, UInt128 totalSupply)
        {
            int decimals = dict.Contains("decimals") ? int.Parse(dict["decimals"]) : 9;
            string symbol = dict["symbol"];
            return new JettonInfo(dict["name"], dict["description"], symbol, dict.Contains("image") ? dict["image"] : dict["image_data"], new Balance(symbol, totalSupply, decimals, 3), decimals, jtonAddress, owner, dict["color"], dict["deployer"]);
        }

        public override string ToString()
        {
            return this.name + " " + jettonAddress;
        }
    }


}
