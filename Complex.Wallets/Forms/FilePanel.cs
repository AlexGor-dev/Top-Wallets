using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Navigation;
using Complex.Drawing;
using Complex.Files;

namespace Complex.Wallets
{
    public class FilePanel : CaptionPanel
    {
        public FilePanel(FilesDialogType type, Wallet wallet, string captionID, string descriptionID, ParamHandler<string> complete, EmptyHandler goBack, EmptyHandler cancel)
            :base(captionID, null, descriptionID, goBack, cancel)
        {

            filePanel = FileDialogPanel.Open("exportWallet", type);
            filePanel.MultiSelect = false;
            filePanel.Dock = DockStyle.Fill;
            filePanel.Filter = "Top-Wallets Export(*.twe)|*.twe;";
            if (filePanel.InitialDirectory == null || !System.IO.Directory.Exists(filePanel.InitialDirectory))
            {
                filePanel.InitialDirectory = Resources.LocalApplicationData + "Wallets\\";
                System.IO.Directory.CreateDirectory(filePanel.InitialDirectory);
            }
            if (type == FilesDialogType.SaveFile)
            {
                if (!string.IsNullOrEmpty(wallet.OriginalName))
                    filePanel.FileName = wallet.OriginalName + ".twe";
                else
                    filePanel.FileName = File.GetNextName("Wallet", filePanel.InitialDirectory, "*.twe") + ".twe";
            }

            filePanel.OkExecuted += (s2) => { complete(filePanel.FileName); };
            filePanel.CancelExecuted += (s2) => { cancel(); };

            this.Add(filePanel);

        }

        private FileDialogPanel filePanel;
        public void Save()
        {
            this.Remove(filePanel);
            filePanel.Save();
            this.Add(filePanel);
        }

    }
}
