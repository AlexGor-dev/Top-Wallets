using System;
using System.Runtime.InteropServices;
using Complex.Collections;
using Complex.Wallets;

namespace Complex.Ton
{
    internal class GetTransactionsQuery : Query
    {
        public GetTransactionsQuery(WalletType walletType, string address, long lt, string hash, int count, ParamHandler<ITransactionBase[], string> paramHandler) 
        {
            this.walletType = walletType;
            this.address = address;
            this.lt = lt;
            this.hash = hash;
            this.count = count;
            this.paramHandler = paramHandler;
            this.cellHandler = this.OnAddTransaction;
        }

        private ParamHandler<ITransactionBase[], string> paramHandler;

        private WalletType walletType;
        private string address;
        private long lt;
        private string hash;
        private int count;

        private readonly QueryLongHandler cellHandler;
        private Fixed cellHandlerFixed;
        private bool endLoaded = false;

        private LiteClient client;

        private Array<RTransaction> transactions = new Array<RTransaction>();
        private RTransaction last = null;

        public ITransactionBase[] GetTransactions()
        {
            if (endLoaded)
                return null;
            if (this.transactions.Count == 0)
                return null;

            Array<ITransactionBase> trs = new Array<ITransactionBase>();
            foreach (RTransaction rtr in this.transactions)
            {
                rawtransaction tr = rtr.tr;
                TransactionType grtype = TransactionType.None;
                Array<ITransactionDetail> tarr = new Array<ITransactionDetail>();
                TransactionType msgtype = TransactionType.None;

                foreach (rawmessage msg in rtr.messages)
                {
                    bool isOut = msg.isOut;
                    string msgAddress = msg.isOut ? msg.Destination : msg.Source;
                    string jettonAddress = msgAddress;
                    JettonTransactionMessage tonTransaction = null;
                    long queryId = 0;
                    msgtype = TransactionType.None;
                    if (msg.meaageType == MessageType.Raw)
                    {
                        using (Slice slice = Slice.FromBoc(Convert.FromBase64String(msg.message)))
                        {
                            TransactionType type = (TransactionType)slice.LoadUint();
                            msgtype = type;
                            switch (type)
                            {
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
                            if(msgtype != TransactionType.None)
                                queryId = slice.LoadLong();
                            if (type != TransactionType.None)
                            {
                                UInt128 amount = slice.LoadCoins();

                                if (isOut)
                                    grtype = type;

                                JettonInfo info = client.GetJettonInfo(jettonAddress);
                                if (info == null)
                                    info = client.GetJettonInfoFromWallet(jettonAddress);
                                if (info != null)
                                    tonTransaction = new JettonTransactionMessage(msgAddress, info, amount, type, isOut);
                            }
                        }
                    }
                    if (!msg.isOut)
                        grtype = msgtype;
                    if (tonTransaction != null)
                        tarr.Add(tonTransaction);
                    TonTransactionMessage detail = new TonTransactionMessage(msgAddress, new Gram((UInt128)msg.value), msg.meaageType == MessageType.Text ? msg.message : null, msgtype != TransactionType.None ? msgtype.ToString2() : null, msg.isOut, queryId);
                    tarr.Add(detail);

                }

                if (tarr.Count == 1)
                    trs.Add(new TonTransaction(null, tr.lt, tr.utime, new Gram((UInt128)tr.fee), tr.Hash, tarr.First as TransactionDetail, tr.msg_hash));
                else
                {
                    TonTransactionGroup group = new TonTransactionGroup(grtype != TransactionType.None ? grtype.ToString2() : null, tr.lt, tr.utime, new Gram((UInt128)tr.fee), tr.Hash, tarr, tr.msg_hash);
                    trs.Add(group);
                }
            }
            return trs.ToArray();
        }

        private void OnAddTransaction(long handle)
        {
            if (handle > 0)
            {
                unsafe
                {
                    int t = Marshal.ReadInt32((IntPtr)handle);
                    if (t == 1)
                    {
                        if (last != null)
                            this.transactions.Add(last);
                        last = new RTransaction(Marshal.PtrToStructure<rawtransaction>((IntPtr)handle));
                    }
                    else if (t == 2)
                    {
                        last.messages.Add(Marshal.PtrToStructure<rawmessage>((IntPtr)handle));
                    }
                    else
                    {
                    }
                }
            }
        }

        protected override void OnResult(long result, string error)
        {
            if (last != null)
                this.transactions.Add(last);
            last = null;

            this.cellHandlerFixed.Dispose();
            if (error != null)
            {
                if (error == "[Error : 0 : transaction list must be non-empty]")
                {
                    error = Language.Current["allTransactionLoaded"];
                    endLoaded = true;
                }
            }
            if(paramHandler != null)
                SingleThread.Run("GetTransactionsQuery", ()=> Events.Invoke(paramHandler, GetTransactions(), error));
            base.OnResult(result, error);
        }

        public override void Send(LiteClient client)
        {
            this.client = client;
            this.FixResultHandler();
            this.cellHandlerFixed = Fixed.Normal(this.cellHandler);
            TonLib.LiteClientGetTransactions(client, this.address, this.lt, Convert.FromBase64String(this.hash), this.resultHandlerFixed, this.count, this.cellHandlerFixed);
        }

        private class RTransaction
        {
            public RTransaction(rawtransaction tr)
            {
                this.tr = tr;
            }

            public readonly rawtransaction tr;
            public readonly Array<rawmessage> messages = new Array<rawmessage>();
        }

        private static TransactionDetail GetTransactionDetail(LiteClient client, WalletType walletType, TransactionType type, bool isOut)
        {

            return null;
        }
    }
}
