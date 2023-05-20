using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Collections;
using Complex.Wallets;

namespace Complex.Ton
{
    public partial class CreateWalletForm : CaptionForm
    {
        public CreateWalletForm(TonAdapter adapter, ParamHandler<Wallet, string> paramHandler)
            : base(new SwitchContainer(false))
        {
            this.adapter = adapter;
            this.paramHandler = paramHandler;
            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 620);

            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);

            this.mainPanel = new MainPanel(this);
            this.switchContainer.Current = this.mainPanel;
        }

        protected override void OnDisposed()
        {
            this.controller.Dispose();
            base.OnDisposed();
        }

        private TonAdapter adapter;
        private ParamHandler<Wallet, string> paramHandler;
        private SwitchContainer switchContainer;
        private MainPanel mainPanel;
        private SwitchFormController controller;
        private KeyData keyData;
        private PasswordPanel passwordPanel;

        private string GetSymbolCoinsText()
        {
            return adapter.Symbol + " coins";
        }

        protected override void CloseCheck()
        {
            if (keyData != null)
                this.adapter.DeleteKey(keyData.PublicKey.keyData, keyData.Secret);
            base.CloseCheck();
        }
        private void ShowPasswodPanel()
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(true, null, CloseCheck);
                this.passwordPanel.DescriptionID = "setWalletPassword";
                this.passwordPanel.Complete += (s) =>
                {
                    this.controller.Wait("waitCreateWalletCaption", " " + adapter.Symbol + " coins", "waitCreateWalletDescription", null);
                    Timer.Delay(300, () =>
                    {
                        SingleThread.Run(() =>
                        {
                            var (w, e) = this.adapter.GetWallet(keyData.Address, this.passwordPanel.Passcode, keyData.PublicKey, keyData.DataToEncrypt);
                            if (w != null)
                            {
                                Application.Invoke(() =>
                                {
                                    this.switchContainer.Current = new DoneWalletPanel(w, UniqueHelper.NextName("Wallet", WalletsData.Wallets), () =>
                                    {
                                        paramHandler(w, null);
                                        this.Close();
                                    });
                                });
                            }
                            else
                            {
                                this.OnError("", e, true, () =>
                                {
                                });
                            }
                        });
                    });
                };
            }
            this.switchContainer.Current = this.passwordPanel;
        }

        private void OnError(string caption, string description, bool hidegoback, EmptyHandler back)
        {
            this.controller.Error(adapter.ThemeColor, caption, description, hidegoback, back);
        }

        private void OnError(string caption, string description, bool hidegoback)
        {
            this.OnError(caption, description, hidegoback, null);
        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(CreateWalletForm form)
                :base(Language.Current["coinWallet", form.GetSymbolCoinsText()], null, Language.Current["walletInfo", form.GetSymbolCoinsText()], null, form.CloseCheck)
            {
                this.form = form;

                this.Add(new Dummy(DockStyle.Top, 0, 70));

                AdapterLabel adapterLabel = new AdapterLabel(form.adapter, form.adapter);
                adapterLabel.Dock = DockStyle.Top;
                adapterLabel.MinHeight = 150;
                this.Add(adapterLabel);


                createWalletButton = new ColorButton("createWallet");
                createWalletButton.BoxColor = form.adapter.ThemeColor;
                createWalletButton.Enabled = form.adapter.IsConnected;
                createWalletButton.Dock = DockStyle.Bottom;
                createWalletButton.MinHeight = 40;
                createWalletButton.Executed += (s) =>
                {
                    form.CreateWallet();
                };
                this.Add(createWalletButton);

                this.Add(new Separator(DockStyle.Bottom, 20));

                importWalletButton = new ColorButton("importWallet");
                importWalletButton.Enabled = form.adapter.IsConnected;
                importWalletButton.BoxColor = form.adapter.ThemeColor;
                importWalletButton.Dock = DockStyle.Bottom;
                importWalletButton.MinHeight = 40;
                importWalletButton.Executed += (s) =>
                {
                    form.ImportWallet();
                };
                this.Add(importWalletButton);

                this.Add(new Separator(DockStyle.Bottom, 20));


                importWalletFromFileButton = new ColorButton("importWalletFromFile");
                importWalletFromFileButton.BoxColor = form.adapter.ThemeColor;
                importWalletFromFileButton.Dock = DockStyle.Bottom;
                importWalletFromFileButton.MinHeight = 40;
                importWalletFromFileButton.Executed += (s) =>
                {
                    form.ImportWalletFromFile();
                };
                this.Add(importWalletFromFileButton);

                form.adapter.Connected += Adapter_Changed;
                form.adapter.Disconnected += Adapter_Changed;
            }

            protected override void OnDisposed()
            {
                form.adapter.Connected -= Adapter_Changed;
                form.adapter.Disconnected -= Adapter_Changed;
                base.OnDisposed();
            }

            private void Adapter_Changed(object sender)
            {
                createWalletButton.BoxColor = form.adapter.ThemeColor;
                importWalletButton.BoxColor = form.adapter.ThemeColor;
                importWalletFromFileButton.BoxColor = form.adapter.ThemeColor;
                createWalletButton.Enabled = form.adapter.IsConnected;
                importWalletButton.Enabled = form.adapter.IsConnected;
                importWalletFromFileButton.Invalidate();
            }


            private CreateWalletForm form;
            private ColorButton createWalletButton;
            private ColorButton importWalletButton;
            private ColorButton importWalletFromFileButton;
        }
    }
}
