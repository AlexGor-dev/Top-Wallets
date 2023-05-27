using System;

namespace Complex.Ton
{
    public class RoyaltyParams
    {
        public RoyaltyParams(int royaltyFactor, int royaltyBase, string royaltyAddress)
        {
            this.royaltyFactor = royaltyFactor;
            this.royaltyBase = royaltyBase;
            this.royaltyAddress = royaltyAddress;
        }

        public readonly int royaltyFactor;
        public readonly int royaltyBase;
        public readonly string royaltyAddress;
    }
}
