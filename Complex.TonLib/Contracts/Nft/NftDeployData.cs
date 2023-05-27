using System;

namespace Complex.Ton
{
    public class NftDeployData : DeployData
    {
        public NftDeployData(string ownerAddress, long queryId, string nftCollectionAddress, string[] nftItemsAddress, ContractDeployData deployParams, string collectioContent, RoyaltyParams royaltyParams, NftMintItem[] items)
            :base(ownerAddress, queryId, nftCollectionAddress, nftItemsAddress, deployParams)
        {
            this.collectioContent = collectioContent;
            this.royaltyParams = royaltyParams;
            this.items = items;
        }

        public readonly string collectioContent;
        public readonly RoyaltyParams royaltyParams;
        public readonly NftMintItem[] items;

        public string NftCollectionAddress => parentAddress;
        public string[] NftItemsAddress => childsAddress;
    }
}
