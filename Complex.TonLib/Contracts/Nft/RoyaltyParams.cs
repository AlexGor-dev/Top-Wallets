using System;

namespace Complex.Ton
{
    public class RoyaltyParams
    {
        public RoyaltyParams(int numerator, int denominator, string destination)
        {
            this.numerator = numerator;
            this.denominator = denominator;
            this.destination = destination;
        }

        public RoyaltyParams(decimal procent, string destination)
        {
            FractionU128 p = (FractionU128)procent;
            this.numerator = (int)p.Numerator;
            this.denominator = (int)p.Denominator * 100;
            this.destination = destination;
        }

        public readonly int numerator;
        public readonly int denominator;
        public readonly string destination;


        public decimal Procent => denominator == 0 ? 0 : 100 * numerator / (decimal)denominator;

    }
}
