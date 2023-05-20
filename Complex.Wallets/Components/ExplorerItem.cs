using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Wallets
{
    public class ExplorerItem : AdapterItem, IExplorerSource
    {
        protected ExplorerItem(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.explorer = data["explorer"] as Explorer;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["explorer"] = this.explorer;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public ExplorerItem(Explorer explorer)
            :base(explorer)
        {
            this.explorer = explorer;
            //this.MinHeight = 70;
            this.Init();
        }

        private void Init()
        {
            adapterLabel = new AdapterLabel(this.Adapter);
            //adapterLabel.DescComponent.Style = Theme.Get<CaptionForeTheme>();
            adapterLabel.TextComponent.Alignment = ContentAlignment.Left;
            adapterLabel.DescComponent.Alignment = ContentAlignment.Left;
            adapterLabel.Dock = DockStyle.Left;
            this.Add(adapterLabel);
        }

        private AdapterLabel adapterLabel;

        private Explorer explorer;
        public Explorer Explorer => explorer;

        protected override void OnDrawBack(Graphics g)
        {
            base.OnDrawBack(g);
        }
        private class AdapterLabel : LargeLabelBase
        {
            public AdapterLabel(WalletAdapter adapter)
                : base(null, "Explorer " + (adapter.IsTestnet ? "Test " : "") + adapter.Symbol + " wallets", adapter, false)
            {
            }

            protected override TextComponent CreateTextComponent(object textParam)
            {
                return new TextLocalizeComponent(textParam as string);
            }

            protected override TextComponent CreateDescComponent(object decsParam)
            {
                return new AdapterWaitLabel(decsParam as WalletAdapter);
            }

        }
    }
}
