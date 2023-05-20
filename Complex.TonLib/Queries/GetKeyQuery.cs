using System;
using Complex.Collections;
namespace Complex.Ton
{
    internal class GetKeyQuery : Query
    {
        public GetKeyQuery(string[] words, ParamHandler<KeyData, string> paramHandler)
        {
            this.words = words;
            this.paramHandler = paramHandler;
            if (this.words == null)
            {
                this.wordHandler = this.AddWord;
                this.wordsList = new Array<string>();
            }
        }

        private string[] words;
        private ParamHandler<KeyData, string> paramHandler;

        private readonly QueryStringHandler wordHandler;

        private Array<string> wordsList;
        private KeyData keyData;
        public KeyData KeyData => keyData;

        private void AddWord(string word)
        {
            this.wordsList.Add(word);
        }

        protected override void OnResult(long result, string error)
        {
            Events.Invoke(paramHandler, keyData, error);
            base.OnResult(result, error);
        }

        public override void Send(LiteClient client)
        {
            byte[] password = TonLib.RandomSecureBytes(64);
            byte[] pubKey = new byte[32];
            byte[] secret = new byte[32];
            string address = null;
            this.FixResultHandler();
            if (this.words == null)
            {
                byte[] seed = TonLib.RandomSecureBytes(32);
                using (Fixed f = Fixed.Normal(this.wordHandler))
                    address = TonLib.LiteClientCreateWalletAddress(client, new byte[0], pubKey, password, secret, seed, f, resultHandlerFixed);
                this.words = this.wordsList.ToArray();
            }
            else
            {
                address = TonLib.LiteClientGetImportKey(client, new byte[0], words, pubKey, password, secret, resultHandlerFixed);
            }
            if (this.Error == null)
                this.keyData = new KeyData(address, new Ton.PublicKey(pubKey), words, password, secret);
        }
    }
}
