using System;
using Complex.Controls;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class MainForm : ComponentForm
    {
        static MainForm()
        {
        }

        protected MainForm(IData data) 
            : base(data)
        {
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            Init();
        }

        [ProtectedIgnory]
        public MainForm() : base(new MainContainer())
        {
            this.MinimumSize.Set(1100, 800);
            this.SetBounds(0, 0, 1200, 800);
            this.Text = Resources.ProductFullName;
            this.StartPosition = FormStartPosition.Manual;
            this.AllowDrop = true;
            this.EffectMode = EffectMode.Blur;

            Init();
        }

        private void Init()
        {
            this.Image = Images.GetBitmap("top_wallets_48.png");
        }
    }
}
