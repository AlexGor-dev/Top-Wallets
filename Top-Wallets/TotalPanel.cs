using System;
using Complex.Controls;
using Complex.Animations;
using Complex.Themes;
using Complex.Drawing;
using Complex.Trader;
using Complex.Collections;

namespace Complex.Wallets
{
    public partial class TotalPanel : Container, IEndAnimation
    {
        protected TotalPanel(IData data)
            : base(data)
        {

        }

        protected override void Load(IData data)
        {
            base.Load(data);
            this.expandButton = data["expandButton"] as ExpandButton;
            this.topSupports = data["topSupports"] as UniqueCollection<TopSupport>;
            this.balloon = data["balloon"] as BalloonComponent;
            this.lastMessages = data["lastMessages"] as UniqueCollection<TopSupport>;
        }

        protected override void Save(IData data)
        {
            base.Save(data);
            data["expandButton"] = this.expandButton;
            data["topSupports"] = this.supportPanel.TopSupports;
            data["balloon"] = this.supportPanel.Balloon;
            data["lastMessages"] = this.supportPanel.LastMessages;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            this.Init();
        }

        public TotalPanel()
        {
            this.SaveComponents = false;
            this.Padding.Set(10, 4, 10, 4);
            this.Inflate.width = 6;
            this.MinHeight = 64;
            this.AnimationPos = this.MinHeight;
            this.expandButton = new ExpandButton(false);
            this.expandButton.MaxSize.Set(28, 28);

            this.topSupports = new UniqueCollection<TopSupport>();
            this.lastMessages = new UniqueCollection<TopSupport>();

            this.Init();
        }

        private void Init()
        {
            this.Measured = false;
            this.animator = new Animator(this);

            this.realPanel = new TotalContainer(false);
            this.realPanel.Dock = DockStyle.Left;
            this.Add(this.realPanel);

            this.leftPanel = new CurrenciesPanel(0);
            this.leftPanel.Dock = DockStyle.Left;
            this.Add(this.leftPanel);


            this.supportPanel = new SupportContainer(this.topSupports, this.lastMessages, this.balloon);
            this.supportPanel.Dock = DockStyle.Fill;
            this.Add(this.supportPanel);


            this.rightPanel = new CurrenciesPanel(2);
            this.rightPanel.Dock = DockStyle.Right;
            this.Add(this.rightPanel);


            this.testPanel = new TotalContainer(true);
            this.testPanel.Dock = DockStyle.Right;
            this.Add(this.testPanel);



            this.expandButton.ToolTipInfo = new ToolTipInfo(this.expandButton.Checked ? "hideTotalPanel" : "showTotalPanel");
            this.expandButton.CheckedChanged += (s) =>
            {
                if (this.expandButton.Checked)
                {
                    this.expandButton.ToolTipInfo = new ToolTipInfo("hideTotalPanel");
                    this.animator.Start(-1);
                }
                else
                {
                    this.expandButton.ToolTipInfo = new ToolTipInfo("showTotalPanel");
                    this.animator.Start(1);
                }
            };

        }


        private Animator animator;
        private ExpandButton expandButton;
        public ExpandButton Button => expandButton;

        public bool Expanded => this.expandButton.Checked;

        private TotalContainer realPanel;
        private TotalContainer testPanel;
        private SupportContainer supportPanel;
        private CurrenciesPanel leftPanel;
        private CurrenciesPanel rightPanel;
        private BalloonComponent balloon;

        private UniqueCollection<TopSupport> topSupports;
        private UniqueCollection<TopSupport> lastMessages;


        protected override Type GetDefaultTheme()
        {
            return typeof(MapBackTheme);
        }

        protected override void OnBoundsChanged(bool locationChanged, bool sizeChinged)
        {
            this.expandButton.Measure();
            this.expandButton.SetBounds(this.Left + (this.Width - this.expandButton.MeasuredWidth) / 2, this.Bottom - 1, this.expandButton.MeasuredWidth, this.expandButton.MeasuredHeight);
            base.OnBoundsChanged(locationChanged, sizeChinged);
        }

        void IAnimation.OnAnimation(Animator animator, float value)
        {
            this.AnimationPos = Helper.GetValue(0, this.MeasuredHeight, value);
            this.Parent.Layout();
        }

        void IEndAnimation.OnEndAnimation(Animator animator, float value)
        {
            this.Update();
        }

        public void Update()
        {
            this.expandButton.BringToFront();
            this.supportPanel.Update();
            if (this.Expanded)
            {
                this.realPanel.Update();
                this.testPanel.Update();
            }
        }
    }
}
