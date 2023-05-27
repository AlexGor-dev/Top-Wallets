using System;
using Complex.Collections;

namespace Complex.Ton
{
    public static class NftController
    {
        public const string NftCollectionCode = "te6cckECFAEAAh8AART/APSkE/S88sgLAQIBYgIDAgLNBAUCASAODwTn0QY4BIrfAA6GmBgLjYSK3wfSAYAOmP6Z/2omh9IGmf6mpqGEEINJ6cqClAXUcUG6+CgOhBCFRlgFa4QAhkZYKoAueLEn0BCmW1CeWP5Z+A54tkwCB9gHAbKLnjgvlwyJLgAPGBEuABcYES4AHxgRgZgeACQGBwgJAgEgCgsAYDUC0z9TE7vy4ZJTE7oB+gDUMCgQNFnwBo4SAaRDQ8hQBc8WE8s/zMzMye1Ukl8F4gCmNXAD1DCON4BA9JZvpSCOKQakIIEA+r6T8sGP3oEBkyGgUyW78vQC+gDUMCJUSzDwBiO6kwKkAt4Ekmwh4rPmMDJQREMTyFAFzxYTyz/MzMzJ7VQALDI0AfpAMEFEyFAFzxYTyz/MzMzJ7VQAPI4V1NQwEDRBMMhQBc8WE8s/zMzMye1U4F8EhA/y8AIBIAwNAD1FrwBHAh8AV3gBjIywVYzxZQBPoCE8trEszMyXH7AIAC0AcjLP/gozxbJcCDIywET9AD0AMsAyYAAbPkAdMjLAhLKB8v/ydCACASAQEQAlvILfaiaH0gaZ/qamoYLehqGCxABDuLXTHtRND6QNM/1NTUMBAkXwTQ1DHUMNBxyMsHAc8WzMmAIBIBITAC+12v2omh9IGmf6mpqGDYg6GmH6Yf9IBhAALbT0faiaH0gaZ/qamoYCi+CeAI4APgCwGlAMbg==";
        public const string NftItemCode = "te6cckECDQEAAdAAART/APSkE/S88sgLAQIBYgIDAgLOBAUACaEfn+AFAgEgBgcCASALDALXDIhxwCSXwPg0NMDAXGwkl8D4PpA+kAx+gAxcdch+gAx+gAw8AIEs44UMGwiNFIyxwXy4ZUB+kDUMBAj8APgBtMf0z+CEF/MPRRSMLqOhzIQN14yQBPgMDQ0NTWCEC/LJqISuuMCXwSED/LwgCAkAET6RDBwuvLhTYAH2UTXHBfLhkfpAIfAB+kDSADH6AIIK+vCAG6EhlFMVoKHeItcLAcMAIJIGoZE24iDC//LhkiGOPoIQBRONkchQCc8WUAvPFnEkSRRURqBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7ABBHlBAqN1viCgBycIIQi3cXNQXIy/9QBM8WECSAQHCAEMjLBVAHzxZQBfoCFctqEssfyz8ibrOUWM8XAZEy4gHJAfsAAIICjjUm8AGCENUydtsQN0QAbXFwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7AJMwMjTiVQLwAwA7O1E0NM/+kAg10nCAJp/AfpA1DAQJBAj4DBwWW1tgAB0A8jLP1jPFgHPFszJ7VSC/dQQb";
        public const string NftSingleCode = "te6cckECFQEAAwoAART/APSkE/S88sgLAQIBYgIDAgLOBAUCASAREgIBIAYHAgEgDxAEuQyIccAkl8D4NDTAwFxsJJfA+D6QPpAMfoAMXHXIfoAMfoAMPACBtMf0z+CEF/MPRRSMLqOhzIQRxA2QBXgghAvyyaiUjC64wKCEGk9OVBSMLrjAoIQHARBKlIwuoAgJCgsAET6RDBwuvLhTYAH2UTfHBfLhkfpAIfAB+kDSADH6AIIK+vCAG6EhlFMVoKHeItcLAcMAIJIGoZE24iDCAPLhkiGOPoIQBRONkchQC88WUAvPFnEkSxRURsBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7ABBnlBAqOVviDACGFl8GbCJwyMsByXCCEIt3FzUhyMv/A9ATzxYTgEBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7AABUFl8GMwHQEoIQqMsArXCAEMjLBVAFzxYk+gIUy2oTyx/LPwHPFsmAQPsAAVyOhzIQRxA2QBXgMTI0NTWCEBoLnVESup9RE8cF8uGaAdTUMBAj8APgXwSED/LwDQCCAo41JvABghDVMnbbEDdGAG1xcIAQyMsFUAfPFlAF+gIVy2oSyx/LPyJus5RYzxcBkTLiAckB+wCTMDQ04lUC8AMB9lE2xwXy4ZH6QCHwAfpA0gAx+gCCCvrwgBuhIZRTFaCh3iLXCwHDACCSBqGRNuIgwv/y4ZIhjj6CEFEaRGPIUArPFlALzxZxJEoUVEawcIAQyMsFUAfPFlAF+gIVy2oSyx/LPyJus5RYzxcBkTLiAckB+wAQV5QQKjhb4g4AggKONSbwAYIQ1TJ22xA3RQBtcXCAEMjLBVAHzxZQBfoCFctqEssfyz8ibrOUWM8XAZEy4gHJAfsAkzAzNOJVAvADABU7UTQ+kD6QNTUMIAAbMhQBM8WWM8WzMzJ7VSACAVgTFAAjvH5/gBGBi4ZGWA5L+4AWggIcAB212v4ATYY6GmH6Yf9IBhAAEbQOngBCBGvgcOUAoqs=";

