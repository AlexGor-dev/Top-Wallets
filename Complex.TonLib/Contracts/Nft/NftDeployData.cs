using System;

namespace Complex.Ton
{
    public class NftDeployData : DeployData
    {
        public NftDeployData(string ownerAddress, string nftCollectionAddress, string[] nftItemsAddress, ContractDeployData deployParams, string collectioContent, RoyaltyParams royaltyParams, NftMintItemInfo[] items)
            :base(ownerAddress, nftCollectionAddress, nftItemsAddress, deployParams)
        {
            this.collectioContent = collectioContent;
            this.royaltyParams = royaltyParams;
            this.items = items;
        }

        public readonly string collectioContent;
        public readonly RoyaltyParams royaltyParams;
        public readonly NftMintItemInfo[] items;

        public string CollectionAddress => parentAddress;
        public string[] ItemsAddress => childsAddress;
    }
}
