using System;
using Complex.Themes;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;

namespace Complex.Ton
{
    public class NftItem : NftWallet
    {
        protected NftItem(IData data)
            : base(data)
        {

        }

        public NftItem(string adapterID, string address, NftInfo data, TonWallet parent)
            :base(adapterID, address, data, parent)
        {
        }

        public void GetStaticData(string passcode, long queryId, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemGetStaticData(this.Address, queryId), resultHanler);
        }

        public void GetRoyaltyParams(string passcode, long queryId, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemGetRoyaltyParams(this.Address, queryId), resultHanler);
        }

        public void TransferEditorship(string passcode, long queryId, string newEditorAddress, UInt128 forwardAmount, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemTransferEditorship(this.Address, queryId, newEditorAddress, this.Parent.Address, forwardAmount), resultHanler);
        }

        public void ChangeContent(string passcode, long queryId, string itemContent, RoyaltyParams royaltyParams, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemChangeContent(this.Address, queryId, itemContent, royaltyParams), resultHanler);
        }

        public override ColorButton CreateMainLeftButton()
        {
            if (this.Info != null && this.Info.Owner == this.Parent.Address)
            {
                ColorButton button = new ColorButton("transferItem");
                button.Padding.Set(6);
                button.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                button.Radius = 6;
                button.Executed += (s) =>
                {
                    MessageView.Show("notImplemented");
                };
                return button;
            }
            return null;
        }

        public override ColorButton CreateMainRightButton()
        {
            if (this.Type == WalletType.NftSingle && this.Info != null && this.Info.Owner == this.Parent.Address)
            {
                ColorButton button = new ColorButton("changeContent");
                button.Padding.Set(6);
                button.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                button.Radius = 6;
                button.Executed += (s) =>
                {
                    MessageView.Show("notImplemented");
                };
                return button;
            }
            return null;
        }
    }
}
