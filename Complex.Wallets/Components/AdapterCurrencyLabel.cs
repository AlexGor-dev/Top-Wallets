using System;
using Complex.Controls;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class AdapterCurrencyLabel : WalletBasePanel
    {
        public AdapterCurrencyLabel(Wallet wallet)
            :base(wallet)
        {

            this.Inflate.Set(4, 0);
            this.Padding.Set(4);
            this.MinHeight = 50;

            //this.imageComponent = new ImageComponent(wallet.Adapter.ImageID);
            //this.imageComponent.MaxSize.Set(32, 32);
            ////this.imageComponent.Image = wallet.Adapter.Extension.Image;
            //this.imageComponent.Dock = DockStyle.Left;
            ////this.Add(this.imageComponent);

            this.textCurrencyLabel = new CurrencyLabel(wallet.Balance.ToKMB(3), wallet.Symbol);
            this.textCurrencyLabel.ValueTextComponent.ForeColor = wallet.ThemeColor;
            this.textCurrencyLabel.Alignment = ContentAlignment.Left;
            this.textCurrencyLabel.Dock = DockStyle.Top;
            this.textCurrencyLabel.TextChanged += (s) => { this.Measured = false; };
            this.Add(this.textCurrencyLabel);

            this.adapterLabel = new AdapterWaitLabel(wallet.Adapter);
            this.adapterLabel.Alignment = ContentAlignment.Left;
            this.adapterLabel.Dock = DockStyle.Fill;
            this.Add(this.adapterLabel);
        }


        //private ImageComponent imageComponent;
        private CurrencyLabel textCurrencyLabel;
        private AdapterWaitLabel adapterLabel;

        //protected override void OnMeasure(float widthMeasure, float heightMeasure)
        //{
        //    this.imageComponent.Measure();
        //    this.textCurrencyLabel.Measure();
        //    this.adapterLabel.Measure();
        //    this.SetMeasured(this.Padding.horizontal + this.imageComponent.MeasuredWidth + this.Inflate.width + Math.Max(this.textCurrencyLabel.MeasuredWidth, this.adapterLabel.MeasuredWidth),
        //        this.Padding.vertical + Math.Max(this.imageComponent.MeasuredHeight, this.textCurrencyLabel.MeasuredHeight + this.Inflate.height + this.adapterLabel.MeasuredHeight));
        //}

        public void Update()
        {
            this.textCurrencyLabel.ValueTextComponent.Text = this.Wallet.Balance.ToKMB(3);
            this.textCurrencyLabel.CurrencyTextComponent.Text = this.Wallet.Symbol;
            this.textCurrencyLabel.Layout();
        }

        protected override void OnConnectionChanged()
        {
            this.textCurrencyLabel.ValueTextComponent.ForeColor = Wallet.ThemeColor;
            base.OnConnectionChanged();
        }
    }
}
