using System;
using Complex.Collections;

namespace Complex.Ton
{
    public class GetWordsQuery : Query
    {
        public GetWordsQuery(byte[] pubKey, byte[] password, byte[] secret, ParamHandler<string[], string> paramHandler)
        {
            this.pubKey = pubKey;
            this.password = password;
            this.secret = secret;
            this.paramHandler = paramHandler;
            this.wordHandler = this.AddWord;
        }

        private byte[] password;
        private byte[] pubKey;
        private byte[] secret;
        private ParamHandler<string[], string> paramHandler;
        private Array<string> wordsList = new Array<string>();
        private readonly QueryStringHandler wordHandler;

        public string[] GetWords()
        {
            return Error != null ? null : wordsList.ToArray();
        }

        private void AddWord(string word)
        {
            this.wordsList.Add(word);
        }

        protected override void OnResult(long result, string error)
        {
            base.OnResult(result, error);
            Events.Invoke(paramHandler, GetWords(), error);
        }

        public override void Send(LiteClient client)
        {
            FixResultHandler();
            using (Fixed f = Fixed.Normal(this.wordHandler))
                TonLib.LiteClientGetWords(client, pubKey, password, secret, f, resultHandlerFixed);

        }

    }
}
