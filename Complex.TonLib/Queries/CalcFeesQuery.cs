using System;

namespace Complex.Ton
{
    internal class CalcFeesQuery : Query
    {
        public CalcFeesQuery(string srcAddress, MessageInfo[] messages, ParamHandler<Gram, string> paramHandler)
        {
            this.srcAddress = srcAddress;
            this.messages = messages;
            this.paramHandler = paramHandler;
        }

        private string srcAddress;
        private MessageInfo[] messages;

        private ParamHandler<Gram, string> paramHandler;

        public Gram Fees => this.Handle > 0 ? new Gram((UInt128)this.Handle) : null;

        protected override void OnResult(long handle, string error)
        {
            base.OnResult(handle, error);
            if (paramHandler != null)
                Events.Invoke(paramHandler, this.Fees, error);
        }

        public override void Send(LiteClient client)
        {
            this.FixResultHandler();
            TonLib.LiteClientCalcFee(client, srcAddress, messages, messages.Length, base.resultHandlerFixed);
        }
    }
}
