using System;
using Complex.Controls;

namespace Complex.Wallets
{
    public class WalletsWorkspace : Workspace
    {
        protected WalletsWorkspace(IData data)
        : base(data)
        {

        }
        public WalletsWorkspace(string name)
            : base(name, name + ".tww")
        {
        }

        public WalletsWorkspace(string name, string fileName)
            : base(name, fileName)
        {
        }

        protected override Container CreateContainer()
        {
            return new WorkspacePanel();
        }

        public static Workspace FromPath(string fileName)
        {
            string name = StringHelper.GetFileNameNotEX(fileName);
            string resDir = Resources.LocalApplicationData;
            string dir = StringHelper.GetDirectory(fileName);
            if (string.Compare(dir, resDir, true) == 0)
                fileName = fileName.Substring(dir.Length);
            Workspace workspace = new WalletsWorkspace(name, fileName);
            return workspace;
        }

    }
}
