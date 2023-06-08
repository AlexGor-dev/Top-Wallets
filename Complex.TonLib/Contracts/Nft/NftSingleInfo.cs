using System;

namespace Complex.Ton
{
    public class NftSingleInfo : NftInfo
    {
        public NftSingleInfo(string address, string ownerAddress, string editorAddress, string content, RoyaltyParams royaltyParams)
            :base(address, -1, ownerAddress, null, null, content)
        {
            this.editorAddress = editorAddress;
            this.royaltyParams = royaltyParams;
        }

        private string editorAddress;
        public string EditorAddress => editorAddress;

        private RoyaltyParams royaltyParams;
        public RoyaltyParams RoyaltyParams => royaltyParams;
    }
}
