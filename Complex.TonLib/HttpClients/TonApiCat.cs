using System;
using System.IO;
using Complex.Remote;
using Complex.Ton;
using Complex.Collections;
using Complex.Wallets;

namespace Complex.Ton
{
    public class TonApiCat
    {
        private static readonly string mainnetCat = "https://api.ton.cat/v2/contracts/address/";
        private static readonly string testnetCat = "https://api.ton.cat/v2/contracts/address/";

        private static (string source, string dest, UInt128 value, TransactionType type, string message, JsonArray action, JsonArray meta) ReadMessage(JsonArray msg)
        {
            return (msg.GetString("source"), msg.GetString("destination"), msg.GetUInt128("value"), (TransactionType)msg.GetLong("op"), msg.GetString("comment"), msg.GetArray("action"), msg.GetArray("meta"));
        }

        public static (ITransactionBase[], string) GetTransactions(LiteClient client, WalletType walletType, string address, long endTime, int count)
        {
            string error = null;
            string url = null;
            try
            {

                url = (client.IsTestnet ? testnetCat : mainnetCat) + address + "/transactions";
                if (count > 0)
                {
                    url += "?limit=" + count;
                    if (endTime > 0)
                        url += "&end_utime=" + endTime;
                }
                else if (endTime > 0)
                    url += "?end_utime=" + endTime;

                //string data = File.ReadAllText(@"E:\Complex\Ton\Crypto Api\toncat_transactions.json");
                string data = Http.GetBrouser2(url);
                JsonArray arr = Json.Parse2(data) as JsonArray;
                if (arr != null && arr.Count > 0)
                {
                    Array<ITransactionBase> transactions = new Array<ITransactionBase>();

                    foreach (JsonArray j in arr)
                    {
                        long utime = j.GetLong("utime");
                        if (endTime <= 0 || utime < endTime)
                        {
                            UInt128 fee = j.GetUInt128("fee");
                            long lt = j.GetLong("lt");
                            string hash = j.GetString("hash");
                            JsonArray in_msg = j.GetArray("in_msg");
                            JsonArray out_msgs = j.GetArray("out_msgs");
                            Array<ITransactionDetail> tarr = new Array<ITransactionDetail>();
                            TransactionType grtype = TransactionType.None;
                            foreach (JsonArray msg in out_msgs)
                            {
                                (string source, string dest, UInt128 mvalue, TransactionType type, string message, JsonArray action, JsonArray meta) = ReadMessage(msg);
                                if (mvalue > 0)
                                {
                                    TransactionDetail detail = new TransactionDetail(dest, new Gram(mvalue), message, type != TransactionType.None ? type.ToString2() : null, true);
                                    tarr.Add(detail);

                                    detail = GetTransactionDetail(client, dest, walletType, type, true, action, meta);
                                    if (detail != null)
                                    {
                                        grtype = (detail as JettonTransactionMessage).type;
                                        tarr.Add(detail);
                                    }
                                }
                            }
                            UInt128 value = in_msg.GetUInt128("value");
                            if (value > 0)
                            {
                                (string source, string dest, UInt128 mvalue, TransactionType type, string message, JsonArray action, JsonArray meta) = ReadMessage(in_msg);
                                TransactionDetail detail = GetTransactionDetail(client, source, walletType, type, false, action, meta);
                                if (detail != null)
                                {
                                    //grtype = (detail as TonTransactionMessage).type;
                                    tarr.Add(detail);
                                }
                                grtype = type;
                                detail = new TransactionDetail(source, new Gram(mvalue), message, type != TransactionType.None ? type.ToString2() : null, false);
                                tarr.Add(detail);
                            }
                            if (tarr.Count > 0)
                            {
                                if (tarr.Count == 1)
                                    transactions.Add(new TonTransaction(grtype != TransactionType.None ? grtype.ToString2() : null, lt, utime, new Gram(fee), hash, tarr.First as TransactionDetail, null));
                                else
                                {
                                    TonTransactionGroup group = new TonTransactionGroup(grtype != TransactionType.None ? grtype.ToString2() : null, lt, utime, new Gram(fee), hash, tarr, null);
                                    transactions.Add(group);
                                }
                            }
                        }
                    }

                    return (transactions.Count == 0 ? null : transactions.ToArray(), null);
                }
                return (null, null);
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return (new ITransactionBase[0], error);
        }

        private static TransactionDetail GetTransactionDetail(LiteClient client, string address, WalletType walletType, TransactionType type, bool isOut, JsonArray action, JsonArray meta)
        {
            if (action != null && meta != null)
            {
                switch (type)
                {
                    case TransactionType.Excesses:
                        break;
                    case TransactionType.Transfer:
                        if (walletType == WalletType.JettonWallet && !isOut)
                            type = TransactionType.None;
                        break;
                    case TransactionType.InternalTransfer:
                        if (walletType == WalletType.JettonMinter)
                        {
                            type = TransactionType.Mint;
                            isOut = !isOut;
                        }
                        break;
                    case TransactionType.TransferNotification:
                        if (walletType == WalletType.JettonWallet)
                            type = TransactionType.None;
                        break;
                    case TransactionType.BurnNotification:
                        if (walletType == WalletType.JettonMinter)
                        {
                            type = TransactionType.Burn;
                            isOut = !isOut;
                        }
                        break;
                    case TransactionType.Burn:
                        if (walletType == WalletType.JettonWallet && !isOut)
                            type = TransactionType.None;
                        if (walletType == WalletType.JettonMinter)
                            type = TransactionType.None;
                        break;
                    default:
                        type = TransactionType.None;
                        break;
                }
                if (type != TransactionType.None)
                {
                    JettonInfo info = client.GetJettonInfo(meta.GetString("jetton_address"));
                    if (info != null)
                        return new JettonTransactionMessage(address, info, action.GetUInt128("amount"), type, isOut);
                    else
                    {

                    }
                }
            }
            return null;
        }
    }

