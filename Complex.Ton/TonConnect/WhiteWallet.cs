using System;
using Complex.Collections;
using Complex.Remote;

namespace Complex.Ton.TonConnect
{
    public class WhiteWallet : IUnique
    {
        const string whiteListUrl = "https://raw.githubusercontent.com/ton-connect/wallets-list/main/wallets.json";

        static WhiteWallet()
        {
            string whiteList = Util.Try(() => Http.Get(whiteListUrl));
            if (whiteList == null)
                whiteList = Resources.GetText("whiteList.json");
            JsonArray arr = Json.Parse2(whiteList) as JsonArray;
            if (arr != null)
            {
                foreach (JsonArray item in arr)
                {
                    try
                    {
                        string universal_url = item.GetString("universal_url");
                        if (universal_url != null)
                        {
                            JsonArray bridge = item.GetArray("bridge");
                            if (bridge != null)
                            {
                                string bridge_url = null;
                                foreach (JsonArray b in bridge)
                                {
                                    if (b.GetString("type") == "sse")
                                    {
                                        bridge_url = b.GetString("url");
                                        if (bridge_url != null)
                                            break;
                                    }
                                }
                                if (bridge_url != null)
                                {
                                    Uri uri = new Uri(universal_url);
                                    WhiteWallet wallet = new WhiteWallet(uri.Host, item.GetString("name"), item.GetString("image"), item.GetString("about_url"), universal_url, bridge_url);
                                    wallets.Add(wallet);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }

        private static UniqueCollection<WhiteWallet> wallets = new UniqueCollection<WhiteWallet>();
        public static UniqueCollection<WhiteWallet> Wallets => wallets;

        public WhiteWallet(string id, string name, string image, string about_url, string universal_url, string bridge_url)
        {
            this.id = id;
            this.name = name;
            this.image = image;
            this.about_url = about_url;
            this.universal_url = universal_url;
            this.bridge_url = bridge_url;
        }

        private string id;
        public string ID => id;

        public readonly string name;
        public readonly string image;
        public readonly string about_url;
        public readonly string universal_url;
        public readonly string bridge_url;

        public override string ToString()
        {
            return this.name + " " + this.id + " " + base.ToString();
        }
    }
}
