using System;
using Complex.Controls;
using Complex.Trader;
using Complex.Themes;
using Complex.Animations;
using Complex.Collections;

namespace Complex.Ton
{
    public class DAppsMenu : Menu
    {
        public DAppsMenu(Array<DApp> dApps)
            :base(new MainPanel(dApps))
        {
            this.MinimumSize.Set(200, 300);
            this.AnimationMode = true;

        }

        private class MainPanel : TabStopContainer, IAnimationComponent
        {
            public MainPanel(Array<DApp> dApps)
            {
                this.dApps = dApps;

                anyView = new AnyViewAnimation();
                anyView.ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
                anyView.Padding.Set(16, 4, 16, 4);
                anyView.Inflate.Set(0, 4);
                anyView.Dock = DockStyle.Fill;
                anyView.MaxHeight = 300;
                anyView.VScrollStep = 20;

                foreach (DApp dapp in this.dApps)
                {
                    LargeButton button = new LargeButton(null, dapp.Name, dapp.Description);
                    button.Inflate.width = 8;
                    button.ImageComponent.MaxSize.Set(32);
                    button.ImageComponent.Image = dapp.LoadImage((image) => { button.ImageComponent.Image = image; button.Invalidate(); });
                    button.DescComponent.MultilineLenght = 40;
                    button.Tag = dapp;
                    button.Executed += (s) =>
                    {
                        this.Form.Hide();
                        DApp d = (s as LargeButton).Tag as DApp;
                        WinApi.ShellExecute(this.Form, d.Url);
                    };
                    anyView.Add(button);
                }

                this.Add(anyView);
            }

            private Array<DApp> dApps;
            private AnyViewAnimation anyView;

            void IAnimationComponent.ClearMode()
            {
                (this.anyView as IAnimationComponent).ClearMode();
            }

            void IAnimationComponent.Restart()
            {
                (this.anyView as IAnimationComponent).Restart();
            }


        }

    }
}
