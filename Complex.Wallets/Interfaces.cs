using System;
using Complex.Collections;
using Complex.Drawing;

namespace Complex.Wallets
{
    public interface IWalletSource
    {
        Wallet Wallet { get; }
    }

    public interface IExplorerSource
    {
        Explorer Explorer { get; }
    }

    public interface IAdapterSource
    {
        WalletAdapter Adapter { get; }
    }

    public interface IWalletsController
    {
        void ExploreWallet(Wallet wallet);
        void ShowMainWallet(Wallet wallet);
    }

    public interface IExplorerPanel
    {
        void ExploreWallet(Wallet wallet);
    }

    public interface IMultiWallet
    {
        ICollectionEvent<Wallet> Wallets { get; }
    }

    public interface IToken : ITokenInfo
    {
        Wallet Parent { get; }
    }

    public interface IBannerImageSource
    {
        string BannerImageID { get; }
    }
}
