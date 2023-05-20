using System;
namespace Complex.Ton
{
    public class GetExportKeyQuery : Query
    {
        public GetExportKeyQuery(string dataPassword, byte[] publicKey, byte[] password, byte[] secret)
        {
            this.dataPassword = dataPassword;
            this.publicKey = publicKey;
            this.password = password;
            this.secret = secret;
        }

        private string dataPassword;
        private byte[] publicKey;
        private byte[] password;
        private byte[] secret;

        private byte[] exportKey;
        public byte[] ExportKey => exportKey;

        public override void Send(LiteClient client)
        {
            this.FixResultHandler();
            int len = 0;
            byte[] keyData = new byte[1024];
            if (TonLib.LiteClientExportKey(client, dataPassword, publicKey, password, secret, keyData, out len, resultHandlerFixed))
            {
                exportKey = new byte[len];
                Array.Copy(keyData, exportKey, len);
            }

        }
    }
}
