using System;

namespace Complex.Ton
{
    public class Dictionary : Native
    {
        public Dictionary(IntPtr handle, int keySizeBits)
        {
            this.Handle = handle;
            this.keySizeBits = keySizeBits;
        }

        public Dictionary(int keySizeBits)
            :this(TonLib.DictionaryCreate(keySizeBits), keySizeBits)
        {
        }
        protected override void OnDisposed()
        {
            TonLib.DictionaryDestroy(this);
            base.OnDisposed();
        }

        private int keySizeBits;
        public int KeySizeBits => keySizeBits;

        public static Dictionary Begin(int keySizeBits)
        {
            return new Dictionary(keySizeBits);
        }

        public Dictionary StoreRef(byte[] key, Cell cell)
        {
            TonLib.DictionaryStoreRef(this, key, key.Length, cell);
            cell.Dispose();
            return this;
        }

        public Cell GetRootCell()
        {
            return new Cell(TonLib.DictionaryGetRootCell(this));
        }

        public Slice FindRef(byte[] key)
        {
            IntPtr slice = TonLib.DictionaryFindRef(this, key, key.Length);
            if(slice != IntPtr.Zero)
                return new Slice(slice);
            return null;
        }

        public Cell End()
        {
            Cell cell = new Cell(TonLib.DictionaryGetRootCell(this));
            this.Dispose();
            return cell;
        }
    }
}
