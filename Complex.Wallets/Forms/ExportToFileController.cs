using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Navigation;
using Complex.Files;

namespace Complex.Wallets
{
    public class ExportToFileController : WalletController
    {
        public ExportToFileController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
            :base(wallet, switchContainer, passcode, closeHandler, doneHandler)
        {
        }

        private InfoPanel infoPanel;
        private FilePanel savePanel;

        public override void Start()
        {
            if (infoPanel == null)
                infoPanel = new InfoPanel(wallet.ThemeColor, Language.Current["exporToFile", wallet.Name], "exporToFileDescription", null, closeHandler, SetPassword);
            this.switchContainer.Current = infoPanel;
        }

        private void SetPassword()
        {
            PasswordPanel panel = new PasswordPanel(true, () => { this.switchContainer.Current = infoPanel; }, closeHandler);
            panel.DescriptionID = "setExportDataPassword";
            panel.Complete += (s) =>
            {
                Wait(wallet.Adapter, "waitExportDataCaption", "waitExportDataDescription");
                Timer.Delay(300, () =>
                {
                    this.wallet.ExportData(this.passcode, (s as PasswordPanel).Passcode, (exportData, error) =>
                    {
                        if (error != null)
                        {
                            Error("errorGetExportData", error, false, doneHandler);
                        }
                        else
                        {
                            Formatter data = new Formatter();
                            data["wallet"] = this.wallet;
                            data["exportData"] = exportData;

                            savePanel = new FilePanel(FilesDialogType.SaveFile, wallet, Language.Current["exporToFile", wallet.Name], null,  (fileName) =>
                            {
                                data.Save(fileName, true);
                                Done("perfect", Language.Current["exportWalletDone"], "close", wallet.ThemeColor, doneHandler);
                                savePanel.Save();
                            },null, () => 
                            { 
                                Error("exportWalletCancelled", "", false, ()=> { this.switchContainer.Current = savePanel; });
                            });
                            this.switchContainer.Current = savePanel;
                        }
                    });
                });
            };
            this.switchContainer.Current = panel;
        }

    }
}
