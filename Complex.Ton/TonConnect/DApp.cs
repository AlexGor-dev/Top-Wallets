using System;
using Complex.Drawing;

namespace Complex.Ton
{
    public class DApp
    {
        private string name;
        public string Name => name;

        private string image;

        private string url;
        public string Url => url;

        private string en;
        private string ru;
        public string Description
        {
            get
            {
                if (Language.Current.IsRus)
                    return ru;
                return en;
            }
        }

        public IImage LoadImage(ParamHandler<IImage> resultHandler)
        {
            return Images.LoadImage(image, "DApp_" + name, "dapp.svg", resultHandler);
        }

    }
}
