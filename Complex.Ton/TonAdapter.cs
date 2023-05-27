using System;
using System.IO;
using System.Net;
using Complex.Remote;
using Complex.Ton;
using Complex.Controls;
using Complex.Collections;
using Complex.Trader;
using Complex.Drawing;
using Complex.Ton.TonConnect;
using System.Collections.Generic;

namespace Complex.Wallets
{
    //https://ton.org/testnet-global.config.json
    //https://ton.org/global-config.json

    public class TonAdapter : WalletAdapter
    {
        const string testUrl = "https://ton.org/testnet-global.config.json";
        const string mainUrl = "https://ton.org/global-config.json";
        const string testComxUrl = "http://complex-soft.com/res/config_test_valid.json";
        const string mainComxUrl = "http://complex-soft.com/res/config_valid.json";

        static TonAdapter()
        {
        }
        protected TonAdapter(IData data)
            : base(data)
        {

        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TonAdapter(TonAdapterExtension extension)
            : base(extension)
        {
            this.Init();
        }


        private void Init()
        {
            string fileName = IsTestnet ? "ton_testnet.state" : "ton_mainnet.state";
            string path = LiteClient.TonDirectory + fileName;
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(LiteClient.TonDirectory);
                File.WriteAllBytes(path, Resources.GetBytes(fileName));
            }
        }

        protected override void OnDisposed()
        {
            if(client != null)
                client.Dispose();
            SingleThread.Remove("TonApiCat");
            SingleThread.Remove("HttpTonApi");
            base.OnDisposed();
        }


        private LiteClient client;
        public LiteClient Client => client;

        public override string Symbol => "TON";

        private double serverTimeOffsetSeconds = double.MaxValue;
        public override DateTime ServerUtcTime
        {
            get
            {
                if (serverTimeOffsetSeconds == double.MaxValue)
                {
                    GetServerTime();
                    if (serverTimeOffsetSeconds == double.MaxValue)
                        return DateTime.UtcNow;
                }
                return DateTime.UtcNow.AddSeconds(serverTimeOffsetSeconds);
            }
        }

        public override bool SupportConnection => true;

        private bool isConnected = false;
        public override bool IsConnected => this.isConnected;

        private Array<DApp> dApps = null;

        public override IEnumerable<Component> TopActionComponents
        {
            get
            {
                if (!IsTestnet)
                {
                    if (this.dApps == null)
                    {
                        this.dApps = new Array<DApp>();
                        Util.Run(() =>
                        {
                            string json = Util.Try(() => Http.Get("http://complex-soft.com/res/dapps.json"));
                            if (json == null)
                                json = Resources.GetText("dapps.json");
                            JsonArray array = Json.Parse2(Resources.GetText("dapps.json")) as JsonArray;
                            foreach (JsonArray jarr in array)
                            {
                                DApp dApp = Json.Deserialize<DApp>(jarr);
                                dApp.LoadImage(null);
                                this.dApps.Add(dApp);
                            }
                        });
                    }

                    CheckedImageButton menuButton = new CheckedImageButton("dapp.svg", false);
                    menuButton.ToolTipInfo = new ToolTipInfo(menuButton.ImageID, "DApps", "");
                    menuButton.MaxHeight = 22;
                    menuButton.CheckedChanged += (s) =>
                    {
                        CheckedImageButton mb = s as CheckedImageButton;
                        if (mb.Checked)
                        {
                            DAppsMenu menu = new DAppsMenu(this.dApps);
                            menu.ActionComponent = mb;
                            mb.IsFixed = true;
                            menu.Hided += (s2) =>
                            {
                                menu = s2 as DAppsMenu;
                                mb.IsFixed = false;
                                mb.Checked = false;
                                menu.Dispose();
                            };
                            menu.Show(mb, MenuAlignment.BottomLeft);
                        }
                    };
                    yield return menuButton;
                }
            }
        }
        private void GetServerTime()
        {
            var (st, err) = this.client.GetServerTime();
            if (st > 0)
            {
                DateTime time = Calendar.FromSeconds((int)st);
                serverTimeOffsetSeconds = (time - DateTime.UtcNow).TotalSeconds;
            }
        }

