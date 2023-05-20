using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class MainContainer : MainWorkspaceContainer
    {
        protected MainContainer(IData data) : base(data)
        {
        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.totalPanel = data["totalPanel"] as TotalPanel;
            this.adaptersPanel = data["adaptersPanel"] as Container;
            this.timeComponent = data["timeComponent"] as DateTimeComponent;
            this.messageButton = data["messageButton"] as CheckedButton;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["totalPanel"] = this.totalPanel;
            data["adaptersPanel"] = this.adaptersPanel;
            data["timeComponent"] = this.timeComponent;
            data["messageButton"] = this.messageButton;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public MainContainer()
        {
            this.StatusBar.Padding.Set(10, 4, 10, 4);
            this.StatusBar.Inflate.Set(4);

            this.totalPanel = new TotalPanel();
            this.totalPanel.Dock = DockStyle.Top | DockStyle.Animation;
            this.Insert(0, this.totalPanel);
            this.Add(this.totalPanel.Button);

            this.adaptersPanel = new Container();
            this.adaptersPanel.Dock = DockStyle.Left;
            this.adaptersPanel.Inflate.width = 4;
            this.adaptersPanel.SaveComponents = false;
            this.StatusBar.Add(this.adaptersPanel);

            this.messageButton = new CheckedButton("message.svg", "messages", false);
            this.messageButton.Dock = DockStyle.Right;
            this.StatusBar.Add(this.messageButton);

            this.StatusBar.Add(new Separator(DockStyle.Right, 4));

            this.timeComponent = new DateTimeComponent(DateTimeViewMode.Time);
            this.timeComponent.Dock = DockStyle.Right;
            this.StatusBar.Add(this.timeComponent);

            this.Init();
        }

        private void Init()
        {
            this.totalPanel.Button.BringToFront();

            this.messageButton.CheckedChanged += (s) =>
            {
                if (this.messageButton.Checked)
                {
                    MessagesMenu menu = Serializable.Serialize["messagesMenu"] as MessagesMenu;
                    if (menu == null)
                    {
                        menu = new MessagesMenu();
                        Serializable.Serialize["messagesMenu"] = menu;
                    }
                    menu.Hided += Menu_Hided;
                    menu.Show(this.messageButton, MenuAlignment.TopRight);
                }
            };
            this.timeComponent.Animate = true;
            this.timeComponent.Tick += (s) => { this.timeComponent.DateTime = DateTime.Now; };

            foreach (WalletAdapter adapter in Controller.Adapters)
                Adapters_Added(null, adapter);

            Controller.Adapters.Added += Adapters_Added;
            Controller.Adapters.Removed += Adapters_Removed;
        }

        protected override void OnDisposed()
        {
            Controller.Adapters.Added -= Adapters_Added;
            Controller.Adapters.Removed -= Adapters_Removed;
            base.OnDisposed();
        }
        private void Adapters_Added(object sender, WalletAdapter adapter)
        {
            if (adapter.SupportConnection)
            {
                adapter.Disposed += Adapter_Disposed;
                AdapterWaitLabel label = new AdapterWaitLabel(adapter);
                label.Dock = DockStyle.Left;
                this.adaptersPanel.Add(label);
                this.StatusBar.Layout();
            }
            if (adapter.TopActionComponents != null)
            {
                foreach (Component component in adapter.TopActionComponents)
                {
                    component.Tag = adapter;
                    this.CaptionBar.LeftPanel.Add(component);
                }
                this.CaptionBar.LeftPanel.Layout();
            }
        }

        private void RemoveAdapter(WalletAdapter adapter)
        {
            if (adapter.SupportConnection)
            {
                if (!this.IsDisposed)
                {
                    foreach (Component component in this.adaptersPanel.Components)
                    {
                        if (component is AdapterWaitLabel label && label.Adapter == adapter)
                        {
                            this.adaptersPanel.Remove(component);
                            component.Dispose();
                            this.StatusBar.Layout();
                            break;
                        }
                    }
                }
            }
            if (adapter.TopActionComponents != null)
            {
                foreach (Component component in this.CaptionBar.LeftPanel.Components)
                    if (component.Tag == adapter)
                        this.CaptionBar.LeftPanel.Remove(component);
                this.CaptionBar.LeftPanel.Layout();
            }
        }

        private void Adapter_Disposed(object sender, EventArgs e)
        {
            this.RemoveAdapter(sender as WalletAdapter);
        }

        private void Adapters_Removed(object sender, WalletAdapter adapter)
        {
            adapter.Disposed -= Adapter_Disposed;
            this.RemoveAdapter(adapter);
        }

        private void Menu_Hided(object sender)
        {
            MessagesMenu menu = sender as MessagesMenu;
            menu.Hided -= Menu_Hided;
            this.messageButton.Checked = false;
        }

        private DateTimeComponent timeComponent;
        private CheckedButton messageButton;
        private Container adaptersPanel;
        private TotalPanel totalPanel;

        protected override string WorkspaceFilter => "Top-Wallets Workspace(*.tww)|*.tww;";

        protected override Workspace CreateDefaultWorkspace()
        {
            return new WalletsWorkspace("Default");
        }

        public override Workspace CreateWorkspace(string name)
        {
            return new WalletsWorkspace(name);
        }

        protected override Workspace FromPath(string fileName)
        {
            return WalletsWorkspace.FromPath(fileName);
        }

        protected override void OnCreated()
        {
            this.totalPanel.Update();
            base.OnCreated();
        }

        protected override void OnCurrentWorkspaceChanged()
        {
            if (this.CurrentWorkspace == null)
                Controller.Stop();
            this.totalPanel.Update();
            base.OnCurrentWorkspaceChanged();
        }

        protected override void InitHelpMenu(Container container)
        {
            string tgCanal = MainSettings.Current.Remote.TgCanal;
            if (!string.IsNullOrEmpty(tgCanal))
            {
                MenuStripButton button = new MenuStripButton("tgCanal");
                button.Executed += (s2) => WinApi.ShellExecute(MainSettings.Current.Remote.TgCanal);
                container.Add(button);
            }
            string tgSupport = MainSettings.Current.Remote.TgSupport;
            if (!string.IsNullOrEmpty(tgSupport))
            {
                MenuStripButton button = new MenuStripButton("tgSupport");
                button.Executed += (s2) => WinApi.ShellExecute(MainSettings.Current.Remote.TgSupport);
                container.Add(button);
            }

            string mail = MainSettings.Current.Remote.SupportMail;
            if (!string.IsNullOrEmpty(mail))
            {
                MenuStripButton button = new MenuStripButton("supportMail");
                button.Executed += (s2) => WinApi.ShellExecute(MainSettings.Current.Remote.SupportMail);
                container.Add(button);
            }
        }
    }
}