    //public class CatTransactionDetail : TransactionDetail, IJettonSource
    //{
    //    public CatTransactionDetail(JettonInfo jetton, UInt128 amount, TransactionType type, bool isOut)
    //        : base(null, new Balance(jetton.Symbol, amount, jetton.Decimals, 3), null, null, isOut)
    //    {
    //        this.jetton = jetton;
    //        this.type = type;
    //    }
    //    private JettonInfo jetton;
    //    public JettonInfo Jetton => jetton;

    //    public readonly TransactionType type;
    //}

    //public class CatTransactionGroup : TransactionGroup, ITonTransaction
    //{
    //    public CatTransactionGroup(string name, long lt, long utime, Gram fee, string transactionHash, Array<ITransactionDetail> details)
    //        : base(name, lt.ToString(), Calendar.FromMilliseconds(utime * 1000), fee, transactionHash, details)
    //    {
    //        this.lt = lt;
    //        this.utime = utime;
    //    }

    //    private long lt;
    //    long ITonTransaction.Lt => lt;

    //    private long utime;
    //    public long UTime => utime;

    //    public override int CompareTo(ITransactionBase transaction)
    //    {
    //        if (transaction != this)
    //        {
    //            ITonTransaction tt = transaction as ITonTransaction;
    //            int dir = this.lt.CompareTo(tt.Lt);
    //            if (dir != 0)
    //                return dir;
    //            dir = this.utime.CompareTo(tt.UTime);
    //            if (dir != 0)
    //                return dir;
    //        }
    //        return 0;
    //    }

    //}

    ////public class CatTransactionDetail : TransactionDetail
    ////{

    ////}

    //public class CatTransaction : Transaction, ITonTransaction
    //{
    //    public CatTransaction(long lt, long utime, Gram fee, string hash, TransactionDetail detail) 
    //        : base(lt.ToString(), Calendar.FromMilliseconds(utime * 1000),  fee, hash, detail.Address, detail.Amount, detail.Message, detail.Type, detail.IsOut)
    //    { 
    //        this.lt = lt;
    //        this.utime = utime;
    //        //this.action = action;
    //        //this.meta = meta;
    //    }

    //    private long lt;
    //    long ITonTransaction.Lt => lt;

    //    private long utime;
    //    public long UTime => utime;

    //    //private TransactionType type = TransactionType.None;
    //    //public TransactionType Type => type;

    //    //private TransferTransaction transfer;
    //    //public TransferTransaction Transfer => transfer;