        public override void Start()
        {
            if (client == null)
            {
                KnownAddress.GetName("");

                string config = Util.Try(() => Http.Get(this.IsTestnet ? testUrl : mainUrl));
                if(config == null)
                    config = Resources.GetText(this.IsTestnet ? "config_test.json" : "config.json");
                string validConfig = Util.Try(() => Http.Get(this.IsTestnet ? testComxUrl : mainComxUrl));
                if(validConfig == null)
                    validConfig = Resources.GetText(this.IsTestnet ? "config_test_valid.json" : "config_valid.json");

                client = new LiteClient(this.IsTestnet, this.IsTestnet ? "testnet" : "mainnet", config, validConfig);
                client.Connected += Client_Connected;
                client.Disconnected += Client_Disconnected;
                client.Error += Client_Error;
            }
            if (!client.IsStarted)
                client.Start();
        }
        private void Client_Connected(object sender)
        {
            SingleThread.Run(() => this.GetServerTime());
            //this.OnConnected();
        }

        private void Client_Disconnected(object sender)
        {
            this.isConnected = false;
            this.OnDisconnected();
        }

        private void Client_Error(object sender, string value)
        {
            //this.OnError(value);
        }

        protected override bool OnRefresh()
        {
            if (!this.IsDisposed)
            {
                this.Start();
                if (!client.IsConnected)
                    client.Connect();
                bool last = client.IsConnected && client.Last();
                if (!last)
                    client.Connect();
                else if (!this.isConnected)
                {
                    this.isConnected = true;
                    this.OnConnected();
                }
                return last;
            }
            return false;
        }

        public override Bitmap GenerateQRCode(string url, int width, int height)
        {
            return QRCode.Generate(url, width, height, Images.GetBitmap("gem_l.png"), true);
        }

        public override string GetKnownAddress(string address)
        {
            return KnownAddress.GetName(address);
        }

        public (AccountState, string) CreateAccountState(string address)
        {
            return client.CreateAccountState(address);
        }

        public void CreateAccountState(string address, ParamHandler<AccountState, string> paramHandler)
        {
            client.CreateAccountState(address, paramHandler);
        }


        public NftInfo GetNftInfo(AccountState state)
        {
            NftInfo info = null;
            switch (state.Type)
            {
                case WalletType.NftCollection:
                    info = HttpTonApi.GetNftCollectionData(state.Address, this.IsTestnet);
                    if(info == null)
                        info = NftController.GetCollectionData(state);
                    break;
                case WalletType.NftItem:
                case WalletType.NftSingle:
                    info = HttpTonApi.GetNftData(state.Address, this.IsTestnet);
                    if(info == null)
                        info = NftController.GetNftData(state);
                    break;
            }
            if (info != null)
                info.Type = state.Version;
            return info;
        }

        public void GetNftInfo(string address, ParamHandler<NftInfo, string> paramHandler)
        {
            CreateAccountState(address, (s, e) =>
            {
                NftInfo info = null;
                if (s != null)
                {
                    info = GetNftInfo(s);
                    s.Dispose();
                }
                paramHandler(info, e);
            });
        }

