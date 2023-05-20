﻿using System;
using Complex.Drawing;

namespace Complex.Ton.TonConnect
{
    public class DAppInfo
    {
        private string url;
        public string Url => url;

        private string name;
        public string Name => name;

        private string iconUrl;

        private string termsOfUseUrl;
        public string TermsOfUseUrl => termsOfUseUrl;

        private string privacyPolicyUrl;
        public string PrivacyPolicyUrl => privacyPolicyUrl;


        public override string ToString()
        {
            return name + " " + base.ToString();
        }

        public IImage LoadImage(ParamHandler<IImage> resultHandler)
        {
            return Images.LoadImage(iconUrl, "DAppInfo_" + name, "dapp.svg", resultHandler);
        }

    }
}
