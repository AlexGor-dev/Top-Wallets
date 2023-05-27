using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Drawing;
using Complex.Themes;
using Complex.Navigation;

namespace Complex.Wallets
{
    public class NftsPanel : WalletBasePanel, ITimerHandler
    {
        protected NftsPanel(IData data)
            : base(data)
        {

        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public NftsPanel(Wallet wallet)
            : base(wallet)
        {
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.chessView = new ChessItemsView();
            this.chessView.Inflate.Set(4);
            this.chessView.Dock = DockStyle.Fill;
            this.chessView.Vertical = true;
            this.chessView.ColumnsCount = 6;

            this.chessView.VScrolled += (s)=>
            {
                if(this.chessView.VScroll.IsEndScroll(this.chessView.Inflate.height))
                    this.LoadNfts();
            };
            this.Add(this.chessView);

            this.waitEffect = new GridWaitEffect(this);

        }


        private GridWaitEffect waitEffect;
        private ChessItemsView chessView;
        private float animationValue;
        private bool waitMode = false;
        private readonly Rect clientRect = new Rect();
        private readonly Rect waitRect = new Rect();


        private int startIndex = 0;
        private int itemsCount = 16;

        protected override void OnCreated()
        {
            if (Wallet.State != WalletState.None)
                this.StartWait();
            if (this.Wallet.Adapter.IsConnected)
                this.LoadNfts();
            base.OnCreated();
        }

        protected override void OnConnectionChanged()
        {
            if (this.startIndex == 0 && this.Wallet.Adapter.IsConnected)
                this.LoadNfts();
            base.OnConnectionChanged();
        }

        private void StartWait()
        {
            waitMode = true;
            animationValue = 0;
            TimerHandler.Instance.Add(this);
        }

        private void StopWait()
        {
            waitMode = false;
            TimerHandler.Instance.Remove(this);
        }

        protected override void OnSizeChanged()
        {
            GetClientRectangle(clientRect);
            waitRect.Set(clientRect);
            waitRect.Inflate(-0.36f * clientRect.width, -0.36f * clientRect.height);
            base.OnSizeChanged();
        }

        protected override void OnDraw(Graphics g)
        {
            if (waitMode)
                WaitDna.DrawWait(g, waitRect, Wallet.ThemeColor, Theme.blue2, animationValue);
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (this.waitEffect.IsRunning)
                this.waitEffect.Draw(g);
        }
        void ITimerHandler.OnTick()
        {
            animationValue -= 2f;
            this.Invalidate();
        }

        private void LoadNfts()
        {
            if (this.chessView.Updating || this.itemsCount == 0)
                return;
            this.waitEffect.Start();
            this.chessView.BeginUpdate();
            if (this.startIndex == 0)
                this.itemsCount = Math.Max(16, this.chessView.VisibleColumns * (int)(this.chessView.Height / this.chessView.ColumnWidth) + 2);
            Wallet.GetNfts(this.startIndex, this.itemsCount, (ts, e) =>
            {
                Application.Invoke(() =>
                {
                    if(this.startIndex == 0)
                        this.StopWait();

                    if (e != null)
                    {
                        if (!string.IsNullOrEmpty(e))
                            MessageView.Show(e);
                        else
                            this.itemsCount = 0;
                    }
                    if (ts != null)
                    {
                        this.startIndex += ts.Length;
                        foreach (INftInfo nft in ts)
                            this.chessView.Add(this.Wallet.CreateNftItem(nft, waitEffect));
                    }
                    this.chessView.EndUpdate();
                    this.waitEffect.Stop();
                });
            });
        }
    }
}
