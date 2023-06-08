using System;
using System.Text;
using Complex.Wallets;
using Complex.Collections;
using Complex.Remote;

namespace Complex.Ton
{
    public static class JettonController
    {
        public const string JettonMinterCode = "te6cckECDQEAApwAART/APSkE/S88sgLAQIBYgIDAgLMBAUCA3pgCwwC8dkGOASS+B8ADoaYGAuNhJL4HwfSB9IBj9ABi465D9ABj9ABgBaY/pn/aiaH0AfSBqahhACqk4XUcZmpqbGyiaY4L5cCSBfSB9AGoYEGhAMGuQ/QAYEogaKCF4BQpQKBnkKAJ9ASxni2ZmZPaqcEEIPe7L7yk4XXGBQGBwCTtfBQiAbgqEAmqCgHkKAJ9ASxniwDni2ZkkWRlgIl6AHoAZYBkkHyAODpkZYFlA+X/5Og7wAxkZYKsZ4soAn0BCeW1iWZmZLj9gEBwDY3NwH6APpA+ChUEgZwVCATVBQDyFAE+gJYzxYBzxbMySLIywES9AD0AMsAyfkAcHTIywLKB8v/ydBQBscF8uBKoQNFRchQBPoCWM8WzMzJ7VQB+kAwINcLAcMAkVvjDQgBpoIQLHa5c1JwuuMCNTc3I8ADjhozUDXHBfLgSQP6QDBZyFAE+gJYzxbMzMntVOA1AsAEjhhRJMcF8uBJ1DBDAMhQBPoCWM8WzMzJ7VTgXwWED/LwCQA+ghDVMnbbcIAQyMsFUAPPFiL6AhLLassfyz/JgEL7AAH+Nl8DggiYloAVoBW88uBLAvpA0wAwlcghzxbJkW3ighDRc1QAcIAYyMsFUAXPFiT6AhTLahPLHxTLPyP6RDBwuo4z+ChEA3BUIBNUFAPIUAT6AljPFgHPFszJIsjLARL0APQAywDJ+QBwdMjLAsoHy//J0M8WlmwicAHLAeL0AAoACsmAQPsAAH2tvPaiaH0AfSBqahg2GPwUALgqEAmqCgHkKAJ9ASxniwDni2ZkkWRlgIl6AHoAZYBk/IA4OmRlgWUD5f/k6EAAH68W9qJofQB9IGpqGD+qkEDvfJl9";
        public const string JettonWalletCode = "te6cckECEQEAAx8AART/APSkE/S88sgLAQIBYgIDAgLMBAUAG6D2BdqJofQB9IH0gahhAgHUBgcCASAICQC7CDHAJJfBOAB0NMDAXGwlRNfA/AM4PpA+kAx+gAxcdch+gAx+gAwAtMfghAPin6lUiC6lTE0WfAJ4IIQF41FGVIgupYxREQD8ArgNYIQWV8HvLqTWfAL4F8EhA/y8IAARPpEMHC68uFNgAgEgCgsAg9QBBrkPaiaH0AfSB9IGoYAmmPwQgLxqKMqRBdQQg97svvCd0JWPlxYumfmP0AGAnQKBHkKAJ9ASxniwDni2Zk9qpAHxUD0z/6APpAIfAB7UTQ+gD6QPpA1DBRNqFSKscF8uLBKML/8uLCVDRCcFQgE1QUA8hQBPoCWM8WAc8WzMkiyMsBEvQA9ADLAMkg+QBwdMjLAsoHy//J0AT6QPQEMfoAINdJwgDy4sR3gBjIywVQCM8WcPoCF8trE8yAwCASANDgCeghAXjUUZyMsfGcs/UAf6AiLPFlAGzxYl+gJQA88WyVAFzCORcpFx4lAIqBOgggnJw4CgFLzy4sUEyYBA+wAQI8hQBPoCWM8WAc8WzMntVAL3O1E0PoA+kD6QNQwCNM/+gBRUaAF+kD6QFNbxwVUc21wVCATVBQDyFAE+gJYzxYBzxbMySLIywES9AD0AMsAyfkAcHTIywLKB8v/ydBQDccFHLHy4sMK+gBRqKGCCJiWgGa2CKGCCJiWgKAYoSeXEEkQODdfBOMNJdcLAYA8QANc7UTQ+gD6QPpA1DAH0z/6APpAMFFRoVJJxwXy4sEnwv/y4sIFggkxLQCgFrzy4sOCEHvdl97Iyx8Vyz9QA/oCIs8WAc8WyXGAGMjLBSTPFnD6AstqzMmAQPsAQBPIUAT6AljPFgHPFszJ7VSAAcFJ5oBihghBzYtCcyMsfUjDLP1j6AlAHzxZQB88WyXGAEMjLBSTPFlAG+gIVy2oUzMlx+wAQJBAjAHzDACPCALCOIYIQ1TJ223CAEMjLBVAIzxZQBPoCFstqEssfEss/yXL7AJM1bCHiA8hQBPoCWM8WAc8WzMntVLxoCgw=";

