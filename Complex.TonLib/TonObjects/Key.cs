using System;
using Complex;

namespace Complex.Ton
{
    public class Key : Serializable
    {
        protected Key(IData data)
        {
            this.keyData = data["keyData"] as byte[];
        }

        protected override void Save(IData data)
        {
            data["keyData"] = this.keyData;
        }

        public Key(byte[] data)
        {
            this.keyData = data;
        }

        public readonly byte[] keyData;

        public bool CompareTo(Key key)
        {
            return WinApi.Compare(this.keyData, key.keyData);
        }
    }
}
