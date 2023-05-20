using System;
using Complex;

namespace Complex.Ton
{
    public delegate void QueryLongHandler(long result);
    public delegate void QueryStringHandler(string text);
    public delegate void QueryResultHandler(long result, string error);

    public abstract class Query
    {
        public Query()
        {
            this.resultHandler = this.OnResult;
        }

        public event Handler Handler;

        protected readonly QueryResultHandler resultHandler;
        protected Fixed resultHandlerFixed;


        private long handle;
        public long Handle => handle;

        private string error;
        public string Error => error;

        private object result;
        public object Result => result;

        protected virtual void OnResult(long result, string error)
        {
            if(this.resultHandlerFixed != null)
                this.resultHandlerFixed.Dispose();
            if (error != null)
            {
                this.error = Language.Current["command"] + ": " + GetType().Name.Replace("Query", "") + "." + Environment.NewLine + error;
                this.handle = -1;
                this.result = this.error;
            }
            else
            {
                this.handle = result;
                this.result = result;
            }
            Events.Invoke(this.Handler, this);
        }

        public void RaiseResult(long result, string error)
        {
            this.OnResult(result, error);
        }

        protected void FixResultHandler()
        {
            this.resultHandlerFixed = Fixed.Normal(resultHandler);
        }

        public virtual void Send(LiteClient client)
        {
            //this.FixResultHandler();
            //TonLib.LiteClientSend(handle, type, paramHandle, resultHandlerFixed);
        }
    }
}
