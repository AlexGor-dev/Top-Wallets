using System;
using Complex.Collections;
using Complex.Navigation;
using Complex.Controls;
using Complex.Trader;
using Complex.Remote;

namespace Complex.Wallets
{
    public class MainSettings : Settings, IMarketDataHandler
    {
        static MainSettings()
        {
            CreateSettings += delegate ()
            {
                return new MainSettings();
            };
        }

        public static new MainSettings Current => Settings.Current as MainSettings;

        protected MainSettings(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.remote = data["remote"] as RemoteSettings;
            this.sounds = data["sounds"] as SoundElementCollection;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["remote"] = this.remote;
            data["sounds"] = this.sounds;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public MainSettings()
        {
            remote = new RemoteSettings();

            this.sounds = new SoundElementCollection();
            this.sounds.Add(new SoundElement("connected", "adapter_connected.mp3"));
            this.sounds.Add(new SoundElement("disconnected", "adapter_disconnected.mp3"));
            this.sounds.Add(new SoundElement("inTransaction", "in_transaction.mp3"));
            this.sounds.Add(new SoundElement("outTransaction", "out_transaction.mp3"));
            this.sounds.Add(new SoundElement("projectSupportMessages", "support_message.mp3", 50));
            this.Add(this.sounds);

            this.Init();
        }

        private void Init()
        {
            JsonArray arr = Json.Parse(Resources.GetText("top_wallets.json")) as JsonArray;
            Json.Deserialize(remote, arr);
            this.adapter = new CoinGecko();

            ThreadTimer.Delay(2000, () =>
             {
                try
                {
                    string data = Http.Get("http://complex-soft.com/res/top_wallets.json");
                    JsonArray array = Json.Parse(data) as JsonArray;
                    int version = array.GetInt("version");
                    if (version != remote.Version)
                    {
                        Json.Deserialize(remote, array);
                        this.ApplyState = true;
                    }

                    }
                catch(Exception e)
                {
                }
             });
        }

        protected override void OnDisposed()
        {
            this.adapter.Dispose();
            base.OnDisposed();
        }

        private CoinsAdapter adapter;
        public CoinsAdapter Adapter => adapter;


        private RemoteSettings remote;
        public RemoteSettings Remote => remote;

        public new WalletsSetting General => base.General as WalletsSetting;

        private SoundElementCollection sounds;
        public SoundElementCollection Sounds => sounds;

        public Coin GetCoin(string symbol)
        {
            return adapter.GetCoin(symbol, General.Currency.ID);
        }

        public Coin GetCoin(string symbol, string currency)
        {
            return adapter.GetCoin(symbol, currency);
        }

        protected override Navigation.HotKeys CreateHotKeys()
        {
            return null;
        }

        protected override GeneralSetting CreateGeneralSetting()
        {
            return new WalletsSetting();
        }

        void IMarketDataHandler.OnMarketData(Instrument instrument, MarketData md)
        {
        }
    }
}
