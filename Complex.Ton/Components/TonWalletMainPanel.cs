using System;
using System.Collections.Generic;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Wallets;
using Complex.Remote;
using Complex.Collections;
using Complex.Ton.TonConnect;

namespace Complex.Ton
{
    public class TonWalletMainPanel : WalletMainPanel
    {
        protected TonWalletMainPanel(IData data) : base(data)
        {
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TonWalletMainPanel(TonUnknownWallet wallet) 
            : base(wallet)
        {
            this.Init();
        }

        private void Init()
        {
            this.wallet = base.Wallet as TonUnknownWallet;
            this.minter = base.Wallet as JettonMinter;
            CaptionStyle style = Theme.Get<CaptionStyle>();

            switch (this.wallet.Type)
            {
                case WalletType.JettonMinter:
                case WalletType.JettonWallet:
                    {
                        Container container = new Container();
                        //container.MinHeight = 40;
                        container.Padding.Set(10, 4, 10, 4);
                        container.Inflate.width = 6;
                        container.Dock = DockStyle.Top;
                        container.Style = Theme.Get<MapBackTheme>();
                        JettonInfo info = this.minter.JettonInfo;

                        TextComponent caption = new TextComponent("Jetton");
                        caption.AppendRightText = ":";
                        caption.Dock = DockStyle.Left;
                        caption.Style = style;
                        container.Add(caption);

                        this.textButton = new CheckedTextButton(info == null ? null : info.Name, false);
                        this.textButton.Enabled = info != null;
                        this.textButton.Dock = DockStyle.Left;
                        this.textButton.MaxWidth = 250;
                        this.textButton.CheckedChanged += (s) =>
                        {
                            info = this.minter.JettonInfo;
                            CheckedTextButton cb = s as CheckedTextButton;
                            if (cb.Checked)
                            {
                                cb.IsFixed = true;
                                JettonMenu menu = new JettonMenu(this.wallet, info, true);
                                menu.Hided += (s2) =>
                                {
                                    cb.IsFixed = false;
                                    cb.Checked = false;
                                };
                                menu.Show(s as Component, MenuAlignment.BottomLeft);
                            }
                        };
                        container.Add(this.textButton);

                        container.Add(new Separator(DockStyle.Left, 4));

                        caption = new TextLocalizeComponent("owner");
                        caption.AppendRightText = ":";
                        caption.Dock = DockStyle.Left;
                        caption.Style = style;
                        container.Add(caption);

                        string owner = this.minter.Owner;
                        if (!string.IsNullOrEmpty(owner))
                        {
                            Wallet wt = WalletsData.GetAnyWallet(this.wallet.AdapterID, owner);
                            ownerButton = new TextButton(wt != null ? wt.Name : owner);
                            ownerButton.MaxWidth = 250;
                            ownerButton.Dock = DockStyle.Left;
                            ownerButton.Executed += (s) =>
                            {
                                Controller.ShowAnyWallet(this.wallet.Adapter, this.wallet.Adapter.Symbol, this.minter.Owner);
                            };
                            container.Add(ownerButton);
                        }
                        else
                        {
                            caption = new TextComponent("null");
                            caption.Dock = DockStyle.Left;
                            container.Add(caption);

                        }

                        if (this.wallet.IsMain && this.wallet is JettonWallet jw && jw.WalletInfo.Owner == jw.WalletInfo.JettonInfo.OwnerAddress)
                        {
                            burnButton = new ColorButton("burnCoins");
                            burnButton.Dock = DockStyle.Right;
                            //burnButton.Padding.Set(6);
                            burnButton.Enabled = this.Adapter.IsConnected && jw.State != WalletState.None;
                            burnButton.Radius = 6;
                            burnButton.BoxColor = Theme.red0;
                            burnButton.Executed += (s) =>
                            {
                                new BurnCoinsForm(jw).Show(burnButton, MenuAlignment.BottomRight);
                            };
                            container.Add(burnButton);

                        }
                        this.Insert(1, new Separator(DockStyle.Top, 20));

                        this.Insert(2, container);
                    }
                    break;
            }
            this.InitTonConnect();
        }
        protected override void OnDisposed()
        {
            if (this.wallet is TonWallet tonWallet)
                tonWallet.Connections.Clear(true);
            base.OnDisposed();
        }


        private TonUnknownWallet wallet;
        private JettonMinter minter;

        private CheckedTextButton textButton;
        private ColorButton burnButton;
        private TextButton ownerButton;
        private TonConnectionContainer connectionContainer;
        private bool tonConnectInited = false;

        protected override void OnWalletChanged()
        {
            if (this.minter != null)
            {
                JettonInfo info = this.minter.JettonInfo;
                this.textButton.Text = info == null ? null : info.Name;
                if (this.ownerButton != null)
                {
                    Wallet wt = WalletsData.GetAnyWallet(this.wallet.AdapterID, info.OwnerAddress);
                    this.ownerButton.Text = wt != null ? wt.Name : info.OwnerAddress;
                }
                this.textButton.Parent.Layout();
            }
            this.InitTonConnect();
            base.OnWalletChanged();
        }
        protected override void OnConnectionChanged()
        {
            if(burnButton != null)
                burnButton.Enabled = this.Adapter.IsConnected && this.wallet.State != WalletState.None;
            base.OnConnectionChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            if (burnButton != null)
                burnButton.BoxColor = Theme.red0;
            base.OnDrawBack(g);
        }

        private void InitTonConnect()
        {
            if (this.tonConnectInited)
                return;
            switch (this.wallet.Type)
            {
                case WalletType.WalletV3:
                case WalletType.WalletV4:
                    if (this.wallet.IsMain && this.wallet.State == WalletState.Active)
                    {
                        this.tonConnectInited = true;
                        QrScannerButton mbutton = new QrScannerButton("tonConnect", "qr_scanner.svg");
                        mbutton.ToolTipInfo = new ToolTipInfo(mbutton.ImageID, "Ton Connect 2.0", "tonConnectDesc");
                        mbutton.Dock = DockStyle.Right;
                        mbutton.QrCompleted += (s, code) =>
                        {
                            Timer.Delay(300, () =>
                            {
                                try
                                {
                                    if (!Uri.TryCreate(code, UriKind.Absolute, out Uri uri))
                                        throw new MessageBoxException("invalidQrcode");
                                    WhiteWallet wallet = WhiteWallet.Wallets[uri.Host];
                                    if (wallet == null)
                                        wallet = WhiteWallet.Wallets["app.tonkeeper.com"];

                                    Dictionary<string, string> dictionary = uri.DecodeQueryParameters();
                                    if (!dictionary.TryGetValue("id", out string appPublicKey) || !dictionary.TryGetValue("r", out string r) || appPublicKey.Length != 64)
                                        throw new MessageBoxException("invalidQueryParameters");

                                    JsonArray v = Json.Parse(r) as JsonArray;
                                    string manifestUrl = v.GetString("manifestUrl");
                                    if (manifestUrl == null)
                                        throw new MessageBoxException("invalidParameters");

                                    string manifest = Util.Try(() => Http.GetBrouser(manifestUrl), (e) =>
                                    {
                                    });
                                    if (manifest == null)
                                        throw new MessageBoxException("invalidManifest");

                                    JsonArray arr = Json.Parse(manifest) as JsonArray;
                                    if (arr == null)
                                        throw new MessageBoxException("invalidManifest");
                                    DAppInfo dApp = Json.Deserialize<DAppInfo>(arr);
                                    ConnectRequest[] requests = Json.Deserialize<ConnectRequest[]>(v.GetArray("items"));

                                    new TonConnectForm(new Connection(this.wallet as TonWallet, wallet, dApp, appPublicKey), requests).Show(s as Component, MenuAlignment.BottomRight);
                                }
                                catch (MessageBoxException e)
                                {
                                    MessageBox.Show(e.captionID, e.desctiptionID, MessageBoxButtons.OK, Application.Form);
                                }
                                catch (Exception e)
                                {
                                    MessageView.Show(e.Message);
                                }
                            });

                        };
                        topContainer.Insert(0, mbutton);

                        this.connectionContainer = new TonConnectionContainer(this.wallet as TonWallet);
                        this.connectionContainer.Visible = false;
                        this.connectionContainer.Dock = DockStyle.Right;
                        topContainer.Insert(0, this.connectionContainer);
                        topContainer.Layout();
                    }
                    break;
            }

        }
        private class ConnectionContainer : ActiveContainer
        {
            public ConnectionContainer(Connection connection, ParamHandler<Connection> execute)
            {
                this.connection = connection;
                this.execute = execute;
                this.Padding.Set(4);
                this.Inflate.width = 2;
                this.DrawBorder = true;
                this.ClickEffect.Enabled = false;

                this.walletLabel = new ImageNameLabel(Resources.Product, "top_wallets_48.png");
                this.walletLabel.Inflate.height = 2;
                this.walletLabel.imageComponent.MaxSize.Set(20);
                this.walletLabel.Dock = DockStyle.Left;
                this.Add(this.walletLabel);

                this.dappLabel = new ImageNameLabel(connection.dapp.Name, "dapp.svg");
                this.dappLabel.Inflate.height = 2;
                this.dappLabel.imageComponent.MaxSize.Set(20);
                this.dappLabel.Dock = DockStyle.Right;
                this.Add(this.dappLabel);

                this.dappLabel.Image = connection.dapp.LoadImage((image) => { this.dappLabel.Image = image; this.dappLabel.Invalidate(); });

            }

            private Connection connection;
            private ParamHandler<Connection> execute;
            private ImageNameLabel walletLabel;
            private ImageNameLabel dappLabel;

            protected override Type GetDefaultTheme()
            {
                return typeof(StateBackTheme);
            }

            protected override void OnExecuted()
            {
                if (this.Form is Menu menu)
                    menu.Hide();
                this.execute(connection);
                base.OnExecuted();
            }

            protected override void OnMouseUp(MouseEvent e)
            {
                if(!(this.GetComponentAt(e.X, e.Y) is MenuButton))
                    base.OnMouseUp(e);
            }
            private class StateBackTheme : StateTheme
            {
                public override void Update()
                {
                    int color = Theme.green1;
                    borderColor = Color.Argb(100, color);
                    noneColor = Color.Argb(20, color);
                    focusedColor = Color.Argb(30, color);
                    clickedColor = Color.Argb(50, color);
                    selectedColor = Color.Argb(40, color);
                    selectedFocusedColor = Color.Argb(50, color);
                    disabledColor = Color.Argb(50, back8);
                    tabStopColor = Color.Argb(50, back10);
                }
            }

        }

        private class TonConnectionContainer : Container
        {
            public TonConnectionContainer(TonWallet wallet)
            {
                this.wallet = wallet;

                this.menuButton = new MenuButton();
                this.menuButton.Visible = false;
                this.menuButton.MaxSize.Set(20);
                this.menuButton.MenuAlignment = MenuAlignment.BottomRight;
                this.menuButton.TwoStrip = false;
                this.menuButton.Dock = DockStyle.Right;
                this.menuButton.InitMenu += (s,c) =>
                {
                    c.Padding.Set(4);
                    c.Clear();
                    c.Padding.Set(10);
                    c.Inflate.height = 10;

                    foreach (Connection connection in this.wallet.Connections)
                        c.Add(new ConnectionContainer(connection, this.ShowForm));

                };
                this.menuButton.CreateContainer += () =>
                {
                    return new AnyView();
                };


                this.wallet.Connections.Added += Connections_Added;
                this.wallet.Connections.Removed += Connections_Removed;
            }

            protected override void OnDisposed()
            {
                this.wallet.Connections.Added -= Connections_Added;
                this.wallet.Connections.Removed -= Connections_Removed;
                base.OnDisposed();
            }

            private void ShowForm(Connection connection)
            {
                new TonConnectionForm(connection).Show(this, MenuAlignment.Bottom);
            }

            private void Connections_Added(object sender, Connection connection)
            {
                this.UpdateConnection(connection);
                connection.Disconnected += (s) => (s as Connection).Dispose();
                connection.SendTransactions += Connection_SendTransactions;
            }

            private void Connection_SendTransactions(object sender, string reqID, ContractDeployData[] datas)
            {
                Application.Invoke(() =>
                {
                    this.Form.Restore();
                    this.Form.Activate();
                    Timer.Delay(300, ()=> new TonMultiSendForm(sender as Connection, reqID, datas).Show(this, MenuAlignment.Bottom));
                });
            }

            private void Connections_Removed(object sender, Connection connection)
            {
                this.UpdateConnection(this.wallet.Connections.Last);
            }

            private void UpdateConnection(Connection connection)
            {
                if (this.connectionContainer != null)
                {
                    this.connectionContainer.Remove(this.menuButton);
                    this.Remove(this.connectionContainer);
                    this.connectionContainer.Dispose();
                }
                if (connection != null)
                {
                    this.connectionContainer = new ConnectionContainer(connection, this.ShowForm);
                    this.connectionContainer.Add(this.menuButton);
                    this.connectionContainer.Dock = DockStyle.Fill;
                    this.Add(this.connectionContainer);
                }
                this.menuButton.Visible = this.wallet.Connections.Count > 1;
                this.Visible = connection != null;

            }

            private TonWallet wallet;
            private MenuButton menuButton;
            private ConnectionContainer connectionContainer;
        }
    }
}
