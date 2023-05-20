using System;
using System.Text;

namespace Complex.Ton
{
    public class PrivateKey : Key
    {
        public PrivateKey(byte[] data)
            : base(data)
        {
        }

        public static PrivateKey Generate()
        {
            return new PrivateKey(TonLib.PrivateKeyGenerate());
        }

        public PublicKey GetPublicKey()
        {
            return new PublicKey(TonLib.PrivateKeyGetPublicKey(keyData));
        }

        public byte[] Sign(Cell cell)
        {
            return TonLib.PrivateKeySign(keyData, cell);
        }

        public byte[] Decrypt(byte[] data)
        {
            return TonLib.PrivateKeyDecrypt(keyData, data);
        }

        public string DecryptText(byte[] data)
        {
            return Encoding.UTF8.GetString(Decrypt(data));
        }

        public static PrivateKey Parse(string key)
        {
            return new PrivateKey(From(key));
        }


        public static byte[] From(string key)
        {
            byte[] data = new byte[32];
            TonLib.PrivateKeyParse(key, data);
            return data;
        }

        public override string ToString()
        {
            return TonLib.PrivateKeyToString(this);
        }

    }
}
