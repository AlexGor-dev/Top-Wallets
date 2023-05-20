using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Complex.Ton
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VmNumber
    {
        public VmNumber(string value)
        {
            this.type = VmObjectType.Number;
            this.value = value;
        }
        public VmObjectType type;
        public string value;

        public override string ToString()
        {
            return this.value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmCell
    {
        public VmCell(IntPtr value)
        {
            this.type = VmObjectType.Cell;
            this.value = value;
        }
        public VmObjectType type;
        public IntPtr value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VmSlice
    {
        public VmSlice(IntPtr value)
        {
            type = VmObjectType.Slice;
            this.value = value;
        }
        public VmObjectType type;
        public IntPtr value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class RawTransaction
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public readonly byte[] CurrentLtHash;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public readonly byte[] PrevLtHash;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public readonly byte[] MsgHash;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public readonly string Source;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public readonly string Destination;


        public readonly long currentLt;
        public readonly long prevLt;
        public readonly long created_lt;
        public readonly long utime;
        public readonly long fee;
        public readonly long value;


        public readonly string message;

        public readonly MessageType messageType;

        public readonly int isOut;

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MessageInfo
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string destAddress;
        public long amount;
        [MarshalAs(UnmanagedType.LPStr)]
        public string message;
        public IntPtr body;
        public IntPtr initState;
        public bool is_encrypted;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct rawtransaction
    {
        public int type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] hash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] msg_hash;
        public long lt;
        public long utime;
        public long fee;

        public string Hash => Convert.ToBase64String(hash);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct rawmessage
    {
        public int type;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public readonly string Source;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public readonly string Destination;
        public long value;
        public string message;
        public MessageType meaageType;
        public bool isOut;
    }
}
