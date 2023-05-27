//using System;
//using Complex.Remote;
//using Complex.Collections;

//namespace Complex.Ton
//{
//    public static class HttpAnton
//    {
//        private const string mainnet = "https://anton.tools/api/v0/";
//        private const string testnet = "https://anton.tools/api/v0/";


//        public static (NftCollectionInfo[], string) GetNftCollections(string address, bool isTestnet)
//        {
//            string error = null;
//            try
//            {
//                //nft_collection|nft_item

//                string url = (isTestnet ? testnet : mainnet) + "accounts?latest=true&interface=nft_collection&order=DESC&limit=16&owner_address=" + address;
//                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\aton_nft_collection.json");
//                string data = Http.GetBrouser(url);
//                JsonArray v = Json.Parse(data) as JsonArray;
//                if (v != null)
//                {
//                    JsonArray array = v.GetArray("results");
//                    if (array != null)
//                    {
//                        if (array.Count > 0)
//                        {
//                            Array<NftCollectionInfo> infos = new Array<NftCollectionInfo>();
//                            foreach (JsonArray arr in array)
//                            {
//                                JsonArray state_data = arr.GetArray("state_data");
//                                if (state_data != null)
//                                {
//                                    string curl = state_data.GetString("content_uri");
//                                    data = Http.GetBrouser(curl);

//                                    JsonArray caddress = state_data.GetArray("address");
//                                    string collAddress = TonLib.AddressFromHex(caddress?.GetString("base64"));

//                                    //JsonArray owner = arr.GetArray("owner");
//                                    //string ownerAddress = TonLib.AddressFromHex(owner?.GetString("address"));
//                                    //string name = metadata.GetString("name");
//                                    //NftCollectionInfo info = new NftCollectionInfo(metadata.GetString("name"), metadata.GetString("description"), metadata.GetString("image"), nftAddress, ownerAddress, metadata.GetString("external_url"), metadata.GetString("external_link"));
//                                    //infos.Add(info);
//                                }
//                                else
//                                {

//                                }
//                            }
//                            return (infos.ToArray(), null);
//                        }
//                        return (null, "");
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                error = e.Message;
//            }
//            return (new NftCollectionInfo[0], error);
//        }

//    }
//}
