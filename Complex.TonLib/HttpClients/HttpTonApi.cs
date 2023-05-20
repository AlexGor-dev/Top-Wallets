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
                        return (new JettonInfo(name, description, symbol, image, new Balance(symbol, total_supply, decimals, 3), decimals, address, null, metadata.GetString("color"), metadata.GetString("deployer")), null);
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
                                        jinfo = new JettonInfo(name, description, symbol, image, new Balance(symbol, 0, decimals, 3), decimals, jetton_address, address, metadata.GetString("color"), metadata.GetString("deployer"));
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


        public static (NftCollectionInfo[], string) GetNftCollections(string address, bool isTestnet)
        {
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v1/nft/getCollections?limit=15&offset=0&account=" + address;
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\nfts.json");
                string data = Http.GetBrouser(url);
                JsonValue v = Json.Parse(data) as JsonValue;
                if (v != null)
                {
                    JsonArray array = v.Value as JsonArray;
                    if (array != null)
                    {
                        if (array.Count > 0)
                        {
                            Array<NftCollectionInfo> infos = new Array<NftCollectionInfo>();
                            foreach (JsonArray arr in array)
                            {
                                JsonArray metadata = arr.GetArray("metadata");
                                if (metadata != null)
                                {
                                    string nftAddress = TonLib.AddressFromHex(arr.GetString("address"));
                                    JsonArray owner = arr.GetArray("owner");
                                    string ownerAddress = TonLib.AddressFromHex(owner?.GetString("address"));
                                    string name = metadata.GetString("name");
                                    NftCollectionInfo info = new NftCollectionInfo(metadata.GetString("name"), metadata.GetString("description"), metadata.GetString("image"), nftAddress, ownerAddress, metadata.GetString("external_url"), metadata.GetString("external_link"));
                                    infos.Add(info);
                                }
                                else
                                {

                                }
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
            return (new NftCollectionInfo[0], error);
        }


        public static (NftItemInfo[], string) GetNftItems(string address, string collAddress, bool isTestnet)
        {
            string error = null;
            try
            {
                string url = (isTestnet ? testnet : mainnet) + "v1/nft/searchItems?owner=" + address + "&collection="+ (collAddress == null ? "no" : collAddress) + "&include_on_sale=true&limit=15&offset=0";
                //string data = System.IO.File.ReadAllText(@"E:\Complex\Ton\Crypto Api\nfts.json");
                string data = Http.GetBrouser(url);
                JsonValue v = Json.Parse(data) as JsonValue;
                if (v != null)
                {
                    JsonArray array = v.Value as JsonArray;
                    if (array != null)
                    {
                        if (array.Count > 0)
                        {
                            Array<NftItemInfo> infos = new Array<NftItemInfo>();
                            foreach (JsonArray arr in array)
                            {
                                JsonArray metadata = arr.GetArray("metadata");
                                if (metadata != null)
                                {
                                    string nftAddress = TonLib.AddressFromHex(arr.GetString("address"));
                                    JsonArray owner = arr.GetArray("owner");
                                    string ownerAddress = TonLib.AddressFromHex(owner?.GetString("address"));

                                    JsonArray collection = arr.GetArray("collection");
                                    string collectionAddress = TonLib.AddressFromHex(collection?.GetString("address"));

                                    string name = metadata.GetString("name");
                                    NftItemInfo info = new NftItemInfo(metadata.GetString("name"), metadata.GetString("description"), metadata.GetString("image"), nftAddress, collectionAddress, ownerAddress, metadata.GetString("external_url"), metadata.GetString("external_link"));
                                    infos.Add(info);
                                }
                                else
                                {

                                }
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
            return (new NftItemInfo[0], error);

        }
    }
}
