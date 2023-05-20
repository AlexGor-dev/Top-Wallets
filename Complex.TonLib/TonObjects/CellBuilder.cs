using System;
using System.Text;
using Complex.Wallets;

namespace Complex.Ton
{
    public class CellBuilder : Native
    {
        private const int CELL_MAX_SIZE_BYTES = 127;

        private CellBuilder()
        {
            this.Handle = TonLib.CellBuilderCreate();
        }

        protected override void OnDisposed()
        {
            TonLib.CellBuilderDestroy(this);
            base.OnDisposed();
        }

        public int Bits => TonLib.CellBuilderBits(this);
        public int Refs => TonLib.CellBuilderRefs(this);

        public static CellBuilder Begin()
        {
            return new CellBuilder();
        }

        public Cell End()
        {
            Cell cell = new Cell(TonLib.CellBuilderFinalize(this));
            this.Dispose();
            return cell;
        }

        public CellBuilder Store(long value, int bits)
        {
            TonLib.CellBuilderStoreLong(this, value, bits);
            return this;
        }

        public CellBuilder Store(Enum value, int bits)
        {
            TonLib.CellBuilderStoreLong(this, Convert.ToInt64(value), bits);
            return this;
        }

        public CellBuilder Offset(int bits)
        {
            while (bits > 0)
            {
                TonLib.CellBuilderStoreLong(this, 0, Math.Min(64, bits));
                bits -= 64;
            }
            return this;
        }

        public CellBuilder Store(bool value)
        {
            return Store(value ? 1 : 0, 1);
        }

        public CellBuilder StoreCoins(UInt128 value)
        {
            if (value == 0)
                return Store(0, 4);
            byte[] data = BigInteger.GetData(value);
            return Store(data.Length, 4).Store(data, 0, data.Length);
        }

        public CellBuilder StoreDict(Dictionary dict)
        {
            TonLib.CellBuilderStoreDict(this, dict);
            if (dict != null)
                dict.Dispose();
            return this;
        }

        public CellBuilder StoreDict(Cell rootCell)
        {
            TonLib.CellBuilderStoreDictFromCell(this, rootCell);
            if(rootCell != null)
                rootCell.Dispose();
            return this;
        }

        public CellBuilder StoreDict(string base64Boc)
        {
            return StoreDict(Cell.FromBase64Boc(base64Boc));
        }

        public CellBuilder StoreDict()
        {
            return StoreDict(null as Dictionary);
        }

        public CellBuilder Store(byte[] value, int offset, int byteCount)
        {
            TonLib.CellBuilderStoreBytes(this, value, offset, byteCount);
            return this;
        }

        public CellBuilder Store(byte[] value)
        {
            TonLib.CellBuilderStoreBytes(this, value, 0, value.Length);
            return this;
        }

        public CellBuilder Store(string value)
        {
            return Store(Encoding.UTF8.GetBytes(value));
        }

        public CellBuilder Store(Slice slice)
        {
            TonLib.CellBuilderStoreSlice(this, slice);
            slice.Dispose();
            return this;
        }

        public CellBuilder StoreSlice(string base64Boc)
        {
            return Store(Slice.FromBase64Boc(base64Boc));
        }

        public static Cell StoreBuffer(byte[] data, int prefix)
        {
            CellBuilder prev = null;
            CellBuilder current = Begin();
            int c = Math.DivRem(data.Length, CELL_MAX_SIZE_BYTES, out int size);
            int offset = c * CELL_MAX_SIZE_BYTES;
            for (int i = c; i >= 0; i--)
            {
                if (i == 0 && prefix >= 0)
                    current.Store(prefix, 8);
                current.Store(data, offset, size);
                if (prev != null)
                    current.StoreRef(prev.End());
                prev = current;
                if (i > 0)
                {
                    current = CellBuilder.Begin();
                    size = CELL_MAX_SIZE_BYTES;
                    offset -= CELL_MAX_SIZE_BYTES;
                }
            }
            return prev.End();
        }

        public static Cell StoreBuffer(byte[] datax)
        {
            return StoreBuffer(datax, -1);
        }

        public static Cell StoreBuffer(string value, int prefix)
        {
            return StoreBuffer(Encoding.UTF8.GetBytes(value), prefix);
        }

        public static Cell StoreBuffer(string value)
        {
            return StoreBuffer(value, -1);
        }

        public CellBuilder StoreRef(Cell cell)
        {
            TonLib.CellBuilderStoreRef(this, cell);
            cell.Dispose();
            return this;
        }

        public CellBuilder StoreRef(string base64Boc)
        {
            return StoreRef(Cell.FromBase64Boc(base64Boc));
        }

        public CellBuilder StoreAddress(string address)
        {
            TonLib.CellBuilderStoreAddress(this, address);
            return this;
        }


        public override string ToString()
        {
            return "bits:(" + Bits + ") refs:(" + Refs + ")";
        }
    }
}
