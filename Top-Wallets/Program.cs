using System;
using System.Diagnostics;
using System.IO;
using Complex.Controls;
using Microsoft.Win32;

namespace Complex.Wallets
{
    static class Program
    {
        // 0 = type;
        // type : u - update; 

        [STAThread]
        static void Main(string[] args)
        {
            string fileName = null;
            string cmd = null;
            if (args.Length > 0)
            {
                switch (args[0].ToLower().Trim())
                {
                    case "u":
                        return;
                    case "w":
                        fileName = args[1];
                        break;
                    case "-url":
                        if (args.Length > 1)
                        {
                            Process[] processes = Process.GetProcessesByName(StringHelper.GetFileNameWithoutExtension(Resources.ExecutablePath));
                            foreach (Process process in processes)
                            {
                                if (process.Id != WinApi.GetCurrentProcessId())
                                {
                                    string text = Clipboard.GetText();
                                    Clipboard.SetText(text + Application.cmd101 + args[1]);
                                    WinApi.PostMessage(process.MainWindowHandle, WM.USER, 0, 101);
                                    return;
                                }
                            }
                            cmd = args[1];
                        }
                        break;
                }
            }

            var keyTest = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
            RegistryKey key = keyTest.CreateSubKey("ton");
            key.SetValue("", "URL:TON coin Transfer Link");
            key.SetValue("URL Protocol", "");
            key.CreateSubKey(@"shell\open\command").SetValue("", "\"" + Resources.ExecutablePath + "\"" + "-url \"%1\"");

            Application.Site = MainSettings.Current.Remote.Site;

            Controller.Extensions.Add(new TonAdapterExtension());
            Controller.Extensions.Add(new TonAdapterExtension(true));
            Application.Start<MainForm>(fileName, cmd);
        }
    }
}
