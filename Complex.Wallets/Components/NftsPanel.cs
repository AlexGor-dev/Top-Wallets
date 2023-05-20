using System;
using Complex.Controls;
using Complex.Collections;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class NftsPanel : WalletBasePanel
    {
        protected NftsPanel(IData data)
            : base(data)
        {

        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public NftsPanel(Wallet wallet)
            :base(wallet)
        {
            this.Init();
        }

        private void Init()
        {

        }

        private static readonly Font font = Font.Create(20, FontStyle.Bold);

        private readonly Rect clientRect = new Rect();

        protected override void OnSizeChanged()
        {
            GetClientRectangle(clientRect);
            base.OnSizeChanged();
        }

        protected override void OnDrawBack(Graphics g)
        {
            base.OnDrawBack(g);
            g.DrawTextExclude(Language.Current["notImplemented"], font, clientRect, ParentBackColor, -15, 25, ContentAlignment.Center);

        }
    }
}
