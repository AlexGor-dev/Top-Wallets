using System;
using Complex.Controls;
using Complex.Collections;

namespace Complex.Wallets
{
    public class MultiSendController : SendController
    {
        public MultiSendController(SwitchContainer switchContainer, EmptyHandler closeHandler, EmptyHandler doneHandler, WalletAdapter adapter, ITranserParams transerParams)
            : base(null, switchContainer, closeHandler, doneHandler)
        {
            this.adapter = adapter;
            this.transerParams = transerParams;
            Hashtable<object, Array<Wallet>> wallets = this.GetSupportWallets();
            if (wallets.Count == 0)
            {
                this.ErrorLang(Language.Current["sendingCoinsToAddress", transerParams.Address],"noWalletsAvailable");
            }
            //else if (wallets.Count == 1 && wallets[0].Count == 1)
            //{
            //    this.SetWallet(wallets[0][0]);
            //    this.sendPanel = new SendMainPanel(this, transerParams.Address, this.Wallet.Balance.ToDecimal(transerParams.Amount), transerParams.Comment, null);
            //    this.SetMainPanel(this.sendPanel);
            //    this.sendPanel.addressBox.Enabled = false;
            //    this.sendPanel.addressesView.Enabled = false;
            //    this.switchContainer.Current = this.sendPanel;
            //}
            else
            {
                this.selectPanel = new MultiWalletsSelectPanel(Language.Current["sendingCoinsToAddress", transerParams.Address], wallets, closeHandler, (coinInfo, wallet) =>
                {
                    this.SetWallet(wallet);
                    if (this.sendPanel == null)
                        this.sendPanel = this.CreateSendMainPanel();
                    this.sendPanel.addressBox.Enabled = false;
                    this.sendPanel.addressesView.Enabled = false;
                    this.SetMainPanel(this.sendPanel);
                    this.switchContainer.Current = this.sendPanel;
                });
                this.switchContainer.Current = this.selectPanel;
            }

        }

        private MultiWalletsSelectPanel selectPanel;
        private SendMainPanel sendPanel;
        protected readonly WalletAdapter adapter;
        protected readonly ITranserParams transerParams;

        protected override void OnWalletAdded(Wallet wallet)
        {
            if (this.sendPanel != null)
                this.sendPanel.SetWallet(wallet);
            base.OnWalletAdded(wallet);
        }

        protected virtual SendMainPanel CreateSendMainPanel()
        {
            return new SendMainPanel(this, transerParams.Address, this.Wallet.Balance.ToDecimal(transerParams.Amount), transerParams.Comment, () => this.switchContainer.Current = this.selectPanel);
        }
        protected virtual Hashtable<object, Array<Wallet>> GetSupportWallets()
        {
            Hashtable<object, Array<Wallet>> wallets = new Hashtable<object, Array<Wallet>>();
            Array<Wallet> arr = new Array<Wallet>();
            foreach (Wallet wallet in WalletsData.Wallets)
            {
                if (wallet.Address != transerParams.Address && wallet.IsMain && wallet.Adapter.Symbol == adapter.Symbol && wallet.Balance > 0)
                    arr.Add(wallet);
            }
            if (arr.Count > 0)
                wallets.Add(new object(), arr);
            return wallets;
        }
    }
}
