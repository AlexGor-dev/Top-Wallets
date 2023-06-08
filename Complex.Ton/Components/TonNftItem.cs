using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Animations;
using Complex.Wallets;
using System.Threading.Tasks;

namespace Complex.Ton
{
    public class TonNftItem : NftInfoItem
    {
        public TonNftItem(TonUnknownWallet wallet, NftInfo nft, GridWaitEffect waitEffect)
            : base(wallet, nft, waitEffect)
        {

            if (!string.IsNullOrEmpty(nft.CollectionAddress))
            {
                string name = nft.CollectionName;
                if (name == null)
                {
                    Wallet wt = WalletsData.GetAnyWallet(wallet.AdapterID, nft.CollectionAddress);
                    if (wt != null)
                        name = wt.Name;
                }

                TextButton addressButton = new TextButton(name != null ? name : Controller.GetKnownAddress(wallet.Adapter, nft.CollectionAddress));
                addressButton.ToolTipInfo = new ToolTipInfo("Nft collection", name);
                addressButton.DrawBorder = false;
                addressButton.Padding.Set(4);
                addressButton.MaxWidth = 250;
                addressButton.MaxHeight = 24;
                addressButton.Font = Theme.font10Bold;
                addressButton.Executed += (s) =>
                {
                    if (this.waitEffect != null)
                        Controller.ShowAnyWallet(wallet.Adapter, wallet.Symbol, nft.CollectionAddress, () => this.waitEffect.Start(), () => this.waitEffect.Stop());
                };
                addressButton.Dock = DockStyle.Bottom;
                this.Add(addressButton);
            }
        }
    }
}
