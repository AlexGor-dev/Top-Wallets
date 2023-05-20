using System;

namespace Complex.Wallets
{
    public class WalletInfo
    {
        public WalletInfo(Balance balance, WalletState state)
        {
            this.balance = balance;
            this.state = state;
        }

        public readonly Balance balance;
        public readonly WalletState state;
    }
}
