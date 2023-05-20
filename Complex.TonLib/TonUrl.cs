using System;
using Complex.Wallets;

namespace Complex.Ton
{
    public class TonUrl : ITranserParams
    {
        private string command;
        public string Command => command;

        private string address;
        public string Address => address;

        private string jetton;
        public string Jetton => jetton;

        private string nft;
        public string Nft => nft;

        private UInt128 feeAmount;
        public UInt128 FeeAmount => feeAmount;

        private UInt128 forwardAmount;
        public UInt128 ForwardAmount => forwardAmount;

        private UInt128 amount;
        public UInt128 Amount => amount;

        private string comment;
        public string Comment => comment;

        public static TonUrl Parse(string text)
        {
            if (!string.IsNullOrEmpty(text) && text.StartsWith("ton://", StringComparison.CurrentCultureIgnoreCase))
            {
                try
                {
                    Uri uri = new Uri(text);
                    TonUrl ton = new TonUrl();
                    ton.command = uri.Authority;
                    ton.address = uri.LocalPath.Replace("/", "");
                    JsonArray array = Json.ParseUrl(uri.ToString());
                    ton.jetton = array.GetString("jetton");
                    ton.nft = array.GetString("nft");
                    ton.amount = array.GetUInt128("amount");
                    ton.comment = array.GetString("text");
                    ton.feeAmount = array.GetUInt128("fee-amount");
                    ton.forwardAmount = array.GetUInt128("forward-amount");
                    return ton;
                }
                catch
                {

                }
            }
            return null;
        }
    }
}
