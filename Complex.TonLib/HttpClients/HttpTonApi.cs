using System;
using Complex.Remote;
using Complex.Wallets;
using Complex.Collections;

namespace Complex.Ton
{
    public class HttpTonApi
    {
        private const string mainnet = "https://tonapi.io/";
        private const string testnet = "https://testnet.tonapi.io/";

        public static (JettonInfo, string) GetJettonInfo(string address, bool isTestnet)
        {
            JettonInfo info = null;
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v1/jetton/getInfo?account=" + address;
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\jettonInfo.json");
                string data = Http.GetBrouser(url);
                JsonArray array = Json.Parse(data) as JsonArray;
                if (array != null && array.Count > 0)
                {
                    if (array.Count > 0)
                    {
                        JsonArray metadata = array.GetArray("metadata");
                        string symbol = metadata.GetString("symbol");
                        string name = metadata.GetString("name");
                        string description = metadata.GetString("description");
                        string image = metadata.GetString("image");
                        //string owner = TonLib.AddressFromHex(metadata.GetString("address"));
                        int decimals = metadata.GetInt("decimals");
                        UInt128 total_supply = array.GetUInt128("total_supply");
                        return (new JettonInfo(name, description, symbol, image, total_supply, decimals, address, null, metadata.GetString("color"), metadata.GetString("deployer")), null);
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return (info, error);
        }

        public static (JettonWalletInfo[], string) GetTokens(LiteClient client, string address, bool isTestnet)
        {
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v1/jetton/getBalances?account=" + address;
                //string url = (isTestnet ? testnet : mainnet) + "/v2/accounts/" + address  + "/jettons";
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\ton_tokens.json");
                string data = Http.GetBrouser(url);
                JsonValue v = Json.Parse(data) as JsonValue;
                if (v != null)
                {
                    JsonArray array = v.Value as JsonArray;
                    if (array != null)
                    {
                        if (array.Count > 0)
                        {
                            Array<JettonWalletInfo> infos = new Array<JettonWalletInfo>();
                            foreach (JsonArray arr in array)
                            {
                                string jetton_address = TonLib.AddressFromHex(arr.GetString("jetton_address"));
                                UInt128 balance = arr.GetUInt128("balance");
                                JsonArray wallet_address = arr.GetArray("wallet_address");
                                string waddress = TonLib.AddressFromHex(wallet_address.GetString("address"));

                                JettonInfo jinfo = null;

                                JsonArray metadata = arr.GetArray("metadata");
                                if (metadata == null)
                                {
                                    //jinfo = client.GetJettonInfo(jetton_address);
                                }
                                else
                                {
                                    string image = metadata.GetString("image");
                                    if (string.IsNullOrEmpty(image))
                                    {
                                        jinfo = client.GetJettonInfo(jetton_address);
                                    }
                                    else
                                    {
                                        string symbol = metadata.GetString("symbol");
                                        string name = metadata.GetString("name");
                                        string description = metadata.GetString("description");
                                        int decimals = metadata.GetInt("decimals");
                                        jinfo = new JettonInfo(name, description, symbol, image, 0, decimals, jetton_address, address, metadata.GetString("color"), metadata.GetString("deployer"));
                                    }
                                }
                                if (jinfo != null)
                                {
                                    JettonWalletInfo info = new JettonWalletInfo(jinfo, new Balance(jinfo.Symbol, balance, jinfo.Decimals, 3), waddress, address);
                                    infos.Add(info);
                                }
                                else
                                {

                                }
                            }
                            return (infos.ToArray(), null);
                        }
                        return (null, null);
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return (new JettonWalletInfo[0], error);
        }


        private static NftInfo GetNftData(JsonArray arr, int previewIndex)
        {
            if (arr != null)
            {
                JsonArray metadata = arr.GetArray("metadata");
                if (metadata != null)
                {
                    string nftAddress = TonLib.AddressFromHex(arr.GetString("address"));
                    JsonArray owner = arr.GetArray("owner");
                    string ownerAddress = TonLib.AddressFromHex(owner?.GetString("address"));

                    JsonArray collection = arr.GetArray("collection");
                    string collectionAddress = TonLib.AddressFromHex(collection?.GetString("address"));

                    string image = null;
                    JsonArray previews = arr.GetArray("previews");
                    if (previews != null && previews.Count > previewIndex)
                        image = previews.GetArray(previewIndex).GetString("url");
                    if (image == null)
                        image = metadata.GetString("image");

                    NftContent content = new NftContent(metadata.GetString("name"), metadata.GetString("description"), image, metadata.GetString("external_url"), metadata.GetString("external_link"));
                    NftInfo nftData = new NftInfo(nftAddress, arr.GetLong("index"), ownerAddress, collection?.GetString("name"), collectionAddress, content);
                    return nftData;
                }
            }
            return null;
        }

        public static NftInfo GetNftCollectionData(string address, bool isTestnet)
        {
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v2/nfts/collections/" + address;
                string data = Http.GetBrouser(url);
                return GetNftData(Json.Parse(data) as JsonArray, 1);
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public static NftInfo GetNftData(string address, bool isTestnet)
        {
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v2/nfts/" + address;
                string data = Http.GetBrouser(url);
                return GetNftData(Json.Parse(data) as JsonArray, 1);
            }
            catch (Exception e)
            {

            }
            return null;
        }

        public static (NftInfo[], string) GetNftItems(string ownerAddress, int offset, int count, bool isTestnet)
        {
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v2/accounts/" + ownerAddress + "/nfts?limit=" + count + "&offset=" + offset + "&indirect_ownership=false";
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\nfts3.json");
                string data = Http.GetBrouser(url);
                JsonValue v = Json.Parse(data) as JsonValue;
                if (v != null)
                {
                    JsonArray array = v.Value as JsonArray;
                    if (array != null)
                    {
                        if (array.Count > 0)
                        {
                            Array<NftInfo> infos = new Array<NftInfo>();
                            foreach (JsonArray arr in array)
                            {
                                NftInfo info = GetNftData(arr, 0);
                                if(info != null)
                                    infos.Add(info);
                            }
                            return (infos.ToArray(), null);
                        }
                        return (null, "");
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return (new NftInfo[0], error);

        }

        public static (NftInfo[], string) GetNftCollections(string address, int offset, int count, bool isTestnet)
        {
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v1/nft/getCollections?limit=" + count + "&offset=" + offset + "&owner=" + address;
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\nfts2.json");
                string data = Http.GetBrouser(url);
                JsonValue v = Json.Parse(data) as JsonValue;
                if (v != null)
                {
                    JsonArray array = v.Value as JsonArray;
                    if (array != null)
                    {
                        if (array.Count > 0)
                        {
                            Array<NftInfo> infos = new Array<NftInfo>();
                            foreach (JsonArray arr in array)
                            {
                                NftInfo info = GetNftData(arr, 0);
                                if (info != null)
                                    infos.Add(info);
                            }
                            return (infos.ToArray(), null);
                        }
                        return (null, "");
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return (new NftInfo[0], error);
        }

        public static (NftInfo[], string) GetNftSingles(string address, int offset, int count, bool isTestnet)
        {
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v1/nft/searchItems?limit=" + count + "&offset=" + offset + "&owner=" + address + "&collection=no";
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\nfts2.json");
                string data = Http.GetBrouser(url);
                JsonValue v = Json.Parse(data) as JsonValue;
                if (v != null)
                {
                    JsonArray array = v.Value as JsonArray;
                    if (array != null)
                    {
                        if (array.Count > 0)
                        {
                            Array<NftInfo> infos = new Array<NftInfo>();
                            foreach (JsonArray arr in array)
                            {
                                NftInfo info = GetNftData(arr, 0);
                                if (info != null)
                                    infos.Add(info);
                            }
                            return (infos.ToArray(), null);
                        }
                        return (null, "");
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return (new NftInfo[0], error);
        }

        public static (NftInfo[], string) GetAllNftItems(string ownerAddress, int offset, int count, bool isTestnet)
        {
            string error = null;
            NftInfo[] infos1 = null;
            NftInfo[] infos2 = null;
            NftInfo[] infos3 = null;
            string e1 = null;
            string e2 = null;
            string e3 = null;
            if (offset == 0)
            {
                //(infos1, e1) = GetNftCollections(ownerAddress, 0, 1000, isTestnet);
                //(infos2, e1) = GetNftSingles(ownerAddress, 0, 1000, isTestnet);
            }
            (infos3, e3) = GetNftItems(ownerAddress, 0, 1000, isTestnet);
            NftInfo[] res = infos1.Concat<NftInfo>(infos2).Concat<NftInfo>(infos3);
            if (res != null)
                return (res, null);
            return (new NftInfo[0], e1 + e2 + e3);
        }
    }
}
