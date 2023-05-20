using System;
using Complex.Collections;

namespace Complex.Ton
{
    public class JettonDeployInfo
    {
        public JettonDeployInfo(string name, string description, string symbol, string image_data, UInt128 totalSupply, int decimals, string color, string deployer)
        {
            this.name = name;
            this.description = description;
            this.symbol = symbol;
            this.image_data = image_data;
            this.totalSupply = totalSupply;
            this.decimals = decimals;
            this.color = color;
            this.deployer = deployer;
        }

        private string name;
        public string Name => name;

        private string description;
        public string Description => description;

        private string symbol;
        public string Symbol => symbol;

        private string image_data;
        public string ImageData => image_data;

        private UInt128 totalSupply;
        public UInt128 TotalSupply => totalSupply;

        private int decimals;
        public int Decimals => decimals;

        private string color;
        public string Color => color;

        public int ThremeColor
        {
            get
            {
                int c = Complex.Drawing.Color.Parse(this.color);
                if (c != 0)
                    return c;
                return Complex.Drawing.Color.From(this.symbol, 55);
            }
        }

        private string deployer;
        public Dict<string> ToDict()
        {
            Dict<string> hash = new Dict<string>();
            hash["symbol"] = this.symbol;
            hash["name"] = this.name;
            hash["description"] = this.description;
            hash["image_data"] = this.image_data;
            hash["decimals"] = this.decimals.ToString();
            hash["color"] = this.color;
            hash["deployer"] = this.deployer;
            return hash;
        }

        public override string ToString()
        {
            return this.name + " " + base.ToString();
        }

    }
}
