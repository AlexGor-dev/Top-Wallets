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

        private ColorButton changeOwnerButton;
        private ColorButton changeContentButton;

        public void TransferEditorship(string passcode, long queryId, string newEditorAddress, UInt128 forwardAmount, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemTransferEditorship(queryId, this.Address, newEditorAddress, this.Parent.Address, forwardAmount), resultHanler);
        }

        public void ChangeContent(string passcode, long queryId, string itemContent, RoyaltyParams royaltyParams, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemChangeContent(queryId, this.Address, itemContent, royaltyParams), resultHanler);
        }

        public override void ChangeOwner(string passcode, long queryId, string newOwner, UInt128 forwardAmount, ParamHandler<object, string> resultHanler)
        {
            this.SendMessage(passcode, NftController.CreateItemTransfer(queryId, this.Address, newOwner, this.Parent.Address, forwardAmount), resultHanler);
        }

        public override void ChangeOwnerCalcFee(long queryId, string newOwner, UInt128 forwardAmount, ParamHandler<Balance, string> resultHanler)
        {
            this.CalcFees(NftController.CreateItemTransfer(queryId, this.Address, newOwner, this.Parent.Address, forwardAmount), resultHanler);
        }

        public override void CalcFees(string destAddress, decimal amount, string message, ParamHandler<Balance, string> resultHanler)
        {
            resultHanler(new Gram(NftController.MessageTransferFee), null);
        }

        protected override bool CheckInfoChanged()
        {
            return base.CheckInfoChanged() || this.PrevInfo is NftSingleInfo ps && this.Info is NftSingleInfo cs && ps.EditorAddress != cs.EditorAddress;
        }

        protected override void OnInfoChanged()
        {
            if (this.changeOwnerButton != null)
            {
                this.changeOwnerButton.Parent.Remove(this.changeOwnerButton);
                this.changeOwnerButton.Dispose();
                this.changeOwnerButton = null;
            }

            if (this.changeContentButton != null)
            {
                this.changeContentButton.Parent.Remove(this.changeContentButton);
                this.changeContentButton.Dispose();
                this.changeContentButton = null;
            }

            base.OnInfoChanged();
        }

        public override ColorButton CreateMainLeftButton()
        {
            if (this.Info != null && this.Info.OwnerAddress == this.Parent.Address)
            {
                changeOwnerButton = new ColorButton("changeOwner");
                changeOwnerButton.Padding.Set(6);
                changeOwnerButton.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                changeOwnerButton.Radius = 6;
                changeOwnerButton.Executed += (s) => new ChangeOwnerForm(this).Show(s as Component, MenuAlignment.Bottom);
                return changeOwnerButton;
            }
            return null;
        }

        public override ColorButton CreateMainRightButton()
        {
            if (this.Info is NftSingleInfo single && single.EditorAddress == this.Parent.Address)
            {
                changeContentButton = new ColorButton("changeContent");
                changeContentButton.Padding.Set(6);
                changeContentButton.Enabled = this.Adapter.IsConnected && this.State != WalletState.None;
                changeContentButton.Radius = 6;
                changeContentButton.Executed += (s) => new ChangeContentNftForm(this).Show(s as Component, MenuAlignment.Bottom);
                return changeContentButton;
            }
            return null;
        }
    }
}
