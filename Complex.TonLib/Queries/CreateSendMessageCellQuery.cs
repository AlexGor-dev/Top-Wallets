using System;
using System.Runtime.InteropServices;

namespace Complex.Ton
{
    internal class CreateSendMessageCellQuery : Query
    {
        public CreateSendMessageCellQuery(string srcAddress, byte[] publicKey, byte[] password, byte[] secret, MessageInfo[] messages, ParamHandler<Cell, string> paramHandler)
        {
            this.srcAddress = srcAddress;
            this.publicKey = publicKey;
            this.password = password;
            this.secret = secret;
            this.messages = messages;
            this.paramHandler = paramHandler;
        }

        private string srcAddress;
        private byte[] publicKey;
        private byte[] password;
        private byte[] secret;
        private MessageInfo[] messages;
        private ParamHandler<Cell, string> paramHandler;

        private byte[] hash;
        public byte[] Hash => hash;

        protected override void OnResult(long result, string error)
        {
            Cell cell = null;
            if (result > 0)
                cell = new Cell((IntPtr)result);
            Events.Invoke(paramHandler, cell, error);
            base.OnResult(result, error);
        }

        public override void Send(LiteClient client)
        {
            this.FixResultHandler();
            TonLib.LiteClientCreateSendMessageCell(client, srcAddress, publicKey, password, secret, messages, messages.Length, resultHandlerFixed);
        }
    }
}