        static UInt128 deployGas = Gram.FromValue(0.25m);
        static UInt128 mintGas = Gram.FromValue(0.2m);
        static UInt128 internalGas = Gram.FromValue(0.01m);
        static UInt128 TransferFee = Gram.FromValue(0.001m);
        public static UInt128 MessageTransferFee = Gram.FromValue(0.05m);
        public static UInt128 BurnFee = Gram.FromValue(0.05m);
        public static UInt128 MintFee = mintGas + deployGas;
        public static UInt128 ChangeOwnerFee = Gram.FromValue(0.005m);

        static Hashtable<byte[], string> hashKeysBytes = new Hashtable<byte[], string>();

        static JettonController()
        {
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("name"), "name");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("description"), "description");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("symbol"), "symbol");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("decimals"), "decimals");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("image"), "image");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("image_data"), "image_data");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("color"), "color");
            hashKeysBytes.Add(TonLib.Sha256ComputeHashText("deployer"), "deployer");
        }

        public static JettonDeployData CreateJetton(long queryId, string owner, JettonInfo info, string offchainUri)
        {
            ContractDeployData deployParams = CreateDeployParams(queryId, owner, info, offchainUri);
            string jettonMinterAddress = ContractController.GetAddress(0, deployParams.stateInit);
            string jettonWalletAddress = ContractController.GetAddress(0, CalculateJettonWalletStateInit(owner, jettonMinterAddress, JettonWalletCode));
            return new JettonDeployData(owner, jettonMinterAddress, jettonWalletAddress, deployParams, info, offchainUri);
        }

        public static MessageData CreateMintData(long queryId, string jettonMinterAddress, string owner, UInt128 jettonAmount)
        {
            return new MessageData(jettonMinterAddress, deployGas, Mint(queryId, owner, jettonAmount, mintGas));
        }

        public static MessageData CreateTransferData(long queryId, string myJettonWalletAddress, string toOwnerAddress, string fromOwnerAddress, UInt128 jettonAmount)
        {
            Cell cell = Transfer(queryId, toOwnerAddress, fromOwnerAddress, jettonAmount);
            return new MessageData(myJettonWalletAddress, MessageTransferFee, cell);
        }

        public static MessageData CreateBurnData(long queryId, string jettonWalletAddress, string owner, UInt128 jettonAmount)
        {
            Cell cell = Burn(queryId, jettonAmount, owner);
            return new MessageData(jettonWalletAddress, BurnFee, cell);
        }

        public static MessageData CreateChangeOwner(long queryId, string jettonMinterAddress, string newOwner, UInt128 forwardAmount)
        {
            Cell cell = ChangeOwner(queryId, newOwner);
            return new MessageData(jettonMinterAddress, forwardAmount, cell);
        }

        public static MessageData CreateChangeContent(long queryId, string jettonMinterAddress, JettonInfo info, string offchainUri)
        {
            Cell cell = ChangeContent(queryId, !string.IsNullOrEmpty(offchainUri) ? BuildJettonOffChainMetadata(offchainUri) : BuildJettonOnchainMetadata(info));
            return new MessageData(jettonMinterAddress, MessageTransferFee, cell);
        }

        private static Cell BuildJettonOffChainMetadata(string offchainUri)
        {
            Cell cell = CellBuilder.Begin()
                .Store(ContentLayout.OFFCHAIN, 8)
                .Store(Encoding.ASCII.GetBytes(offchainUri))
                .End();
            return cell;
        }

        private static JettonInfo JettonFrommOffChain(Slice cs, string jtonAddress, string owner, UInt128 totalSupply)
        {
            string url = cs.LoadString();
            url = url.Replace("ipfs://", "https://ipfs.io/ipfs/");
            try
            {
                string data = Http.Get(url);
                JsonArray array = Json.Parse(data) as JsonArray;
                if (array != null && array.Count > 0)
                {
                    int desimals = array.Contains("decimals") ? array.GetInt("decimals") : 9;
                    string image = array.Contains("image") ? array.GetString("image").Replace("ipfs://", "https://ipfs.io/ipfs/") : array.GetString("image_data");
                    string symbol = array.GetString("symbol");
                    return new JettonInfo(array.GetString("name"), array.GetString("description"), symbol, image, totalSupply, desimals, jtonAddress, owner, array.GetString("color"), array.GetString("deployer"));
                }
            }
            catch (Exception e)
            {

            }
            return null;
        }


        private static Cell BuildJettonOnchainMetadata(JettonInfo info)
        {
            Dictionary dict = new Dictionary(256);
            Dict<string> hash = info.ToDict();
            foreach (KeyValue<byte[], string> kv in hashKeysBytes.EnumKeyValue())
            {
                string value = hash[kv.Value];
                if (!string.IsNullOrEmpty(value))
                {
                    Cell root = CellBuilder.StoreBuffer(value, (int)DataFormat.SNAKE);
                    dict.StoreRef(kv.Key, root);
                }
            }
            return CellBuilder.Begin().Store(ContentLayout.ONCHAIN, 8).StoreDict(dict).End();
        }

        public static string LoadText(Slice slice)
        {
            string value = null;
            DataFormat format = (DataFormat)slice.LoadInt(8);
            if (format == DataFormat.SNAKE)
                value = Slice.LoadString(slice);
            else
                slice.Dispose();
            return value;
        }

        private static JettonInfo JettonFromOnchain(Slice cs, string jtonAddress, string owner, UInt128 totalSupply)
        {
            Dict<string> hash = new Dict<string>();
            Dictionary dict = cs.LoadDict(256);
            if (dict != null)
            {
                foreach (KeyValue<byte[], string> kv in hashKeysBytes.EnumKeyValue())
                {
                    Slice slice = dict.FindRef(kv.Key);
                    if (slice != null)
                    {
                        string value = null;
                        if (slice.Bits > 0)
                            value = LoadText(slice);
                        else if (slice.Refs > 0)
                            value = LoadText(slice.LoadRef());
                        slice.Dispose();
                        if (value != null)
                            hash.Add(kv.Value, value);
                    }
                }
                dict.Dispose();
            }
            if (hash.Count > 0)
                return JettonInfo.FromDict(hash, jtonAddress, owner, totalSupply);
            return null;
        }


        private static Cell PackJettonWalletData(UInt128 balance, string owner, string jettonMinnterAddress, string jettonWalletCode)
        {
            return CellBuilder.Begin()
                .StoreCoins(balance)
                .StoreAddress(owner)
                .StoreAddress(jettonMinnterAddress)
                .StoreRef(jettonWalletCode)
                .End();
        }

        private static Cell CalculateJettonWalletStateInit(string owner, string jettonMinnterAddress, string jettonWalletCode)
        {
            try
            {
                return CellBuilder.Begin()
                    .Store(0, 2)
                    .StoreDict(jettonWalletCode)
                    .StoreDict(PackJettonWalletData(0, owner, jettonMinnterAddress, jettonWalletCode))
                    .Store(0, 1)
                    .End();
            }
            catch (Exception e)
            {
            }
            return null;
        }

        private static Cell JettonInitData(string owner, JettonInfo info, string offchainUri)
        {
            return CellBuilder.Begin()
                .StoreCoins(0)
                .StoreAddress(owner)
                .StoreRef(!string.IsNullOrEmpty(offchainUri) ? BuildJettonOffChainMetadata(offchainUri) : BuildJettonOnchainMetadata(info))
                .StoreRef(JettonWalletCode).End();
        }

        private static Cell InternalMessage(long queryId, string owner, UInt128 jettonValue)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.InternalTransfer, 32)
                .Store(queryId, 64)
                .StoreCoins(jettonValue)
                .StoreAddress(owner)
                .StoreAddress(owner)
                .StoreCoins(internalGas)
                .Store(false) // forward_payload in this slice, not separate cell
                .End();
        }

        private static Cell Mint(long queryId, string owner, UInt128 jettonValue, UInt128 mintGas)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.Mint, 32)
                .Store(0, 64) // queryid
                .StoreAddress(owner)
                .StoreCoins(mintGas)
                .StoreRef(InternalMessage(queryId, owner, jettonValue))
                .End();
        }

        private static Cell Burn(long queryId, UInt128 jettonAmount, string responseAddress)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.Burn, 32) // action
                .Store(queryId, 64) // query-id
                .StoreCoins(jettonAmount)
                .StoreAddress(responseAddress)
                .StoreDict()
                .End();
        }

        private static Cell Transfer(long queryId, string to, string from, UInt128 jettonAmount)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.Transfer, 32)
                .Store(queryId, 64)
                .StoreCoins(jettonAmount)
                .StoreAddress(to)
                .StoreAddress(from)
                .StoreDict()
                .StoreCoins(TransferFee)
                .Store(false) // forward_payload in this slice, not separate cell
                .End();
        }

        private static Cell ChangeOwner(long queryId, string newOwner)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.ChangeOwner, 32)
                .Store(queryId, 64)
                .StoreAddress(newOwner)
                .End();
        }

        private static Cell ChangeContent(long queryId, Cell content)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.ChangeContent, 32)
                .Store(queryId, 64)
                .StoreRef(content)
                .End();
        }

        public static JettonInfo GetJettonInfo(AccountState state)
        {
            JettonInfo info = null;
            if (state != null && state.Type == WalletType.JettonMinter)
            {
                object[] stack = state.RunMethod("get_jetton_data");
                if (stack != null)
                {
                    if (stack.Length >= 4)
                    {
                        UInt128 totalSupply = UInt128.Parse(stack[0] as string);
                        string owner = (stack[2] as Slice).LoadAddress();
                        Slice cs = stack[3] as Slice;
                        ContentLayout contentLayout = (ContentLayout)cs.LoadInt(8);
                        switch (contentLayout)
                        {
                            case ContentLayout.ONCHAIN:
                                info = JettonFromOnchain(cs, state.Address, owner, totalSupply);
                                break;
                            case ContentLayout.OFFCHAIN:
                                info = JettonFrommOffChain(cs, state.Address, owner, totalSupply);
                                break;
                        }

                    }
                    Disposable.Dispose(stack);
                }
            }
            return info;
        }

        public static JettonWalletInfo GetJettonWalletInfo(AccountState state)
        {
            JettonWalletInfo winfo = null;
            if (state != null && state.Type == WalletType.JettonWallet)
            {
                object[] stack = state.RunMethod("get_wallet_data");
                if (stack != null)
                {
                    if (stack.Length >= 4)
                    {
                        UInt128 balance = UInt128.Parse(stack[0] as string);
                        string ownerAddress = (stack[1] as Slice).LoadAddress();
                        string jettonMasterAddress = (stack[2] as Slice).LoadAddress();
                        JettonInfo info = state.client.GetJettonInfo(jettonMasterAddress, false);
                        if (info != null)
                            winfo = new JettonWalletInfo(info, new Balance(info.Symbol, balance, info.Decimals, 3), state.Address, ownerAddress);
                    }
                    Disposable.Dispose(stack);
                }
            }
            return winfo;
        }

        private static ContractDeployData CreateDeployParams(long queryId, string owner, JettonInfo info, string offchainUri)
        {
            return new ContractDeployData(owner, null, ContractController.GetStateInit(Cell.FromBase64Boc(JettonMinterCode), JettonInitData(owner, info, offchainUri)), deployGas, Mint(queryId, owner, info.TotalSupply.Value, mintGas));
        }
    }
}
