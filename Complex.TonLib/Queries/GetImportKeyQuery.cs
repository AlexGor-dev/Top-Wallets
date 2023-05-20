using System;

namespace Complex.Ton
{
    public class GetImportKeyQuery : Query
    {
        public GetImportKeyQuery(string dataPassword, byte[] password, byte[] keyData)
        {
            this.dataPassword = dataPassword;
            this.password = password;
            this.keyData = keyData;
        }

        private string dataPassword;
        private byte[] password;
        private byte[] keyData;

        private byte[] importKey;
        public byte[] ImportKey => importKey;

        public override void Send(LiteClient client)
        {
            FixResultHandler();
            byte[] secret = new byte[32];
            if (TonLib.LiteClientImportKey(client, dataPassword, password, secret, keyData, keyData.Length, resultHandlerFixed))
                this.importKey = secret;
        }
    }
}
