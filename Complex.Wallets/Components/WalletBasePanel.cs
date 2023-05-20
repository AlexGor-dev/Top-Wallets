using System;
using Complex.Controls;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class WalletBasePanel : Container, IWalletSource
    {
        protected WalletBasePanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.wallet = data["wallet"] as Wallet;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["wallet"] = this.wallet;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletBasePanel(Wallet wallet)
        {
            this.wallet = wallet;
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {

            this.wallet.Changed += Wallet_Changed;
            this.wallet.Adapter.Connected += Adapter_Connected;
            this.wallet.Adapter.Disconnected += Adapter_Connected;
            this.wallet.Adapter.EndUpdated += Adapter_EndUpdated;

        }

        protected override void OnDisposed()
        {
            this.wallet.Adapter.EndUpdated -= Adapter_EndUpdated;
            this.wallet.Changed -= Wallet_Changed;
            this.wallet.Adapter.Connected -= Adapter_Connected;
            this.wallet.Adapter.Disconnected -= Adapter_Connected;
            base.OnDisposed();
        }

        private void Adapter_EndUpdated(object sender)
        {
            this.OnAdapterEndUpdated();
        }

        private void Wallet_Changed(object sender)
        {
            this.OnWalletChanged();
        }

        private void Adapter_Connected(object sender)
        {
            this.OnConnectionChanged();
        }

        private Wallet wallet;
        public Wallet Wallet => wallet;

        protected virtual void OnConnectionChanged()
        {

        }

        protected virtual void OnWalletChanged()
        {

        }

        protected virtual void OnAdapterEndUpdated()
        {

        }

    }
}