        public override void GetWallet(string address, ParamHandler<Wallet, string> paramHandler)
        {
            TonUnknownWallet wallet = WalletsData.GetWallet(this.ID, address, false) as TonUnknownWallet;
            if (wallet != null)
                paramHandler(wallet, null);
            else
            {
                CreateAccountState(address, (s, e) =>
                {
                    if (s != null)
                    {
                        switch (s.Type)
                        {
                            case WalletType.JettonWallet:
                                JettonWalletInfo walletInfo = JettonController.GetJettonWalletInfo(s);
                                if (walletInfo != null)
                                    wallet = new JettonWallet(this.ID, address, walletInfo, null);
                                else
                                    e = "GetJettonWalletInfoError";
                                break;
                            case WalletType.JettonMinter:
                                JettonInfo info = JettonController.GetJettonInfo(s);
                                if (info != null)
                                    wallet = new JettonMinter(this.ID, address, info, null);
                                else
                                    e = "GetJettonInfoError";
                                break;
                            case WalletType.NftItem:
                            case WalletType.NftSingle:
                                NftInfo data = GetNftInfo(s);
                                if (data != null)
                                    wallet = new NftItem(this.ID, address, data, null);
                                else
                                    e = "GetNftDataError";
                                break;
                            case WalletType.NftCollection:
                                NftInfo collectionData = GetNftInfo(s);
                                if (collectionData != null)
                                    wallet = new NftCollection(this.ID, address, collectionData, null);
                                else
                                    e = "GetNftCollectionDataError";
                                break;
                        }
                        if(wallet == null)
                            wallet = new TonUnknownWallet(this.ID, address);
                        wallet.Update(s);
                        s.Dispose();
                        WalletsData.Wallets.Add(wallet);

                    }
                    paramHandler(wallet, e);

                });
            }
        }

        public override void GetWalletInfo(string address, ParamHandler<WalletInfo, string> paramHandler)
        {
            Wallet wallet = WalletsData.GetAnyWallet(this.ID, address);
            if (wallet != null)
                paramHandler(new WalletInfo(wallet.Balance, wallet.State), null);
            else
            {
                CreateAccountState(address, (s, e) =>
                {
                    WalletInfo info = null;
                    if (s != null)
                    {
                        info = new WalletInfo(new Gram((UInt128)s.Balance), s.GetWalletState());
                        s.Dispose();
                    }
                    paramHandler(info, e);

                });
            }
        }

        public override bool IsValidAddress(string address)
        {
            using (Address a = Address.Parse(address))
                return a.IsValid;
        }

        public void GetTransactions(WalletType walletType, string address, long lt, string hash, int count, ParamHandler<ITransactionBase[], string> resultHanler)
        {
            client.GetTransactions(walletType, address, lt, hash, count, resultHanler);
        }

        public void GetTransactions(TonUnknownWallet wallet, long end_utime, int count, ParamHandler<ITransactionBase[], string> resultHanler)
        {
            SingleThread.Run("TonApiCat", () =>
            {
                var (ts, e) = TonApiCat.GetTransactions(client, wallet.Type, wallet.Address, end_utime, count);
                resultHanler(ts, e);
                WinApi.Sleep(1000);
            });
        }

        public void GetKeyData(string[] words, ParamHandler<KeyData, string> paramHandler)
        {
            client.GetKeyData(words, paramHandler);
        }

        public (KeyData keyData, string error) GetKeyData(string[] words)
        {
            return client.GetKeyData(words);
        }

        public (byte[] keyData, string error) ExportKey(string dataPassword, byte[] pubKey, byte[] password, byte[] secret)
        {
            return client.ExportKey(dataPassword, pubKey, password, secret);
        }

        public (byte[] secret, string error) ImportKey(string dataPassword, byte[] password, byte[] keyData)
        {
            return client.ImportKey(dataPassword, password, keyData);
        }

        public (byte[] sign, string error) Sign(byte[] message, byte[] pubKey, byte[] password, byte[] secret)
        {
            return client.Sign(message, pubKey, password, secret);
        }

        public (byte[] privateKey, string error) GetPrivateKey(byte[] pubKey, byte[] password, byte[] secret)
        {
            return client.GetPrivateKey(pubKey, password, secret);
        }

        public (byte[] seed, string error) GetSeed(byte[] pubKey, byte[] password, byte[] secret)
        {
            return client.GetSeed(pubKey, password, secret);
        }

        public string DeleteKey(byte[] pubKey, byte[] secret)
        {
            return client.DeleteKey(pubKey, secret);
        }

