using System;
using Complex.Controls;
using Complex.Trader;
using Complex.Collections;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public static class Controller
    {
        static Controller()
        {
            Application.Exited += () =>
            {
                Stop();
                extensions.Clear(true);
                markets.Clear(true);
            };
            Application.Created += () => Start();
            timer = new ThreadTimer(10000);
            timer.Tick += (s, p) =>
            {
                timer.Stop();
                RefreshAdapters();
                timer.Start();
            };
        }

        private static ThreadTimer timer;

        private static IWalletsController walletsController;
        public static IWalletsController WalletsController
        {
            get => walletsController;
            set
            {
                if (walletsController == value) return;
                walletsController = value;
                Start();
            }
        }

        private static UniqueCollection<CoinMarket> markets = new UniqueCollection<CoinMarket>();

        private static UniqueCollection<WalletAdapter> adapters = new UniqueCollection<WalletAdapter>();
        public static IUniqueCollection<WalletAdapter> Adapters => adapters;

        private static UniqueCollection<WalletAdapterExtension> extensions = new UniqueCollection<WalletAdapterExtension>();
        public static UniqueCollection<WalletAdapterExtension> Extensions => extensions;


        public static WalletAdapter GetAdapter(WalletAdapterExtension extension)
        {
            lock (adapters)
            {
                WalletAdapter adapter = adapters[extension.ID];
                if (adapter == null)
                {
                    adapter = extension.CreateAdapter();
                    adapters.Add(adapter);
                }
                return adapter;
            }
        }

        public static WalletAdapter GetAdapter(string adapterID)
        {
            WalletAdapter adapter = null;
            lock (adapters)
            {
                adapter = adapters[adapterID];
                if (adapter == null)
                {
                    WalletAdapterExtension extension = extensions[adapterID];
                    if (extension != null)
                    {
                        adapter = extension.CreateAdapter();
                        adapters.Add(adapter);
                        if (Application.IsCreated)
                            adapter.Start();
                    }
                }
            }
            return adapter;
        }

        private static void Start()
        {
            if (walletsController != null && !timer.IsRunning && Application.IsCreated)
            {
                timer.Start();
                RefreshAdapters();
            }
        }

        public static void Stop()
        {
            timer.Stop();
            timer.Dispose();
            adapters.Clear(true);
        }
        public static bool IsAdapterConnected(string adapterID)
        {
            WalletAdapter adapter = adapters[adapterID];
            return adapter != null && adapter.IsConnected;
        }

        private static void RefreshAdapter(WalletAdapter adapter)
        {
            if (Application.IsCreated)
            {
                SingleThread.Run("Adapter" + adapter.ID, () =>
                {
                    if (adapter.Refresh())
                    {
                        foreach (Wallet wallet in WalletsData.Wallets)
                            if (wallet.Adapter == adapter && wallet.Adapter.IsConnected)
                                wallet.Update();
                    }
                });
            }
        }

        private static void RefreshAdapters()
        {
            lock (adapters)
                foreach (WalletAdapter adapter in adapters)
                    RefreshAdapter(adapter);
        }

        //public static DialogResult ShowSendForm(Wallet wallet, IBoundsElement component, MenuAlignment alignment, string address = "EQDgw-wlvhETZh5Ize8uEAAXgxl4jy_QksfwAhsdcXxFFj9V", double amount = 0.01, string comment = "0123")

        public static DialogResult ShowSendForm(Wallet wallet, IBoundsElement component, MenuAlignment alignment, string address = null, decimal amount = 0, string comment = null)
        {
            return new SendForm(wallet, address, amount, comment).Show(component, alignment);
        }

        public static DialogResult ShowReceiveForm(Wallet wallet, IBoundsElement component, MenuAlignment alignment)
        {
            return new ReceiveForm(wallet).Show(component, alignment);
        }

        public static void ExploreWallet(Wallet wallet)
        {
            walletsController.ExploreWallet(wallet);
        }

        public static void ShowMainWallet(Wallet wallet)
        {
            walletsController.ShowMainWallet(wallet);
        }

        public static void ShowAnyWallet(WalletAdapter adapter, string symbol, string address, EmptyHandler start = null, EmptyHandler end = null)
        {
            Wallet wt = WalletsData.GetAnyWallet(adapter.ID, address);
            if (wt != null)
            {
                if (wt.IsMain)
                    Controller.ShowMainWallet(wt);
                else
                    Controller.ExploreWallet(wt);
            }
            else
            {
                Events.Invoke(start);
                if (adapter.IsValidAddress(address))
                {
                    adapter.GetWallet(address, (w, e) =>
                    {
                        Application.Invoke(() =>
                        {
                            if (w != null)
                                Controller.ExploreWallet(w);
                            else
                                MessageView.Show(e);
                            Events.Invoke(end);
                        });
                    });
                }
                else
                {
                    MessageView.Show("invalidAddress");
                    Events.Invoke(end);
                }
            }
        }

        public static string GetKnownAddress(WalletAdapter adapter, string address)
        {
            string res = adapter.GetKnownAddress(address);
            return res == null ? address : res;
        }

        public static void AddCoinImage(string imageID, int size, ThemeColor color, string centerImageID)
        {
            IImage image = Images.Get(imageID);
            if (image == null)
                Images.Add(imageID, new CoinImage(size, size, color, centerImageID));
        }

        public static CoinMarket GetCoinMarket(string symbol)
        {
            lock (markets)
            {
                CoinMarket market = markets[symbol];
                if (market == null)
                {
                    market = new CoinMarket(symbol);
                    markets.Add(market);
                }
                return market;
            }
        }
    }

}
