using System;
using Complex.Wallets;
using Complex.Controls;
using Complex.Collections;
using Complex.Ton.TonConnect;
using Complex.Drawing;
using Complex.Themes;

namespace Complex.Ton
{
    public class TonMultiSendForm : CaptionForm
    {
        public TonMultiSendForm(Connection connection, string reqID, ContractDeployData[] datas)
            :base(new SwitchContainer(false))
        {
            this.connection = connection;
            this.reqID = reqID;
            this.datas = datas;
            this.wallet = this.connection.wallet;
            this.wallet.TransactionComplete += Wallet_TransactionComplete;

            this.switchContainer = this.Component as SwitchContainer;
            this.MinimumSize.Set(500, 550);

            this.controller = new SwitchFormController(this.switchContainer, CloseCheck, null);

            this.controller.Wait("pleaseWait", null, "commissionCalculation", Cancel);

            this.totalSend = this.wallet.Balance.Clone(0);
            this.totalFees = this.wallet.Balance.Clone(0);

            this.messages = new MessageInfo[datas.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                ContractDeployData data = datas[i];
                this.messages[i] = new MessageInfo { destAddress = data.destAddress, amount = (long)data.amount, message = "", initState = data.stateInit, body = data.message };
                this.totalSend.Add(data.amount);
            }
            this.wallet.CalcFees((fee, e) =>
            {
                if (fee != null)
                    this.totalFees = fee;
                foreach (ContractDeployData data2 in datas)
                {
                    this.wallet.Adapter.GetWalletInfo(data2.destAddress, (w, ew) =>
                    {
                        initSendInfos.Add(new SendInfo { data = data2, error = w != null && w.state != WalletState.Active ? Language.Current["walletSendWarningText", wallet.Symbol + " coins"] : null, balance = wallet.Balance.Clone(data2.amount) });
                        if (initSendInfos.Count == datas.Length)
                        {
                            Timer.Delay(300, () =>
                            {
                                this.mainPanel = new MainPanel(this);
                                this.switchContainer.Current = this.mainPanel;
                            });
                        }
                    });
                }

            }, this.messages);
        }

        protected override void OnDisposed()
        {
            this.wallet.TransactionComplete -= Wallet_TransactionComplete;
            this.controller.Dispose();
            foreach (ContractDeployData data in datas)
                data.Dispose();
            base.OnDisposed();
        }

        private void Wallet_TransactionComplete(object sender, ITransactionBase transaction, object hash)
        {
            this.transactionWaitPanel.AddTransactions(new TransactionsInfo(this.wallet, this.wallet.Name, transaction));
            if (this.hash == hash)
            {
                this.transactionWaitPanel.StopWait();
                this.transactionWaitPanel.ContinueEnabled(true);
            }
        }

        private Connection connection;
        private string reqID;
        private ContractDeployData[] datas;
        private MessageInfo[] messages;
        private TonWallet wallet;
        private SwitchContainer switchContainer;
        private SwitchFormController controller;
        private MainPanel mainPanel;
        private PasswordPanel passwordPanel;
        private TransactionWaitPanel transactionWaitPanel;
        private Array<SendInfo> initSendInfos = new Array<SendInfo>();
        private Balance totalSend;
        private Balance totalFees;
        private object hash;

        private string GetSymbolCoinsText()
        {
            return wallet.Symbol + " coins";
        }

        private void Cancel()
        {
            this.connection.SendError(this.reqID, ErrorCode.UserDeclinedTheConnection, null, null);
            this.CloseCheck();
        }

        private void Complete()
        {
            this.controller.Done(Language.Current["sendingCompleted", GetSymbolCoinsText() + " " + wallet.Adapter.NetName], Language.Current["walletSendDoneText", totalSend, GetSymbolCoinsText(), ""], "close", wallet.ThemeColor, CloseCheck);
        }

        private void Send()
        {
            if (this.transactionWaitPanel == null)
                this.transactionWaitPanel = new TransactionWaitPanel("sendTransactions", " " + wallet.Symbol + " coins " + wallet.Adapter.NetName, "walletSendingGramsInfo", this.CloseCheck, "continue", this.wallet.ThemeColor, this.Complete);
            this.transactionWaitPanel.ContinueEnabled(false);
            this.transactionWaitPanel.StartWait();
            this.switchContainer.Next = this.transactionWaitPanel;
            this.wallet.SendMessages(this.passwordPanel.Passcode, (cell, e) =>
            {
                if (cell != null)
                {
                    this.hash = cell.GetHash();
                    this.wallet.WaitTransactions.Add(this.hash);
                    this.connection.ConfirmSendTransaction(this.reqID, cell.ToBoc().ToBase64(), null);
                    cell.Dispose();
                }
                else
                {
                    this.transactionWaitPanel.StopWait();
                    this.controller.Error(this.wallet.ThemeColor, "errorOccurred", e, "repeat", Send);
                }
            }, this.messages);

        }

