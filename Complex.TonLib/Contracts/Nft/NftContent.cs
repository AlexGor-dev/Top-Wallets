using System;
using System.Net;
using Complex.Drawing;

namespace Complex.Ton
{
    public class NftContent
    {
        public NftContent(string name, string description, string image, string external_url, string external_link)
        {
            this.name = name;
            this.description = description;
            this.image = image;
            this.external_url = external_url;
            this.external_link = external_link;
        }

        private string name;
        public string Name => name;

        private string description;
        public string Description => description;


        private string image;

        private string external_url;
        public string ExternalUrl => external_url;

        private string external_link;
        public string ExternalLink => external_link;

        public void LoadImage(int imageSize, float ovalRadius, ParamHandler<IImage, string> resultHandler)
        {
            if (StringHelper.GetExtension(image).ToLower() == "svg")
                resultHandler(null, Language.Current["notSupportSvg"]);
            else
            {
                ImageLoader.Load(image, imageSize, ovalRadius, (i, e) =>
                //Images.LoadImage(image, "Nft_" + name, "nft_token.svg", imageSize, ovalRadius, (i, e) =>
                {
                    if (e is WebException we && we.Status == WebExceptionStatus.SecureChannelFailure)
                        resultHandler(null, Language.Current["hostNotSupportCurrentWindowsVersion", new Uri(image).Host]);
                    else
                        resultHandler(i, null);
                });
            }
        }

        public override string ToString()
        {
            return this.name + " " + base.ToString();
        }

    }
}
