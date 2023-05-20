using System;

namespace Complex.Ton
{
    public class LastQuery : Query
    {
        public LastQuery(ParamHandler<bool, string> paramHandler)
        {
            this.paramHandler = paramHandler;
        }

        private ParamHandler<bool, string> paramHandler;

        protected override void OnResult(long result, string error)
        {
            Events.Invoke(paramHandler, result >= 0, error);
            base.OnResult(result, error);
        }

        public override void Send(LiteClient client)
        {
            this.FixResultHandler();
            TonLib.LiteClientLast(client, this.resultHandlerFixed);
        }
    }
}
