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

        public void LoadImage(ParamHandler<IImage> resultHandler)
        {
            Images.LoadImage(image, "DApp_" + name, "dapp.svg", 96, 16, resultHandler);
        }

    }
}
