using System;
using Complex.Collections;

namespace Complex.Wallets
{
    public static class Incr
    {
        private static Hashtable<WalletAdapter, int> adapters = new Hashtable<WalletAdapter, int>();

        public static event ParamHandler<WalletAdapter> Removed;
        public static bool Contains(WalletAdapter adapter)
        {
            if (adapter != null)
                lock (adapters)
                    return adapters.ContainsKey(adapter);
            return false;
        }
        public static void Increment(WalletAdapter adapter)
        {
            if (adapter != null)
            {
                lock (adapters)
                {
                    int value = adapters[adapter];
                    value++;
                    adapters[adapter] = value;
                }
            }
        }

        public static void Decrement(WalletAdapter adapter)
        {
            if (adapter != null)
            {
                lock (adapters)
                {
                    int value = adapters[adapter];
                    value--;
                    if (value <= 0)
                    {
                        adapters.Remove(adapter);
                        Events.Invoke(Removed, adapter);

                    }
                    else
                        adapters[adapter] = value;
                }
            }
        }

    }
}
