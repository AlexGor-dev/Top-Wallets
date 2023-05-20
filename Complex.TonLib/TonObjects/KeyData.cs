using System;

namespace Complex.Ton
{
    public class KeyData
    {
        public KeyData(string address, PublicKey publicKey, string[] words, byte[] password, byte[] secret)
        {
            this.address = address;
            this.publicKey = publicKey;
            this.words = words;
            this.password = password;
            this.secret = secret;
            this.dataToEncrypt = Crypto.Encrypt(password, secret);
        }

        private string[] words;
        public string[] Words => words;

        private PublicKey publicKey;
        public PublicKey PublicKey => publicKey;

        private byte[] password;
        public byte[] Password => password;

        private byte[] secret;
        public byte[] Secret => secret;

        private byte[] dataToEncrypt;
        public byte[] DataToEncrypt => dataToEncrypt;


        private string address;
        public string Address => address;


        public override string ToString()
        {
            return this.address;
        }

    }
}
