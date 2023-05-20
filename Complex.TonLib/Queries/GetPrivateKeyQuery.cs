using System;

namespace Complex.Ton
{
    public class GetPrivateKeyQuery : Query
    {
        public GetPrivateKeyQuery(byte[] publicKey, byte[] password, byte[] secret)
        {
            this.publicKey = publicKey;
            this.password = password;
            this.secret = secret;
        }

        private byte[] publicKey;
        private byte[] password;
        private byte[] secret;

        private byte[] privateKey;
        public byte[] PrivateKey => privateKey;


        public override void Send(LiteClient client)
        {
            FixResultHandler();
            byte[] res = new byte[32];
            if (TonLib.LiteClientGetPrivateKey(client, publicKey, password, secret, res, resultHandlerFixed))
                this.privateKey = res;
        }

    }
}
