using System;

namespace Complex.Ton
{
    public class NftCollectionInfo : NftInfo
    {
        public NftCollectionInfo(string name, string description, string imageUrl, string address, string owner, string external_url, string external_link)
            : base(name, description, imageUrl, address, owner)
        {
            this.external_url = external_url;
            this.external_link = external_link;
        }

        private string external_url;
        public string ExternalUrl => external_url;

        private string external_link;
        public string ExternalLink => external_link;
    }
}
