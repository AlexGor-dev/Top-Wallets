using System;
using Complex.Collections;
using Complex.Drawing;

namespace Complex.Wallets
{
    public interface ITransactionBase : IUnique
    {
        string Name { get; }

        DateTime Time { get; }
        Balance Fee { get; }
        string Hash { get; }
        decimal GetAmount(string symbol);
        int CompareTo(ITransactionBase transaction);
    }

    public interface ITransactionDetail
    {
        string Address{ get; }
        Balance Amount { get; }
        string Message { get; }
        string Type { get; }
        bool IsOut { get; }

    }

    public interface ITranserParams
    {
        string Address { get; }
        UInt128 Amount { get; }
        string Comment { get; }
    }

    public interface ITransaction : ITransactionDetail, ITransactionBase
    {
    }

    public interface ITransactionGroup : ITransactionBase
    {
        Array<ITransactionDetail> Details { get; }
    }

    public interface ITokenInfo : IUnique
    {
        string Name { get; }
        Balance Balance { get; }
        string Address { get; }
        string Owner { get; }
        int Color { get; }
        void LoadImage(ParamHandler<IImage> resultHandler);
    }

    public interface ITokenInfoSource
    {
        ITokenInfo TokenInfo { get; set; }
    }

    public interface INftInfo : IUnique
    {
        string Name { get; }
        string Address { get; }
        string Owner { get; }
        void LoadImage(int imageSize, float ovalRadius, ParamHandler<IImage, string> resultHandler);

    }
}
