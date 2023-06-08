using System;

namespace Complex.Ton
{
    public class NftSingleDeployData : DeployData
    {
        public NftSingleDeployData(string singleAddress, ContractDeployData deployParams, NftSingleInfo info)
            : base(info.OwnerAddress, null, new string[] { singleAddress }, deployParams)
        {
            this.info = info;
        }

        public readonly NftSingleInfo info;

        public string SingleAddress => childsAddress[0];

    }
}
