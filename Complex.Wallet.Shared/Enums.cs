using System;

namespace Complex.Wallets
{
    public enum WalletState
    {
        None,
        Inactive,
        Active,
    }

    public enum TransactionMessageType
    {
        None,
        Message,
        TransactionType,
        FunctionName
    }
}
