using System;

namespace Complex.Ton
{
    public class ContractDeployData : MessageData
    {
        public ContractDeployData(string deployer, string destAddress, Cell stateInit, UInt128 amount, Cell message)
            :base(destAddress, amount, message)
        {
            this.deployer = deployer;
            this.stateInit = stateInit;
        }

        protected override void OnDisposed()
        {
            if(this.stateInit != null)
                this.stateInit.Dispose();
            base.OnDisposed();
        }

        public readonly Cell stateInit;
        public readonly string deployer;

    }
}
