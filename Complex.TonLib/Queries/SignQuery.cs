using System;

namespace Complex.Ton
{
    public class SignQuery : Query
    {
        public SignQuery(byte[] message, byte[] publicKey, byte[] password, byte[] secret)
        {
            this.message = message;
            this.publicKey = publicKey;
            this.password = password;
            this.secret = secret;
        }

        private byte[] message;
        private byte[] publicKey;
        private byte[] password;
        private byte[] secret;

        private byte[] sign;
        public byte[] Sign => sign;


        public override void Send(LiteClient client)
        {
            FixResultHandler();
            byte[] res = new byte[64];
            if (TonLib.LiteClientSign(client, publicKey, password, secret, message, message.Length, res, resultHandlerFixed))
                this.sign = res;
        }

    }
}
