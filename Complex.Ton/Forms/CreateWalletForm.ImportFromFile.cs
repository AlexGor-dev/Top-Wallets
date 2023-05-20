using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Collections;
using Complex.Wallets;

namespace Complex.Ton
{
    public partial class CreateWalletForm
    {
        private FilePanel openFilePanel;
        private PasswordPanel walletPasswordPanel;
        private PasswordPanel exportPasswordPanel;
        private WalletInfoPanel walletInfoPanel;

        private void ImportWalletFromFile(string fileName)
        {
            try
            {
                Formatter data = new Formatter();
                data.Load(fileName);
                Wallet wallet = data["wallet"] as Wallet;
                object exportData = data["exportData"];
                if (wallet == null)
                {
                    OnError(Language.Current["invalidFile", fileName], null, false, () => this.switchContainer.Current = this.openFilePanel);
                }
                else
                {
                    WalletAdapter adapter = Controller.GetAdapter(wallet.AdapterID);
                    if (adapter == null)
                        OnError(Language.Current["notFoundAdapter", wallet.AdapterID], null, false, () => this.switchContainer.Current = this.openFilePanel);
                    else if (WalletsData.Wallets.Contains(wallet.ID))
                        OnError(Language.Current["walletAlreadyExist", wallet.Address], null, false, () => this.switchContainer.Current = this.openFilePanel);
                    else
                        this.InitImportWallet(wallet, exportData);
                    this.openFilePanel.Save();

                }
            }
            catch (Exception e)
            {
                OnError(Language.Current["invalidFile", fileName], null, false, () => this.switchContainer.Current = this.openFilePanel);
            }

        }

        private void ImportWalletFromFile()
        {
            this.controller.Wait("pleaseWait", null, null, CloseCheck);
            Application.Run(() =>
            {
                if (this.openFilePanel == null)
                {
                    this.openFilePanel = new FilePanel(Navigation.FilesDialogType.OpenFile, null, "importWalletFromFile", "", (fileName) =>
                    {
                        if (System.IO.File.Exists(fileName))
                            this.ImportWalletFromFile(fileName);
                        else
                            OnError(Language.Current["notFoundFile", fileName], null, false, () => this.switchContainer.Current = this.openFilePanel);
                    },
                    () => this.switchContainer.Current = this.mainPanel, 
                    () => OnError("importWalletCanceled", null, false, () => this.switchContainer.Current = this.openFilePanel));
                }
            },
            ()=> Timer.Delay(500, () => this.switchContainer.Next = this.openFilePanel));
        }

        private void InitImportWallet(Wallet wallet, object exportData)
        {
            if (this.walletInfoPanel == null)
                this.walletInfoPanel = new WalletInfoPanel(this);
            this.walletInfoPanel.Update(wallet, exportData);
            this.switchContainer.Current = this.walletInfoPanel;
        }

        private void EnterWalletPassword(Wallet wallet, object exportData)
        {
            if (this.walletPasswordPanel == null)
            {
                this.walletPasswordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.walletInfoPanel, CloseCheck);
                this.walletPasswordPanel.Complete += (s) =>
                {
                    this.controller.Wait("pleaseWait", null, null, CloseCheck);
                    wallet.CheckPassword(this.walletPasswordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                EnterExportDataPassword(wallet, exportData);
                            else
                                this.controller.ErrorLang("error", e, () => this.switchContainer.Current = this.walletPasswordPanel);
                        });
                    });
                };
            }
            this.walletPasswordPanel.ClearPasscode();
            this.walletPasswordPanel.DescriptionID = "enterWalletPassword";
            this.switchContainer.Next = this.walletPasswordPanel;
        }

        private void EnterExportDataPassword(Wallet wallet, object exportData)
        {
            if (this.exportPasswordPanel == null)
            {
                this.exportPasswordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.walletInfoPanel, CloseCheck);
                this.exportPasswordPanel.Complete += (s) =>
                {
                    this.controller.Wait("pleaseWait", null, null, CloseCheck);
                    wallet.ImportData(this.walletPasswordPanel.Passcode, this.exportPasswordPanel.Passcode, exportData, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e != null)
                            {
                                OnError("exportDataError", e, false, () => this.switchContainer.Current = this.walletPasswordPanel);
                            }
                            else
                            {
                                DoneWalletPanel donePanel = new DoneWalletPanel(wallet, UniqueHelper.NextName("Wallet1", WalletsData.Wallets), () =>
                                {
                                    paramHandler(wallet, null);
                                    this.CloseCheck();
                                });
                                this.switchContainer.Current = donePanel;
                            }

                        });
                    });
                };
            }
            this.exportPasswordPanel.ClearPasscode();
            this.exportPasswordPanel.DescriptionID = "enterExportDataPassword";
            this.switchContainer.Next = this.exportPasswordPanel;

        }

        private class WalletInfoPanel : InfoPanel
        {
            public WalletInfoPanel(CreateWalletForm form)
                :base(form.adapter.ThemeColor, "", "", () => form.switchContainer.Current = form.openFilePanel, form.CloseCheck, ()=> { })
            {
                this.form = form;

                anyView = new AnyView();
                anyView.Dock = DockStyle.Fill;
                anyView.VScrollStep = 20;

                this.Add(anyView);

            }

            private CreateWalletForm form;
            private AnyView anyView;
            private Wallet wallet;
            private IMultiWallet multiWallet;
            private object exportData;
            protected override void Continue()
            {
                if (this.multiWallet != null)
                    foreach (Component component in anyView.Components)
                        if (component is WalletCheckedContainer cc && !cc.Checked)
                            this.multiWallet.Wallets.Remove(cc.wallet.ID);
                form.EnterWalletPassword(this.wallet, this.exportData);
            }

            public void Update(Wallet wallet, object exportData)
            {
                this.wallet = wallet;
                this.exportData = exportData;
                this.multiWallet = null;
                if (this.wallet is IMultiWallet mw && mw.Wallets.Count > 0)
                    this.multiWallet = mw;

                this.BeginUpdate();

                this.caption.Text = Language.Current["yourWallet", wallet.Address];
                this.descriptionComponent.TextID = multiWallet == null ? "importFromFileInfo" : "selectChildsToImport";

                anyView.Components.Clear(true);
                anyView.Add(new WalletItem(this.wallet));

                if (this.multiWallet != null)
                    foreach (Wallet child in this.multiWallet.Wallets)
                        anyView.Add(new WalletCheckedContainer(child));

                this.EndUpdate();
            }

            private class WalletCheckedContainer : Container
            {
                public WalletCheckedContainer(Wallet wallet)
                {
                    this.wallet = wallet;

                    this.checkBox = new CheckBox(true);
                    this.checkBox.Dock = DockStyle.Left;
                    this.Add(checkBox);

                    Component component = wallet.CreateWalletItem();
                    component.Dock = DockStyle.Fill;
                    this.Add(component);
                }

                public readonly Wallet wallet;
                private CheckBox checkBox;

                public bool Checked => checkBox.Checked;
            }
        }
    }
}
