using System;
using Complex.Collections;

namespace Complex.Wallets
{
    public class WalletsData : Disposable
    {
        private static WalletsData current;

        public static bool IsCurrentExist => current != null;
        protected WalletsData(IData data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.wallets = data["wallets"] as UniqueCollection<Wallet>;
            this.explorers = data["explorers"] as UniqueCollection<Explorer>;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["wallets"] = this.wallets;
            data["explorers"] = this.explorers;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletsData()
        {
            this.wallets = new UniqueCollection<Wallet>();
            this.explorers = new UniqueCollection<Explorer>();

            this.Init();
        }

        private void Init()
        {
            current = this;
        }

        protected override void OnDisposed()
        {
            if (current == this)
                current = null;
            this.wallets.Clear(true);
            this.explorers.Clear(true);
            base.OnDisposed();
        }

        private static UniqueCollection<Explorer> fakeExplorers = new UniqueCollection<Explorer>();

        private UniqueCollection<Explorer> explorers;
        public static UniqueCollection<Explorer> Explorers
        {
            get
            {
                WalletsData data = current;
                if (data != null)
                    return data.explorers;
                lock (fakeExplorers)
                    fakeExplorers.Clear();
                return fakeExplorers;
            }
        }

        private static UniqueCollection<Wallet> fakeWallets = new UniqueCollection<Wallet>();
        private UniqueCollection<Wallet> wallets;
        public static UniqueCollection<Wallet> Wallets
        {
            get
            {
                WalletsData data = current;
                if (data != null)
                    return data.wallets;
                lock (fakeWallets)
                    fakeWallets.Clear();
                return fakeWallets;
            }
        }

        public static Wallet GetWallet(string adapterID, string address, bool isMain)
        {
            return current?.wallets[Wallet.GetID(adapterID, address, isMain)];
        }

        public static Wallet GetWallet(WalletAdapter adapter, string address, bool isMain)
        {
            return current?.wallets[Wallet.GetID(adapter, address, isMain)];
        }

        public static Wallet GetAnyWallet(string adapterID, string address)
        {
            Wallet wallet = GetWallet(adapterID, address, true);
            if (wallet != null)
                return wallet;
            return GetWallet(adapterID, address, false);
        }
    }
}
