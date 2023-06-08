using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Collections;
using Complex.Ton.TonConnect;

namespace Complex.Ton
{
    public class TonWallet : TonUnknownWallet, IMultiWallet
    {
        protected TonWallet(IData data)
            :base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.publicKey = data["publicKey"] as PublicKey;
            this.encryption = data["encryption"] as string;
            this.passcodeSalt = data["passcodeSalt"] as byte[];
            this.wallets = data["wallets", ()=> new Hashtable<string, Wallet>()] as Hashtable<string, Wallet>;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["publicKey"] = this.publicKey;
            data["encryption"] = this.encryption;
            data["passcodeSalt"] = this.passcodeSalt;
            data["wallets"] = this.wallets;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }
        public TonWallet(string adapterID, string address, PublicKey publicKey, string encryption, byte[] passcodeSalt)
            :base(adapterID, address)
        {
            this.publicKey = publicKey;
            this.encryption = encryption;
            this.passcodeSalt = passcodeSalt;
            this.wallets = new Hashtable<string, Wallet>();
            this.Init();
        }

        private void Init()
        {
            this.wallets.Added += (s, w) =>
            {
                if (w is TokenWallet token)
                    token.Parent = this;
            };
            //this.wallets.Removed += (s, w) =>
            //{
            //    if (w is TokenWallet token && token.Parent == this)
            //        token.Parent = null;
            //};

        }

        private PublicKey publicKey;
        public PublicKey PublicKey => publicKey;

        private string encryption;
        private byte[] passcodeSalt;

        public override bool IsMain => true;
        public override bool IsSupportSendText => true;
        public override bool IsSupportInvoiceUrl => true;

        private Hashtable<string, Wallet> wallets;
        public Hashtable<string, Wallet> Wallets => wallets;
        ICollectionEvent<Wallet> IMultiWallet.Wallets => wallets;

        private UniqueCollection<Connection> connections = new UniqueCollection<Connection>();
        public UniqueCollection<Connection> Connections => connections;

        protected override void UpdateWaitTransactions(ITransactionBase last, ITransactionBase[] ts)
        {
            if (last == null && ts != null && this.WaitTransactions.Count > 0)
            {
                foreach (ITransactionBase transaction in ts)
                {
                    if (transaction is ITonTransaction tr)
                    {
                        foreach (object value in this.WaitTransactions.ToArray())
                        {
                            if (value is byte[] hash)
                            {
                                if (Util.Compare(hash, tr.MsgHash))
                                {
                                    this.WaitTransactions.Remove(hash);
                                    Util.Run(() => { this.OnTransactionComplete(transaction, hash); });
                                }
                            }
                            else if (value is long qid)
                            {
                                this.CheckQueryIdTransaction(transaction);
                            }
                        }
                        if (this.WaitTransactions.Count == 0)
                            break;
                    }
                }
            }
        }

