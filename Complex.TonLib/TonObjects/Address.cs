using System;
using System.Text;
using Complex;

namespace Complex.Ton
{
    public class Address : Native
    {
        const int workchainInvalid = unchecked((int)0x80000000);

        public Address(byte workchainID, byte[] data, bool testnet)
        {
            this.Handle = TonLib.AddressCreate(workchainID, data, true, testnet);
        }

        private Address(IntPtr handle)
        {
            this.Handle = handle;
        }

        protected override void OnDisposed()
        {
            TonLib.AddressDestroy(this);
            base.OnDisposed();
        }

        public bool IsValid => this.Workchain != workchainInvalid;

        public static Address Parse(string address)
        {
            return new Address(TonLib.AddressParse(address));
        }

        public bool CompareTo(Address address)
        {
            return this.ToString().CompareTo(address.ToString()) == 0;
        }


        private int workchain = -20;
        public int Workchain
        {
            get
            {
                if (workchain == -20)
                    workchain = TonLib.AddressWorkchain(this);
                return workchain;
            }
        }

        private byte[] data;
        public byte[] Data
        {
            get
            {
                if (data == null)
                {
                    data = new byte[33];
                    TonLib.AddressData(this, data);
                }
                return data;
            }
        }

        public static (int workchain, byte[] address) GetData(string address)
        {
            byte[] data = new byte[32];
            int workchain = TonLib.AddressGetData(address, data);
            return (workchain, data);
        }

        public static string ToString(int wc, byte[] rdata, bool bounceable, bool testnet)
        {
            return TonLib.AddressFromDataToString(wc, rdata, bounceable, testnet);
        }

        public static string ToHex(string address)
        {
            return TonLib.AddressToHex(address);
        }

        public static string FromHex(string hexAddress)
        {
            return TonLib.AddressFromHex(hexAddress);
        }
        public override string ToString()
        {
            return TonLib.AddressToString(this);
        }
    }
}
