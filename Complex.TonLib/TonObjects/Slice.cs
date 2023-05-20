using System;
using System.Text;
using Complex.Wallets;

namespace Complex.Ton
{
    public class Slice : Native
    {
        public static Slice Create(Cell cell)
        {
            return new Slice(TonLib.CellSliceCreate(cell));
        }

        public Slice(IntPtr handle)
        {
            this.Handle = handle;
        }

        protected override void OnDisposed()
        {
            TonLib.CellSliceDestroy(this);
            base.OnDisposed();
        }

        public int Bits => TonLib.CellSliceBits(this);
        public int Refs => TonLib.CellSliceRefs(this);

        public bool IsValid => TonLib.CellSliceIsValid(this);

        public byte[] LoadBytes(int len)
        {
            return TonLib.CellSliceLoadBytes(this, len);
        }

        public byte[] LoadBytes()
        {
            return LoadBytes(Bits / 8);
        }

        public string LoadString(int len)
        {
            return Encoding.UTF8.GetString(LoadBytes(len));
        }

        public string LoadString()
        {
            return LoadString(Bits / 8);
        }

        public static string LoadString(Slice slice)
        {
            string value = null;
            if (slice.Bits > 0)
                value = slice.LoadString(slice.Bits / 8);
            if (slice.Refs > 0)
                value += LoadString(slice.LoadRef());
            slice.Dispose();
            return value;
        }

        public long LoadLong(int bits)
        {
            return (long)TonLib.CellSliceLoadLong(this, bits);
        }

        public long PreLoadLong(int bits)
        {
            return (long)TonLib.CellSlicePreLoadLong(this, bits);
        }

        public int PreLoadInt(int bits)
        {
            return (int)TonLib.CellSlicePreLoadLong(this, bits);
        }

        public long LoadLong()
        {
            return (long)TonLib.CellSliceLoadLong(this, 64);
        }

        public ulong LoadUlong()
        {
            return TonLib.CellSliceLoadLong(this, 64);
        }

        public ulong LoadUlong(int bits)
        {
            return TonLib.CellSliceLoadLong(this, bits);
        }

        public int LoadInt(int bits)
        {
            return (int)LoadLong(bits);
        }

        public int LoadInt()
        {
            return (int)LoadLong(32);
        }

        public uint LoadUint(int bits)
        {
            return (uint)LoadLong(bits);
        }

        public uint LoadUint()
        {
            return (uint)LoadLong(32);
        }

        public bool LoadBool()
        {
            int res = LoadInt(1);
            return res == 1 ? true : false ;
        }

        public Slice LoadRef()
        {
            return new Slice(TonLib.CellSliceLoadRef(this));
        }

        public Cell LoadRefCell()
        {
            return new Cell(TonLib.CellSliceLoadRefCell(this));
        }

        public Slice LoadSlice()
        {
            return new Slice(TonLib.CellSliceLoadSlice(this));
        }

        public Dictionary LoadDict(int keySizeBits)
        {
            return new Dictionary(TonLib.CellSliceLoadDict(this, keySizeBits), keySizeBits);
        }

        public int BSelect(int bits, int mask)
        {
            return TonLib.CellSliceBSelect(this, bits, mask);
        }

        public PublicKey LoadKey()
        {
            return new PublicKey(LoadBytes(32));
        }

        public UInt128 LoadBigInt(int bits)
        {
            return TonLib.CellSliceLoadBigInt(this, bits);
        }

        public UInt128 LoadUInt128(int length)
        {
            int size = (int)Math.Ceiling(Math.Log(length, 2));
            int bits = LoadInt(size) * 8;
            if(bits <= 64)
                return LoadUlong(bits);
            return LoadBigInt(bits);
        }

        public UInt128 LoadCoins()
        {
            return LoadUInt128(16);
        }

        public Int128 GetFee()
        {
            return TonLib.CellSliceGetFee(this);
        }

        public Int128 GetStorageFeeRef()
        {
            return TonLib.CellSliceGetStorageFeeRef(this);
        }

        public string LoadAddress()
        {
            int flag = LoadInt(2);
            if (flag == 2)
            {
                int offset = LoadInt(1);
                return Address.ToString(LoadInt(8), LoadBytes(32), true, false);
            }
            return null;
        }

        public void SkipRefs(int count)
        {

        }
        public void SkipDict()
        {
            if (LoadInt(1) != 0)
                SkipRefs(1);

        }
        public string GetAddressExt()
        {
            return TonLib.CellSliceGetAddressExt(this);
        }

        public Address LoadAddress(bool testnet)
        {
            return new Address(LoadBytes(1)[0], LoadBytes(32), testnet);
        }

        public static Slice FromBoc(byte[] bocData)
        {
            return new Slice(TonLib.CellSliceFromBoc(bocData));
        }

        public static Slice FromBase64Boc(string base64)
        {
            return new Slice(TonLib.CellSliceFromBoc(Convert.FromBase64String(base64)));
        }

        public override string ToString()
        {
            return "bits:(" + Bits + ") refs:(" + Refs + ")";
        }
    }
}
