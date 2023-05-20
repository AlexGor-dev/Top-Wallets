using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Drawing;

namespace Complex.Wallets
{
    public partial class TotalPanel
    {
        public class ExpandButton : Controls.ExpandButton
        {
            protected ExpandButton(IData data)
                : base(data)
            {

            }

            public ExpandButton(bool isChecked)
                : base(isChecked)
            {
                this.Padding.Set(0);
            }

            protected override void OnMeasure(float widthMeasure, float heightMeasure)
            {
                this.SetMeasured(32, 16);
            }

            protected override int GetBackColor()
            {
                if (this.StateManager.State == ButtonState.None || this.StateManager.State == ButtonState.Selected)
                    return Theme.mapBackColor;
                return base.GetBackColor();
            }

            protected override int GetBorderColor()
            {
                return 0;
            }

            protected override void OnDrawBack(Graphics g)
            {
                this.StateManager.DrawV(g, clientRect, 0, clientRect.height / 3, this.GetBackColor(), GetBorderColor());
            }
        }

    }
}