        private void SetPasscode()
        {
            if (this.passwordPanel == null)
            {
                this.passwordPanel = new PasswordPanel(false, () => this.switchContainer.Current = this.mainPanel, Cancel);
                this.passwordPanel.Complete += (s) =>
                {
                    controller.Wait("pleaseWait", null, null, CloseCheck);
                    wallet.CheckPassword(this.passwordPanel.Passcode, (e) =>
                    {
                        Timer.Delay(300, () =>
                        {
                            if (e == null)
                                this.Send();
                            else
                                this.controller.ErrorLang("error", e, () => this.switchContainer.Current = this.passwordPanel);
                        });
                    });
                };
            }
            this.passwordPanel.DescriptionID = "enterWalletPassword";
            this.passwordPanel.ClearPasscode();
            this.switchContainer.Next = this.passwordPanel;

        }

        private class MainPanel : CaptionPanel
        {
            public MainPanel(TonMultiSendForm form)
                : base(Language.Current["sendTransactions"]," " + form.wallet.Symbol + " " + form.wallet.Adapter.NetName, null, null, form.Cancel, Language.Current["send"] + " " + form.totalSend.GetTextSharps(8) + " " + form.GetSymbolCoinsText(), form.wallet.ThemeColor, form.SetPasscode)
            {

                AnyView anyView = new AnyView();
                anyView.Padding.Set(4);
                anyView.Inflate.height = 4;
                anyView.Dock = DockStyle.Fill;
                anyView.VScrollStep = 20;

                foreach (SendInfo info in form.initSendInfos)
                    anyView.Add(new SendInfoContainer(form, info));

                this.Add(anyView);

                this.Add(new Separator(DockStyle.Bottom, 10));

                Container container = new Container();
                container.Dock = DockStyle.Bottom;

                TextComponent text = new TextLocalizeComponent("totalSend");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                CurrencyLabel balance = new CurrencyLabel(form.totalSend.GetTextSharps(8), form.totalSend.Symbol);
                balance.ValueTextComponent.ForeColor = form.wallet.ThemeColor;
                balance.Dock = DockStyle.Right;
                container.Add(balance);
                this.Add(container);

                container = new Container();
                container.Dock = DockStyle.Bottom;

                if (form.totalFees > 0)
                {
                    text = new TextLocalizeComponent("totalFees");
                    text.MinWidth = 200;
                    text.Alignment = ContentAlignment.Left;
                    text.AppendRightText = ":";
                    text.Dock = DockStyle.Left;
                    container.Add(text);

                    balance = new CurrencyLabel("≈" + form.totalFees.GetTextSharps(8), form.totalFees.Symbol);
                    balance.ValueTextComponent.Font = Theme.font10;
                    balance.Dock = DockStyle.Right;
                    container.Add(balance);
                    this.Add(container);

                }

                this.Add(new Separator(DockStyle.Bottom, 10));

                container = new Container();
                container.Dock = DockStyle.Bottom;

                text = new TextLocalizeComponent("balance");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                balance = new CurrencyLabel(form.wallet.Balance.GetTextSharps(8), form.wallet.Balance.Symbol);
                balance.ValueTextComponent.ForeColor = form.wallet.ThemeColor;
                balance.Dock = DockStyle.Right;
                container.Add(balance);
                this.Add(container);

                this.continueButton.Enabled = form.wallet.Balance > form.totalSend + form.totalFees;
                this.continueButton.BringToFront();
            }

        }
        private class SendInfoContainer : Container
        {
            public SendInfoContainer(TonMultiSendForm form, SendInfo info)
            {
                this.Padding.Set(4);
                this.Inflate.height = 6;

                Caption caption = new Caption("send");
                caption.AppendRightText = " " +info.balance.Symbol;
                caption.Dock = DockStyle.Top;
                this.Add(caption);

                Container container = new Container();
                container.Dock = DockStyle.Top;

                TextComponent text = new TextLocalizeComponent("destination");
                text.MinWidth = 200;
                text.Alignment = ContentAlignment.Left;
                text.AppendRightText = ":";
                text.Dock = DockStyle.Left;
                text.Style = Theme.Get<CaptionStyle>();
                container.Add(text);

                text = new TextComponent(info.data.destAddress);
                text.MaxWidth = 300;
                text.Dock = DockStyle.Right;
                container.Add(text);
                this.Add(container);

                container = new Container();
                container.Dock = DockStyle.Top;

                text = new TextLocalizeComponent("amount");
                text.MinWidth = 200;
                text.AppendRightText = ":";
                text.Alignment = ContentAlignment.Left;
                text.Dock = DockStyle.Left;
                container.Add(text);
                
                CurrencyLabel balance = new CurrencyLabel(info.balance.GetTextSharps(8), info.balance.Symbol);
                balance.ValueTextComponent.ForeColor = form.wallet.ThemeColor;
                balance.Dock = DockStyle.Right;
                container.Add(balance);

                this.Add(container);

                if (info.error != null)
                {
                    this.Add(new Separator(DockStyle.Top, 20));

                    text = new TextComponent(info.error);
                    text.MultilineLenght = 50;
                    text.ForeColor = Theme.red0;
                    //text.Font = Theme.font9Bold;
                    text.MinHeight = 40;
                    text.Dock = DockStyle.Top;
                    this.Add(text);

                }
            }

            protected override void OnDrawBack(Graphics g)
            {
                g.Smoosh(() => g.FillRoundRect(0, 0, Width, Height, 10, Theme.unselectedItemBackColor));
            }
        }

        private class SendInfo
        {
            public ContractDeployData data;
            public Balance balance;
            public string error;
        }
    }
}