        public static UInt128 Fee = Gram.FromValue(0.5m);
        public static UInt128 mintGas = Gram.FromValue(0.2m);
        static UInt128 deployGas = Gram.FromValue(0.25m);

        public static NftDeployData CreateNftCollection(long queryId, string owner, string collectioContent, RoyaltyParams royaltyParams, NftMintItem[] items)
        {
            ContractDeployData deployParams = CreateDeployParams(queryId, owner, collectioContent, royaltyParams, items);
            string nftCollectionAddress = ContractController.GetAddress(0, deployParams.stateInit);
            //string nftItemAddress = ContractController.GetAddress(0, CalculateJettonWalletStateInit(owner, jettonMinterAddress, NftItemCode));
            return new NftDeployData(owner, queryId, nftCollectionAddress, null, deployParams, collectioContent, royaltyParams, items);
        }

        public static MessageData CreateMintData(string collectionAddress, long queryId, NftMintItem[] items)
        {
            return new MessageData(collectionAddress, Fee * (UInt128)items.Length, BatchMint(queryId, items));
        }

        public static MessageData CreateItemTransferData(string myNftItemAddress,long queryId, string fromOwnerAddress, string toOwnerAddress,  UInt128 passAmount)
        {
            return new MessageData(myNftItemAddress, Fee, ItemTransfer(queryId, toOwnerAddress, fromOwnerAddress, passAmount));
        }

        public static MessageData CreateChangeOwner(string collectionAddress, long queryId, string newOwner)
        {
            return new MessageData(collectionAddress, Fee, ChangeOwner(queryId, newOwner));
        }

        public static MessageData CreateChangeContent(string collectionAddress, long queryId, string collectionContent, string commonContent, RoyaltyParams royaltyParams)
        {
            return new MessageData(collectionAddress, Fee, ChangeContent(queryId, collectionContent, commonContent, royaltyParams));
        }

        public static MessageData CreateGetRoyaltyParams(string collectionAddress, long queryId)
        {
            return new MessageData(collectionAddress, Fee , GetRoyaltyParams(queryId));
        }

        public static MessageData CreateItemGetStaticData(string itemAddress, long queryId)
        {
            return new MessageData(itemAddress, Fee, ItemGetStaticData(queryId));
        }

        public static MessageData CreateItemGetRoyaltyParams(string itemAddress, long queryId)
        {
            return new MessageData(itemAddress, Fee, ItemGetRoyaltyParams(queryId));
        }

        public static MessageData CreateItemTransferEditorship(string itemAddress, long queryId, string newEditorAddress, string responseTo, UInt128 forwardAmount)
        {
            return new MessageData(itemAddress, Fee, ItemTransferEditorship(queryId, newEditorAddress, responseTo, forwardAmount));
        }

        public static MessageData CreateItemChangeContent(string itemAddress, long queryId, string itemContent, RoyaltyParams royaltyParams)
        {
            return new MessageData(itemAddress, Fee, ItemChangeContent(queryId, itemContent, royaltyParams));
        }

