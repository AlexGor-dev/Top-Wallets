using System;

namespace Complex.Ton
{
    public class DeleteKeyQuery : Query
    {
        public DeleteKeyQuery(byte[] pubKey, byte[] secret)
        {
            this.pubKey = pubKey;
            this.secret = secret;
        }

        private byte[] pubKey;
        private byte[] secret;

        public override void Send(LiteClient client)
        {
            FixResultHandler();
            TonLib.LiteClientDeleteKey(client, pubKey, secret, resultHandlerFixed);
        }
    }
}
