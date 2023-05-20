using System;
using System.Text;

namespace Complex.Ton
{
    public class ContractController
    {
        //public static int CELL_MAX_SIZE_BYTES = (int)Math.Floor((1023.0 - 8.0) / 8.0);
        public const int CELL_MAX_SIZE_BYTES = 127;
        public static Cell GetStateInit(Cell code, Cell data)
        {
            return CellBuilder.Begin()
                .Store(0, 2)
                .Store(1, 1)
                .Store(1, 1)
                .Store(0, 1)
                .StoreRef(code)
                .StoreRef(data)
                .End();
        }

        public static string GetAddress(int workchain, Cell stateInit, bool bounceable)
        {
            return Address.ToString(workchain, stateInit.GetHash(), bounceable, false);
        }

        public static string GetAddress(int workchain, Cell stateInit)
        {
            return GetAddress(workchain, stateInit, true);
        }

        public static string GetAddress(int workchain, Cell code, Cell data, bool bounceable)
        {
            using (Cell stateInit = GetStateInit(code, data))
                return GetAddress(workchain, stateInit, bounceable);
        }

    }
}
