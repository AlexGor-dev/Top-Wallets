using System;

namespace Complex.Ton
{
    public class GetServerTimeQuery : Query
    {
        public GetServerTimeQuery(ParamHandler<long, string> resultHandler)
        {
            this.resultHandler = resultHandler;
        }

        private new ParamHandler<long, string> resultHandler;

        protected override void OnResult(long result, string error)
        {
            base.OnResult(result, error);
            if(this.resultHandler != null)
                this.resultHandler(result, error);
        }

        public override void Send(LiteClient client)
        {
            this.FixResultHandler();
            TonLib.LiteClientGetServerTime(client, resultHandlerFixed);
        }
    }
}
