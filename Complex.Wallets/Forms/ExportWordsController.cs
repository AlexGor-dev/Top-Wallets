using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class ExportWordsController : WalletController
    {
        public ExportWordsController(Wallet wallet, SwitchContainer switchContainer, string passcode, EmptyHandler closeHandler, EmptyHandler doneHandler)
            : base(wallet, switchContainer, passcode, closeHandler, doneHandler)
        {
        }

        public override void Start()
        {
            WaitPanel waitPanel = new WaitPanel("waitCreateWordsCaption", " " + wallet.Symbol + " coins", "waitCreateWordsDescription", null);
            waitPanel.StartWait();
            this.switchContainer.Current = waitPanel;
            Timer.Delay(300, () =>
            {
                wallet.GetWords(this.passcode, (ws, e) =>
                {
                    if (e != null)
                    {
                        this.Error("error", e, true, null);
                    }
                    else
                    {
                        WordsPanel wordsPanel = new WordsPanel(wallet.Adapter, false, null, null, closeHandler, doneHandler);
                        wordsPanel.UpdateWorts(ws);
                        this.switchContainer.Current = wordsPanel;
                    }
                });
            });

        }
    }
}
