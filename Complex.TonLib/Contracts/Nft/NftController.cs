using System;
using Complex.Collections;

namespace Complex.Ton
{
    public static class NftController
    {
        public const string NftCollectionCode = "te6cckECFAEAAh8AART/APSkE/S88sgLAQIBYgIDAgLNBAUCASAODwTn0QY4BIrfAA6GmBgLjYSK3wfSAYAOmP6Z/2omh9IGmf6mpqGEEINJ6cqClAXUcUG6+CgOhBCFRlgFa4QAhkZYKoAueLEn0BCmW1CeWP5Z+A54tkwCB9gHAbKLnjgvlwyJLgAPGBEuABcYES4AHxgRgZgeACQGBwgJAgEgCgsAYDUC0z9TE7vy4ZJTE7oB+gDUMCgQNFnwBo4SAaRDQ8hQBc8WE8s/zMzMye1Ukl8F4gCmNXAD1DCON4BA9JZvpSCOKQakIIEA+r6T8sGP3oEBkyGgUyW78vQC+gDUMCJUSzDwBiO6kwKkAt4Ekmwh4rPmMDJQREMTyFAFzxYTyz/MzMzJ7VQALDI0AfpAMEFEyFAFzxYTyz/MzMzJ7VQAPI4V1NQwEDRBMMhQBc8WE8s/zMzMye1U4F8EhA/y8AIBIAwNAD1FrwBHAh8AV3gBjIywVYzxZQBPoCE8trEszMyXH7AIAC0AcjLP/gozxbJcCDIywET9AD0AMsAyYAAbPkAdMjLAhLKB8v/ydCACASAQEQAlvILfaiaH0gaZ/qamoYLehqGCxABDuLXTHtRND6QNM/1NTUMBAkXwTQ1DHUMNBxyMsHAc8WzMmAIBIBITAC+12v2omh9IGmf6mpqGDYg6GmH6Yf9IBhAALbT0faiaH0gaZ/qamoYCi+CeAI4APgCwGlAMbg==";
        public const string NftItemCode = "te6cckECDQEAAdAAART/APSkE/S88sgLAQIBYgIDAgLOBAUACaEfn+AFAgEgBgcCASALDALXDIhxwCSXwPg0NMDAXGwkl8D4PpA+kAx+gAxcdch+gAx+gAw8AIEs44UMGwiNFIyxwXy4ZUB+kDUMBAj8APgBtMf0z+CEF/MPRRSMLqOhzIQN14yQBPgMDQ0NTWCEC/LJqISuuMCXwSED/LwgCAkAET6RDBwuvLhTYAH2UTXHBfLhkfpAIfAB+kDSADH6AIIK+vCAG6EhlFMVoKHeItcLAcMAIJIGoZE24iDC//LhkiGOPoIQBRONkchQCc8WUAvPFnEkSRRURqBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7ABBHlBAqN1viCgBycIIQi3cXNQXIy/9QBM8WECSAQHCAEMjLBVAHzxZQBfoCFctqEssfyz8ibrOUWM8XAZEy4gHJAfsAAIICjjUm8AGCENUydtsQN0QAbXFwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7AJMwMjTiVQLwAwA7O1E0NM/+kAg10nCAJp/AfpA1DAQJBAj4DBwWW1tgAB0A8jLP1jPFgHPFszJ7VSC/dQQb";
        public const string NftSingleCode = "te6cckECFQEAAwoAART/APSkE/S88sgLAQIBYgIDAgLOBAUCASAREgIBIAYHAgEgDxAEuQyIccAkl8D4NDTAwFxsJJfA+D6QPpAMfoAMXHXIfoAMfoAMPACBtMf0z+CEF/MPRRSMLqOhzIQRxA2QBXgghAvyyaiUjC64wKCEGk9OVBSMLrjAoIQHARBKlIwuoAgJCgsAET6RDBwuvLhTYAH2UTfHBfLhkfpAIfAB+kDSADH6AIIK+vCAG6EhlFMVoKHeItcLAcMAIJIGoZE24iDCAPLhkiGOPoIQBRONkchQC88WUAvPFnEkSxRURsBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7ABBnlBAqOVviDACGFl8GbCJwyMsByXCCEIt3FzUhyMv/A9ATzxYTgEBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7AABUFl8GMwHQEoIQqMsArXCAEMjLBVAFzxYk+gIUy2oTyx/LPwHPFsmAQPsAAVyOhzIQRxA2QBXgMTI0NTWCEBoLnVESup9RE8cF8uGaAdTUMBAj8APgXwSED/LwDQCCAo41JvABghDVMnbbEDdGAG1xcIAQyMsFUAfPFlAF+gIVy2oSyx/LPyJus5RYzxcBkTLiAckB+wCTMDQ04lUC8AMB9lE2xwXy4ZH6QCHwAfpA0gAx+gCCCvrwgBuhIZRTFaCh3iLXCwHDACCSBqGRNuIgwv/y4ZIhjj6CEFEaRGPIUArPFlALzxZxJEoUVEawcIAQyMsFUAfPFlAF+gIVy2oSyx/LPyJus5RYzxcBkTLiAckB+wAQV5QQKjhb4g4AggKONSbwAYIQ1TJ22xA3RQBtcXCAEMjLBVAHzxZQBfoCFctqEssfyz8ibrOUWM8XAZEy4gHJAfsAkzAzNOJVAvADABU7UTQ+kD6QNTUMIAAbMhQBM8WWM8WzMzJ7VSACAVgTFAAjvH5/gBGBi4ZGWA5L+4AWggIcAB212v4ATYY6GmH6Yf9IBhAAEbQOngBCBGvgcOUAoqs=";