        public static NftInfo GetCollectionData(AccountState state)
        {
            NftInfo data = null;
            if (state != null && state.Type == WalletType.NftCollection)
            {
                object[] stack = state.RunMethod("get_collection_data");
                if (stack != null)
                {
                    if (stack.Length >= 3)
                    {
                        long nextItemId = long.Parse(stack[0] as string);
                        string content = Slice.LoadString(stack[1] as Slice);
                        string owner = (stack[2] as Slice).LoadAddress();
                        data = new NftInfo(state.Address, nextItemId, owner, null, null, content?.Replace("\u0001", ""));
                    }
                    Disposable.Dispose(stack);
                }
            }
            return data;

        }

        public static string GetNftAddressByIndex(AccountState state, long index)
        {
            string address = null;
            if (state != null && state.Type == WalletType.NftCollection)
            {
                object[] stack = state.RunMethod("get_nft_address_by_index", index);
                if (stack != null)
                {
                    if (stack.Length >= 1)
                        address = (stack[0] as Slice).LoadAddress();
                    Disposable.Dispose(stack);
                }
            }
            return address;
        }

        public static RoyaltyParams GetRoyaltyParams(AccountState state)
        {
            RoyaltyParams royaltyParams = null;
            if (state != null && (state.Type == WalletType.NftCollection || state.Type == WalletType.NftSingle))
            {
                object[] stack = state.RunMethod("royalty_params");
                if (stack != null)
                {
                    if (stack.Length >= 3)
                    {
                        int royaltyFactor = int.Parse(stack[0] as string);
                        int royaltyBase = int.Parse(stack[1] as string);
                        string royaltyAddress = (stack[2] as Slice).LoadAddress();
                        royaltyParams = new RoyaltyParams(royaltyFactor, royaltyBase, royaltyAddress);
                    }
                    Disposable.Dispose(stack);
                }
            }
            return royaltyParams;
        }

        public static string GetNftContent(AccountState state, long index, Cell nftIndividualContent)
        {
            string content = null;
            if (state != null && state.Type == WalletType.NftCollection)
            {
                object[] stack = state.RunMethod("get_nft_content", index, nftIndividualContent);
                if (stack != null)
                {
                    if (stack.Length >= 1)
                        content = Slice.LoadString(stack[0] as Slice);
                    Disposable.Dispose(stack);
                }
            }
            return content;
        }

        public static NftInfo GetNftData(AccountState state)
        {
            NftInfo data = null;
            if (state != null && (state.Type == WalletType.NftItem || state.Type == WalletType.NftSingle))
            {
                object[] stack = state.RunMethod("get_nft_data");
                if (stack != null)
                {
                    if (stack.Length >= 3)
                    {
                        bool inited = int.Parse(stack[0] as string) != 0;
                        long index = long.Parse(stack[1] as string);
                        string collectionAddress = (stack[2] as Slice).LoadAddress();
                        if (inited)
                        {
                            string ownerAddress = (stack[3] as Slice).LoadAddress();
                            string content = Slice.LoadString(stack[4] as Slice);
                            data = new NftInfo(state.Address, index, ownerAddress, null, collectionAddress, content?.Replace("\u0001", ""));
                        }
                        else
                        {
                            data = new NftInfo(state.Address, index, null, null, collectionAddress, null as string);
                        }
                    }
                    Disposable.Dispose(stack);
                }
            }
            return data;
        }

        public static string GetEditor(AccountState state)
        {
            string address = null;
            if (state != null && state.Type == WalletType.NftSingle)
            {
                object[] stack = state.RunMethod("get_editor");
                if (stack != null)
                {
                    if (stack.Length >= 1)
                        address = (stack[0] as Slice).LoadAddress();
                    Disposable.Dispose(stack);
                }
            }
            return address;
        }

        private static Cell Mint(long queryId, NftMintItem item)
        {
            Cell content = CellBuilder.StoreBuffer(item.content);
            Cell nftItemMessage = CellBuilder.Begin().StoreAddress(item.ownerAddress).StoreRef(content).End();
            return CellBuilder.Begin()
                .Store(TransactionType.NftMint, 32)
                .Store(queryId, 64)
                .Store(item.index, 64)
                .StoreCoins(item.passAmount)
                .StoreRef(nftItemMessage)
                .End();
        }


