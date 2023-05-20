using System;

namespace Complex.Ton.TonConnect
{
    public class AppRequest
    {
        public string method;
        public string id;
    }

    public class WalletResponse
    {
        public WalletResponse(string id) => this.id = id;
        private string id;
    }

    public class SendTransactionPayload
    {
        public long valid_until;
        private int network;
        public bool IsTestnet => this.network == (int)NETWORK.TESTNET;
        public string from;
        public Message[] messages;

        public class Message
        {
            public string address; // message destination
            public long amount; // number of nanocoins to send.
            public string payload; // raw one-cell BoC encoded in Base64.
            public string stateInit; // raw once-cell BoC encoded in Base64.
        }
    }

    public class SendTransactionRequest : AppRequest
    {
        public object @params;
    }

    public class SendTransactionResponseSuccess : WalletResponse
    {
        public SendTransactionResponseSuccess(string id, string result)
            : base(id)
        {
            this.result = result;
        }

        private string result;
    }

    public class SendTransactionError : WalletResponse
    {
        public SendTransactionError(string id, ErrorCode code, string message)
            :base(id)
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

    public class SendTransactionException : Exception
    {
        public SendTransactionException(string id, ErrorCode code, string message)
        {
            this.error = new SendTransactionError(id, code, message);
        }

        public readonly SendTransactionError error;
    }

    public class SignDataRequest : AppRequest
    {
        public Payload @params;
        public class Payload
        {
            public int schema_crc;
            public string cell;
            public string publicKey;
        }
    }

}
