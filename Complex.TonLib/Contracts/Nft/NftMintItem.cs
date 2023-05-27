using System;

namespace Complex.Ton
{
    public class NftMintItem
    {
        public NftMintItem(UInt128 passAmount, long index, string ownerAddress, string content)
        {
            this.passAmount = passAmount;
            this.index = index;
            this.ownerAddress = ownerAddress;
            this.content = content;
        }

        public readonly UInt128 passAmount;
        public readonly long index;
        public readonly string ownerAddress;
        public readonly string content;

        public override string ToString()
        {
            return index + " " + base.ToString();
        }
    }
}
