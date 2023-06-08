using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Trader;
using Complex.Themes;

namespace Complex.Wallets
{
    public class WalletLiteItem : AdapterItem, IWalletSource
    {
        protected WalletLiteItem(IData data)
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

        public WalletLiteItem(Wallet wallet)
            : base(wallet)
        {
            this.wallet = wallet;
            this.Padding.Set(4);
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.MinHeight = 70;
            this.Inflate.height = 4;

            this.nameCaption = new Caption(wallet.Name);
            this.nameCaption.TextChanged += (s) => { this.nameCaption.Invalidate(); };
            this.nameCaption.Padding.Set(30, 0, 30, 0);
            this.nameCaption.Dock = DockStyle.Top;
            this.Add(this.nameCaption);

            //this.Add(new Dummy(DockStyle.Top, 0, 6));

            topContainer = new Container();
            topContainer.Dock = DockStyle.Top;
            topContainer.MinHeight = 50;

            adapterLabel = new AdapterCurrencyLabel(this.wallet);
            adapterLabel.Dock = DockStyle.Left;
            topContainer.Add(adapterLabel);


            this.Add(topContainer);

            this.clickEffect = new ClickEffect(this, ClickEffectMode.Quad);

            this.wallet.Changed += Wallet_Changed;

        }

        protected override void OnDisposed()
        {
            this.wallet.Changed -= Wallet_Changed;
            base.OnDisposed();
        }

        protected override void OnConnected()
        {
            this.Invalidate();
            base.OnConnected();
        }

        protected override void OnDisconnected()
        {
            this.Invalidate();
            base.OnDisconnected();
        }

        protected virtual void OnWalletChanged()
        {

        }

        private void Wallet_Changed(object sender)
        {
            Application.Invoke(() =>
            {
                this.OnWalletChanged();
                adapterLabel.Update();
                this.nameCaption.Text = this.wallet.Name;
                topContainer.Layout();
            });
        }

        private Wallet wallet;
        public Wallet Wallet => wallet;

        protected Caption nameCaption;
        protected Container topContainer;

        private AdapterCurrencyLabel adapterLabel;
        private ClickEffect clickEffect;

        public override void GetDisplayRectangle(Rect rect)
        {
            base.GetDisplayRectangle(rect);
            if (this.wallet is IToken token && token.Parent != null)
                rect.OffsetResize(20, 0);
        }

        protected override void OnMouseUp(MouseEvent e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.wallet.CreateMenu(this, (menu) =>
                {
                    if (menu != null)
                    {
                        e.Handled = true;
                        Point point = new Point(e.X, e.Y);
                        this.PointToScreen(point);
                        menu.Show(point.x, point.y);
                    }
                });
            }
            base.OnMouseUp(e);
        }

        protected override void OnMeasure(float widthMeasure, float heightMeasure)
        {
            this.nameCaption.SetMeasured(200, 26);
            base.OnMeasure(widthMeasure, heightMeasure);
        }

    }
}
