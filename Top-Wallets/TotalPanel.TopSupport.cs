using System;
using Complex.Collections;

namespace Complex.Wallets
{
    public partial class TotalPanel
    {
        [Serializable]
        public class TopSupport : IUnique
        {
            public TopSupport(string adapterID, string trID, string name, string message, decimal amount, decimal volume, string symbol, DateTime time, int signCount)
            {
                this.adapterID = adapterID;
                this.trID = trID;
                this.name = name;
                this.message = message;
                this.amount = amount;
                this.volume = volume;
                this.symbol = symbol;
                this.time = time;
                this.signCount = signCount;
            }

            private string trID;

            private string adapterID;
            public string AdapterID => adapterID;

            public string ID => adapterID + trID;

            private string name;
            public string Name => name;

            private string message;
            public string Message => message;

            private decimal amount;
            public decimal Amount => amount;

            [field: NonSerialized]
            private decimal volume;
            public decimal Volume
            {
                get => volume;
                set => volume = value;
            }

            private int signCount;
            public int SignCount => signCount;

            private string symbol;
            public string Symbol => symbol;

            private DateTime time;
            public DateTime Time => time;

            public override string ToString()
            {
                return time + " " + name + " " + adapterID + " " + symbol + " amount=" + amount + " volume=" + volume;
            }
        }
    }
}
