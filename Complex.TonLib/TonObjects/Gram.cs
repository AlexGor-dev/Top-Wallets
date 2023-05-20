using System;
using Complex.Wallets;

namespace Complex.Ton
{
    public class Gram : Balance, ICustomInit
    {
        private static UInt128 div = 1000000000;
        public Gram(UInt128 value)
            : base("TON", value, div, 2)
        {
        }

        public static UInt128 FromValue(decimal value)
        {
            return (UInt128)Math.Round(value * (decimal)div);
        }

        void ICustomInit.Init()
        {
            if (this.DefaultSignCount == 0)
            {
                this.DefaultSignCount = 2;
                this.Update(0, div);
            }
            this.Symbol = "TON";
        }
    }
}
