using System;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Collections;

namespace Complex.Ton
{
    [Serializable]
    public class JettonWalletInfo : ITokenInfo
    {
        public JettonWalletInfo(JettonInfo jettonInfo, Balance balance, string address, string owner)
        {
            this.jettonInfo = jettonInfo;
            this.balance = balance;
            this.address = address;
            this.owner = owner;
        }

        private JettonInfo jettonInfo;
        public JettonInfo JettonInfo => jettonInfo;

        private Balance balance;
        public Balance Balance => balance;

        private string address;
        public string Address => address;

        string IUnique.ID => address;

        private string owner;
        public string Owner => owner;

        string ITokenInfo.Name => this.jettonInfo.Name;

        int ITokenInfo.Color => this.jettonInfo.ThremeColor;

        IImage ITokenInfo.LoadImage(ParamHandler<IImage> resultHandler)
        {
            return this.jettonInfo.LoadImage(resultHandler);
        }


    }
}
