using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class AdapterWaitLabel : TextComponent, ITimerHandler
    {
        public AdapterWaitLabel(WalletAdapter adapter)
            :base(adapter.FullName)
        {
            this.adapter = adapter;
            this.Padding.Set(4);

            this.adapter.BeginUpdated += Adapter_BeginUpdated;
            this.adapter.EndUpdated += Adapter_EndUpdated;
            this.adapter.Connected += Adapter_Connected;
            this.adapter.Disconnected += Adapter_Disconnected;

            if (!adapter.IsConnected || this.adapter.Updating)
                this.Start();
        }

        protected override void OnDisposed()
        {
            this.adapter.Connected -= Adapter_Connected;
            this.adapter.Disconnected -= Adapter_Disconnected;
            this.adapter.BeginUpdated -= Adapter_BeginUpdated;
            this.adapter.EndUpdated -= Adapter_EndUpdated;
            base.OnDisposed();
        }

        private void Start()
        {
            TimerHandler.Instance.Remove(this);
            animationValue = 0;
            TimerHandler.Instance.Add(this);
        }

        private void Stop()
        {
            TimerHandler.Instance.Remove(this);
            this.Invalidate();
        }

        private void Adapter_Connected(object sender)
        {
            this.Stop();
        }

        private void Adapter_Disconnected(object sender)
        {
            this.Start();
        }

        private void Adapter_BeginUpdated(object sender)
        {
            if(adapter.IsConnected)
                this.Start();
        }

        private void Adapter_EndUpdated(object sender)
        {
            Timer.Delay(1000, () =>
            {
                if (adapter.IsConnected)
                    this.Stop();
            });
        }


        private WalletAdapter adapter;
        public WalletAdapter Adapter => adapter;

        private float animationValue;

        private readonly Rect adapterRect = new Rect();
        private readonly Rect adapterWaitRect = new Rect();

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();
            adapterRect.Set(textBounds);
            adapterRect.Inflate(Padding.left, Padding.top);
            adapterRect.Align(this.MeasuredWidth, this.MeasuredHeight, this.Alignment);
            adapterWaitRect.Set(adapterRect);
            adapterWaitRect.Inflate(0, (adapterRect.width - adapterRect.height) / 2);
        }

        void ITimerHandler.OnTick()
        {
            animationValue += 3f;
            this.Invalidate();
        }

        protected override void OnDrawBack(Graphics g)
        {
            if (this.Alpha > 0)
            {
                g.Smoosh(() =>
                {
                    int s = g.Save();
                    g.SetClip(adapterRect, adapterRect.height / 2);
                    g.FillRoundRect(adapterRect, adapterRect.height / 2, Theme.back6);
                    if (adapter.IsConnected)
                        ProgressBar.DrawGlow(g, adapterRect, false, animationValue, adapterRect.height / 2, Color.Argb(100, adapter.ThemeColor), this.Alpha);
                    else
                        WaitCircle.FillWait(g, adapterWaitRect, Theme.back4, animationValue / 2);
                    g.Restore(s);
                });
            }
        }
    }
}
