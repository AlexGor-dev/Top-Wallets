using System;
using Complex.Controls;
using Complex.Collections;

namespace Complex.Wallets
{
    public class WalletsContent : DockContent
    {
        protected WalletsContent(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.walletsPanels = data["walletsPanels"] as Hashtable<Wallet, Component>;
            this.walletsItems = data["walletsItems"] as Hashtable<Wallet, Component>;
            this.anyView = data["anyView"] as AnyView;
            this.createButton = data["createButton"] as MenuButton;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["walletsPanels"] = this.walletsPanels;
            data["walletsItems"] = this.walletsItems;
            data["anyView"] = this.anyView;
            data["createButton"] = this.createButton;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WalletsContent()
        {
            this.ImageID = "wallets.svg";
            this.TextID = "wallets";
            this.MinWidth = 270;
            this.Closable = false;

            this.walletsPanels = new Hashtable<Wallet, Component>();
            this.walletsItems = new Hashtable<Wallet, Component>();

            this.anyView = new AnyView();
            this.anyView.UseAlphaLayout = true;
            this.anyView.VScrollStep = 20;
            this.anyView.Dock = DockStyle.Fill;
            this.anyView.Inflate.Set(4, 4);
            this.Add(this.anyView);

            this.createButton = new MenuButton("create_wallet.svg", null);
            this.createButton.TwoStrip = false;
            this.createButton.Dock = DockStyle.Left;
            this.createButton.MenuAnimationMode = true;
            this.createButton.MenuAlignment = MenuAlignment.BottomLeft;
            this.Caption.Insert(0, this.createButton);

            this.Init();
        }

        private void Init()
        {
            this.createButton.ToolTipInfo = new ToolTipInfo(this.createButton.Image, "createWallet", null);

            this.anyView.SelectedComponentChanged += (s) =>
            {
                this.OnCurrentComponentChanged();
            };
            this.createButton.InitMenu += delegate (object s, Container menu)
            {
                (menu as MenuStripView).ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
                menu.Padding.Set(10);
                menu.Inflate.Set(0, 4);
                menu.Add(new MenuStripLabelLocalize("createWallet"));
                foreach (WalletAdapterExtension extension in Controller.Extensions)
                {
                    if (extension.SupportWallets)
                    {
                        MenuStripLargeButton textButton = new MenuStripLargeButton(extension.SmallImageID, extension.FullName, Controller.IsAdapterConnected(extension.ID) ? "connected" : "noConnection");
                        textButton.Tag = extension;
                        textButton.Executed += delegate (object s2)
                        {
                            WalletAdapterExtension ext = (s2 as MenuStripLargeButton).Tag as WalletAdapterExtension;
                            WalletAdapter adapter = Controller.GetAdapter(ext);
                            adapter.CreateWallet(this.createButton, (w, e) =>
                            {
                                if (w != null)
                                {
                                    WalletsData.Wallets.Add(w);
                                    Component item = this.walletsItems[w];
                                    if (w is IMultiWallet mw)
                                    {
                                        foreach(Wallet c in mw.Wallets)
                                            WalletsData.Wallets.Add(c);
                                    }
                                    this.anyView.Relayout();
                                    item.Focused = true;
                                }
                            });
                        };
                        menu.Add(textButton);
                    }
                }

            };



            WalletsData.Wallets.Added += Wallets_Added;
            WalletsData.Wallets.Removed += Wallets_Removed;
        }


        protected override void OnDisposed()
        {
            if (WalletsData.IsCurrentExist)
            {
                WalletsData.Wallets.Added -= Wallets_Added;
                WalletsData.Wallets.Removed -= Wallets_Removed;
            }
            base.OnDisposed();
        }

        private void Wallets_Removed(object sender, Wallet wallet)
        {
            if (wallet.IsMain)
            {
                Component component = this.walletsItems[wallet];
                if (component != null)
                {
                    this.walletsItems.Remove(wallet);
                    this.anyView.Remove(component);
                    Component panel = walletsPanels[wallet];
                    if (panel != null)
                    {
                        Events.Invoke(this.CurrentComponentChanged, this, null, 0);
                        walletsPanels.Remove(wallet);
                        panel.Parent.Remove(panel);
                        panel.Dispose();
                    }
                    component.Dispose();
                    this.anyView.Relayout();
                }
            }
        }

        private void Wallets_Added(object sender, Wallet wallet)
        {
            if (wallet.IsMain)
                this.CreateWalletItem(wallet);
        }

        public event Handler<Component,int> CurrentComponentChanged;

        private AnyView anyView;
        private MenuButton createButton;

        private Hashtable<Wallet, Component> walletsPanels;
        private Hashtable<Wallet, Component> walletsItems;

        public int ComponentsCount => this.anyView.Components.Count;

        protected override void OnShow()
        {
            if (!this.IsMoved)
                this.OnCurrentComponentChanged();
            base.OnShow();
        }

        protected override void OnShowed()
        {
            this.OnCurrentComponentChanged();
            base.OnShowed();
        }

        protected override void OnHided()
        {
            Events.Invoke(this.CurrentComponentChanged, this, null, 0);
            base.OnHided();
        }

        private void OnCurrentComponentChanged()
        {
            Component res = null;
            Component component = this.anyView.SelectedComponent;
            int index = 0;
            if (component != null)
            {
                index = this.anyView.Components.IndexOf(component);
                IWalletSource item = component as IWalletSource;
                if (item != null)
                {
                    res = walletsPanels[item.Wallet];
                    if (res == null)
                    {
                        res = item.Wallet.CreateWalletPanel();
                        walletsPanels.Add(item.Wallet, res);
                    }
                }
            }
            Events.Invoke(this.CurrentComponentChanged, this, res, index);
        }

        private Component CreateWalletItem(Wallet wallet)
        {
            Component item = this.walletsItems[wallet];
            if (item == null)
            {
                item = wallet.CreateWalletItem();
                if (wallet is IToken token && token.Parent != null)
                {
                    Component parentIten = this.walletsItems[token.Parent];
                    if (parentIten != null)
                    {
                        int index = this.walletsItems.IndexOf(parentIten) + 1;
                        for (int i = index; i < this.walletsItems.Count; i++)
                        {
                            IWalletSource ws = this.walletsItems[i] as IWalletSource;
                            if (ws.Wallet is IToken t && t.Parent == token.Parent)
                            {
                                index = i + 1;
                                continue;
                            }
                            break;
                        }
                        this.walletsItems.Insert(wallet, item, index);
                        this.anyView.Insert(index, item);
                        return item;
                    }
                }
                this.walletsItems.Add(wallet, item);
                this.anyView.Add(item);
            }
            return item;
        }

        public void ShowMainWallet(Wallet wallet)
        {
            this.Button.Checked = true;
            Component item = this.walletsItems[wallet];
            if (item == null)
                item = this.CreateWalletItem(wallet);
            this.anyView.Relayout();
            item.Focused = true;

        }

        public void SelectMainWallet(Wallet wallet)
        {
            Component item = this.walletsItems[wallet];
            this.anyView.Relayout();
            item.Focused = true;
        }
    }
}
