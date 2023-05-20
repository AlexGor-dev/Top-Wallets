using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Drawing;
using Complex.Themes;
using Complex.Animations;
using Complex.Trader;

namespace Complex.Wallets
{
    public class TransactionPanel : WalletBasePanel, ITimerHandler
    {
        protected TransactionPanel(IData data)
            : base(data)
        {

        }
        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TransactionPanel(Wallet wallet)
            :base(wallet)
        {
            this.SaveComponents = false;
            this.Init();
        }

        private void Init()
        {
            this.listView = new ListView();
            this.listView.Dock = DockStyle.Fill;
            this.Add(this.listView);

            this.detailPanel = new TransactionDetailPanel(this.Wallet);
            this.detailPanel.AnimationDock = DockStyle.Bottom;
            this.detailPanel.Hiding += (s) =>
            {
                ExpandButton eb = this.detailPanel.Tag as ExpandButton;
                if (eb != null)
                    eb.Checked = false;
            };
            this.Add(this.detailPanel);

            MainSettings.Current.General.RelativeTimeChanged += General_RelativeTimeChanged;
            this.waitEffect = new GridWaitEffect(this);
        }


        protected override void OnDisposed()
        {
            MainSettings.Current.General.RelativeTimeChanged -= General_RelativeTimeChanged;
            base.OnDisposed();
        }

        private void General_RelativeTimeChanged(object sender)
        {
            this.listView.Invalidate();
        }

        protected override void OnAdapterEndUpdated()
        {
            if (!this.allTransactionLoaded && this.transactions.Count == 0)
                this.GetTransactions(null);
            else
                this.listView.Invalidate();
        }

        protected override void OnWalletChanged()
        {
            allTransactionLoaded = false;
            GetTransactions(null);
        }

        protected override void OnConnectionChanged()
        {
            if(this.Wallet.Adapter.IsConnected)
                GetTransactions(null);
        }
        private void Adapter_Connected(object sender)
        {
            GetTransactions(null);
        }

