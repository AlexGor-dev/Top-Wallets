using System;
using System.Collections.Generic;
using Complex.Collections;

namespace Complex.Ton
{
    public static class SerializeDict
    {

        public static Cell Serialize(Dict src, int keyLength, ParamHandler<object, CellBuilder> serializer)
        {
            object tree = BuildTree(src, keyLength);
            CellBuilder dest = CellBuilder.Begin();
            WriteEdge(tree as Tuple<string, object>, keyLength, serializer, dest);
            return dest.End();
        }

        private static int LabelShortLength(string src)
        {
            return 1 + src.Length + 1 + src.Length;
        }

        private static int LabelLongLength(string src, int keyLength)
        {
            return 1 + 1 + (int)Math.Ceiling(Math.Log(keyLength + 1,2)) + src.Length;
        }

        private static int LabelSameLength(int keyLength)
        {
            return 1 + 1 + 1 + (int)Math.Ceiling(Math.Log(keyLength + 1, 2));
        }

        private static bool IsSame(string src)
        {
            if (src.Length == 0 || src.Length == 1)
                return true;
            for (int i = 1; i < src.Length; i++)
                if (src[i] != src[0])
                    return false;
            return true;
        }
        private static string DetectLabelType(string src, int keyLength)
        {
            string kind = "short";
            int kindLength = LabelShortLength(src);
            int longLength = LabelLongLength(src, keyLength);
            if (longLength < kindLength)
            {
                kindLength = longLength;
                kind = "long";
            }
            if (IsSame(src))
            {
                int sameLength = LabelSameLength(keyLength);
                if (sameLength < kindLength)
                {
                    kindLength = sameLength;
                    kind = "same";
                }
            }
            return kind;
        }

        private static CellBuilder WriteLabelShort(string src, CellBuilder to)
        {
            to.Store(0, 1);
            for (int i = 0; i < src.Length; i++)
                to.Store(1, 1);
            to.Store(0, 1);
            for (int i = 0; i < src.Length; i++)
                to.Store(src[i] == '1');
            return to;
        }

        private static CellBuilder WriteLabelLong(string src, int keyLength, CellBuilder to)
        {
            to.Store(1, 1);
            to.Store(0, 1);
            int length = (int)Math.Ceiling(Math.Log(keyLength + 1, 2));
            to.Store(src.Length, length);
            for (int i = 0; i < src.Length; i++)
                to.Store(src[i] == '1');
            return to;
        }

        private static void WriteLabelSame(bool value, int length, int keyLength, CellBuilder to)
        {
            to.Store(1, 1);
            to.Store(1, 1);
            to.Store(value);
            int lenLen = (int)Math.Ceiling(Math.Log(keyLength + 1, 2));
            to.Store(length, lenLen);
        }
        private static void WriteLabel(string src, int keyLength, CellBuilder to)
        {
            string type = DetectLabelType(src, keyLength);
            switch (DetectLabelType(src, keyLength))
            {
                case "short":
                    WriteLabelShort(src, to);
                    break;
                case "long":
                    WriteLabelLong(src, keyLength, to);
                    break;
                case "same":
                    WriteLabelSame(src[0] == '1', src.Length, keyLength, to);
                    break;

            }
        }

        private static void WriteNode(Tuple<string, object, object> src, int keyLength, ParamHandler<object, CellBuilder>  serializer, CellBuilder to)
        {
            if (src.Item1 == "leaf")
            {
                serializer(src.Item2, to);
            }
            else if (src.Item1 == "fork")
            {
                CellBuilder leftCell = CellBuilder.Begin();
                CellBuilder rightCell = CellBuilder.Begin();
                WriteEdge(src.Item2 as Tuple<string, object>, keyLength - 1, serializer, leftCell);
                WriteEdge(src.Item3 as Tuple<string, object>, keyLength - 1, serializer, rightCell);
                to.StoreRef(leftCell.End());
                to.StoreRef(rightCell.End());
            }
        }
        private static void WriteEdge(Tuple<string, object> src, int keyLength, ParamHandler<object, CellBuilder> serializer, CellBuilder to)
        {
            WriteLabel(src.Item1, keyLength, to);
            WriteNode(src.Item2 as Tuple<string, object, object>, keyLength - src.Item1.Length, serializer, to);
        }

        private static string FindCommonPrefix(string[] src)
        {
            if (src.Length == 0)
                return "";
            if (src.Length == 1)
                return src[0];

            Array.Sort(src);
            int size = 0;
            for (int i = 0; i < src[0].Length; i++)
            {
                if (src[0][i] != src[src.Length - 1][i])
                    break;
                size++;
            }
            return src[0].Substring(0, size);
        }

        private static Dict RemovePrefixMap(Dict src, int length)
        {
            if (length == 0)
                return src;
            else
            {
                Dict res = new Dict();
                foreach(string k in src.Keys)
                {
                    object d = src[k];
                    res.Add(k.Substring(length), d);
                }
                return res;
            }
        }

        private static (object, object) ForkMap(Dict  src)
        {
            if (src.Count == 0)
                throw new Exception("Internal inconsistency");

            Dict left = new Dict();
            Dict right = new Dict();

            foreach (string k in src.Keys)
            {
                object d = src[k];
                if (k.StartsWith("0"))
                    left.Add(k.Substring(1), d);
                else
                    right.Add(k.Substring(1), d);
            }
            if (left.Count == 0)
                throw new Exception("Internal inconsistency. Left emtpy.");
            if (right.Count == 0)
                throw new Exception("Internal inconsistency. Right emtpy.");
            return (left, right);
        }

        private static object BuildEdge(Dict src)
        {
            string label = FindCommonPrefix(src.GetKeys());
            Tuple<string, object, object> tuple = buildNode(RemovePrefixMap(src, label.Length));
            return Tuple.Create(label, (object)tuple);
        }

        private static Tuple<string, object, object> buildNode(Dict src)
        {
            if (src.Count == 0)
                throw new Exception("Internal inconsistency");
            if (src.Count == 1)
                return Tuple.Create("leaf", src[0], null as object);
            var(left, right) = ForkMap(src);
            object l = BuildEdge(left as Dict);
            object r = BuildEdge(right as Dict);
            return Tuple.Create("fork", l, r);
        }

        private static string Pad(string src, int size)
        {
            while (src.Length < size)
                src = "0" + src;
            return src;
        }

        private static object BuildTree(Dict src, int keyLength)
        {
            Dict converted = new Dict();
            foreach (string k in src.Keys)
            {
                long index = long.Parse(k);
                string padded = Pad(Convert.ToString(index, 2), keyLength);
                converted.Add(padded, src[k]);
            }
            return BuildEdge(converted);
        }
    }
}
