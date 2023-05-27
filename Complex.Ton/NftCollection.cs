using System;
using Complex.Themes;
using Complex.Wallets;
using Complex.Drawing;
using Complex.Controls;

namespace Complex.Ton
{
    public class NftCollection : NftWallet
    {
        protected NftCollection(IData data)
            : base(data)
        {

        }

        public NftCollection(string adapterID, string address, NftInfo data, TonWallet parent)
            :base(adapterID, address, data, parent)
        {
        }

        public void MintItems(string passcode, long queryId, NftMintItem[] items, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateMintData(this.Address, queryId, items), resultHanler);
        }

        public void ChangeOwner(string passcode, long queryId, string newOwner, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateChangeOwner(this.Address, queryId, newOwner), resultHanler);
        }

        public void ChangeContent(string passcode, long queryId, string collectionContent, string commonContent, RoyaltyParams royaltyParams, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateChangeContent(this.Address, queryId, collectionContent, commonContent, royaltyParams), resultHanler);
        }

        public void GetRoyaltyParams(string passcode, long queryId, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateGetRoyaltyParams(this.Address, queryId), resultHanler);
        }

        public override ColorButton CreateMainLeftButton()
        {
            if (this.Info != null && this.Info.Owner == this.Parent.Address)
            {
                ColorButton button = new ColorButton("changeContract");
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
            if (this.Info != null && this.Info.Owner == this.Parent.Address)
            {
                ColorButton button = new ColorButton("mintNft");
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
