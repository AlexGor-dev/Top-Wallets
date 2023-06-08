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
        public string OwnerAddress => owner;

        string ITokenInfoBase.Name => this.jettonInfo.Name;

        int ITokenInfo.Color => this.jettonInfo.ThremeColor;

        void ITokenInfo.LoadImage(ParamHandler<IImage> resultHandler)
        {
            this.jettonInfo.LoadImage(resultHandler);
        }


    }
}
