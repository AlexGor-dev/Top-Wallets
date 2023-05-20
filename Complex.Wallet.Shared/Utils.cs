using System;
using System.Text;

namespace Complex.Wallets
{
    public static class Utils
    {
        private readonly static Random random2 = new Random();


        public static int Random(int max)
        {
            return random2.Next(0, max);
        }

        public static double Random(double max)
        {
            return random2.NextDouble() * max;
        }

    }
}
