using System;
using System.Runtime.InteropServices;

namespace Complex.Ton
{
    public class CreateWalletQuery : Query
    {
        public CreateWalletQuery(byte[] publicKey, byte[] password, byte[] secret, ParamHandler<object, string> paramHandler)
        {
            this.publicKey = publicKey;
            this.password = password;
            this.secret = secret;
            this.paramHandler = paramHandler;
        }

        private byte[] publicKey;
        private byte[] password;
        private byte[] secret;
        private ParamHandler<object, string> paramHandler;

        private byte[] hash;
        public byte[] Hash => hash;

        protected override void OnResult(long result, string error)
        {
            if (result > 0)
            {
                hash = new byte[32];
                Marshal.Copy((IntPtr)result, hash, 0, hash.Length);
            }
            Events.Invoke(paramHandler, hash, error);
            base.OnResult(result, error);
        }

        public override void Send(LiteClient client)
        {
            FixResultHandler();
            TonLib.LiteClientCreateWallet(client, publicKey, password, secret, resultHandlerFixed);
        }
    }
}
