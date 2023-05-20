using System;
using Complex.Drawing;
using Complex.Wallets;
using Complex.Collections;

namespace Complex.Ton
{
    public class NftInfo : INftInfo
    {
        public NftInfo(string name, string description, string imageUrl, string address, string owner)
        {
            this.name = name;
            this.description = description;
            this.imageUrl = imageUrl;
            this.address = address;
            this.owner = owner;
        }

        private string name;
        public string Name => name;

        private string description;
        public string Description => description;

        private string address;
        public string Address => address;

        string IUnique.ID => address;

        private string owner;
        public string Owner => owner;

        private string imageUrl;
        public string ImageUrl => imageUrl;

        private string ImageID => "Nft_" + this.address;

        public IImage LoadImage(ParamHandler<IImage> resultHandler)
        {
            return Images.LoadImage(imageUrl, ImageID, "nft.svg", resultHandler);
        }

        public override string ToString()
        {
            return this.name + " " + base.ToString();
        }
    }
}
