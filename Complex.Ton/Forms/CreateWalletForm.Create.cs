using System;
using Complex.Controls;
using Complex.Drawing;
using Complex.Themes;
using Complex.Collections;
using Complex.Wallets;

namespace Complex.Ton
{
    public partial class CreateWalletForm
    {
        private WordsPanel createWordsPanel;
        private InfoPanel skipInfoPanel;

        private void CreateWallet()
        {
            this.controller.Wait("waitCreateWordsCaption", "waitCreateWordsDescription");
            SingleThread.Run(() =>
            {
                var (kd, e) = adapter.GetKeyData(null);
                if (e != null)
                {
                    this.OnError("createWalletError", e, false);
                    paramHandler(null, e);
                }
                else
                {
                    this.keyData = kd;
                    Timer.Delay(300, () =>
                    {
                        if (WalletsData.Wallets.Contains(Wallet.GetID(adapter, keyData.Address, true)))
                        {
                            this.OnError(Language.Current["walletAlreadyExist", keyData.Address], "", true);
                            paramHandler(null, "walletAlreadyExist");
                        }
                        else
                        {
                            this.switchContainer.Current = new DonePanel("congratulations", Language.Current["walletCongratulationsinfo", GetSymbolCoinsText()], "continue", adapter.ThemeColor, ShowCreatedWords);
                        }
                    });
                }
            });
        }

        private void SkipWords()
        {
            if (this.skipInfoPanel == null)
                this.skipInfoPanel = new InfoPanel(this.adapter.ThemeColor, "skipSaveWords", "warnSkipSaveWords", ShowCreatedWords, CloseCheck, DoneWords);
            this.switchContainer.Next = this.skipInfoPanel;
        }

        private void ShowCreatedWords()
        {
            if (this.createWordsPanel == null)
                this.createWordsPanel = new WordsPanel(this.adapter, false, SkipWords, () => switchContainer.Current = mainPanel, CloseCheck, () => ShowTestPanel());
            this.createWordsPanel.UpdateWorts(keyData.Words);
            this.switchContainer.Current = this.createWordsPanel;
        }

        private void ShowTestPanel()
        {
            if (this.testWordsPanel == null)
                this.testWordsPanel = new TestWordsPanel(this);
            this.testWordsPanel.Update();
            switchContainer.Current = this.testWordsPanel;
        }

        private void DoneWords()
        {
            switchContainer.Current = new DonePanel("perfect", Language.Current["walletPerfectInfo"], "setPasscode", adapter.ThemeColor, ShowPasswodPanel);
        }

        private class TestWordsPanel : CaptionPanel
        {
            public TestWordsPanel(CreateWalletForm form)
                : base("testTime", null, "", () => form.switchContainer.Current = form.createWordsPanel, null, "continue", form.adapter.ThemeColor, () => { })
            {
                this.form = form;
                this.UseTab = true;
                this.Add(new Dummy(DockStyle.Top, 0, 50));

                Container container = new Container();
                container.Inflate.Set(0, 25);
                container.Padding.Set(4);
                container.Dock = DockStyle.Fill;

                editBoxes = new Array<EditBoxEx>();
                for (int i = 0; i < 3; i++)
                {
                    EditBoxEx editBox = new EditBoxEx();
                    editBox.LeftOffset = 20;
                    //editBox.LeftTextID = (checkWordIndices[i] + 1).ToString();
                    editBox.Dock = DockStyle.TopCenter;
                    editBox.MinWidth = 200;
                    editBox.MaxWidth = 200;
                    editBox.MaxHeight = 30;
                    editBox.MinHeight = 30;

                    editBox.ApplyOnLostFocus = true;
                    editBox.ErrorMode = true;
                    editBox.TabStop = true;
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

                                foreach (string wors in words)
                                {
                                    MenuStripButton menuButton = new MenuStripButton(wors);
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

                    container.Add(editBox);
                    editBoxes.Add(editBox);
                }

                this.Add(container);

                this.continueButton.BringToFront();
            }

            private CreateWalletForm form;
            private Array<int> checkWordIndices = new Array<int>();
            private Array<EditBoxEx> editBoxes;
            private MenuStrip menu = null;

            protected override void Continue()
            {
                bool correct = true;
                for (int i = 0; i < checkWordIndices.Count; i++)
                {
                    if (editBoxes[i].Text.Trim() != form.keyData.Words[checkWordIndices[i]])
                    {
                        correct = false;
                        break;
                    }
                }
                if (correct)
                    form.DoneWords();
                else
                    form.OnError("walletTestTimeAlertTitle", Language.Current["walletTestTimeAlertText"], false);
            }

            protected override void OnCreated()
            {
                base.OnCreated();
                editBoxes[0].StartEdit();
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

            public void Update()
            {
                checkWordIndices.Clear();
                while (checkWordIndices.Count < 3)
                {
                    int index = Utils.Random(23);
                    if (checkWordIndices.Contains(index))
                        continue;
                    checkWordIndices.Add(index);
                }
                checkWordIndices.Sort((a, b) => { return a.CompareTo(b); });
                this.descriptionComponent.Text = Language.Current["walletTestTimeInfo", checkWordIndices[0] + 1, checkWordIndices[1] + 1, checkWordIndices[2] + 1];
                for (int i = 0; i < checkWordIndices.Count; i++)
                    editBoxes[i].LeftTextID = (checkWordIndices[i] + 1).ToString();
            }
        }


    }
}
