using System;

namespace Complex.Ton
{
    public class NftData
    {
        public NftData(bool inited, long index, string collectionAddress, string ownerAddress, string content)
        {
            this.inited = inited;
            this.index = index;
            this.collectionAddress = collectionAddress;
            this.ownerAddress = ownerAddress;
            this.content = content;
        }

        public NftData(bool inited, long index, string collectionAddress)
            :this(inited, index, collectionAddress, null, null)
        {

        }

        private bool inited;
        private long index;
        private string collectionAddress;
        private string ownerAddress;
        private string content;
    }
}
