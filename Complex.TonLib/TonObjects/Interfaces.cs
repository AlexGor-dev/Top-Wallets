using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complex.Ton
{
    public interface ITonTransaction
    {
        long Lt { get; }
        long UTime { get; }
        string Hash { get; }
        byte[] MsgHash { get; }
    }

    public interface IJettonSource
    {
        JettonInfo Jetton { get; set; }
    }
}
