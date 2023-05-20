using System;

namespace Complex.Ton
{
    public class JettonDeployData : Disposable
    {
        public JettonDeployData(string ownerAddress, JettonDeployInfo info, string offchainUri, long queryId, string jettonMinterAddress, string jettonWalletAddress, ContractDeployData deployParams)
        {
            this.ownerAddress = ownerAddress;
            this.info = info;
            this.offchainUri = offchainUri;
            this.queryId = queryId;
            this.jettonMinterAddress = jettonMinterAddress;
            this.jettonWalletAddress = jettonWalletAddress;
            this.deployParams = deployParams;
        }

        protected override void OnDisposed()
        {
            deployParams.Dispose();
            base.OnDisposed();
        }

        public readonly string ownerAddress;
        public readonly JettonDeployInfo info;
        public readonly string offchainUri;
        public readonly long queryId;
        public readonly string jettonMinterAddress;
        public readonly string jettonWalletAddress;
        public readonly ContractDeployData deployParams;
    }
}