        public override void CheckPassword(string passcode, ParamHandler<string> resultHanler)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) => resultHanler(e));
        }

        public void CreateSendMessageCell(string passcode, ParamHandler<Cell, string> resultHanler, params MessageInfo[] messages)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                    this.Adapter.CreateSendMessageCell(this, publicKey.keyData, password, secret, messages, resultHanler);
                else
                    resultHanler(null, e);
            });
        }

        public void SendMessages(string passcode, ParamHandler<Cell, string> resultHanler, params MessageInfo[] messages)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                {
                    if (this.State != WalletState.Active)
                    {
                        this.Adapter.CreateWallet(publicKey.keyData, password, secret, (h, e2) =>
                        {
                            if (e2 != null)
                                resultHanler(null, e2);
                            else
                            {
                                Handler handler = null;
                                handler = (s) =>
                                {
                                    this.Created -= handler;
                                    this.Adapter.Run(()=> this.Adapter.SendMessage(this, publicKey.keyData, password, secret, messages, resultHanler));
                                };
                                this.Created += handler;
                            }
                        });
                    }
                    else
                    {
                        this.Adapter.SendMessage(this, publicKey.keyData, password, secret, messages, resultHanler);
                    }
                }
                else
                    resultHanler(null, e);
            });
        }

        public void SendMessage(string passcode, string destAddress, UInt128 amount, string message, Cell body, Cell initState, ParamHandler<object, string> resultHanler)
        {
            this.SendMessages(passcode, (cell, e) =>
            {
                if (cell != null)
                {
                    resultHanler(cell.GetHash(), null);
                    cell.Dispose();
                }
                else
                {
                    resultHanler(null, e);
                }
            }, new MessageInfo { destAddress = destAddress, amount = (long)amount, message = message, body = body, initState = initState });
        }

        public override void SendAmount(string passcode, string destAddress, decimal amount, string message, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, destAddress, this.Balance.FromDecimal(amount), message, null, null, resultHanler);
        }

        public void SendMessage(string passcode, MessageData data, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, data.destAddress, data.amount, "", data.message, null, resultHanler);
        }

        public void CreateJetton(string passcode, JettonDeployData data, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, data.JettonMinterAddress, data.deployParams.amount, "", data.deployParams.message, data.deployParams.stateInit, resultHanler);
        }

        public void CreateNftCollection(string passcode, NftDeployData data, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, data.CollectionAddress, data.deployParams.amount, "", data.deployParams.message, data.deployParams.stateInit, resultHanler);
        }

        public void CreateNftSingle(string passcode, NftSingleDeployData data, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, data.SingleAddress, data.deployParams.amount, "", data.deployParams.message, data.deployParams.stateInit, resultHanler);
        }

        public override void CreateWallet(string passcode, ParamHandler<object, string> resultHanler)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                    this.Adapter.CreateWallet(publicKey.keyData, password, secret, resultHanler);
                else
                    resultHanler(null, e); ;
            });
        }

        public override void CalcFees(string destAddress, decimal amount, string message, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(destAddress, amount, message, null, null, resultHanler);
        }
        public void CalcFees(ParamHandler<Balance, string> resultHanler, params MessageInfo[] messages)
        {
            if (this.State != WalletState.Active)
                resultHanler(this.Balance.Clone(this.Balance.FromDecimal(0.015m)), null);
            else
                this.Adapter.CalcFees(this.Address, messages, resultHanler);
        }

        public void CalcFees(string destAddress, UInt128 amount, string message, Cell body, Cell initState, ParamHandler<Balance, string> resultHanler)
        {
            CalcFees(resultHanler, new MessageInfo { destAddress = destAddress, amount = (long)amount, message = message, body = body, initState = initState });
        }

        public void CalcFees(string destAddress, decimal amount, string message, Cell messageBody, Cell messageInitState, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(destAddress, this.Balance.FromDecimal(amount), message, messageBody, messageInitState, resultHanler);
        }

        public override void GetWords(string passcode, ParamHandler<string[], string> resultHanler)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                    this.Adapter.GetWords(publicKey.keyData, password, secret, resultHanler);
                else
                    resultHanler(null, e);
            });
        }

        public override void ExportData(string passcode, string dataPassword, ParamHandler<object, string> resultHanler)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                {
                    var(keyData, error) = this.Adapter.ExportKey(dataPassword, publicKey.keyData, password, secret);
                    resultHanler(keyData, error);
                }
                else
                    resultHanler(null, e);
            });
        }

        public byte[] Sign(string passcode, byte[] message)
        {
            var (password, secret) = Crypto.Decrypt(passcode, encryption, passcodeSalt);
            return this.Adapter.Sign(message, publicKey.keyData, password, secret).sign;
        }

        public byte[] GetPrivateKey(string passcode)
        {
            var (password, secret) = Crypto.Decrypt(passcode, encryption, passcodeSalt);
            return this.Adapter.GetPrivateKey(publicKey.keyData, password, secret).privateKey;
        }

        public byte[] GetSeed(string passcode)
        {
            var (password, secret) = Crypto.Decrypt(passcode, encryption, passcodeSalt);
            return this.Adapter.GetSeed(publicKey.keyData, password, secret).seed;
        }

        public override void ImportData(string passcode, string dataPassword, object exportData, ParamHandler<string> resultHanler)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                {
                    var (sec, error) = this.Adapter.ImportKey(dataPassword, password, exportData as byte[]);
                    if (sec != null)
                    {
                        byte[] dataToEncrypt = Crypto.Encrypt(password, sec);
                        (string encryption, byte[] passcodeSalt) = Crypto.SetPasscode(passcode, dataToEncrypt);
                        this.encryption = encryption;
                        this.passcodeSalt = passcodeSalt;
                    }
                    resultHanler(error);
                }
                else
                    resultHanler(e);
            });
        }

        public override bool CheckSendWallet(Wallet wallet)
        {
            return base.CheckSendWallet(wallet) && !(wallet is JettonMinter);
        }

        public TonUnknownWallet GetMainChild(string symbol, WalletType walletType)
        {
            foreach (TonUnknownWallet wallet in this.wallets)
                if (wallet.IsMain && wallet.Symbol == symbol && wallet.Type == walletType)
                    return wallet;
            return null;
        }

        public override void Delete(string passcode, ParamHandler<string> resultHanler)
        {
            this.Adapter.Decrypt(passcode, encryption, passcodeSalt, (password, secret, e) =>
            {
                if (password != null)
                    resultHanler(this.Adapter.DeleteKey(publicKey.keyData, secret));
                else
                    resultHanler(e);
            });
        }

        public override void CreateMenu(Component item, ParamHandler<Menu> paramHandler)
        {
            base.CreateMenu(item, (menu)=>
            {
                MenuStrip ms = menu as MenuStrip;
                ms.Add("createToken").Executed += (s) => new CreateJettonForm(this).ShowDialog(Application.Form);
                ms.Add("createNft").Executed += (s) => new CreateNftForm(this).ShowDialog(Application.Form);
                paramHandler(menu);
            });
        }


        public override void CreateAddressMenu(string address, ParamHandler<MenuStrip> paramHandler)
        {
            if (this.wallets.ContainsKey(GetID(this.AdapterID, address, this.IsMain)))
            {
                base.CreateAddressMenu(address, paramHandler);
            }
            else
            {
                this.Adapter.CreateAccountState(address, (s, e) =>
                {
                    base.CreateAddressMenu(address, (menu) =>
                    {
                        if (s != null)
                        {
                            switch (s.Type)
                            {
                                case WalletType.JettonMinter:
                                    {
                                        JettonInfo info = JettonController.GetJettonInfo(s);
                                        if (info != null && info.OwnerAddress == this.Address)
                                            menu.Add(null, "attachJetton", true).Executed += (s2) =>  new AttachJettonForm(this, info).ShowDialog(Application.Form);
                                    }
                                    break;
                                case WalletType.JettonWallet:
                                    {
                                        JettonWalletInfo info = JettonController.GetJettonWalletInfo(s);
                                        if (info != null && info.OwnerAddress == this.Address)
                                            menu.Add(null, "attachJettonWallet", true).Executed += (s2) => new AttachJettonWalletForm(this, info).ShowDialog(Application.Form);
                                    }
                                    break;
                                case WalletType.NftSingle:
                                case WalletType.NftItem:
                                case WalletType.NftCollection:
                                    {
                                        NftInfo info = this.Adapter.GetNftInfo(s);
                                        if(info != null && (info.OwnerAddress == this.Address || info is NftSingleInfo si && si.EditorAddress == this.Address))
                                            menu.Add(null, "attach" + s.Type, true).Executed += (s2) => new AttachNftForm(this, info).ShowDialog(Application.Form);
                                    }
                                    break;
                            }
                            s.Dispose();
                            paramHandler(menu);
                        }
                    });
                });
            }
        }

        public override void CreateTokenInfoAddressMenu(ITokenInfoBase token, ParamHandler<MenuStrip> paramHandler)
        {
            if (token is JettonWalletInfo jwi)
            {
                string address = jwi.JettonInfo.JettonAddress;
                if (this.wallets.ContainsKey(GetID(this.AdapterID, address, this.IsMain)))
                {
                    base.CreateTokenInfoAddressMenu(token, paramHandler);
                }
                else
                {
                    this.Adapter.CreateAccountState(address, (s, e) =>
                    {
                        base.CreateTokenInfoAddressMenu(token, (menu) =>
                        {
                            if (s != null)
                            {
                                if (s.Type == WalletType.JettonMinter)
                                {
                                    JettonInfo info = JettonController.GetJettonInfo(s);
                                    if (info != null && info.OwnerAddress == this.Address)
                                    {
                                        menu.Add(null, "attachJetton", true).Executed += (s2) =>
                                        {
                                            new AttachJettonForm(this, info).ShowDialog(Application.Form);
                                        };
                                    }
                                }
                                s.Dispose();
                                paramHandler(menu);
                            }
                        });
                    });
                }

            }
            else
                base.CreateTokenInfoAddressMenu(token, paramHandler);
        }


        public void GetStateInit(ParamHandler<string, string> paramHandler)
        {
            this.Adapter.CreateAccountState(this.Address, (s, e) =>
            {
                if (s != null)
                {
                    string stateInit = this.Adapter.Client.GetStateInit(s, this.publicKey.keyData);
                    s.Dispose();
                    paramHandler(stateInit, null);
                }
                else
                {
                    paramHandler(null, e);
                }
            });
        }

    }
}
