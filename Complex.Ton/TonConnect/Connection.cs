using System;
using Complex.Remote;
using Complex.Collections;
using Complex.Controls;

namespace Complex.Ton.TonConnect
{
    public class Connection : Disposable, IUnique
    {
        public Connection(TonWallet wallet, WhiteWallet whiteWallet, DAppInfo dapp, string appPublicKey)
        {
            this.wallet = wallet;
            this.whiteWallet = whiteWallet;
            this.dapp = dapp;
            this.appPublicKey = appPublicKey;
            this.bridge_url = whiteWallet.bridge_url;
            var(publicKey, privateKey) = Nacl.Generate();
            this.nacl = new Nacl(this.appPublicKey.HexToBytes(), privateKey);
            this.publicKeyHex = publicKey.ToHexString().ToLower();
        }

        protected override void OnDisposed()
        {
            this.Disconnect();
            SingleThread.Remove(this.publicKeyHex);
            base.OnDisposed();
        }

        public event Handler Disconnected;
        public event Handler<Exception> Error;
        public event Handler<string, ContractDeployData[]> SendTransactions;

        public readonly TonWallet wallet;
        public readonly WhiteWallet whiteWallet;
        public readonly DAppInfo dapp;
        public readonly string appPublicKey;
        public readonly string bridge_url;
        private EventSource eventSource;
        private long last_event_id;

        private Nacl nacl;

        private string publicKeyHex;

        public bool IsConnected => this.eventSource != null && this.eventSource.IsConnected;

        private DataStream dataStream = new DataStream(8192);

        string IUnique.ID => appPublicKey;


        public void Connect(string passcode, ConnectRequest[] requests, ParamHandler<string> result)
        {
            if (!this.IsConnected)
            {
                this.last_event_id = Calendar.Milliseconds;
                this.eventSource = new EventSource(bridge_url + "/events?client_id=" + this.publicKeyHex + "&last_event_id=" + this.last_event_id);

                this.eventSource.Error += (s, e) => this.OnError(e);
                this.eventSource.Message += EventSource_Message;
                this.eventSource.Connect();
                this.wallet.GetStateInit((stateInit, e) =>
                {
                    if (stateInit != null)
                    {
                        Array<ConnectItemReply> items = new Array<ConnectItemReply>();
                        foreach (ConnectRequest request in requests)
                        {
                            switch (request.name)
                            {
                                case "ton_addr":
                                    items.Add(new TonAddressItemReply(Address.ToHex(this.wallet.Address), this.wallet.Adapter.IsTestnet ? NETWORK.TESTNET : NETWORK.MAINNET, this.wallet.PublicKey.keyData.ToHexString(), stateInit));
                                    break;
                                case "ton_proof":
                                    (int workchain, byte[] address) = Address.GetData(this.wallet.Address);
                                    long timestamp = Calendar.ToSeconds(this.wallet.Adapter.ServerUtcTime);
                                    Uri uri = new Uri(dapp.Url);
                                    Domain domain = new Domain { value = uri.Host, lengthBytes = uri.Host.Length };
                                    ParsedMessage message = new ParsedMessage { Workchain = workchain, Address = address, Timestamp = timestamp, Domain = domain, Payload = request.payload };
                                    byte[] signature = this.wallet.Sign(passcode, message.ToBytes());
                                    items.Add(new TonProofItemReplySuccess(timestamp, signature.ToBase64(), request.payload, domain));
                                    break;
                            }
                        }
                        this.Send(new ConnectEventSuccess(items.ToArray()), result);
                    }
                    else
                    {
                        result(e);
                    }
                });
            }
            else
            {
                result("isConnected");
            }
        }

