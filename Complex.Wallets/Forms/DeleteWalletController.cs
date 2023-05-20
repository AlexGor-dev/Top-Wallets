using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class DeleteWalletController : WalletController
    {
        public DeleteWalletController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
            : base(wallet, switchContainer, passcode, closeHandler, doneHandler)
        {
            this.mainPanel = new MainPanel(this);
        }

        protected override void OnDisposed()
        {
            if (this.exportWords != null)
                this.exportWords.Dispose();
            if (this.exportToFile != null)
                this.exportToFile.Dispose();
            base.OnDisposed();
        }
        private MainPanel mainPanel;
        private ExportWordsController exportWords;
        private ExportToFileController exportToFile;
        private InfoPanel infoPanel;

        public override void Start()
        {
            this.switchContainer.Current = this.mainPanel;
        }

        private void DeleteWallet()
        {
            this.infoPanel = new InfoPanel(wallet.ThemeColor, "removingWallet", Language.Current["deleteWalletInfo", wallet.Name, wallet is IMultiWallet m && m.Wallets.Count > 0 ? Language.Current["deleteChildWallets"] : ""], null, closeHandler, () =>
            {
                Wait(wallet.Adapter, "waitDeleteWalletCaption", "waitDeleteWalletDescription");
                SingleThread.Run(() =>
                {
                    wallet.Delete(passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (wallet is IMultiWallet mw)
                            {
                                foreach (Wallet w in mw.Wallets)
                                    WalletsData.Wallets.Remove(w);
                            }
                            WalletsData.Wallets.Remove(wallet);
                            Done("perfect", Language.Current["walletDeletedSuccessfully"], "close", wallet.ThemeColor, doneHandler);
                        });
                    });
                });
            });
            this.switchContainer.Current = this.infoPanel;
        }

        private void WriteSecretWords()
        {
            if (exportWords == null)
                exportWords = new ExportWordsController(wallet, switchContainer, passcode, closeHandler, Start);
            exportWords.Start();
        }

        private void ExportToFile()
        {
            if (exportToFile == null)
                exportToFile = new ExportToFileController(wallet, switchContainer, passcode, closeHandler, Start);
            exportToFile.Start();
        }

        private class MainPanel : InfoPanel
        {
            public MainPanel(DeleteWalletController controller)
                :base(controller.wallet.ThemeColor, Language.Current["deletingWallet", controller.wallet.Name], "deleteWalletDesc", "deleteWallet", null, controller.closeHandler, controller.DeleteWallet)
            {
                this.controller = controller;

                WalletItem item = new WalletItem(controller.wallet);
                item.Dock = DockStyle.Top;
                this.Insert(1, item);

                ColorButton button = new ColorButton("writeSecretWords");
                button.BoxColor = controller.wallet.ThemeColor;
                button.Dock = DockStyle.Top;
                button.MinHeight = 40;
                button.Executed += (s) => { controller.WriteSecretWords(); };
                this.Add(button);

                button = new ColorButton("exporToFile2");
                button.BoxColor = controller.wallet.ThemeColor;
                button.Dock = DockStyle.Top;
                button.MinHeight = 40;
                button.Executed += (s) => { controller.ExportToFile(); };
                this.Add(button);

            }

            private DeleteWalletController controller;
        }
    }
}
