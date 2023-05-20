using System;

namespace Complex.Ton
{
    public class GetAccountStateQuety : Query
    {
        public GetAccountStateQuety(string address, ParamHandler<AccountState, string> paramHandler)
        {
            this.address = address;
            this.paramHandler = paramHandler;
        }

        private string address;
        private ParamHandler<AccountState, string> paramHandler;
        private LiteClient client;

        protected override void OnResult(long result, string error)
        {
            if(paramHandler != null)
                Util.Run(()=> Events.Invoke(paramHandler, result > 0 ? new AccountState(this.client, (IntPtr)result) : null, error));
            base.OnResult(result, error);
        }

        public override void Send(LiteClient client)
        {
            this.client = client;
            FixResultHandler();
            TonLib.LiteClientGetAccountState(client, this.address, this.resultHandlerFixed);
        }
    }
}
