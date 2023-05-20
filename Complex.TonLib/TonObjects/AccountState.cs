using System;
using Complex.Wallets;

namespace Complex.Ton
{
    public class AccountState : Native
    {
        public AccountState(LiteClient client, IntPtr handle)
        {
            this.client = client;
            this.Handle = handle;
        }

        protected override void OnDisposed()
        {
            TonLib.AccountDestroy(this);
            base.OnDisposed();
        }

        public readonly LiteClient client;

        private string address;
        public string Address
        {
            get
            {
                if (address == null)
                    address = TonLib.AccountAddress(this);
                return address;

            }
        }

        
        private long balance = -1;
        public long Balance
        {
            get
            {
                if(balance == -1)
                    balance = TonLib.AccountBalance(this);
                return balance;
            }
        }

        private long transaction = -1;
        public long Transaction
        {
            get
            {
                if(transaction == -1)
                    transaction = TonLib.AccountTransaction(this);
                return transaction;
            }
        }

        private byte[] transactionHash;
        public byte[] TransactionHash
        {
            get
            {
                if (transactionHash == null)
                {
                    transactionHash = new byte[32];
                    TonLib.AccountTransactionHash(this, transactionHash);
                }
                return transactionHash;
            }
        }

        private byte[] msgHash;
        public byte[] MsgHash
        {
            get
            {
                if (msgHash == null)
                {
                    msgHash = new byte[32];
                    TonLib.AccountGetMsgHash(this, msgHash);
                }
                return msgHash;
            }
        }

        public long Seqno => TonLib.AccountSeqno(this);

        private DateTime time = DateTime.MinValue;
        public DateTime Time
        {
            get
            {
                if (time == DateTime.MinValue)
                    time = Calendar.FromMilliseconds(TonLib.AccountTime(this) * 1000);
                return time;
            }
        }

        private ContractState state = ContractState.Invalid;
        public ContractState State
        {
            get
            {
                if(state == ContractState.Invalid)
                    state = TonLib.AccountGetState(this);
                return state;
            }
        }

        private string version;
        public string Version
        {
            get
            {
                if (version == null)
                    version = TonLib.GetAccountVersion(this);
                return version;
            }
        }

        private WalletType type = WalletType.Empty;
        public WalletType Type
        {
            get
            {
                if (type == WalletType.Empty)
                    type = TonLib.AccountType(this);
                return type;
            }
        }

        public WalletState GetWalletState()
        {
            switch (State)
            {
                case ContractState.Active:
                    return WalletState.Active;
                case ContractState.Initialized:
                case ContractState.Frozen:
                    return WalletState.Inactive;
            }
            return WalletState.None;
        }

        public object[] RunMethod(string methodName, params object[] args)
        {
            if (State != ContractState.Active)
                return new object[] { };
            return TonLib.AccountRunMethod(this, methodName, args);
        }
    }
}
