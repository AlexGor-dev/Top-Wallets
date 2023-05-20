using System;
using System.IO;
using System.Net;
using Complex.Remote;
using Complex.Ton;
using Complex.Collections;

namespace Complex.Wallets
{
    public class TonHttpAdapter
    {
        private static readonly string mainnet = "https://toncenter.com/";
        private static readonly string testnet = "https://testnet.toncenter.com/";
        private static readonly string api = "api/v2/";

        //protected TonHttpAdapter(IData data)
        //    : base(data)
        //{

        //}

        //public TonHttpAdapter(TonAdapterExtension extension)
        //    : base(extension)
        //{

        //}



        //public override Complex.Trader.Instrument GetInstrument()
        //{
        //    return CoinGecko.instance.Instruments["the-open-network"];
        //}

        private static string GetUrl(bool isTestnet)
        {
            if (isTestnet)
                return testnet + api;
            return mainnet + api;
        }

        private static (string source, string dest, long value, long created_lt, string message) ReadMessage(JsonArray msg)
        {
            return (msg.GetString("source"), msg.GetString("destination"), msg.GetLong("value"), msg.GetLong("created_lt"), msg.GetString("message"));
        }



        public static ITransactionBase[] GetTransactions(bool isTestnet, string address)
        {
            try
            {
                //string data = File.ReadAllText(@"E:\Complex\Ton\Top-Wallets\transaction.json");
                string data = Http.GetNotSecurity(GetUrl(isTestnet) + "getTransactions?address=" + address + "&archival=true");
                JsonArray arr = Json.Parse(data) as JsonArray;
                if (arr != null && arr.Count > 1 && arr.GetString("ok") == "true")
                {
                    JsonArray tsarr = arr.GetArray("result");
                    //Array<TransactionRecord> transactions = new Array<TransactionRecord>();
                    //foreach (JsonArray array in tsarr)
                    //{
                    //    long utime = array.GetLong("utime");
                    //    long fee = array.GetLong("fee");

                    //    JsonArray transaction_id = array.GetArray("transaction_id");
                    //    long lt = transaction_id.GetLong("lt");
                    //    byte[] ltHash = Convert.FromBase64String(transaction_id.GetString("hash"));

                    //    JsonArray in_msg = array.GetArray("in_msg");
                    //    if (in_msg.GetLong("value") > 0)
                    //    {
                    //        (string source, string dest, long value, long created_lt, string message) = ReadMessage(in_msg);
                    //        JsonArray msg_data = in_msg.GetArray("msg_data");
                    //    }
                    //    JsonArray out_msgs = array.GetArray("out_msgs");
                    //    foreach (JsonArray msg in out_msgs)
                    //    {
                    //        (string source, string dest, long value, long created_lt, string message) = ReadMessage(msg);
                    //    }
                    //}
                    //return transactions.ToArray();
                }

            }
            catch (Exception e)
            {

            }

            return null;
        }
    }
}
