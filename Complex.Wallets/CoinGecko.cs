using System;
using System.IO;
using Complex.Collections;
using Complex.Remote;
using Complex.Trader;
using Complex.Controls;

namespace Complex.Wallets
{
    public class CoinGecko : CoinsAdapter
    {
        private const string url = "https://api.coingecko.com/api/v3";
        private const string ohlc = "";
        private static readonly string exchange = "CoinGecko";

        protected CoinGecko(IData data)
            : base(data)
        {

        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public CoinGecko()
        {
            this.Init();
        }

        private void Init()
        {
            timer = new ThreadTimer(30000);
            timer.Tick += Timer_Tick;
            Application.Created += Application_Created;
        }


        protected override void OnDisposed()
        {
            Application.Created -= Application_Created;
            timer.Dispose();
            base.OnDisposed();
        }

        private void Application_Created()
        {
            Application.Run(() => Timer_Tick(null, null));
        }


        private static void GetQuotes(Subscribtion[] subscribtions)
        {
            const string format = "yyyy-MM-ddTHH:mm:ss.fffZ";
            string ul = null;
            try
            {
                if (subscribtions.Length > 0)
                {
                    Hashtable<string, Hashtable<string, Subscribtion>> hash = new Hashtable<string, Hashtable<string, Subscribtion>>();

                    foreach (Subscribtion subscribtion in subscribtions)
                    {
                        if ((subscribtion.type & MarketDataType.Quote) != 0)
                        {
                            Hashtable<string, Subscribtion> ss = hash[subscribtion.instrument.Currency];
                            if (ss == null)
                            {
                                ss = new Hashtable<string, Subscribtion>();
                                hash.Add(subscribtion.instrument.Currency, ss);
                            }
                            ss.Add(subscribtion.instrument.Symbol, subscribtion);
                        }
                    }

                    foreach (KeyValue<string, Hashtable<string, Subscribtion>> kv in hash.EnumKeyValue())
                    {
                        string coinsIDs = null;
                        foreach (Subscribtion subscribtion in kv.Value)
                            coinsIDs += subscribtion.info.GeckoSymbol + ",";
                        coinsIDs = coinsIDs.Substring(0, coinsIDs.Length - 1);
                        ul = url + "/coins/markets?vs_currency=" + kv.Key + "&ids=" + coinsIDs;
                        //string data = File.ReadAllText(@"E:\quores.json");
                        string data = Http.GetBrouser(ul);
                        JsonArray arr = Json.Parse2(data) as JsonArray;
                        if (arr != null)
                        {
                            foreach(JsonArray b in arr)
                            {
                                string id = b.GetString("symbol").ToUpper();
                                Subscribtion s = kv.Value[id];
                                Instrument instr = s.instrument;
                                DateTime time = DateTime.ParseExact(b.GetString("last_updated"), format, null);
                                instr.Send(time, b.GetDecimal("price_change_percentage_24h"), 0, MarketDataType.NetChange);
                                instr.Send(time, b.GetDecimal("high_24h"), 0, MarketDataType.High);
                                instr.Send(time, b.GetDecimal("low_24h"), 0, MarketDataType.Low);
                                instr.Send(time, b.GetDecimal("total_volume"), 0, MarketDataType.Volume);
                                instr.Send(time, b.GetDecimal("current_price"), 0, MarketDataType.LastPrice);
                                instr.Send(time, b.GetDecimal("current_price"), 0, MarketDataType.Tick);
                                instr.Send(time, 0, 0, MarketDataType.Quote);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageView.Show("CoinGecko GetQuotes" + Environment.NewLine + ul + Environment.NewLine + e.Message);
            }
        }

        private void Timer_Tick(object sender, object value)
        {
            if (this.subscribtions.Count > 0)
            {
                Subscribtion[] subs = subscribtions.ToArray();
                GetQuotes(subs);
            }
            this.timer.Start();
        }

        private ThreadTimer timer;

        private UniqueCollection<Subscribtion> subscribtions = new UniqueCollection<Subscribtion>();

        public override bool IsConnected => true;


        public static Array<Bar> GetBars(Instrument instrument, string days)
        {
            string ul = null;
            try
            {
                CryptoCoinInfo info = instrument.Tag as CryptoCoinInfo;
                ul = url + "/coins/" + info.GeckoSymbol + "/ohlc?vs_currency=" + instrument.Currency.ToLower() + "&days=" + days;
                string data = Http.Get(ul);
                JsonArray arr = Json.Parse(data) as JsonArray;
                if (arr != null && arr.Count > 0)
                {
                    Array<Bar> bars = new  Array<Bar>();
                    for (int i = 0; i < arr.Count; i++)
                    {
                        JsonArray b = arr.GetArray(i);
                        decimal close = b.GetDecimal(4);
                        if (i == arr.Count - 1 && instrument.LastPrice != 0)
                            close = instrument.LastPrice;
                        bars.Add(new Bar(Calendar.FromMilliseconds(b.GetLong(0)), b.GetDecimal(1), b.GetDecimal(2), b.GetDecimal(3), close, 0));
                    }
                    return bars;
                }
            }
            catch (Exception e)
            {
                MessageView.Show("CoinGecko GetBars" + Environment.NewLine + ul + Environment.NewLine + e.Message);
            }
            return null;
        }

        public static Array<Bar> GetBars2(Instrument instrument, Interval interval, string days)
        {
            string ul = null;
            try
            {
                CryptoCoinInfo info = instrument.Tag as CryptoCoinInfo;

                ul = MainSettings.Current.Remote.CoinGecko.BarsUrl + info.GeckoID + "/" + instrument.Currency.ToLower() + "/" + days + ".json";
                string data = Http.GetBrouser(ul);
                //string data = File.ReadAllText("e:\\geckoData.json");
                JsonArray arr = Json.Parse(data) as JsonArray;
                if (arr != null && arr.Count > 0)
                {
                    BarsCalculator calculator = new BarsCalculator(interval);

                    Array<Bar> bars = new Array<Bar>();
                    JsonArray stats = arr.GetArray("stats");
                    JsonArray total_volumes = arr.GetArray("total_volumes");
                    for (int i = 0; i < stats.Count; i++)
                    {
                        JsonArray priceArr = stats[i] as JsonArray;
                        JsonArray volumeArr = total_volumes[i] as JsonArray;
                        DateTime time = Calendar.FromMilliseconds(priceArr.GetLong(0));
                        decimal price = priceArr.GetDecimal(1);
                        decimal volume = 0;
                        if(volumeArr != null)
                            volume = volumeArr.GetDecimal(1);
                        MarketData md = new MarketData(time, price, volume, MarketDataType.Tick);
                        var (bar, addNew) = calculator.Add(md);
                        if (addNew)
                            bars.Add(bar);
                    }
                    Bar last = bars.Last;
                    if (last != null && instrument.LastPrice != 0)
                        last.Add(instrument.LastPrice, 0);
                    return bars;
                }
            }
            catch (Exception e)
            {
                MessageView.Show("CoinGecko GetBars2" + Environment.NewLine + ul + Environment.NewLine + e.Message);
            }
            return null;
        }

        public override void Connect()
        {
            this.OnConnected();
        }

        public override void Disconnect()
        {
            this.OnDisconnected(null);
        }

        public override Array<Bar> GetBars(Instrument instrument, Interval interval, DateTime from, DateTime to)
        {
            string days = "";
            switch (interval)
            {
                case Interval.M30:
                    days = "1";
                    break;
                case Interval.H2:
                    days = "7";
                    break;
                case Interval.H4:
                    days = "14";
                    break;
                case Interval.H8:
                    days = "30";
                    break;
                case Interval.D1:
                    days = "90";
                    break;
                case Interval.D2:
                    days = "180";
                    break;
                case Interval.D4:
                    days = "365";
                    break;
                case Interval.W1:
                    days = "max";
                    break;
            }
            bool mode2 = MainSettings.Current.Remote.CoinGecko.UseBarsUrl;
            if (mode2)
            {
                if (days == "1")
                    days = "24_hours";
                else if (days != "max")
                    days += "_days";
                return GetBars2(instrument, interval, days);
            }

            return GetBars(instrument, days);
        }

        public override Array<Tick> GetTicks(Instrument instrument, DateTime from, DateTime to)
        {
            return null;
        }

        public override Array<Instrument> LoadInstruments()
        {
            return null;
        }

        public override Coin GetCoin(string symbol, string currency)
        {
            string id = symbol + "_" + currency;
            Instrument instrument = this.Instruments[id];
            if (instrument == null)
            {
                CryptoCoinInfo info = MainSettings.Current.Remote.CryptoCoins[symbol];
                if (info != null)
                {
                    Coin coin = new Coin(this, id, symbol, exchange, currency, info.TickSize == 0 ? 0.001m : info.TickSize);
                    coin.Tag = info;
                    this.Instruments.Add(coin);
                    return coin;
                }
            }
            return instrument as Coin;
        }

        public override void Subscribe(Instrument instrument, MarketDataType type)
        {
            type = MarketDataType.Quote;
            if (!subscribtions.Contains(instrument.ID + type))
            {
                Subscribtion subscribtion = new Subscribtion(instrument, type);
                subscribtions.Add(subscribtion);
                if (Application.IsCreated)
                {
                    ThreadTimer.Delay(1000, ()=> Timer_Tick(null, null));
                    if (subscribtions.Count == 1)
                        this.timer.Start();
                }
            }
        }

        public override void Unsubscribe(Instrument instrument, MarketDataType type)
        {
            if (type == MarketDataType.Quote)
            {
                if (subscribtions.Contains(instrument.ID + type))
                {
                    subscribtions.Remove(instrument.ID + type);
                    if (subscribtions.Count == 0)
                        this.timer.Stop();
                }
            }
        }

        private class Subscribtion : IUnique
        {
            public Subscribtion(Instrument instrument, MarketDataType type)
            {
                this.instrument = instrument;
                this.type = type;
                this.info = instrument.Tag as CryptoCoinInfo;
            }

            public readonly Instrument instrument;
            public readonly CryptoCoinInfo info;

            public readonly MarketDataType type;

            public string ID => instrument.ID + type;
        }
    }
}
