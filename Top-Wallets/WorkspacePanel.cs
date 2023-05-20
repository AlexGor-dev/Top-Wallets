using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class WorkspacePanel : Container, IWalletsController
    {
        protected WorkspacePanel(IData data)
            : base(data)
        {
        }

        protected override void Load(IData data)
        {
            this.walletsData = data["walletsData"] as WalletsData;
            base.Load(data);
            this.dockView = data["dockView"] as DockView;
            this.walletsContent = data["walletsContent"] as WalletsContent;
            this.explorersContent = data["explorersContent"] as ExplorersContent;
            this.switchContainer = data["switchContainer"] as SwitchContainer;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["walletsData"] = this.walletsData;
            data["dockView"] = this.dockView;
            data["walletsContent"] = this.walletsContent;
            data["explorersContent"] = this.explorersContent;
            data["switchContainer"] = this.switchContainer;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public WorkspacePanel()
        {
            this.walletsData = new WalletsData();

            this.Dock = DockStyle.Fill;

            this.dockView = new DockView();
            this.dockView.Dock = DockStyle.Fill;
            this.dockView.DockPaneSize = 60;
            this.Add(this.dockView);

            this.walletsContent = new WalletsContent();
            this.dockView.Add(this.walletsContent, DockStyle.Left, false);

            this.explorersContent = new ExplorersContent();
            this.dockView.Add(this.explorersContent, DockStyle.Left, false);


            this.switchContainer = new SwitchContainer(true);
            this.switchContainer.MinWidth = 700;
            this.switchContainer.InsertMode = true;
            this.dockView.Add(this.switchContainer, DockStyle.Fill, true);


            this.Init();

        }

        private void Init()
        {
            Controller.WalletsController = this;

            this.walletsContent.CurrentComponentChanged += (s, c, i) =>
            {
                this.switchContainer.InsertCurrent(c, i);
            };
            this.explorersContent.CurrentComponentChanged += (s, c, i) =>
            {
                this.switchContainer.InsertCurrent(c, 1000 + i);
            };

            Application.ExecuteCommand += Application_ExecuteCommand;
        }


        protected override void OnDisposed()
        {
            Application.ExecuteCommand -= Application_ExecuteCommand;
            this.walletsData.Dispose();
            base.OnDisposed();
        }

        private void Application_ExecuteCommand(string param)
        {
            if (this.IsCreated)
                this.SendCommand(param);
            else
                this.command = param;
        }

        private WalletsData walletsData;

        private string command;

        private DockView dockView;
        private WalletsContent walletsContent;
        private ExplorersContent explorersContent;

        private SwitchContainer switchContainer;

        public void ExploreWallet(Wallet wallet)
        {
            this.explorersContent.ExploreWallet(wallet);
        }

        public void ShowMainWallet(Wallet wallet)
        {
            this.walletsContent.ShowMainWallet(wallet);
        }

        public WalletAdapter GetAdapter(string adapterID)
        {
            return Controller.GetAdapter(adapterID);
        }

        public void DeleteWallet(Wallet wallet)
        {
            WalletsData.Wallets.Remove(wallet);
        }

        private void SendCommand(string command)
        {
            foreach (WalletAdapter adapter in Controller.Adapters)
                if (adapter.ExecuteCmd(command))
                    break;
        }
        protected override void OnCreated()
        {
            base.OnCreated();
            if (this.command != null)
                this.SendCommand(this.command);
        }
    }
}
