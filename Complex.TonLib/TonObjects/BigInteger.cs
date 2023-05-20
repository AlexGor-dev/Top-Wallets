using System;
using System.Collections.Generic;
using System.Text;

namespace Complex.Ton
{
    public class BigInteger
    {
        public static byte[] FromString(string value)
        {
            return TonLib.BigIntegerGetData(value);
        }

        public static byte[] GetData(UInt128 value)
        {
            return TonLib.BigIntegerGetData(value);
        }

    }
}