    //    //private JettonInfo jetton;
    //    //public JettonInfo Jetton => jetton;

    //    //private JsonArray action;
    //    //private JsonArray meta;

    //    //bool ITonTransaction.CheckJeton(LiteClient client)
    //    //{
    //    //    if (action != null && meta != null)
    //    //    {
    //    //        JsonArray jarr = meta.GetArray("jetton");
    //    //        if (jarr != null)
    //    //        {
    //    //            //string type = action.GetString("type");
    //    //            string addr = null;
    //    //            //switch (type)
    //    //            //{
    //    //            //    case "jetton:excesses":
    //    //            //        this.type = TransactionType.Excesses;
    //    //            //        break;
    //    //            //    case "jetton:transfer":
    //    //            //        this.type = TransactionType.Transfer;
    //    //            //        addr = action.GetString(IsOut ? "source" : "destination");
    //    //            //        break;
    //    //            //    case "jetton:internal_transfer":
    //    //            //        this.type = TransactionType.InternalTransfer;
    //    //            //        addr = action.GetString(IsOut ? "from" : "to");
    //    //            //        break;
    //    //            //    case "jetton:transfer_notification":
    //    //            //        this.type = TransactionType.TransferNotification;
    //    //            //        addr = action.GetString("sender");
    //    //            //        break;
    //    //            //    case "jetton:burn_notification":
    //    //            //        this.type = TransactionType.BurnNotification;
    //    //            //        addr = action.GetString("sender");
    //    //            //        break;
    //    //            //    case "jetton:burn":
    //    //            //        this.type = TransactionType.Burn;
    //    //            //        addr = action.GetString("sender");
    //    //            //        break;
    //    //            //}


    //    //            switch (this.type)
    //    //            {
    //    //                case TransactionType.Excesses:
    //    //                    break;
    //    //                case TransactionType.Transfer:
    //    //                    addr = action.GetString(IsOut ? "source" : "destination");
    //    //                    break;
    //    //                case TransactionType.InternalTransfer:
    //    //                    addr = action.GetString(IsOut ? "from" : "to");
    //    //                    break;
    //    //                case TransactionType.TransferNotification:
    //    //                    addr = action.GetString("sender");
    //    //                    break;
    //    //                case TransactionType.BurnNotification:
    //    //                    addr = action.GetString("sender");
    //    //                    break;
    //    //                case TransactionType.Burn:
    //    //                    addr = action.GetString("sender");
    //    //                    break;
    //    //            }

    //    //            if (this.type != TransactionType.None)
    //    //            {
    //    //                if (this.Message == null)
    //    //                {
    //    //                    this.Message = this.type.ToString();
    //    //                    this.MessageType = TransactionMessageType.TransactionType;
    //    //                }

    //    //                //JettonWalletInfo walletInfo = client.GetJettonWalletInfo(
    //    //                //int decimals = jarr.GetInt("decimals");
    //    //                //this.jetton = new JettonInfo(jarr.GetString("name"), jarr.GetString("description"), jarr.GetString("symbol"), jarr.GetString("image_data"), new Balance(0, decimals, 3), decimals, meta.GetString("jetton_address"));
    //    //                this.jetton = client.GetJettonInfo(meta.GetString("jetton_address"));
    //    //                this.transfer = new TransferTransaction(action.GetULong("query_id"), new Balance(this.jetton.Symbol, action.GetUInt128("amount"), jetton.Decimals, 3), addr);
    //    //                return true;
    //    //            }
    //    //        }
    //    //    }

    //    //    return false;
    //    //}

    //    public override int CompareTo(ITransactionBase transaction)
    //    {
    //        if (transaction != this)
    //        {
    //            ITonTransaction tt = transaction as ITonTransaction;
    //            int dir = this.lt.CompareTo(tt.Lt);
    //            if (dir != 0)
    //                return dir;
    //            dir = this.utime.CompareTo(tt.UTime);
    //            if (dir != 0)
    //                return dir;
    //            ITransaction tr = transaction as ITransaction;
    //            if (tr != null)
    //            {
    //                dir = this.IsOut.CompareTo(tr.IsOut);
    //                if (dir != 0)
    //                    return dir;
    //            }
    //            return dir;
    //        }
    //        return 0;
    //    }

    //}

}
