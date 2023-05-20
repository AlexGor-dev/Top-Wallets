using System;
using Complex.Controls;
using Complex.Trader;
using Complex.Themes;
using Complex.Animations;

namespace Complex.Wallets
{
    public class BuySellMenu : Menu
    {
        public BuySellMenu(WalletAdapter adapter, BuySellActor[] actors, OrderAction action)
            :base(new MainPanel(adapter, actors, action))
        {
            this.MinimumSize.Set(200, 300);
            this.AnimationMode = true;
        }

        private class MainPanel : TabStopContainer, IAnimationComponent
        {
            public MainPanel(WalletAdapter adapter, BuySellActor[] actors, OrderAction action)
            {
                this.actors = actors;
                this.action = action;

                Caption caption = new Caption(Language.Current["OrderAction." + action] + " " + adapter.Symbol + " coins");
                caption.Font = Theme.font11Bold;
                caption.Dock = DockStyle.Top;
                caption.MinHeight = 30;
                this.Add(caption);

                anyView = new AnyViewAnimation();
                anyView.ShowAnimationMode = AnimationComponentMode.RotateTopAxis;
                anyView.Padding.Set(16, 4, 16, 4);
                anyView.Inflate.Set(0, 2);
                anyView.Dock = DockStyle.Fill;
                anyView.MaxHeight = 300;
                anyView.VScrollStep = 20;

                foreach (BuySellActor info in this.actors)
                {
                    if ((info.Action & action) != 0)
                    {
                        LargeButton button = new LargeButton(null, info.Name, action == OrderAction.Buy ? info.BuyText : info.SellText);
                        button.ImageComponent.Image = info.Image;
                        button.Tag = info;
                        button.DescComponent.MultilineLenght = 40;
                        button.Executed += (s) =>
                        {
                            this.Form.Hide();
                            BuySellActor buySellInfo = (s as LargeButton).Tag as BuySellActor;
                            WinApi.ShellExecute(this.Form, action == OrderAction.Buy ? buySellInfo.UrlBuy : buySellInfo.UrlSell);
                        };
                        anyView.Add(button);
                    }
                }

                this.Add(anyView);
            }

            private BuySellActor[] actors;
            private OrderAction action;
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
