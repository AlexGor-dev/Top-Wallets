using System;
using System.Net;
using Complex.Collections;
using Complex.Drawing;
using Complex.Remote;
using Complex.Wallets;

namespace Complex.Ton
{
    public class NftInfo : INftInfo
    {
        public NftInfo(string address, long index, string ownerAddress, string collectionName, string collectionAddress, string content)
        {
            this.address = address;
            this.index = index;
            this.ownerAddress = ownerAddress;
            this.collectionName = collectionName;
            this.collectionAddress = collectionAddress;
            this.content = content;
        }

        public NftInfo(string address, long index, string ownerAddress, string collectionName, string collectionAddress, NftContent content)
            : this(address, index, ownerAddress, collectionName, collectionAddress, null as string)
        {
            this.nftContent = content;
        }


        private string ownerAddress;
        public string OwnerAddress => ownerAddress;

        private long index;
        public long Index => index;

        private string collectionName;
        public string CollectionName => collectionName;

        private string collectionAddress;
        public string CollectionAddress => collectionAddress;

        private string address;
        public string Address => address;

        private NftContent nftContent;

        string ITokenInfoBase.Name => nftContent?.Name;

        string IUnique.ID => address;

        private string content;
        public string Content => content;
        public string Type { get; set; }


        public void LoadIContent(ParamHandler<NftContent, string> paramHandler)
        {
            if (this.nftContent != null)
                paramHandler(this.nftContent, null);
            else
            {
                SingleThread.Run("NftContent", () =>
                 {
                     try
                     {
                         string data = Http.Get(this.content);
                         JsonArray array = Json.Parse(data) as JsonArray;
                         if (array != null)
                             paramHandler(Json.Deserialize<NftContent>(array), null);
                         else
                             paramHandler(null, "invalidJson");
                     }
                     catch (WebException e)
                     {
                         if (e.Status == WebExceptionStatus.SecureChannelFailure)
                         {
                             if (Uri.TryCreate(this.content, UriKind.Absolute, out Uri uri))
                             {
                                 paramHandler(null, Language.Current["hostNotSupportCurrentWindowsVersion", uri.Host]);
                             }
                             else
                             {
                                 paramHandler(null, "invalidUrl");
                             }
                         }
                         else
                             paramHandler(null, e.Message);
                     }
                     catch (Exception e)
                     {
                         paramHandler(null, e.Message);
                     }
                 });
            }
        }

        void INftInfo.LoadImage(int imageSize, float ovalRadius, ParamHandler<IImage, string> resultHandler)
        {
            nftContent?.LoadImage(imageSize, ovalRadius, resultHandler);
        }
    }
}
