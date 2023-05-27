using System;

namespace Complex.Ton
{
    public class MessageData : Disposable
    {
        public MessageData(string destAddress, UInt128 amount, Cell message)
        {
            this.destAddress = destAddress;
            this.amount = amount;
            this.message = message;
        }

        protected override void OnDisposed()
        {
            if(message != null)
                this.message.Dispose();
            base.OnDisposed();
        }

        public readonly string destAddress;
        public readonly UInt128 amount;
        public readonly Cell message;
    }
}
