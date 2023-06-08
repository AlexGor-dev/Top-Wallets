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
        private WordsPanel importWordsPanel;
        private TestWordsPanel testWordsPanel;
        private void ImportWallet()
        {
            if (this.importWordsPanel == null)
                this.importWordsPanel = new WordsPanel(this.adapter, true, null, () => switchContainer.Current = mainPanel, CloseCheck, () => ImportWallet(this.importWordsPanel.GetWords()));
            this.switchContainer.Current = this.importWordsPanel;
        }

        private void ImportWallet(string[] words)
        {
            this.controller.Wait("waitImportCaption", "waitImportDescription");
            SingleThread.Run(() =>
            {
                var (kd, e) = adapter.GetKeyData(words);
                if (e != null)
                {
                    if (e.IndexOf("addressNotFoundFromPublicKey") != -1)
                        e = Language.Current["addressNotFoundFromPublicKey"];
                    this.OnError("importWalletError", e, false, ImportWallet);
                }
                else
                {
                    this.keyData = kd;
                    Timer.Delay(300, () =>
                    {
                        if (WalletsData.Wallets.Contains(Wallet.GetID(adapter, keyData.Address, true)))
                        {
                            this.OnError(Language.Current["walletAlreadyExist", keyData.Address], "", true);
                            paramHandler(null, "walletAlreadyExist");
                        }
                        else
                        {
                            this.switchContainer.Current = new DonePanel("congratulations", Language.Current["walletCongratulationsinfo", GetSymbolCoinsText()], null, CloseCheck, "setPasscode", adapter.ThemeColor, ShowPasswodPanel);
                        }
                    });
                }
            });
        }



    }
}
