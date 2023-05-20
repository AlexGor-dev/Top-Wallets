using System;
using System.Text;

namespace Complex.Ton
{
    public class PublicKey : Key
    {
        protected PublicKey(IData data)
            : base(data)
        {
        }

        public PublicKey(byte[] data)
                : base(data)
        {
        }

        public byte[] Encrypt(byte[] data)
        {
            return TonLib.PublicKeyEncrypt(keyData, data);
        }

        public byte[] Encrypt(string text)
        {
            return Encrypt(Encoding.UTF8.GetBytes(text));
        }

        public static PublicKey Parse(string key)
        {
            return new PublicKey(From(key));
        }

        public static byte[] From(string key)
        {
            byte[] data = new byte[32];
            TonLib.PublicKeyParse(key, data);
            return data;
        }

        public static bool Verify(byte[] publicKey, byte[] message, byte[] signature)
        {
            return TonLib.PublicKeyVerify(publicKey, message, message.Length, signature);
        }

        public override string ToString()
        {
            return TonLib.PublicKeyToString(this);
        }

    }
}
