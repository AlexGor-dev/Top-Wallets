using System;

namespace Complex.Wallets
{
    public class Balance : FractionU128
    {
        public static readonly Balance Empty = new Balance(null, 0, 0, 0);
        public Balance(string symbol, UInt128 value, UInt128 div, int defaultSignCount)
            :base(value, div)
        {
            this.symbol = symbol;
            this.defaultSignCount = defaultSignCount;
        }

        public Balance(string symbol, UInt128 value, int precision, int defaultSignCount)
            :this(symbol, value, UInt128.Pow(10, (uint)precision), defaultSignCount)
        {

        }

        public event Handler SymbolChanged;

        public long Long => (long)this.Numerator;
        public UInt128 Value => (UInt128)this.Numerator;

        private int defaultSignCount;
        public int DefaultSignCount
        {
            get => defaultSignCount;
            protected set => defaultSignCount = value;
        }

        private string symbol;
        public string Symbol
        {
            get => symbol;
            set
            {
                if (symbol == value) return;
                symbol = value;
                Events.Invoke(this.SymbolChanged, this);
            }
        }

        public decimal Maximum => (decimal)UInt128.MaxValue;

        public Balance Clone(UInt128 amount)
        {
            return new Balance(symbol, amount, Denominator, defaultSignCount);
        }

        public void Add(UInt128 amount)
        {
            this.Update(this.Numerator + amount);
        }

        public void Sub(UInt128 amount)
        {
            this.Update(this.Numerator - amount);
        }

        public UInt128 FromDecimal(decimal value)
        {
            return (UInt128)Math.Round(value * (decimal)this.Denominator);
        }

        public decimal ToDecimal(UInt128 amount)
        {
            return (decimal)new FractionU128(amount, this.Denominator);
        }
        public static string ToString(decimal value)
        {
            return value.GetTextSharps(2);
        }

        public static implicit operator decimal(Balance b)
        {
            return (decimal)(FractionU128)b;
        }

        public string GetTextSharps(int signCount)
        {
            return ((decimal)this).GetTextSharps(signCount);
        }

        public string ToKMBPlus(int signCount)
        {
            return ((decimal)this).ToKMBPlus(signCount);
        }

        public string ToKMB(int signCount)
        {
            return ((decimal)this).ToKMB(signCount);
        }

        public override string ToString()
        {
            return this.GetTextSharps(8);
        }
    }
}
