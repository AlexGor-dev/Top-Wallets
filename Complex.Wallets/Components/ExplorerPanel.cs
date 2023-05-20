using System;
using Complex.Controls;
using Complex.Themes;

namespace Complex.Wallets
{
    public class ExplorerPanel : Container, IExplorerSource, IExplorerPanel, ISelectableComponent
    {
        protected ExplorerPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.explorer = data["explorer"] as Explorer;
            this.pager = data["pager"] as ModePager;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["explorer"] = this.explorer;
            data["pager"] = this.pager;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public ExplorerPanel(Explorer explorer)
        {
            this.explorer = explorer;
            this.SaveComponents = false;

            this.pager = new ModePager();
            this.pager.AutoMoveSwitch = true;
            this.pager.Dock = DockStyle.Fill;

            this.Init();
        }

        private void Init()
        {

            Container top = new Container();
            top.Padding.Set(6, 18, 6, 6);
            top.Inflate.Set(10, 0);
            top.Dock = DockStyle.Top;
            top.Style = Theme.Get<MultiPagerTheme>();

            adapterLabel = new TextComponent("Explorer " + (explorer.Adapter.IsTestnet ? "Test " : "") + explorer.Adapter.Symbol + " wallets");
            adapterLabel.Padding.Set(6, 0, 6, 0);
            adapterLabel.Font = Theme.font11Bold;
            //adapterLabel.Style = Theme.Get<RoundLabelTheme>();
            //adapterLabel.RoundBack = true;
            //adapterLabel.Font = Theme.font13Bold;
            adapterLabel.Dock = DockStyle.Left;
            top.Add(adapterLabel);

            addressBox = new TextBox();
            addressBox.EditEnded += (s) =>
            {
                this.ExploreWallet(this.explorer.Adapter, addressBox.Text.Trim());
            };
            addressBox.EditCanceled += (s) =>
            {
                if(addressBox.Parent.FocusedComponent == this.findButton)
                    this.ExploreWallet(this.explorer.Adapter, addressBox.Text.Trim());
            };
            addressBox.Enabled = explorer.Adapter.IsConnected;
            addressBox.Padding.Set(10, 0, 10, 0);
            addressBox.BorderRadius.Set(16);
            addressBox.Dock = DockStyle.Left;
            addressBox.MinWidth = 300;
            addressBox.MaxWidth = 300;
            addressBox.HintTextID = "enterWalletAddress";
            top.Add(addressBox);

            findButton = new ImageButton("find.svg");
            findButton.Dock = DockStyle.Left;
            findButton.Executed += (s) =>
            {
                this.ExploreWallet(this.explorer.Adapter, addressBox.Text.Trim());
            };
            top.Add(findButton);

            ImageButton closeButton = new ImageButton("close.svg");
            closeButton.MaxHeight = 24;
            closeButton.ToolTipInfo = new ToolTipInfo("closeExplorer");
            closeButton.Dock = DockStyle.Right;
            closeButton.Executed += (s) =>
            {
                foreach (WalletPage page in this.pager.Pages)
                    WalletsData.Wallets.Remove(page.Wallet);
                WalletsData.Explorers.Remove(this.explorer);
            };
            top.Add(closeButton);

            this.Add(top);

            this.Add(this.pager);

            explorer.Adapter.Connected += Adapter_Connected;
            explorer.Adapter.Disconnected += Adapter_Disconnected;
        }

        protected override void OnDisposed()
        {
            explorer.Adapter.Connected -= Adapter_Connected;
            explorer.Adapter.Disconnected -= Adapter_Disconnected;
            base.OnDisposed();
        }

        private void Adapter_Disconnected(object sender)
        {
            addressBox.Enabled = explorer.Adapter.IsConnected;
            this.pager.Header.Invalidate();
        }

        private void Adapter_Connected(object sender)
        {
            addressBox.Enabled = explorer.Adapter.IsConnected;
            this.pager.Header.Invalidate();
            if (this.Alpha > 0)
                Application.Invoke(() => { addressBox.Focused = true; });
        }


        private Explorer explorer;
        public Explorer Explorer => explorer;

        private TextComponent adapterLabel;
        private TextBox addressBox;
        private ImageButton findButton;

        private ModePager pager;

        protected override void OnShow()
        {
            if(!this.Updating && this.Alpha > 0)
                addressBox.Focused = true;
            base.OnShow();
        }

        protected override void OnEndUpdated()
        {
            addressBox.Focused = true;
            base.OnEndUpdated();
        }

        private WalletPage GetExistPage(string symbol, string address)
        {
            foreach (WalletPage page in this.pager.Pages)
                if (string.Compare(symbol, page.Wallet.Symbol, true) == 0 && string.Compare(page.Wallet.Address, address, true) == 0)
                    return page;
            return null;
        }

        private void ExploreWallet(WalletAdapter adapter, string address)
        {
            WalletPage epage = GetExistPage(adapter.Symbol, address);
            if (epage != null)
            {
                epage.Checked = true;
                addressBox.Text = null;
            }
            else
            {
                if (explorer.Adapter.IsValidAddress(address))
                {
                    explorer.Adapter.GetWallet(address, (w, e) =>
                    {
                        Application.Invoke(() =>
                        {
                            if (w != null)
                            {
                                WalletPage page = new WalletPage(pager, w);
                                this.pager.AddPage(page);
                                page.Checked = true;
                                WalletsData.Wallets.Add(w);
                                addressBox.Text = null;
                            }
                            else
                            {
                                MessageView.Show(e);
                            }
                        });
                    });
                }
                else
                {
                    MessageView.Show("invalidAddress");
                }
            }
        }

        public void ExploreWallet(Wallet wallet)
        {
            WalletPage epage = GetExistPage(wallet.Symbol, wallet.Address);
            if (epage != null)
            {
                epage.Checked = true;
            }
            else
            {
                WalletPage page = new WalletPage(pager, wallet);
                this.pager.AddPage(page);
                page.Checked = true;
                WalletsData.Wallets.Add(wallet);
            }
        }

        void ISelectableComponent.OnSelectedChanged()
        {
            if (this.Alpha > 0)
                addressBox.Focused = true;
        }
    }
}
