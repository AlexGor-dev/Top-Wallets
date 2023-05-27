using System;
using System.Text;
using System.Threading;
using Complex.Wallets;
using Complex.Collections;
using Complex.Remote;

namespace Complex.Ton
{
    public class LiteClient : Native
    {
        public LiteClient(bool isTestnet, string name, string config, string validConfig)
        {
            this.isTestnet = isTestnet;
            this.name = name;
            this.config = config;
            this.validConfig = validConfig;
            connectedHandler = this.OnConnected;
            this.connectedHandlerFixed = Fixed.Normal(this.connectedHandler);
        }

        protected override void OnDisposed()
        {
            this.Stop();
            this.connectedHandlerFixed.Dispose();
            base.OnDisposed();
        }

        public event Handler<string> Error;
        public event Handler Connected;
        public event Handler Disconnected;

        private readonly QueryLongHandler connectedHandler;

        private Fixed connectedHandlerFixed;

        private object jettonsLock = new object();

        private bool isTestnet;
        public bool IsTestnet => isTestnet;

        private UniqueCollection<JettonInfo> jettons = new UniqueCollection<JettonInfo>();

        private string name;
        private string config;
        private string validConfig;

        private int serverIndex = -1;
        public int ServerIndex
        {
            get
            {
                if (serverIndex == -1)
                    serverIndex = TonLib.LiteClientGetServerIndex(this);
                return serverIndex;
            }
        }

        public bool IsStarted => IsHandleCreated;
        public bool IsConnected => IsHandleCreated && !this.IsDisposed && TonLib.LiteClientIsConnected(this);

        private object sync = new object();

        private ManualResetEvent e = new ManualResetEvent(false);
        private ManualResetEvent connectedEvent = new ManualResetEvent(false);

        private DictKey<Query> queries = new DictKey<Query>();

        public static string TonDirectory => Resources.LocalApplicationData + "TON\\";

        private void OnConnected(long param)
        {
            connectedEvent.Set();
            if (param == 0)
            {
                this.queries.Clear();
                Events.Invoke(this.Disconnected, this);
            }
            else if (param == 1)
                Events.Invoke(this.Connected, this);
        }

        public void SendAsync(Query query)
        {
            if (this.IsDisposed)
                query.RaiseResult(-1, "LiteClientDisposed");
            else
            {
                lock (queries)
                    queries.Add(query);
                query.Handler += (s) => queries.Remove(s as Query);
                query.Send(this);
            }
        }

        public long Send(Query query)
        {
            lock (sync)
            {
                if (!IsHandleCreated)
                    return 0;
                e.Reset();
                query.Handler += (s) => e.Set();
                this.SendAsync(query);
                e.WaitOne();
                long res = query.Handle;
                if (res == -1)
                    Events.Invoke(this.Error, this, query.Error);
                return res;
            }
        }

        public void Start()
        {
            lock (sync)
            {
                if (!IsHandleCreated && !this.IsDisposed)
                {
                    connectedEvent.Reset();
                    System.IO.Directory.CreateDirectory(Resources.LocalApplicationData);
                    Handle = TonLib.LiteClientCreate(name, TonDirectory, config, validConfig, this.connectedHandlerFixed);
                    connectedEvent.WaitOne();
                }
            }
        }

        private void Stop()
        {
            if (IsHandleCreated)
            {
                int tick = Environment.TickCount;
                while (queries.Count > 0 && Environment.TickCount - tick < 10000)
                    WinApi.Sleep(100);
                this.queries.Clear();
                TonLib.LiteClientDestroy(this);
                this.DeleteHandle();
            }
        }

        public void Connect()
        {
            if (!this.IsDisposed)
            {
                if (!IsHandleCreated)
                    this.Start();
                else
                {
                    connectedEvent.Reset();
                    TonLib.LiteClientConnect(this);
                    connectedEvent.WaitOne();
                }
            }
        }

        public void GetServerTime(ParamHandler<long, string> paramHandler)
        {
            this.SendAsync(new GetServerTimeQuery(paramHandler));
        }

