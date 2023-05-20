using System;
using Complex.Controls;
using Complex.Collections;
namespace Complex.Wallets
{
    public class Explorer : Serializable, IUnique, IAdapterSource
    {
        protected Explorer(IData data)
        {
        }

        protected override void Load(IData data)
        {
            this.adapterID = data["adapterID"] as string;
        }

        protected override void Save(IData data)
        {
            data["adapterID"] = this.adapterID;
        }

        public Explorer(string adapterID)
        {
            this.adapterID = adapterID;
        }

        private string adapterID;

        private WalletAdapter adapter;
        public WalletAdapter Adapter
        {
            get
            {
                if (adapter == null)
                    adapter = Controller.GetAdapter(this.adapterID);
                return adapter;
            }
        }

        public string ID => adapterID;

        public virtual Component CreateExplorerItem()
        {
            return new ExplorerItem(this);
        }

        public virtual Component CreateExplorerPanel()
        {
            return new ExplorerPanel(this);
        }

    }
}
