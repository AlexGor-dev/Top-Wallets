using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class AdapterItem : Container, IFocusedComponent
    {
        protected AdapterItem(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.source = data["source"] as IAdapterSource;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["source"] = this.source;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public AdapterItem(IAdapterSource source)
        {
            this.source = source;
            this.Padding.Set(4);
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.Adapter.Connected += Adapter_Connected;
            this.Adapter.Disconnected += Adapter_Disconnected;
        }

        protected override void OnDisposed()
        {
            this.Adapter.Connected -= Adapter_Connected;
            this.Adapter.Disconnected -= Adapter_Disconnected;
            base.OnDisposed();
        }

        protected virtual void OnConnected()
        {

        }

        protected virtual void OnDisconnected()
        {

        }

        private void Adapter_Connected(object sender)
        {
            this.OnConnected();
        }

        private void Adapter_Disconnected(object sender)
        {
            this.OnDisconnected();
        }

        private IAdapterSource source;
        public WalletAdapter Adapter => source.Adapter;

        private readonly Rect dispRect = new Rect();

        protected override void OnSizeChanged()
        {
            GetDisplayRectangle(dispRect);
            base.OnSizeChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            g.Smoosh(() =>
            {
                int color = Theme.unselectedItemBackColor;
                if (this.Selected)
                    color = Theme.selectedItemBackColor;
                g.FillRoundRect(dispRect, 10, color);
            });

        }

        void IFocusedComponent.OnFocusedChanged()
        {
        }
    }
}
