using System;
using Complex.Controls;
using Complex.Themes;
using Complex.Navigation;
using Complex.Collections;
using System.Globalization;
using Complex.Drawing;

namespace Complex.Wallets
{
    public class MessagesMenu : Menu
    {
        protected MessagesMenu(IData data)
            : base(data)
        {

        }

        public MessagesMenu()
            :base(new MainPanel())
        {
            this.MinimumSize.Set(300, 200);
            this.AnimationMode = true;
            this.HideOnClose = true;
            this.Sizable = true;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
        protected override HitTest OnHittest(float x, float y)
        {
            HitTest hitTest = base.OnHittest(x, y);
            if (hitTest == HitTest.CAPTION)
            {
                if (y < 50 && x > 60 && x < Width - 60)
                    return HitTest.CAPTION;
                return HitTest.CLIENT;
            }
            return hitTest;
        }

        private class MainPanel : Container, IExecutor
        {
            protected MainPanel(IData data)
                : base(data)
            {

            }

            protected override void Load(IData data)
            {
                base.Load(data);
                this.listView = data["listView"] as NavigationListView;
                this.adapter = data["adapter"] as MessagesAdapter;
                this.closeButton = data["closeButton"] as ImageButton;
            }

            protected override void Save(IData data)
            {
                base.Save(data);
                data["listView"] = this.listView;
                data["adapter"] = this.adapter;
                data["closeButton"] = this.closeButton;
            }

            protected override void OnLoaded()
            {
                base.OnLoaded();
                this.Init();
            }
            public MainPanel()
            {
                Caption caption = new Caption("messages");
                caption.Dock = DockStyle.Top;
                caption.MinHeight = 30;
                caption.Font = Theme.font11Bold;

                closeButton = new ImageButton("close.svg");
                closeButton.Dock = DockStyle.Right;
                caption.Add(closeButton);

                this.Add(caption);


                this.listView = new NavigationListView();
                this.listView.Dock = DockStyle.Fill;
                this.listView.Executor = this;
                this.listView.MinRowHeight = 26;
                //this.listView.ShowItemsToolTip = true;
                this.Add(this.listView);

                this.adapter = new MessagesAdapter("messages");

                this.Init();
            }

            private void Init()
            {
                closeButton.Executed += (s) =>
                {
                    (this.Form as Menu).Hide();
                };
            }

            protected override void OnDisposed()
            {
                this.adapter.Dispose();
                base.OnDisposed();
            }

            private NavigationListView listView;
            private MessagesAdapter adapter;

            private ImageButton closeButton;

            protected override void OnCreated()
            {
                this.listView.Load(this.adapter, true);
                base.OnCreated();
            }

            public bool Execute(object param)
            {
                return false;
            }

            protected override void OnMeasure(float widthMeasure, float heightMeasure)
            {
                if (this.Width == 0)
                    this.SetMeasured(650, 400);
                else
                    this.SetMeasured(this.Width, this.Height);
            }

            private class MessagesAdapter : NavigationAdapter
            {
                protected MessagesAdapter(IData data)
                    : base(data)
                {

                }

                public MessagesAdapter(string id) 
                    : base(null, id)
                {
                    this.ColumnsID = "Messages";
                }

                //public NavigationViewState viewState;

                public override ColumnInfoCollection CreateColumns()
                {
                    ColumnInfoCollection columns = new ColumnInfoCollection();
                    columns.Add("time", 150);
                    columns.Add("caption", 200);
                    columns.Add("message", 300);
                    columns.SortedColumn = "time";
                    columns.SortDirection = SortDirection.Descending;
                    return columns;
                }

                public override int Compare(string column, INavigationItem a, INavigationItem b)
                {
                    IMessageData ad = (a as MessageItem).data;
                    IMessageData bd = (b as MessageItem).data;
                    int res = 0;
                    switch (column)
                    {
                        case "message":
                            res = StringHelper.CompareNumbers(ad.Message, bd.Message);
                            if(res == 0)
                                res = StringHelper.CompareNumbers(ad.Caption, bd.Caption);
                            if(res == 0)
                                goto default;
                            break;
                        case "caption":
                            res = StringHelper.CompareNumbers(ad.Caption, bd.Caption);
                            if (res == 0)
                                goto default;
                            break;
                        default:
                            res = ad.Time.CompareTo(bd.Time);
                            break;
                    }
                    return res;
                }

                public override void LoadItems(INavigationView view, ItemsLoadHandler handler, RetParamHandler<bool> cancelHanler)
                {
                    Array<INavigationItem> items = new Array<INavigationItem>();
                    foreach(IMessageData data in MessageView.Messages)
                        items.Add(new MessageItem(data));
                    handler(this, items);
                }

                public override void Subscribe()
                {
                    MessageView.Messages.Added += Messages_Added;
                    MessageView.Messages.Removed += Messages_Removed;
                    MessageView.Messages.Cleared += Messages_Cleared;
                }


                public override void Unsubscribe()
                {
                    MessageView.Messages.Added -= Messages_Added;
                    MessageView.Messages.Removed -= Messages_Removed;
                    MessageView.Messages.Cleared -= Messages_Cleared;
                }

                private void Messages_Added(object sender, IMessageData value)
                {
                    this.OnAdded(new MessageItem(value));
                }

                private void Messages_Removed(object sender, IMessageData value)
                {
                    this.OnRemoved(value.ID);
                }

                private void Messages_Cleared(object sender)
                {
                    this.OnCleared();
                }

                public override void ShowMenu(INavigationView view, float x, float y)
                {
                    INavigationItem[] selItems = view.SelectedItems;
                    if (selItems.Length == 0 && view.FocusedItem != null)
                        selItems = new INavigationItem[] { view.FocusedItem };
                    if (selItems.Length > 0)
                    {
                        MenuStrip menu = new MenuStrip();
                        MenuStripButton button = new MenuStripButton("copyOperation.svg", "copyMessage");
                        button.Executed += (s) =>
                        {
                            string text = "";
                            foreach (MessageItem item in selItems)
                                text += Language.Current[item.data.Message] + Environment.NewLine;
                            Clipboard.SetText(text);
                        };
                        menu.Add(button);
                        menu.Add(ViewCopyMode.Columns).Executed += (object s) => view.Copy(selItems, (ViewCopyMode)(s as MenuStripButton).Tag);

                        menu.Add(new MenuStripSeparator());

                        button = new MenuStripButton("deleteOperation.svg", "deleteCmd");
                        button.Executed += (s) =>
                        {
                            foreach (MessageItem item in selItems)
                                MessageView.Messages.Remove(item.data);
                        };
                        menu.Add(button);

                        button = new MenuStripButton("clearAll");
                        button.Executed += (s) =>
                        {
                            MessageView.Messages.Clear();
                        };
                        menu.Add(button);

                        menu.Show(x, y);
                    }
                }
            }

            private class MessageItem : NavigationItem
            {
                public MessageItem(IMessageData data)
                    :base(data.ID)
                {
                    this.data = data;
                }

                public readonly IMessageData data;

                //private MessageInfo messageInfo;
                //public override IMessageInfo ToolTipInfo
                //{
                //    get
                //    {
                //        if (messageInfo == null)
                //                messageInfo = new MessageInfo(data.caption, data.message);
                //        return messageInfo;
                //    }
                //}

                protected override IImage GetImage()
                {
                    return Images.Get(data.Type.GetImage());
                }

                public static string GetDateString(DateTime time)
                {
                    if (time == DateTime.MinValue)
                        return "";
                    return time.ToShortDateString() + " " + time.ToString("t", DateTimeFormatInfo.InvariantInfo);
                }

                public override Component GetSubitem(string name)
                {
                    switch (name)
                    {
                        case "time":
                            return new TextComponent(GetDateString(data.Time));
                        case "caption":
                            return new TextLocalizeComponent(data.Caption);
                        case "message":
                            return new TextLocalizeComponent(data.Message.Replace(Environment.NewLine, " "));
                    }
                    return null;
                }
            }
        }
    }
}
