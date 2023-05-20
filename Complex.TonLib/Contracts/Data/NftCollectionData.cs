using System;

namespace Complex.Ton
{
    public class NftCollectionData
    {
        public NftCollectionData(long nextItemId, string collectionContent, string ownerAddress)
        {
            this.nextItemId = nextItemId;
            this.collectionContent = collectionContent;
            this.ownerAddress = ownerAddress;
        }


        long nextItemId;
        string collectionContent;
        string ownerAddress;
    }
}