        private void EventSource_Message(object sender, EventSource.ServerSentEvent m)
        {
            string id = null;
            try
            {
                JsonArray array = Json.Parse(m.Data) as JsonArray;
                string from = array.GetString("from");
                byte[] d = Convert.FromBase64String(array.GetString("message"));
                byte[] decrypted = this.nacl.Decrypt(d.Slice(24), d.Slice(0, 24));
                string text = decrypted.GetString().Replace("\\", "");
                JsonArray jarr = Json.Parse(text) as JsonArray;
                id = jarr.GetString("id");
                switch (jarr.GetString("method"))
                {
                    case "disconnect":
                        this.Disconnect();
                        break;
                    case "sendTransaction":
                        SendTransactionPayload payload = Json.Deserialize<SendTransactionPayload>(jarr.GetArray("params").GetArray(0));
                        if (payload.messages == null || payload.messages.Length == 0)
                            throw new SendTransactionException(id, ErrorCode.BadRequest, "Bad request");
                        if (payload.valid_until < Calendar.ToMilliseconds(this.wallet.Adapter.ServerUtcTime))
                            throw new SendTransactionException(id, ErrorCode.BadRequest, "Request timed out");
                        if (payload.IsTestnet != this.wallet.Adapter.IsTestnet)
                            throw new SendTransactionException(id, ErrorCode.BadRequest, "Invalid network");

                        Array<ContractDeployData> datas = new Array<ContractDeployData>();
                        foreach (SendTransactionPayload.Message message in payload.messages)
                        {
                            Cell msg = message.payload != null ? Cell.FromBase64Boc(message.payload) : null;
                            Cell stateInit = message.stateInit != null ? Cell.FromBase64Boc(message.stateInit) : null;
                            datas.Add(new ContractDeployData(this.wallet.Address, Address.FromHex(message.address), stateInit, (UInt128)message.amount, msg));
                        }
                        Events.Invoke(this.SendTransactions, this, id, datas.ToArray());
                        break;
                }
            }
            catch (SendTransactionException e)
            {
                this.Send(e.error, null);
            }
            catch (Exception e)
            {
                SendError(id, ErrorCode.BadRequest, e.Message, null);
            }
        }

        public void SendError(string id, ErrorCode code, string message, ParamHandler<string> result)
        {
            this.Send(new SendTransactionError(id, code, message), result);
        }

        private string Send(object message)
        {
            string json = Json.Serialize(message);
            byte[] encrypted = this.nacl.EncryptConcat(json.ToBytes());
            string url = bridge_url + "/message?client_id=" + this.publicKeyHex + "&to=" + this.appPublicKey + "&ttl=300";
            return Http.Post(url, encrypted.ToBase64());
        }

        public void ConfirmSendTransaction(string id, string resultBoc, ParamHandler<string> resultHandler)
        {
            this.Send(new SendTransactionResponseSuccess(id, resultBoc), resultHandler);
        }

        public void Send(object message, ParamHandler<string> resultHandler)
        {
            SingleThread.Run(this.publicKeyHex, () =>
            {
                try
                {
                    string resp = this.Send(message);
                    if (resultHandler != null)
                    {
                        BridgeResponse bridgeResponse = Json.Deserialize<BridgeResponse>(resp);
                        if (bridgeResponse.statusCode != 200)
                            resultHandler(bridgeResponse.message);
                        else
                            resultHandler(null);
                    }
                }
                catch (Exception e)
                {
                    if (resultHandler != null)
                        resultHandler(e.Message);
                }
            });
        }

        public void Decline(ParamHandler<string> result)
        {
            this.Send(new ConnectEventError(ErrorCode.UserDeclinedTheConnection, "UserDeclinedTheConnection"),result);
        }


        public void Disconnect()
        {
            if (this.IsConnected)
            {
                this.Send(new DisconnectEvent());
                if (this.eventSource != null)
                    this.eventSource.Dispose();
                this.wallet.Connections.Remove(this);
                this.eventSource = null;
            }
            this.OnDisconnected();
        }

        private void OnDisconnected()
        {
            Events.Invoke(this.Disconnected, this);
        }

        private void OnError(Exception e)
        {
            Events.Invoke(this.Error, this, e);
        }

        private class BridgeResponse
        {
            public string message;
            public int statusCode;
            public override string ToString()
            {
                return message + " " + statusCode;
            }
        }
    }
}