        public readonly static UInt128 Fee = Gram.FromValue(0.5m);
        public readonly static UInt128 mintGas = Gram.FromValue(0.2m);
        public readonly static UInt128 deployGas = Gram.FromValue(0.25m);
        public static UInt128 MessageTransferFee = Gram.FromValue(0.05m);

        public static NftDeployData CreateNftCollection(long queryId, string owner, string collectioContent, RoyaltyParams royaltyParams, NftMintItemInfo[] items)
        {
            ContractDeployData deployParams = CreateCollectionDeployParams(queryId, owner, collectioContent, royaltyParams, items);
            string nftCollectionAddress = ContractController.GetAddress(0, deployParams.stateInit);
            //string nftItemAddress = ContractController.GetAddress(0, CalculateJettonWalletStateInit(owner, jettonMinterAddress, NftItemCode));
            return new NftDeployData(owner, nftCollectionAddress, null, deployParams, collectioContent, royaltyParams, items);
        }

        public static NftSingleDeployData CreateNftSingle(NftSingleInfo info, UInt128 passAmount)
        {
            ContractDeployData deployParams = CreateSingleDeployParams(info, passAmount);
            return new NftSingleDeployData(ContractController.GetAddress(0, deployParams.stateInit), deployParams, info);
        }

        public static MessageData CreateMintData(long queryId, string collectionAddress, NftMintItemInfo[] items)
        {
            return new MessageData(collectionAddress, Fee * (UInt128)items.Length, BatchMint(queryId, items));
        }

        public static MessageData CreateItemTransfer(long queryId, string myNftItemAddress, string toOwnerAddress, string responseTo, UInt128 forwardAmount)
        {
            return new MessageData(myNftItemAddress, Fee, ItemTransfer(queryId, toOwnerAddress, responseTo, forwardAmount));
        }

        public static MessageData CreateChangeOwner(long queryId, string collectionAddress, string newOwner)
        {
            return new MessageData(collectionAddress, Fee, ChangeOwner(queryId, newOwner));
        }

        public static MessageData CreateChangeContent(long queryId, string collectionAddress, string collectionContent, string commonContent, RoyaltyParams royaltyParams)
        {
            return new MessageData(collectionAddress, Fee, ChangeContent(queryId, collectionContent, commonContent, royaltyParams));
        }

        public static MessageData CreateGetRoyaltyParams(long queryId, string collectionAddress)
        {
            return new MessageData(collectionAddress, Fee , GetRoyaltyParams(queryId));
        }

        public static MessageData CreateItemGetStaticData(long queryId, string itemAddress)
        {
            return new MessageData(itemAddress, Fee, ItemGetStaticData(queryId));
        }

        public static MessageData CreateItemGetRoyaltyParams(long queryId, string itemAddress)
        {
            return new MessageData(itemAddress, Fee, ItemGetRoyaltyParams(queryId));
        }

        public static MessageData CreateItemTransferEditorship(long queryId, string itemAddress, string newEditorAddress, string responseTo, UInt128 forwardAmount)
        {
            return new MessageData(itemAddress, Fee, ItemTransferEditorship(queryId, newEditorAddress, responseTo, forwardAmount));
        }

