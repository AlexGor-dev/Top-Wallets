using System;

namespace Complex.Ton
{
    public abstract class DeployData : Disposable
    {
        public DeployData(string ownerAddress, string parentAddress, string[] childsAddress, ContractDeployData deployParams)
        {
            this.ownerAddress = ownerAddress;
            this.parentAddress = parentAddress;
            this.childsAddress = childsAddress;
            this.deployParams = deployParams;
        }

        protected override void OnDisposed()
        {
            deployParams.Dispose();
            base.OnDisposed();
        }

        public readonly string ownerAddress;
        public readonly string parentAddress;
        public readonly string[] childsAddress;
        public readonly ContractDeployData deployParams;
    }
}
