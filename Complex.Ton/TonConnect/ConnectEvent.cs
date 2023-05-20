using System;

namespace Complex.Ton.TonConnect
{
    public class ConnectEvent
    {
        public ConnectEvent(string eventID)
        {
            this.@event = eventID;
            this.id = Calendar.Milliseconds;
        }

        private string @event;
        private long id;
    }

    public class ConnectRequest
    {
        public string name;
        public string payload;
        public override string ToString()
        {
            return name + " " + base.ToString();
        }
    }

    public class DisconnectEvent : ConnectEvent
    {
        public DisconnectEvent()
            :base("disconnect")
        {

        }

        private object payload;
    }
    public class ConnectEventSuccess : ConnectEvent
    {
        public ConnectEventSuccess(ConnectItemReply[] items, DeviceInfo device)
            : base("connect")
        {
            this.payload = new Payload { items = items, device = device };
        }

        public ConnectEventSuccess(ConnectItemReply[] items)
            :this(items, DeviceInfo.defaultDevice)
        {
        }
        private Payload payload;
        private class Payload
        {
            public ConnectItemReply[] items;
            public DeviceInfo device;
        }
    }

    public class ConnectEventError : ConnectEvent
    {
        public ConnectEventError(ErrorCode code, string message)
            : base("connect_error")
        {
            this.payload = new Payload { code = (int)code, message = message };
        }
        private Payload payload;
        private class Payload
        {
            public int code;
            public string message;
        }
    }


    public class DeviceInfo
    {
        public static DeviceInfo defaultDevice = new DeviceInfo { platform = "windows", appName = Resources.Product, appVersion = Resources.Version, maxProtocolVersion = 2, features = new Feature[] { new Feature {name = "SendTransaction", maxMessages = 10 } } };

        private string platform;
        private string appName; // e.g. "Tonkeeper"  
        private string appVersion; // e.g. "2.3.367"
        private int maxProtocolVersion;
        private Feature[] features; // list of supported features and methods in RPC

        private class Feature
        {
            public string name;
            public int maxMessages;
        }
    }

    public class ConnectItemReply
    {
        public ConnectItemReply(string name) => this.name = name;
        private string name;
    }

    public class TonAddressItemReply : ConnectItemReply
    {
        public TonAddressItemReply(string address, NETWORK network, string publicKey, string walletStateInit)
            : base("ton_addr")
        {
            this.address = address;
            this.network = "" + (int)network;
            this.publicKey = publicKey;
            this.walletStateInit = walletStateInit;
        }
        private string address; // TON address raw (`0:<hex>`)
        private string network; // network global_id
        private string publicKey; // HEX string without 0x
        private string walletStateInit; // Base64 (not url safe) encoded stateinit cell for the wallet contract
    }
    public class ConnectItemReplyError : ConnectItemReply
    {
        public ConnectItemReplyError(string name, ErrorCode code, string message)
            : base(name)
        {
            this.error = new Error { code = (int)code, message = message };
        }

        private Error error;

        private class Error
        {
            public int code;
            public string message;
        }
    }
    public class Domain
    {
        public int lengthBytes; // AppDomain Length
        public string value;  // app domain name (as url part, without encoding) 
    }

    public class TonProofItemReplySuccess : ConnectItemReply
    {
        public TonProofItemReplySuccess(long timestamp, string signature, string payload, Domain domain)
            : base("ton_proof")
        {
            this.proof = new Proof { timestamp = timestamp, signature = signature, payload = payload, domain = domain };
        }
        private Proof proof;
        private class Proof
        {
            public long timestamp; // 64-bit unix epoch time of the signing operation (seconds)
            public string signature;// base64-encoded signature
            public string payload;// payload from the request
            public Domain domain;
        }

    }


    public class TonProofItemReplyError : ConnectItemReplyError
    {
        public TonProofItemReplyError(ErrorCode code, string message)
            :base("ton_proof", code, message)
        {

        }
    }

    public class ParsedMessage
    {
        public int Workchain;
        public byte[] Address;
        public long Timestamp;
        public Domain Domain;
        public string Payload;

        public byte[] ToBytes()
        {
            byte[] m;

            using (DataStream stream = new DataStream())
            {
                stream.WriteBuffer("ton-proof-item-v2/".ToBytes());
                stream.WriteBE(Workchain);
                stream.WriteBuffer(Address);
                stream.Write(Domain.lengthBytes);
                stream.WriteBuffer(Domain.value.ToBytes());
                stream.Write(Timestamp);
                stream.WriteBuffer(Payload.ToBytes());

                m = stream.ToBytes();
            }
            byte[] messageHash = Crypto.Sha256ComputeHash(m);
            byte[] fullMes;
            using (DataStream stream = new DataStream())
            {
                stream.WriteBuffer(new byte[] { 0xff, 0xff });
                stream.WriteBuffer("ton-connect".ToBytes());
                stream.WriteBuffer(messageHash);
                fullMes = stream.ToBytes();
            }
            byte[] res = Crypto.Sha256ComputeHash(fullMes);
            return res;
        }
    }

    public class CheckProofReq
    {
        public CheckProofReq(string address, NETWORK network, string walletStateInit, TonProofItemReplySuccess proof)
        {
            this.address = address;
            this.network = (int)network;
            this.proof = new Proof {proof = proof, state_init = walletStateInit };
        }

        private string address; // TON address raw (`0:<hex>`)
        private int network; // network global_id
        private Proof proof;

        public class Proof
        {
            public TonProofItemReplySuccess proof;
            public string state_init;
        }
    }

    public enum NETWORK
    {
        MAINNET = -239,
        TESTNET = -3
    }

    public enum ErrorCode
    {
        Unknown = 0,
        BadRequest = 1,
        AppManifestNotFound = 2,
        AppManifestContentError = 3,
        UnknownApp = 100,
        UserDeclinedTheConnection = 300,
        MethodIsNotSupported = 400
    }

}
