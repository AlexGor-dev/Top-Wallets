using System;

namespace Complex.Ton
{
    public class Cell : Native
    {
        public Cell(IntPtr handle)
        {
            this.Handle = handle;
        }

        protected override void OnDisposed()
        {
            TonLib.CellDestroy(this);
            base.OnDisposed();
        }

        public int Bits => TonLib.CellBits(this);
        public int Refs => TonLib.CellRefs(this);

        public Slice ToSlice()
        {
            return Slice.Create(this);
        }

        public byte[] GetHash()
        {
            return TonLib.CellGetHash(this);
        }

        public byte[] ToBoc()
        {
            return TonLib.CellGetData(this);
        }

        public static Cell FromBoc(byte[] bocData)
        {
            return new Cell(TonLib.CellFromBoc(bocData));
        }

        public static Cell FromBase64Boc(string base64)
        {
            return new Cell(TonLib.CellFromBoc(base64.FromBase64()));
        }

        public override string ToString()
        {
            return "bits:(" + Bits + ") refs:(" + Refs + ")";
        }
    }
}