        protected override void OnCreated()
        {
            if (Wallet.Transactions.Count > 0)
            {
                this.AddTransactions(Wallet.Transactions.ToArray());
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

        private GridWaitEffect waitEffect;
        public GridWaitEffect WaitEffect => waitEffect;

        private Collection<ITransactionBase> transactions = new Collection<ITransactionBase>();
        private TransactionDetailPanel detailPanel;
        private ListView listView;

        private static readonly Font font = Font.Create(20, FontStyle.Bold);

        private readonly Rect clientRect = new Rect();
        private readonly Rect waitRect = new Rect();


        private float animationValue;
        private bool waitMode = false;
        private bool allTransactionLoaded = false;


        private void GetTransactions()
        {
            GetTransactions(this.transactions.Last);
        }

        private void ClearTransactions()
        {
            this.listView.Components.Clear(true);
            this.transactions.Clear();
            this.listView.Restart();
            this.allTransactionLoaded = false;
        }

        private void GetTransactions(ITransactionBase last)
        {
            if (Wallet.State == WalletState.None && !allTransactionLoaded)
                allTransactionLoaded = true;
            if (!allTransactionLoaded && !this.listView.Updating && Wallet.Adapter.IsConnected)
            {
                this.waitEffect.Start();
                this.listView.BeginUpdate();
                Application.Run(() =>
                {
                    Wallet.GetTransactions(last, 16, (ts, e) =>
                    {
                        if (e != null)
                            MessageView.Show(e);
                        if (ts != null)
                        {
                            if (ts.Length > 0)
                            {
                                if (last == null && this.transactions.Count > 0)
                                    this.InsertTransactions(ts);
                                else
                                    this.AddTransactions(ts);
                            }
                        }
                        else
                        {
                            this.allTransactionLoaded = true;
                        }
                        Application.Invoke(() =>
                        {
                            if (transactions.Count > 0 || allTransactionLoaded)
                                this.StopWait();
                            //this.listView.Relayout();
                            this.waitEffect.Stop();
                            this.listView.EndUpdate();
                            this.Invalidate();
                        });
                    });
                });
            }
        }

        private void AddTransactions(ITransactionBase[] ts)
        {
            DateTime time = DateTime.MaxValue;
            ITransactionBase last = this.transactions.Last;
            if (last != null)
                time = last.Time.ToLocalTime();
            foreach (ITransactionBase transaction in ts)
            {
                DateTime ttime = transaction.Time.ToLocalTime();
                if (time.Month != ttime.Month || time.Year != ttime.Year)
                {
                    time = ttime;
                    this.listView.Add(new TimeItem(time));
                }
                this.transactions.Add(transaction);
                Component transactionItem = this.Wallet.CreateTransactionItem(transaction, this.waitEffect);
                this.listView.Add(transactionItem);
            }
            this.Measured = false;
        }

        private void InsertTransactions(ITransactionBase[] ts)
        {
            ITransactionBase first = this.transactions.First;
            Array<Array<ITransactionBase>> insertTransactions = new Array<Array<ITransactionBase>>();
            Array<ITransactionBase> groupTransactions = new Array<ITransactionBase>();
            insertTransactions.Add(groupTransactions);

            DateTime time = ts[0].Time.ToLocalTime();
            int index = 0;
            foreach (ITransactionBase transaction in ts)
            {
                if (first != null && transaction.CompareTo(first) <= 0)
                    break;
                this.transactions.Insert(index, transaction);
                index++;
                DateTime ttime = transaction.Time.ToLocalTime();
                if (time.Month == ttime.Month && time.Year == ttime.Year)
                {
                    groupTransactions.Add(transaction);
                }
                else
                {
                    time = ttime;
                    groupTransactions = new Array<ITransactionBase>();
                    insertTransactions.Add(groupTransactions);
                    groupTransactions.Add(transaction);
                }
            }

            bool visible = this.VisibleHierarchy;
            TimeItem timeItem = this.listView.Components[0] as TimeItem;
            int pos = 0;
            foreach (Array<ITransactionBase> trs in insertTransactions)
            {
                TimeItem currentTimeItem = null;
                Array<Component> components = new Array<Component>();
                foreach (ITransactionBase transaction in trs)
                {
                    DateTime ttime = transaction.Time.ToLocalTime();
                    if (currentTimeItem == null)
                    {
                        if (timeItem != null && ttime.Month == timeItem.time.Month && ttime.Year == timeItem.time.Year)
                        {
                            currentTimeItem = timeItem;
                            pos = this.listView.Components.IndexOf(timeItem) + 1;
                        }
                        else
                        {
                            currentTimeItem = new TimeItem(ttime);
                            components.Add(currentTimeItem);
                        }
                    }
                    components.Add(this.Wallet.CreateTransactionItem(transaction, this.waitEffect));
                }
                if (visible)
                {
                    this.listView.InsertAnimation(pos, components);
                    pos += components.Count;
                }
                else
                {
                    foreach (Component component in components)
                    {
                        this.listView.Insert(pos, component);
                        pos++;
                    }
                }
            }
            this.Measured = false;
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
            waitRect.Inflate(- 0.36f * clientRect.width, -0.36f * clientRect.height);
            base.OnSizeChanged();
        }


        protected override void OnDraw(Graphics g)
        {
            if (waitMode)
            {
                WaitDna.DrawWait(g, waitRect, Wallet.ThemeColor, Theme.blue2, animationValue);
            }
            else if (transactions.Count == 0 && allTransactionLoaded)
            {
                g.DrawTextExclude(Language.Current["noTransaction"], font, clientRect, ParentBackColor, -15, 25, ContentAlignment.Center);
            }
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

        private class ListView : InsertedAnyView
        {
            public ListView()
            {
                this.ScrollVisible = true;
                this.ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
                this.InsertAnimationMode = AnimationComponentMode.RotateTopAxis;
                this.InsertAnimator.Harmonic = true;
                this.Inflate.Set(0, 6);
            }

            public override float VScrollStep => 20;

            protected override void OnVScroll()
            {
                if (this.VScroll.IsEndScroll)
                {
                    (this.Parent as TransactionPanel).GetTransactions();
                }
                base.OnVScroll();
            }
        }

        private class TimeItem : TextComponent
        {
            public TimeItem(DateTime time)
            {
                this.time = time;
                this.Alignment = ContentAlignment.Center;
                this.Style = Theme.Get<CaptionStyle>();
                this.Font = Theme.font13Bold;
                this.Text = time.ToString("MMMM yyyy", Language.Current.Culture);
            }

            public readonly DateTime time;

            protected override void OnDrawBack(Graphics g)
            {
                this.Text = time.ToString("MMMM yyyy", Language.Current.Culture);
                base.OnDrawBack(g);
            }
        }

    }
}
