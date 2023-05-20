using System;
using Complex.Controls;
using Complex.Animations;
using Complex.Themes;
using Complex.Drawing;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public partial class TotalPanel
    {
        private class TotalContainer : Complex.Controls.ButtonsPanel, IMarketDataHandler
        {
            public TotalContainer(bool isTestnet)
            {
                this.isTestnet = isTestnet;
                this.Padding.Set(4);

                this.MinWidth = 230;
                this.MaxWidth = 230;
                this.BackRadius = 10;

                this.largeLabel = new LargeLabel(null, "total1", isTestnet ? "Test" : "Real");
                this.largeLabel.TextComponent.Alignment = ContentAlignment.Center;
                this.largeLabel.DescComponent.Alignment = ContentAlignment.Center;
                this.largeLabel.Dock = isTestnet ? DockStyle.Right : DockStyle.Left;

                this.totalLabel = new LargeCurrencyLabel(null, null, null, null);
                this.totalLabel.TextCurrencyLabel.Alignment = ContentAlignment.Right;
                this.totalLabel.DescCurrencyLabel.Alignment = ContentAlignment.Right;
                this.totalLabel.DescCurrencyLabel.ValueTextComponent.AppendLeftText = "≈";
                this.totalLabel.DescCurrencyLabel.ValueTextComponent.Font = Theme.font10;
                this.totalLabel.Dock = isTestnet ? DockStyle.Right : DockStyle.Left;

                this.changesLabel = new LargeCurrencyLabel(null, null, null, null);
                this.changesLabel.TextCurrencyLabel.Alignment = ContentAlignment.Right;
                this.changesLabel.DescCurrencyLabel.Alignment = ContentAlignment.Right;
                this.changesLabel.TextCurrencyLabel.ValueTextComponent.Font = Theme.font9Bold;
                this.changesLabel.DescCurrencyLabel.ValueTextComponent.Font = Theme.font9;
                this.changesLabel.Dock = isTestnet ? DockStyle.Right : DockStyle.Left;

                if (isTestnet)
                {
                    this.Add(this.changesLabel);
                    this.Add(this.totalLabel);
                    this.Add(this.largeLabel);
                }
                else
                {
                    this.Add(this.largeLabel);
                    this.Add(this.totalLabel);
                    this.Add(this.changesLabel);
                }
                MainSettings.Current.General.CurrencyChanged += Instance_CurrencyChanged;
            }

            protected override void OnDisposed()
            {
                MainSettings.Current.General.CurrencyChanged -= Instance_CurrencyChanged;
                if (WalletsData.IsCurrentExist)
                {
                    WalletsData.Wallets.Added -= Wallets_Added;
                    WalletsData.Wallets.Removed -= Wallets_Removed;
                }
                this.Unsubscribe();
                base.OnDisposed();
            }

            private bool isTestnet;
            private LargeCurrencyLabel totalLabel;
            private LargeCurrencyLabel changesLabel;
            private LargeLabel largeLabel;

            private Array<Coin> coins = new Array<Coin>();
            private UniqueCollection<Balance> balances = new UniqueCollection<Balance>();
            private Balance lastChanges = new Balance(null);

            protected override void OnDrawBack(Graphics g)
            {
                base.OnDrawBack(g);
                int color = isTestnet ? Theme.gray2 : Theme.green1;
                g.Smoosh(() => g.FillRoundRect(largeLabel.Left, largeLabel.Top, largeLabel.Width, largeLabel.Height, 9, Color.Argb(100, color)));
            }

            private void Instance_CurrencyChanged(object sender)
            {
                this.Unsubscribe();
                this.Subscribe();
                this.Calculate();
            }

            private void Unsubscribe()
            {
                foreach (Coin coin in this.coins)
                    coin.Unsubscribe(this, MarketDataType.Quote);
                this.coins.Clear();
            }

            private void Subscribe()
            {
                foreach (Wallet wallet in WalletsData.Wallets)
                {
                    if (IsMain(wallet))
                    {
                        Coin coin = wallet.Market.Coin;
                        coin.Subscribe(this, MarketDataType.Quote);
                        this.coins.Add(coin);
                    }
                }
            }

            private bool IsMain(Wallet wallet) => wallet.IsMain && wallet.Adapter.IsTestnet == this.isTestnet && wallet.Adapter.Symbol == wallet.Symbol && wallet.IsSupportMarket;

            public void Update()
            {
                this.Unsubscribe();
                if (WalletsData.IsCurrentExist)
                {
                    WalletsData.Wallets.Added += Wallets_Added;
                    WalletsData.Wallets.Removed += Wallets_Removed;
                    foreach (Wallet wallet in WalletsData.Wallets)
                    {
                        if (IsMain(wallet))
                        {
                            wallet.Changed += Wallet_Changed;
                            wallet.TransactionsNew += Wallet_TransactionsNew;
                        }
                    }
                    this.Subscribe();
                    if (this.IsCreated)
                        this.Calculate();
                }
            }

            private void Wallet_TransactionsNew(object sender, ITransactionBase[] value)
            {
                this.Calculate();
            }

            private void Wallet_Changed(object sender)
            {
                this.Calculate();
            }

            private void Wallets_Added(object sender, Wallet wallet)
            {
                if (IsMain(wallet))
                {
                    wallet.Changed += Wallet_Changed;
                    wallet.TransactionsNew += Wallet_TransactionsNew;
                    Coin coin = wallet.Market.Coin;
                    coin.Subscribe(this, MarketDataType.Quote);
                    this.coins.Add(coin);
                    this.Calculate();
                }
            }
            private void Wallets_Removed(object sender, Wallet wallet)
            {
                if (IsMain(wallet))
                {
                    wallet.Changed -= Wallet_Changed;
                    wallet.TransactionsNew -= Wallet_TransactionsNew;
                    Coin coin = wallet.Market.Coin;
                    coin.Unsubscribe(this, MarketDataType.Quote);
                    this.coins.Remove(coin);
                    this.Calculate();
                }
            }

            void IMarketDataHandler.OnMarketData(Instrument instrument, MarketData md)
            {
                if (md.type == MarketDataType.LastPrice)
                    this.UpdateBalance();
            }

            private void UpdateBalance()
            {
                decimal curBalance = 0;
                foreach (Balance balance in this.balances.ToArray())
                    curBalance += balance.GetBalance();

                decimal chenges = this.lastChanges.GetBalance();

                string totalText = this.totalLabel.TextCurrencyLabel.ValueTextComponent.Text;
                string changeText = this.changesLabel.TextCurrencyLabel.ValueTextComponent.Text;

                if (this.balances.Count == 1)
                {
                    this.totalLabel.TextCurrencyLabel.Visible = true;
                    this.changesLabel.TextCurrencyLabel.Visible = true;
                    this.totalLabel.TextCurrencyLabel.ValueTextComponent.Text = this.balances[0].balance.ToKMB(3);
                    this.totalLabel.TextCurrencyLabel.CurrencyTextComponent.Text = this.balances[0].coin.Symbol;
                    this.changesLabel.TextCurrencyLabel.CurrencyTextComponent.Text = this.balances[0].coin.Symbol;

                }
                else
                {
                    this.totalLabel.TextCurrencyLabel.Visible = false;
                    this.changesLabel.TextCurrencyLabel.Visible = false;
                }
                this.totalLabel.DescCurrencyLabel.ValueTextComponent.Text = curBalance.ToKMB(3);
                this.changesLabel.DescCurrencyLabel.ValueTextComponent.Text = chenges.ToKMBPlus(3);
                this.changesLabel.TextCurrencyLabel.ValueTextComponent.Text = this.lastChanges.balance.ToKMBPlus(3);
                this.changesLabel.DescCurrencyLabel.CurrencyTextComponent.Text = MainSettings.Current.General.Currency.ID;
                this.totalLabel.DescCurrencyLabel.CurrencyTextComponent.Text = MainSettings.Current.General.Currency.ID;
                if (chenges > 0)
                {
                    this.changesLabel.TextCurrencyLabel.ValueTextComponent.ForeColor = Theme.up;
                    this.changesLabel.DescCurrencyLabel.ValueTextComponent.ForeColor = Theme.up;
                }
                else if (chenges < 0)
                {
                    this.changesLabel.TextCurrencyLabel.ValueTextComponent.ForeColor = Theme.orange0;
                    this.changesLabel.DescCurrencyLabel.ValueTextComponent.ForeColor = Theme.orange0;
                }
                else
                {
                    this.changesLabel.TextCurrencyLabel.ValueTextComponent.ForeColor = 0;
                    this.changesLabel.DescCurrencyLabel.ValueTextComponent.ForeColor = 0;
                }

                if (totalText != this.totalLabel.TextCurrencyLabel.ValueTextComponent.Text)
                    this.totalLabel.Measured = false;
                if (changeText != this.changesLabel.TextCurrencyLabel.ValueTextComponent.Text)
                    this.changesLabel.Measured = false;
                if (!this.totalLabel.Measured || !this.changesLabel.Measured)
                {
                    this.ClearMeasured();
                    Application.Invoke(() =>
                    {
                        this.RelayoutAll();
                        this.Invalidate();
                    });
                }

            }

            private void Calculate()
            {
                lock (this.balances)
                {
                    decimal balance = 0;
                    this.balances.Clear();
                    ITransactionBase last = null;
                    Coin lastCoin = null;
                    Wallet lastWallet = null;
                    foreach (Wallet wallet in WalletsData.Wallets)
                    {
                        if (IsMain(wallet))
                        {
                            balance += wallet.Market.LastPrice * wallet.Balance;
                            Balance b = this.balances[wallet.Market.Coin.ID];
                            if (b == null)
                            {
                                b = new Balance(wallet.Market.Coin);
                                this.balances.Add(b);
                            }
                            b.balance += wallet.Balance;
                            ITransactionBase lt = wallet.LastTransaction;
                            if (lt != null)
                            {
                                if (last == null || lt.Time > last.Time)
                                {
                                    last = lt;
                                    lastWallet = wallet;
                                    lastCoin = wallet.Market.Coin;
                                }
                            }
                        }
                    }
                    this.lastChanges = new Balance(lastCoin);
                    if (last != null)
                        this.lastChanges.balance = last.GetAmount(lastWallet.Symbol);
                }
                this.UpdateBalance();

            }

            private class Balance : IUnique
            {
                public Balance(Coin coin)
                {
                    this.coin = coin;
                }

                public readonly Coin coin;
                public string ID => coin.ID;

                public decimal balance;

                public decimal GetBalance()
                {
                    if (coin == null)
                        return 0;
                    return balance * coin.LastPrice;
                }
            }

        }

    }
}
