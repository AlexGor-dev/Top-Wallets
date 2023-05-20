using System;

namespace Complex.Ton
{
    public static class Crypto
    {

        public static (string dataToEncrypt, byte[] passcodeSalt) SetPasscode(string passcode, byte[] dataToEncrypt)
        {
            byte[] passcodeSalt = TonLib.RandomSecureBytes(32);
            byte[] hash = TonLib.ComputePBKDF2(passcode, passcodeSalt);
            byte[] key = new byte[32];
            byte[] iv = new byte[32];
            Array.Copy(hash, 0, key, 0, key.Length);
            Array.Copy(hash, key.Length, iv, 0, iv.Length);
            TonLib.UtilsAesIgeEncryption(dataToEncrypt, key, iv, true, 0, dataToEncrypt.Length);
            return (Convert.ToBase64String(dataToEncrypt), passcodeSalt);
        }

        public static (byte[] password, byte[] secret) Decrypt(string passcode, string encryption, byte[] passcodeSalt)
        {
            byte[] decrypted = Convert.FromBase64String(encryption);
            byte[] hash = TonLib.ComputePBKDF2(passcode, passcodeSalt);
            byte[] key = new byte[32];
            byte[] iv = new byte[32];
            Array.Copy(hash, 0, key, 0, key.Length);
            Array.Copy(hash, key.Length, iv, 0, iv.Length);
            TonLib.UtilsAesIgeEncryption(decrypted, key, iv, false, 0, decrypted.Length);
            if (decrypted[1] == 'o' && decrypted[2] == 'k')
            {
                int padding = decrypted[0];
                byte[] password = new byte[64];
                byte[] secret = new byte[decrypted.Length - 64 - padding - 3];
                Array.Copy(decrypted, 3, password, 0, password.Length);
                Array.Copy(decrypted, 3 + password.Length, secret, 0, secret.Length);
                return (password, secret);
            }
            return (null, null);
        }

        public static byte[] Encrypt(byte[] password, byte[] secret)
        {
            int len = 1 + 2 + password.Length + secret.Length;
            int padding = len % 16;
            if (padding != 0)
            {
                padding = 16 - padding;
                len += padding;
            }
            byte[] dataToEncrypt = new byte[len];
            dataToEncrypt[0] = (byte)padding;
            dataToEncrypt[1] = (byte)'o';
            dataToEncrypt[2] = (byte)'k';
            Array.Copy(password, 0, dataToEncrypt, 3, password.Length);
            Array.Copy(secret, 0, dataToEncrypt, 3 + password.Length, secret.Length);
            if (padding > 0)
                Array.Copy(TonLib.RandomSecureBytes(padding), 0, dataToEncrypt, 3 + password.Length + secret.Length, padding);
            return dataToEncrypt;
        }

        public static byte[] RandomSecure(int len)
        {
            return TonLib.RandomSecureBytes(len);
        }
        public static byte[] EncryptData(string data, PublicKey public_key, PrivateKey private_key, byte[] salt)
        {
            return TonLib.EncryptData(data, public_key.keyData, private_key.keyData, salt);
        }

        public static string DecryptData(byte[] data, PublicKey public_key, PrivateKey private_key, byte[] salt)
        {
            return TonLib.DecryptData(data, public_key.keyData, private_key.keyData, salt);
        }

        public static byte[] Sha256ComputeHashText(string text)
        {
            return TonLib.Sha256ComputeHashText(text);
        }

        public static byte[] Sha256ComputeHash(byte[] data)
        {
            return TonLib.Sha256ComputeHash(data);
        }

    }
}