        public static Cell BatchMint(long queryId, NftMintItem[] items)
        {
            if (items.Length == 0)
                return null;
            if (items.Length == 1)
                return Mint(queryId, items[0]);
            Dict dict = new Dict();
            foreach (NftMintItem item in items)
                dict.Add(item.index.ToString(), item);

            Cell dictCell = SerializeDict.Serialize(dict, 64, (o, cell) =>
            {
                NftMintItem item = o as NftMintItem;
                Cell content = CellBuilder.Begin().Store(item.content).End();
                Cell nftItemMessage = CellBuilder.Begin().StoreAddress(item.ownerAddress).StoreRef(content).End();
                cell.StoreCoins(item.passAmount).StoreRef(nftItemMessage);
            });
            return CellBuilder.Begin()
                .Store(TransactionType.NftBatchMint, 32)
                .Store(queryId, 64)
                .StoreRef(dictCell)
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

        private static Cell GetRoyaltyParams(long queryId)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.NftGetRoyaltyParams, 32)
                .Store(queryId, 64)
                .End();
        }


        private static Cell RoyaltyParamsToCell(RoyaltyParams royaltyParams)
        {
            return CellBuilder.Begin()
                .Store(royaltyParams.royaltyFactor, 16)
                .Store(royaltyParams.royaltyBase, 16)
                .StoreAddress(royaltyParams.royaltyAddress)
                .End();
        }

        private static Cell ContentToCell(string content)
        {
            return CellBuilder.Begin().Store(content).End();
        }

        private static Cell ChangeContent(long queryId, string collectionContent, string commonContent, RoyaltyParams royaltyParams)
        {
            Cell contentCell = CellBuilder.Begin().StoreRef(ContentToCell(collectionContent)).StoreRef(ContentToCell(commonContent)).End();

            return CellBuilder.Begin()
                .Store(TransactionType.ChangeContent, 32)
                .Store(queryId, 64)
                .StoreRef(contentCell)
                .StoreRef(RoyaltyParamsToCell(royaltyParams))
                .End();
        }

        private static Cell ItemTransfer(long queryId, string newOwner, string responseTo, UInt128 forwardAmount)
        {
            return CellBuilder.Begin()
                    .Store(TransactionType.NftTransfer, 32)
                    .Store(queryId, 64)
                    .StoreAddress(newOwner)
                    .StoreAddress(responseTo)
                    .Store(false)
                    .StoreCoins(forwardAmount)
                    .Store(0, 1)
                    .End();
        }

        private static Cell ItemGetStaticData(long queryId)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.NftGetStaticData, 32)
                .Store(queryId, 64)
                .End();
        }

        private static Cell ItemGetRoyaltyParams(long queryId)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.NftGetRoyaltyParams, 32)
                .Store(queryId, 64)
                .End();
        }

        private static Cell ItemChangeContent(long queryId, string content, RoyaltyParams royaltyParams)
        {
            return CellBuilder.Begin()
                    .Store(TransactionType.ChangeContent, 32)
                    .Store(queryId, 64)
                    .StoreRef(ContentToCell(content))
                    .StoreRef(RoyaltyParamsToCell(royaltyParams))
                    .End();
        }

        private static Cell ItemTransferEditorship(long queryId, string newEditorAddress, string responseTo, UInt128 forwardAmount)
        {
            return CellBuilder.Begin()
                .Store(TransactionType.NftTransferEditorship, 32)
                .Store(queryId, 64)
                .StoreAddress(newEditorAddress)
                .StoreAddress(responseTo)
                .Store(false)
                .StoreCoins(forwardAmount)
                .Store(0, 1)
                .End();
        }

        private static Cell CollectionInitData(string owner, string content, RoyaltyParams royaltyParams)
        {
            return CellBuilder.Begin()
                .StoreAddress(owner)
                .Store(-1, 64)
                .StoreRef(ContentToCell(content))
                .StoreRef(NftItemCode)
                .StoreRef(RoyaltyParamsToCell(royaltyParams))
                .End();
        }

        private static ContractDeployData CreateDeployParams(long queryId, string owner, string content, RoyaltyParams royaltyParams, NftMintItem[] items)
        {
            return new ContractDeployData(owner, null, ContractController.GetStateInit(Cell.FromBase64Boc(NftCollectionCode), CollectionInitData(owner, content, royaltyParams)), deployGas, BatchMint(queryId, items));
        }

    }
}