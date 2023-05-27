using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class TokensPanel : WalletBasePanel, ITimerHandler
    {
        protected TokensPanel(IData data)
            : base(data)
        {

        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TokensPanel(Wallet wallet)
            : base(wallet)
        {
            this.Init();
        }

        private void Init()
        {
            this.listView = new InsertedAnyView();
            this.listView.ScrollVisible = true;
            this.listView.ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
            this.listView.InsertAnimationMode = AnimationComponentMode.RotateTopAxis;
            this.listView.InsertAnimator.Harmonic = true;
            this.listView.Inflate.Set(0, 2);
            this.listView.Dock = DockStyle.Fill;
            this.listView.VScrollStep = 10;
            this.Add(this.listView);

            this.waitEffect = new GridWaitEffect(this);

        }

        private InsertedAnyView listView;
        private Dict<Component> items = new Dict<Component>();
        private bool allTokensLoaded = false;
        private GridWaitEffect waitEffect;
        public GridWaitEffect WaitEffect => waitEffect;

        private float animationValue;
        private bool waitMode = false;
        private static readonly Font font = Font.Create(20, FontStyle.Bold);

        private readonly Rect clientRect = new Rect();
        private readonly Rect waitRect = new Rect();

        protected override void OnAdapterEndUpdated()
        {
            //if (!this.allTokensLoaded && this.tokens.Count == 0)
            //    this.GetTokens();
            //else
            //    this.listView.Invalidate();
        }

        protected override void OnWalletChanged()
        {
            allTokensLoaded = false;
            GetTokens();
        }

        protected override void OnConnectionChanged()
        {
            if (this.Wallet.Adapter.IsConnected)
                GetTokens();
        }
        private void Adapter_Connected(object sender)
        {
            GetTokens();
        }

        protected override void OnCreated()
        {
            if (Wallet.Tokens.Count > 0)
            {
                this.AddTokens(Wallet.Tokens.ToArray());
            }
            else
            {
                if (Wallet.State != WalletState.None)
                    this.StartWait();
                if (this.Wallet.Adapter.IsConnected)
                    Adapter_Connected(null);
            }
            base.OnCreated();
        }

        private void GetTokens()
        {
            if (Wallet.State == WalletState.None && !allTokensLoaded)
                allTokensLoaded = true;
            if (!allTokensLoaded && !this.listView.Updating && Wallet.Adapter.IsConnected)
            {
                this.waitEffect.Start();
                this.listView.BeginUpdate();
                Application.Run(() =>
                {
                    Wallet.GetTokens((ts, e) =>
                   {
                       if (e != null)
                           MessageView.Show(e);
                       if (ts != null)
                       {
                           if (ts.Length > 0)
                               this.AddTokens(ts);
                       }
                       else
                       {
                           this.allTokensLoaded = true;
                       }
                       Application.Invoke(() =>
                       {
                           if (this.items.Count > 0 || allTokensLoaded)
                               this.StopWait();
                           this.waitEffect.Stop();
                           this.listView.EndUpdate();
                           this.Invalidate();
                       });
                   });
                });
            }

        }

        private void AddTokens(ITokenInfo[] tokens)
        {
            bool added = this.items.Count == 0;
            Array<Component> components = new Array<Component>();
            foreach (ITokenInfo token in tokens)
            {
                Component cp = this.items[token.ID];
                if (cp != null)
                {
                    if (cp is ITokenInfoSource ts)
                        ts.TokenInfo = token;
                }
                else
                {
                    cp = this.Wallet.CreateTockenItem(token, this.waitEffect);
                    if (added)
                        this.listView.Add(cp);
                    else
                        components.Add(cp);
                    this.items.Add(token.ID, cp);
                }
            }
            if (components.Count > 0)
                this.listView.InsertAnimation(0, components);
        }

        private void StartWait()
        {
            this.waitMode = true;
            this.animationValue = 0;
            TimerHandler.Instance.Add(this);
        }

        private void StopWait()
        {
            this.waitMode = false;
            TimerHandler.Instance.Remove(this);
        }

        void ITimerHandler.OnTick()
        {
            this.animationValue -= 2f;
            this.Invalidate();
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
            {
                WaitDna.DrawWait(g, waitRect, Wallet.ThemeColor, Theme.blue2, animationValue);
            }
            else if (items.Count == 0 && allTokensLoaded)
            {
                g.DrawTextExclude(Language.Current["noTokens"], font, clientRect, ParentBackColor, -15, 25, ContentAlignment.Center);
            }
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            if (this.waitEffect.IsRunning)
                this.waitEffect.Draw(g);
        }

    }
}
