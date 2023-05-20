using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Collections;

namespace Complex.Wallets
{
    public class WordsPanel : CaptionPanel
    {
        public WordsPanel(WalletAdapter adapter, bool isImport, EmptyHandler skipHandler, EmptyHandler goBack, EmptyHandler close, EmptyHandler ok)
            :base("24SecretWords", null, isImport ? "importWalletDescription" : "createWalletDescription", isImport ? goBack : null, close, "done", adapter.ThemeColor, ok)
        {
            this.isImport = isImport;
            this.skipHandler = skipHandler;

            this.UseTab = true;
            container = new Container();
            container.Padding.Set(4);
            container.Dock = DockStyle.Fill;

            float top = 0;
            float x = 20;
            float y = top;
            float width = 130;
            float height = 30;
            editBoxes = new Array<EditBoxEx>();
            for (int i = 1; i <= 24; i++)
            {
                EditBoxEx editBox = new EditBoxEx();
                editBox.LeftOffset = 20;
                editBox.LeftTextID = i.ToString();
                editBox.SetBounds(x, y, width, height);

                if (isImport)
                {
                    editBox.TabStop = true;
                    editBox.ClearSelectedOnFreeDown = true;
                    editBox.ApplyOnLostFocus = true;
                    editBox.ErrorMode = true;
                    editBox.TextChanged += (s) =>
                    {
                        EditBoxEx edit = (EditBoxEx)s;
                        edit.ErrorMode = !Words.Contains(edit.Text);
                        if (menu != null)
                            menu.Dispose();
                        menu = null;
                        if (edit.ErrorMode)
                        {
                            continueButton.Enabled = false;
                            string[] words = Words.GetWords(edit.Text, 8);
                            if (words.Length > 0)
                            {
                                menu = new MenuStrip();
                                menu.Container.Inflate.Set(1);
                                menu.Container.MinWidth = edit.Width;

                                foreach (string word in words)
                                {
                                    MenuStripButton menuButton = new MenuStripButton(null);
                                    menuButton.Text = word;
                                    menuButton.Executed += delegate (object s2)
                                    {
                                        edit.Text = (s2 as MenuStripButton).Text;
                                        this.SelectNextTab();
                                    };
                                    menu.Add(menuButton);
                                }

                                menu.Show(edit, MenuAlignment.BottomRight);

                            }
                        }
                        else
                        {
                            this.CheckEhabled();
                        }
                    };

                }
                else
                {
                    editBox.ReadOnly = true;
                }

                container.Add(editBox);
                editBoxes.Add(editBox);
                if (i == 8 || i == 16)
                {
                    y = top;
                    x += width + 20;
                }
                else
                {
                    y += height + 20;
                }
            }
            //container.MinSize.Set(x + width + 20, y + height + 20);

            this.Add(container);

            if (this.skipHandler != null)
            {
                TextButton button = new TextButton("skip");
                button.MaxHeight = 20;
                button.Dock = DockStyle.Bottom;
                button.DrawBorder = false;
                button.ForeColor = Theme.red2;
                button.Executed += (s) => this.skipHandler();
                this.Add(button);
            }
            this.continueButton.BringToFront();
        }

        private bool isImport;
        private Container container;
        private MenuStrip menu = null;
        private Array<EditBoxEx> editBoxes;
        private EmptyHandler skipHandler;

        protected override void OnCreated()
        {
            base.OnCreated();
            editBoxes[0].StartEdit();
        }

        public void UpdateWorts(string[] words)
        {
            for (int i = 0; i < editBoxes.Count; i++)
                editBoxes[i].Text = words[i];
        }

        public string[] GetWords()
        {
            string[] words = new string[editBoxes.Count];
            for (int i = 0; i < editBoxes.Count; i++)
                words[i] = editBoxes[i].Text.Trim();
            return words;
        }

        private void CheckEhabled()
        {
            bool enabled = true;
            foreach (EditBoxEx editBox in editBoxes)
            {
                if (editBox.ErrorMode)
                {
                    enabled = false;
                    break;
                }
            }
            this.continueButton.Enabled = enabled;
            this.UseEnterTab = !continueButton.Enabled;
        }

    }
}