        public JettonInfo GetJettonInfo(AccountState state)
        {
            JettonInfo info = null;
            if (state != null)
            {
                info = JettonController.GetJettonInfo(state);
                if(info != null)
                    lock (this.jettons)
                        this.jettons.Add(info);
            }
            else
            {
                var (j, e) = HttpTonApi.GetJettonInfo(state.Address, isTestnet);
                if (j != null)
                {
                    info = j;
                    lock (this.jettons)
                        this.jettons.Add(info);
                }
            }
            return info;
        }

        public void RemoveJetton(string address)
        {
            lock (this.jettons)
                this.jettons.Remove(address);
        }

        public JettonInfo GetJettonInfo(string address, bool useTemp)
        {
            JettonInfo info = null;
            if (!string.IsNullOrEmpty(address))
            {
                if(useTemp)
                    info = this.jettons[address];
                if (info == null)
                {
                    var (state, error) = this.CreateAccountState(address);
                    if (state != null)
                    {
                        info = GetJettonInfo(state);
                        state.Dispose();
                    }
                    else
                    {
                        var (j, e) = HttpTonApi.GetJettonInfo(address, isTestnet);
                        if (j != null)
                        {
                            info = j;
                            lock (this.jettons)
                                this.jettons.Replace(info);
                        }
                    }
                }
            }
            return info;
        }

        public JettonInfo GetJettonInfo(string address)
        {
            return this.GetJettonInfo(address, true);
        }

        public JettonInfo GetJettonInfoFromWallet(string address)
        {
            JettonInfo info = null;
            var (state, error) = this.CreateAccountState(address);
            if (state != null)
            {
                if (state.Type == WalletType.JettonWallet)
                {
                    object[] stack = state.RunMethod("get_wallet_data");
                    if (stack != null)
                    {
                        if (stack.Length >= 4)
                            info = GetJettonInfo((stack[2] as Slice).LoadAddress());
                        Disposable.Dispose(stack);
                    }
                }
                state.Dispose();
            }
            return info;
        }


        public JettonWalletInfo GetJettonWalletInfo(string address)
        {
            var (state, error) = this.CreateAccountState(address);
            if (state != null)
            {
                JettonWalletInfo winfo = JettonController.GetJettonWalletInfo(state);
                state.Dispose();
                return winfo;
            }
            return null;
        }
        public (long time, string err) GetServerTime()
        {
            GetServerTimeQuery quety = new GetServerTimeQuery(null);
            long res = Send(quety);
            return (res, quety.Error);
        }

        public bool Last()
        {
            LastQuery quety = new LastQuery(null);
            return Send(quety) >= 0;
        }

        public void Last(ParamHandler<bool, string> paramHandler)
        {
            this.SendAsync(new LastQuery(paramHandler));
        }

        public void CreateAccountState(string address, ParamHandler<AccountState, string> paramHandler)
        {
            this.SendAsync(new GetAccountStateQuety(address, paramHandler));
        }

        public (AccountState state, string error) CreateAccountState(string address)
        {
            GetAccountStateQuety quety = new GetAccountStateQuety(address, null);
            long res = Send(quety);
            return (res > 0 ? new AccountState(this, (IntPtr)res) : null, quety.Error);
        }

        public void GetTransactions(WalletType walletType, string address, long lt, string hash, int count, ParamHandler<ITransactionBase[], string> paramHandler)
        {
            this.SendAsync(new GetTransactionsQuery(walletType, address, lt, hash, count, paramHandler));
        }

        public (ITransactionBase[] transactions, string error) GetTransactions(WalletType walletType, string address, long lt, string hash, int count)
        {
            GetTransactionsQuery query = new GetTransactionsQuery(walletType, address, lt, hash, count, null);
            Send(query);
            return (query.GetTransactions(), query.Error);
        }

        public void CalcFees(string srcAddress, MessageInfo[] messages, ParamHandler<Gram, string> paramHandler)
        {
            this.SendAsync(new CalcFeesQuery(srcAddress, messages, paramHandler));
        }

