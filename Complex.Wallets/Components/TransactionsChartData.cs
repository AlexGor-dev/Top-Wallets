using System;
using Complex.Trader;
using Complex.Collections;
using Complex.Controls;

namespace Complex.Wallets
{
    public class TransactionsChartData : UpdateElement, IInstrumentChartData, ITimeSeries
    {
        public TransactionsChartData(Wallet wallet)
        {
            this.wallet = wallet;
            this.wallet.TransactionsNew += Wallet_TransactionsNew;
            this.wallet.TransactionsOld += Wallet_TransactionsOld;
            if (this.wallet.Transactions.Count > 0)
                this.LoadBars();
        }

        protected override void OnDisposed()
        {
            this.wallet.TransactionsNew -= Wallet_TransactionsNew;
            this.wallet.TransactionsOld -= Wallet_TransactionsOld;
            base.OnDisposed();
        }

        private void Wallet_TransactionsOld(object sender, ITransactionBase[] transactions)
        {
            this.LoadBars(transactions);
       }

        private void Wallet_TransactionsNew(object sender, ITransactionBase[] transactions)
        {
            if (this.Count == 0)
            {
                this.LoadBars(transactions);
                return;
            }
            this.AddBars(transactions);
        }

        public event Handler InstrumentChanged;
        public event Handler<int> Changed;
        public event Handler CollectionChanged;

        private Wallet wallet;

        public string ID => wallet.Symbol + "_" + this.interval;

        private Array<Bar> bars = new Array<Bar>();

        public Bar this[int index] => bars[index];

        public bool IsInstrumentChaned => false;

        private bool intervalChaned = false;
        public bool IsIntervalChaned => intervalChaned;

        private MarketData lastTick = null;
        public MarketData LastTick => lastTick;

        public IBarSeries Bars => this;

        public Instrument Instrument { get; set; }

        private Interval interval = Interval.M1;
        public Interval Interval => interval;

        public DateTime LastTickTime => lastTick.time;

        public int Count => bars.Count;

        DateTime IDataSeries<DateTime>.this[int index] 
        {
            get
            {
                Bar bar = this.bars[index];
                if (bar != null)
                    return bar.time;
                return DateTime.MinValue;
            }
            set { }
        }

        public int GetIndex(DateTime time)
        {
            return QuickSort.GetIndex(this, time);
        }

        private void OnCollectionChanged()
        {
            Events.InvokeInverse(this.CollectionChanged, this);
        }

        private void OnChanged(int index)
        {
            Events.InvokeInverse(this.Changed, this, index);
        }

        public void LoadBars()
        {
            this.LoadBars(this.wallet.Transactions.ToArray());
        }

        private void LoadBars(ITransactionBase[] transactions)
        {
            if (transactions.Length > 0)
            {
                if (!this.Updating)
                {
                    this.BeginUpdate();
                    Application.Run(() =>
                    {
                        Array<Bar> data = CalcBars(transactions);
                        if (this.bars == null)
                        {
                            this.bars = data;
                            this.OnCollectionChanged();
                        }
                        else if (data.Count > 0)
                        {
                            Bar lbar = data.Last;
                            for (int i = 0; i < bars.Count; i++)
                            {
                                Bar bar = bars[i];
                                if (bar.time > lbar.time)
                                    data.Add(new Bar(bar.time, bar.open + lbar.close, bar.high + lbar.close, bar.low + lbar.close, bar.close + lbar.close, bar.volume));
                            }
                            bars = data;
                            this.OnCollectionChanged();
                        }
                    }, () =>
                    {
                        this.EndUpdate();
                    });
                }
            }

        }

        private Array<Bar> CalcBars(ITransactionBase[] transactions)
        {
            Array<Bar> arr = new Array<Bar>();
            if (transactions.Length > 0)
            {
                BarsCalculator calculator = new BarsCalculator(interval, new Bar(transactions.Last().Time, 0, 0));
                arr.Add(calculator.LastBar);
                for(int i = transactions.Length - 1; i >= 0; i--)
                {
                    ITransactionBase tr = transactions[i];
                    decimal amount = tr.GetAmount(this.wallet.Symbol);
                    decimal price = calculator.LastBar.close + amount;
                    MarketData md = new MarketData(tr.Time, price, 0, amount < 0 ? MarketDataType.Sell : MarketDataType.Buy);
                    var (bar, add) = calculator.Add(md);
                    if (add)
                        arr.Add(bar);
                }
            }
            return arr;
        }

        private void AddBars(ITransactionBase[] transactions)
        {
            if (transactions.Length > 0)
            {
                Application.Run(() =>
                {
                    Bar last = this.bars.Last;
                    if (last == null)
                        last = new Bar(transactions.Last().Time, 0, 0);
                    BarsCalculator calculator = new BarsCalculator(interval, last);
                    for (int i = transactions.Length - 1; i >= 0; i--)
                    {
                        ITransactionBase tr = transactions[i];
                        decimal amount = tr.GetAmount(this.wallet.Symbol);
                        decimal price = calculator.LastBar.close + amount;
                        MarketData md = new MarketData(tr.Time, price, 0, amount < 0 ? MarketDataType.Sell : MarketDataType.Buy);
                        var (bar, add) = calculator.Add(md);
                        if (add)
                            this.bars.Add(bar);
                        this.lastTick = md;
                        this.OnChanged(this.bars.Count - 1);
                    }
                });
            }
        }

        public bool SubscribeBars()
        {
            return true;
        }
    }
}
