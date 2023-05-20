using System;

namespace Complex.Ton
{
    public class GetSeedQuery : Query
    {
        public GetSeedQuery(byte[] publicKey, byte[] password, byte[] secret)
        {
            this.publicKey = publicKey;
            this.password = password;
            this.secret = secret;
        }

        private byte[] publicKey;
        private byte[] password;
        private byte[] secret;

        private byte[] seed;
        public byte[] Seed => seed;


        public override void Send(LiteClient client)
        {
            FixResultHandler();
            byte[] res = new byte[32];
            if (TonLib.LiteClientGetSeed(client, publicKey, password, secret, res, resultHandlerFixed))
                this.seed = res;
        }

    }
}
