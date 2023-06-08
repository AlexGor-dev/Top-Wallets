using System;
using Complex.Controls;
using Complex.Wallets;
using Complex.Themes;

namespace Complex.Ton
{
    public partial class CreateNftController
    {
        private NftCollectionEnterInfoPanel enterCollectionInfoPanel;

        private void CreateCollection()
        {
            if (enterCollectionInfoPanel == null)
                enterCollectionInfoPanel = new NftCollectionEnterInfoPanel(this.wallet, "createNftCollection", () => switchContainer.Current = mainPanel, closeHandler, wallet.ThemeColor, () => Wait(null, "pleaseWait"), (info) =>
                {
                });
            this.switchContainer.Current = this.enterCollectionInfoPanel;
        }
    }
}
