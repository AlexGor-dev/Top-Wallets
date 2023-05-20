using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Navigation;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public class WalletsSetting : GeneralSetting
    {
        protected WalletsSetting(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.currency = data["currency"] as Currency;
            this.currencies = data["currencies"] as UniqueCollection<Currency>;
            this.relativeTimeTransactions = (bool)data["relativeTimeTransactions"];
            this.showProjectSupportMessages = (bool)data["showProjectSupportMessages"];
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["currency"] = this.currency;
            data["currencies"] = this.currencies;
            data["relativeTimeTransactions"] = this.relativeTimeTransactions;
            data["showProjectSupportMessages"] = this.showProjectSupportMessages;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletsSetting()
        {
            this.currencies = new UniqueCollection<Currency>();
            this.currencies.Add(new Currency("USD"));
            this.currencies.Add(new Currency("EUR"));
            this.currencies.Add(new Currency("RUB"));
            this.currency = this.currencies["USD"];
            this.Init();
        }

        private void Init()
        {
        }

        protected override void OnDisposed()
        {
            this.currencies.Clear(true);
            base.OnDisposed();
        }

        public event Handler CurrencyChanged;
        public event Handler RelativeTimeChanged;

        private UniqueCollection<Currency> currencies;
        [System.ComponentModel.Browsable(false)]
        public UniqueCollection<Currency> Currencies => currencies;

        private Currency currency;
        [Currencies, System.ComponentModel.Category("wallets")]
        public Currency Currency
        {
            get => currency;
            set
            {
                if (currency == value) return;
                currency = value;
                Events.Invoke(this.CurrencyChanged, this);
            }
        }

        private bool relativeTimeTransactions = true;
        [System.ComponentModel.Category("wallets")]
        public bool RelativeTimeTransactions
        {
            get => relativeTimeTransactions;
            set
            {
                if (relativeTimeTransactions == value) return;
                relativeTimeTransactions = value;
                Events.Invoke(this.RelativeTimeChanged, this);
            }
        }

        private bool showProjectSupportMessages = true;
        [System.ComponentModel.Category("messages")]
        public bool ShowProjectSupportMessages
        {
            get => showProjectSupportMessages;
            set => showProjectSupportMessages = value;
        }

        private bool showTransactionMessages = true;
        [System.ComponentModel.Category("messages")]
        public bool ShowTransactionMessages
        {
            get => showTransactionMessages;
            set => showTransactionMessages = value;
        }

        [System.ComponentModel.Browsable(false)]
        public override IImage Image => Images.GetSvg("wallets.svg");

        protected override Component CreateComponent()
        {
            return new PropertyView();
        }

        protected override void OnComponentCreated()
        {
            (this.Component as PropertyView).SelectedObject = this;
        }

        public override void Apply()
        {
            if (this.IsComponentCreated)
                (this.Component as PropertyView).Apply();
            base.Apply();
        }

        public override void Cancel()
        {
            if (this.IsComponentCreated)
                (this.Component as PropertyView).Reset();
            base.Cancel();
        }

        protected override void GetAttributes(AttributeNameCollection attr)
        {
            base.GetAttributes(attr);
            if(!MainSettings.Current.Remote.Support.Enabled)
                attr.AddBrowsable("ShowProjectSupportMessages", false);
        }
    }

    public class CurrenciesAttribute : ElementsAttribute
    {
        public CurrenciesAttribute()
            : base(MainSettings.Current.General.Currencies.ToArray())
        {
        }
    }

}
