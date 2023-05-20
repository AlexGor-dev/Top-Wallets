using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complex.Wallets
{
    public class TatumClient
    {
        const string mainnetURL = "https://api.tatum.io/v3/tron/";
        const string testnetURL = "https://api.tatum.io/v3/tron/";


        //public static (TrcContractInfo contractInfo, string error) GetTrc10ContractInfo(string contractAggress, bool isTestnet)
        //{
        //    string error = null;
        //    try
        //    {
        //        string url = (isTestnet ? testnetTatumURL : mainnetTatumURL) + "trc10/detail/" + contractAggress;
        //        string data = Http.GetBrouser(url);
        //        JsonArray arr = Json.Parse(data) as JsonArray;
        //        //return (new TrcContractInfo(arr.GetString("id"), arr.GetString("name"), arr.GetString("abbr"), Base58Encoder.EncodeFromHex(arr.GetString("ownerAddress"), 65), arr.GetInt("precision"), TrcType.Trc10), null);
        //    }
        //    catch (Exception e)
        //    {
        //        error = e.Message;
        //    }
        //    return (null, error);
        //}

        //public static (TrcContractInfo contractInfo, string error) GetTrc20ContractInfo(string id, bool isTestnet)
        //{
        //    string error = null;
        //    try
        //    {
        //        string url = (isTestnet ? testnetTatumURL : mainnetTatumURL) + "trc10/detail/" + id;
        //        string data = Http.GetBrouser(url);
        //        JsonArray arr = Json.Parse(data) as JsonArray;
        //        //return (new TrcContractInfo(arr.GetString("id"), arr.GetString("name"), arr.GetString("abbr"), Base58Encoder.EncodeFromHex(arr.GetString("ownerAddress"), 65), arr.GetInt("precision"), TrcType.Trc10), null);
        //    }
        //    catch (Exception e)
        //    {
        //        error = e.Message;
        //    }
        //    return (null, error);
        //}

    }
}