        public (Gram fees, string error) CalcFees(string srcAddress, MessageInfo[] messages)
        {
            CalcFeesQuery query = new CalcFeesQuery(srcAddress, messages, null);
            Send(query);
            return (query.Fees, query.Error);
        }

        public void SendMessage(string srcAddress, byte[] publicKey, byte[] password, byte[] secret, MessageInfo[] messages, ParamHandler<Cell, string> paramHandler)
        {
            this.SendAsync(new SendMessageQuery(srcAddress, publicKey, password, secret, messages, paramHandler));
        }

        public void CreateSendMessageCell(string srcAddress, byte[] publicKey, byte[] password, byte[] secret, MessageInfo[] messages, ParamHandler<Cell, string> paramHandler)
        {
            this.SendAsync(new CreateSendMessageCellQuery(srcAddress, publicKey, password, secret, messages, paramHandler));
        }

        public void CreateWallet(byte[] publicKey, byte[] password, byte[] secret, ParamHandler<object, string> paramHandler)
        {
            this.SendAsync(new CreateWalletQuery(publicKey, password, secret, paramHandler));
        }

        public (byte[] hash, string error) CreateWallet(byte[] publicKey, byte[] password, byte[] secret)
        {
            CreateWalletQuery query = new CreateWalletQuery(publicKey, password, secret, null);
            Send(query);
            return (query.Hash, query.Error);
        }

        public void GetKeyData(string[] words, ParamHandler<KeyData, string> paramHandler)
        {
            this.SendAsync(new GetKeyQuery(words, paramHandler));
        }

        public (KeyData keyData, string error) GetKeyData(string[] words)
        {
            GetKeyQuery query = new GetKeyQuery(words, null);
            Send(query);
            return (query.KeyData, query.Error);
        }

        public void GetWords(byte[] pubKey, byte[] password, byte[] secret, ParamHandler<string[], string> paramHandler)
        {
            this.SendAsync(new GetWordsQuery(pubKey, password, secret, paramHandler));
        }

        public (string[] words, string error) GetWords(byte[] pubKey, byte[] password, byte[] secret)
        {
            GetWordsQuery query = new GetWordsQuery(pubKey, password, secret, null);
            Send(query);
            return (query.GetWords(), query.Error);
        }

        public (byte[] keyData, string error) ExportKey(string dataPassword, byte[] pubKey, byte[] password, byte[] secret)
        {
            GetExportKeyQuery query = new GetExportKeyQuery(dataPassword, pubKey, password, secret);
            Send(query);
            return (query.ExportKey, query.Error);
        }

        public (byte[] secret, string error) ImportKey(string dataPassword, byte[] password, byte[] keyData)
        {
            GetImportKeyQuery query = new GetImportKeyQuery(dataPassword, password, keyData);
            Send(query);
            return (query.ImportKey, query.Error);
        }

        public (byte[] sign, string error) Sign(byte[] message, byte[] pubKey, byte[] password, byte[] secret)
        {
            SignQuery query = new SignQuery(message, pubKey, password, secret);
            Send(query);
            return (query.Sign, query.Error);
        }

        public (byte[] privateKey, string error) GetPrivateKey(byte[] pubKey, byte[] password, byte[] secret)
        {
            GetPrivateKeyQuery query = new GetPrivateKeyQuery(pubKey, password, secret);
            Send(query);
            return (query.PrivateKey, query.Error);
        }

        public (byte[] seed, string error) GetSeed(byte[] pubKey, byte[] password, byte[] secret)
        {
            GetSeedQuery query = new GetSeedQuery(pubKey, password, secret);
            Send(query);
            return (query.Seed, query.Error);
        }

        public string DeleteKey(byte[] pubKey, byte[] secret)
        {
            DeleteKeyQuery query = new DeleteKeyQuery(pubKey, secret);
            Send(query);
            return query.Error;
        }

        public string GetStateInit(AccountState state, byte[] publicKey)
        {
            return TonLib.LiteClientGetStateInit(this, state, publicKey);
        }
        public override string ToString()
        {
            return this.name + " " + (IsConnected ? "connected" : "disconnected") + " " + "Server index " + ServerIndex;
        }
    }
}
