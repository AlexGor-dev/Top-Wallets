using System;

namespace Complex.Ton
{
    public class JettonDeployData : DeployData
    {
        public JettonDeployData(string ownerAddress, string jettonMinterAddress, string jettonWalletAddress, ContractDeployData deployParams, JettonInfo info, string offchainUri)
            :base(ownerAddress, jettonMinterAddress, new string[] { jettonWalletAddress }, deployParams)
        {
            this.info = info;
            this.offchainUri = offchainUri;
        }

        public readonly JettonInfo info;
        public readonly string offchainUri;

        public string JettonMinterAddress => parentAddress;
        public string JettonWalletAddress => childsAddress[0];
    }
}