        public static MessageData CreateItemChangeContent(long queryId, string itemAddress, string itemContent, RoyaltyParams royaltyParams)
        {
            return new MessageData(itemAddress, Fee, ItemChangeContent(queryId, itemContent, royaltyParams));
        }

        public static NftInfo GetCollectionData(AccountState state)
        {
            NftInfo info = null;
            if (state != null && state.Type == WalletType.NftCollection)
            {
                object[] stack = state.RunMethod("get_collection_data");
                if (stack != null)
                {
                    if (stack.Length >= 3)
                    {
                        long nextItemId = long.Parse(stack[0] as string);
                        string owner = (stack[2] as Slice).LoadAddress();
                        Slice slice = stack[1] as Slice;
                        string content = Slice.LoadString(slice);
                        info = new NftInfo(state.Address, nextItemId, owner, null, null, content?.Replace("\u0001", ""));
                    }
                    Disposable.Dispose(stack);
                }
            }
            return info;

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
            NftInfo info = null;
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
                            Slice slice = stack[4] as Slice;
                            string content = Slice.LoadString(slice);
                            if (state.Type == WalletType.NftSingle)
                            {
                                RoyaltyParams royaltyParams = GetRoyaltyParams(state);
                                string editor = GetEditor(state);
                                info = new NftSingleInfo(state.Address, ownerAddress, editor, content?.Replace("\u0001", ""), royaltyParams);
                            }
                            else
                            {
                                string collectionUrl = null;
                                if (state.Type == WalletType.NftItem && !string.IsNullOrEmpty(collectionAddress))
                                {
                                    AccountState cs = state.client.CreateAccountState(collectionAddress).state;
                                    if (cs != null)
                                    {
                                        NftInfo cinfo = GetCollectionData(cs);
                                        cs.Dispose();
                                    }
                                }
                                info = new NftInfo(state.Address, index, ownerAddress, null, collectionAddress, collectionUrl + content?.Replace("\u0001", ""));
                            }
                        }
                        else
                        {
                            info = new NftInfo(state.Address, index, null, null, collectionAddress, null as string);
                        }
                    }
                    Disposable.Dispose(stack);
                }
            }
            return info;
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

        private static Cell Mint(long queryId, NftMintItemInfo item)
        {
            Cell nftItemMessage = CellBuilder.Begin().StoreAddress(item.ownerAddress).StoreRef(ContentToCell(item.content)).End();
            return CellBuilder.Begin()
                .Store(TransactionType.NftMint, 32)
                .Store(queryId, 64)
                .Store(item.index, 64)
                .StoreCoins(item.passAmount)
                .StoreRef(nftItemMessage)
                .End();
        }

        public static Cell BatchMint(long queryId, NftMintItemInfo[] items)
        {
            if (items.Length == 0)
                return null;
            if (items.Length == 1)
                return Mint(queryId, items[0]);
            Dict dict = new Dict();
            foreach (NftMintItemInfo item in items)
                dict.Add(item.index.ToString(), item);

            Cell dictCell = SerializeDict.Serialize(dict, 64, (o, cell) =>
            {
                NftMintItemInfo item = o as NftMintItemInfo;
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
                .Store(royaltyParams.numerator, 16)
                .Store(royaltyParams.denominator, 16)
                .StoreAddress(royaltyParams.destination)
                .End();
        }

        private static Cell ContentToCell(string content)
        {
            return CellBuilder.StoreBuffer(content);
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


        private static Cell BuildSingle(NftSingleInfo info)
        {
            return CellBuilder.Begin()
                .StoreAddress(info.OwnerAddress)
                .StoreAddress(info.EditorAddress)
                .StoreRef(ContentToCell("\u0001" + info.Content))
                .StoreRef(RoyaltyParamsToCell(info.RoyaltyParams))
                .End();
        }

        private static ContractDeployData CreateCollectionDeployParams(long queryId, string owner, string content, RoyaltyParams royaltyParams, NftMintItemInfo[] items)
        {
            return new ContractDeployData(owner, null, ContractController.GetStateInit(Cell.FromBase64Boc(NftCollectionCode), CollectionInitData(owner, content, royaltyParams)), deployGas, BatchMint(queryId, items));
        }

        private static ContractDeployData CreateSingleDeployParams(NftSingleInfo info, UInt128 passAmount)
        {
            return new ContractDeployData(info.OwnerAddress, null, ContractController.GetStateInit(Cell.FromBase64Boc(NftSingleCode), BuildSingle(info)), passAmount, null);
        }

    }
}