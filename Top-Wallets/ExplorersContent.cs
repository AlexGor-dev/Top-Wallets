using System;
using Complex.Controls;
using Complex.Collections;

namespace Complex.Wallets
{
    public class ExplorersContent : DockContent
    {
        protected ExplorersContent(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.anyView = data["anyView"] as AnyView;
            this.explorersItems = data["explorersItems"] as Hashtable<Explorer, Component>;
            this.explorersPanels = data["explorersPanels"] as Hashtable<Explorer, Component>;
            this.createButton = data["createButton"] as MenuButton;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["anyView"] = this.anyView;
            data["explorersItems"] = this.explorersItems;
            data["explorersPanels"] = this.explorersPanels;
            data["createButton"] = this.createButton;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public ExplorersContent()
        {
            this.ImageID = "explorers.svg";
            this.TextID = "explorers";
            this.MinWidth = 270;
            this.MinHeight = 250;
            this.Closable = false;

            this.explorersItems = new Hashtable<Explorer, Component>();
            this.explorersPanels = new Hashtable<Explorer, Component>();

            this.anyView = new AnyView();
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
            this.createButton.ToolTipInfo = new ToolTipInfo(this.createButton.Image, "createExplorer", null);

            this.anyView.SelectedComponentChanged += (s) =>
            {
                this.OnCurrentComponentChanged();
            };
            this.createButton.InitMenu += delegate (object s, Container menu)
            {
                (menu as MenuStripView).ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
                menu.Padding.Set(10);
                menu.Inflate.Set(0, 4);
                menu.Add(new MenuStripLabelLocalize("createExplorer"));
                foreach (WalletAdapterExtension extension in Controller.Extensions)
                {
                    if (extension.SupportExplorer && !WalletsData.Explorers.Contains(extension.ID))
                    {
                        MenuStripLargeButton textButton = new MenuStripLargeButton(extension.ImageID, extension.FullName, Controller.IsAdapterConnected(extension.ID) ? "connected" : "noConnection");
                        textButton.ImageComponent.MaxSize.Set(24, 24);
                        textButton.Tag = extension;
                        textButton.Executed += delegate (object s2)
                        {
                            WalletAdapterExtension ext = (s2 as MenuStripLargeButton).Tag as WalletAdapterExtension;
                            WalletsData.Explorers.Add(new Explorer(ext.ID));
                        };
                        menu.Add(textButton);
                    }
                }

            };
            WalletsData.Explorers.Added += Explorers_Added;
            WalletsData.Explorers.Removed += Explorers_Removed;
        }

        protected override void OnDisposed()
        {
            if (WalletsData.IsCurrentExist)
            {
                WalletsData.Explorers.Added -= Explorers_Added;
                WalletsData.Explorers.Removed -= Explorers_Removed;
            }
            base.OnDisposed();
        }
        private void Explorers_Added(object sender, Explorer explorer)
        {
            Component item = explorer.CreateExplorerItem();
            this.explorersItems.Add(explorer, item);
            this.anyView.Add(item);
            this.anyView.Relayout();
            item.Selected = true;
        }

        private void Explorers_Removed(object sender, Explorer explorer)
        {
            int index = 0;
            Component item = this.explorersItems[explorer];
            if (item != null)
            {
                index = this.anyView.Components.IndexOf(item);
                this.explorersItems.Remove(explorer);
                this.anyView.Remove(item);
                Component panel = explorersPanels[explorer];
                if (panel != null)
                {
                    Events.Invoke(this.CurrentComponentChanged, this, null, 0);
                    explorersPanels.Remove(explorer);
                    panel.Parent.Remove(panel);
                    panel.Dispose();
                }

            }
            this.anyView.Relayout();
            if (this.anyView.Components.Count > 0)
            {
                index = Math.Min(index, this.anyView.Components.Count - 1);
                this.anyView.Components[index].Selected = true;
            }
        }



        public event Handler<Component, int> CurrentComponentChanged;

        private MenuButton createButton;

        private AnyView anyView;
        private Hashtable<Explorer, Component> explorersPanels;
        private Hashtable<Explorer, Component> explorersItems;

        public int ComponentsCount => this.anyView.Components.Count;

        protected override void OnShow()
        {
            if(!this.IsMoved)
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
                IExplorerSource item = component as IExplorerSource;
                if (item != null)
                {
                    res = explorersPanels[item.Explorer];
                    if (res == null)
                    {
                        res = item.Explorer.CreateExplorerPanel();
                        explorersPanels.Add(item.Explorer, res);
                    }

                }
            }
            Events.Invoke(this.CurrentComponentChanged, this, res, index);
        }

        private (IExplorerPanel panel, Component item)  GetExplorerPanel(WalletAdapter adapter)
        {
            foreach (Component component in this.anyView.Components)
            {
                IExplorerSource item = component as IExplorerSource;
                if (item != null && item.Explorer.Adapter == adapter)
                    return (this.explorersPanels[item.Explorer] as IExplorerPanel, component);
            }
            return (null, null);
        }

        public void ExploreWallet(Wallet wallet)
        {
            this.Button.Checked = true;
            var(explorerPanel, item) = this.GetExplorerPanel(wallet.Adapter);
            if (explorerPanel == null)
            {
                WalletsData.Explorers.Add(new Explorer(wallet.Adapter.ID));
                (explorerPanel, item) = this.GetExplorerPanel(wallet.Adapter);
            }
            if (explorerPanel != null)
            {
                this.anyView.SelectedComponent = item;
                explorerPanel.ExploreWallet(wallet);
            }
        }
    }
}