        public (Wallet, string) GetWallet(string address, string passcode, PublicKey publicKey, byte[] dataToEncrypt)
        {
            var (s, e) = CreateAccountState(address);
            if (s != null)
            {
                var(encryption, passcodeSalt) = Crypto.SetPasscode(passcode, dataToEncrypt);
                TonWallet wallet = new TonWallet(this.ID, address, publicKey, encryption, passcodeSalt);
                wallet.Update(s);
                s.Dispose();
                return (wallet, null);
            }
            return (null, e);
        }

        public void Decrypt(string passcode, string encryption, byte[] passcodeSalt, ParamHandler<byte[], byte[], string> resultHandler)
        {
            this.Run(()=>
            {
                try
                {
                    var (password, secret) = Crypto.Decrypt(passcode, encryption, passcodeSalt);
                    resultHandler(password, secret, password == null ? "invalidPassword" : null);
                }
                catch (Exception e)
                {
                    resultHandler(null, null, e.Message);
                }
            });
        }

        public void SendMessage(Wallet wallet, byte[] publicKey, byte[] password, byte[] secret, Ton.MessageInfo[] messages, ParamHandler<Cell, string> resultHandler)
        {
            client.SendMessage(wallet.Address, publicKey, password, secret, messages, resultHandler);
        }

        public void CreateSendMessageCell(Wallet wallet, byte[] publicKey, byte[] password, byte[] secret, Ton.MessageInfo[] messages, ParamHandler<Cell, string> resultHandler)
        {
            client.CreateSendMessageCell(wallet.Address, publicKey, password, secret, messages, resultHandler);
        }

        public void CreateWallet(byte[] publicKey, byte[] password, byte[] secret, ParamHandler<object, string> resultHandler)
        {
            client.CreateWallet(publicKey, password, secret, resultHandler);
        }

        public void CalcFees(string srcAddress, Ton.MessageInfo[] messages, ParamHandler<Balance, string> resultHanler)
        {
            client.CalcFees(srcAddress, messages, (g, e) => { resultHanler(g, e); });
        }

        public void GetWords(byte[] pubKey, byte[] password, byte[] secret, ParamHandler<string[], string> paramHandler)
        {
            client.GetWords(pubKey, password, secret, paramHandler);
        }

        public void GetTokens(string address, ParamHandler<ITokenInfo[], string> resultHanler)
        {
            SingleThread.Run("HttpTonApi", () =>
            {
                var (ts, e) = HttpTonApi.GetTokens(this.client, address, IsTestnet); ;
                resultHanler(ts, e);
                WinApi.Sleep(1000);
            });
        }

        public void GetNfts(string address, int offset, int count, ParamHandler<INftInfo[], string> resultHanler)
        {
            SingleThread.Run("HttpTonApi", () =>
            {
                var (ts, e) = HttpTonApi.GetNftItems(address, offset, count, IsTestnet);
                resultHanler(ts, e);
                WinApi.Sleep(1000);
            });
        }

        public override void CreateWallet(Component component, ParamHandler<Wallet, string> paramHandler)
        {
            new CreateWalletForm(this, paramHandler).Show(component, MenuAlignment.BottomLeft);
        }

        public override bool ExecuteCmd(string cmd)
        {
            TonUrl tonUrl = TonUrl.Parse(cmd);
            if (tonUrl != null && tonUrl.Command == "transfer")
            {
                Timer.Delay(300, () =>
                {
                    if (!string.IsNullOrEmpty(tonUrl.Jetton))
                    {
                        new JettonMultiSendForm(this, tonUrl).ShowDialog(Application.Form);
                    }
                    else if (!string.IsNullOrEmpty(tonUrl.Nft))
                    {

                    }
                    else
                    {
                        new MultiSendForm(this, tonUrl).ShowDialog(Application.Form);
                    }
                });
                return true;
            }
            return false;
        }

    }
}
