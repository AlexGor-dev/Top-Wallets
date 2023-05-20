using System;
using System.Runtime.InteropServices;

namespace Complex.Ton
{
    public class Nacl
    {
        const int crypto_secretbox_KEYBYTES = 32;
        const int crypto_secretbox_NONCEBYTES = 24;
        const int crypto_secretbox_ZEROBYTES = 32;
        const int crypto_secretbox_BOXZEROBYTES = 16;
        const int crypto_secretbox_BEFORENMBYTES = 32;
        const int crypto_sign_BYTES = 64;
        const int crypto_sign_PUBLICKEYBYTES = 32;
        const int crypto_sign_SECRETKEYBYTES = 64;

        public Nacl(byte[] publicKey, byte[] privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
            crypto_box_beforenm(precomputed, publicKey, privateKey);
            this.nonce = GenNonce();
        }

        public Nacl()
        {
            (this.publicKey, this.privateKey) = Generate();
            crypto_box_beforenm(precomputed, publicKey, privateKey);
            this.nonce = GenNonce();
        }

        public readonly byte[] publicKey;
        private byte[] privateKey;
        private byte[] precomputed = new byte[crypto_secretbox_BEFORENMBYTES];
        public readonly byte[] nonce;

        private const string Dll = "tonlib.dll";

        [DllImport(Dll)]
        private static extern int crypto_box_keypair(byte[] publicKey, byte[] privateKey);

        [DllImport(Dll)]
        private static extern int crypto_box_beforenm(byte[] precomputed, byte[] publicKey, byte[] privateKey);

        [DllImport(Dll)]
        private static extern int crypto_box_afternm(byte[] output, byte[] paddedinput, int paddedinputLength, byte[] nonce, byte[] precomputed);

        [DllImport(Dll)]
        private static extern int crypto_box_open_afternm(byte[] paddedoutput, byte[] input, int inputlength, byte[] nonce, byte[] precomputed);

        [DllImport(Dll)]
        private static extern int crypto_sign(byte[] sig, ref int sigLen, byte[] message, int messagelength, byte[] privateKey);

        [DllImport(Dll)]
        private static extern int crypto_sign_open(byte[] m, ref int mlen, byte[] sm, int n, byte[] publicKey);

        [DllImport(Dll)]
        private static extern int crypto_sign_keypair(byte[] publicKey, byte[] privateKey, bool seeded);

        [DllImport(Dll)]
        private static extern void randombytes(byte[] data, int len);

        public byte[] Encrypt(byte[] input, byte[] nonce)
        {
            return Encrypt(precomputed, input, nonce);
        }

        public byte[] EncryptConcat(byte[] input)
        {
            return nonce.Concat(Encrypt(precomputed, input, nonce));
        }

        public byte[] Encrypt(byte[] input)
        {
            return Encrypt(precomputed, input, nonce);
        }

        private static byte[] Encrypt(byte[] precomputed, byte[] input, byte[] nonce)
        {
            byte[] paddedinput = new byte[input.Length + crypto_secretbox_ZEROBYTES];
            byte[] output = new byte[input.Length + crypto_secretbox_ZEROBYTES];
            paddedinput.Write(crypto_secretbox_ZEROBYTES, input);
            crypto_box_afternm(output, paddedinput, paddedinput.Length, nonce, precomputed);
            return output.Slice(crypto_secretbox_BOXZEROBYTES);
        }

        public static byte[] Encrypt(byte[] publicKey, byte[] privateKey, byte[] input, byte[] nonce)
        {
            byte[] precomputed = new byte[crypto_secretbox_BEFORENMBYTES];
            crypto_box_beforenm(precomputed, publicKey, privateKey);
            return Encrypt(precomputed, input, nonce);
        }

        public byte[] Decrypt(byte[] input, byte[] nonce)
        {
            return Decrypt(precomputed, input, nonce);
        }

        public byte[] Decrypt(byte[] input)
        {
            return Decrypt(precomputed, input, nonce);
        }

        private static byte[] Decrypt(byte[] precomputed, byte[] input, byte[] nonce)
        {
            byte[] paddedoutput = new byte[crypto_secretbox_BOXZEROBYTES + input.Length];
            byte[] output = new byte[paddedoutput.Length];
            paddedoutput.Write(crypto_secretbox_BOXZEROBYTES, input);
            crypto_box_open_afternm(output, paddedoutput, paddedoutput.Length, nonce, precomputed);
            return output.Slice(crypto_secretbox_ZEROBYTES);
        }

        public static byte[] Decrypt(byte[] publicKey, byte[] privateKey, byte[] input, byte[] nonce)
        {
            byte[] precomputed = new byte[crypto_secretbox_BEFORENMBYTES];
            crypto_box_beforenm(precomputed, publicKey, privateKey);
            return Decrypt(precomputed, input, nonce);
        }

        public static (byte[] publicKey, byte[] privateKey) Generate()
        {
            byte[] publicKey = new byte[crypto_secretbox_KEYBYTES];
            byte[] privateKey = new byte[crypto_secretbox_KEYBYTES];
            crypto_box_keypair(publicKey, privateKey);
            return (publicKey, privateKey);
        }

        public static byte[] SignDetached(byte[] message, byte[] privateKey)
        {
            byte[] sig = new byte[crypto_sign_BYTES + message.Length];
            int len = sig.Length;
            crypto_sign(sig, ref len, message, message.Length, privateKey);
            return sig.Slice(0, crypto_sign_BYTES);
        }

        public static bool SignDetachedvVerify(byte[] message, byte[] sign, byte[] publicKey)
        {
            byte[] sm = new byte[crypto_sign_BYTES + message.Length];
            byte[] m = new byte[crypto_sign_BYTES + message.Length];
            sm.Write(sign);
            sm.Write(sign.Length, message);
            int len = m.Length;
            int res = crypto_sign_open(m, ref len, sm, sm.Length, publicKey);
            return res >= 0;
        }

        public byte[] SignDetached(byte[] message)
        {
            return SignDetached(message, privateKey);
        }

        public static byte[] GenNonce()
        {
            byte[] data = new byte[crypto_secretbox_NONCEBYTES];
            randombytes(data, data.Length);
            return data;
        }

        public static (byte[] publicKey, byte[] privateKey) KeyPairFromSeed(byte[] seed)
        {
            byte[] publicKey = new byte[crypto_sign_PUBLICKEYBYTES];
            byte[] privateKey = new byte[crypto_sign_SECRETKEYBYTES];
            privateKey.Write(seed);
            crypto_sign_keypair(publicKey, privateKey, true);
            return (publicKey, privateKey);
        }
    }
}
