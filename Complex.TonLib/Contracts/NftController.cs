using System;
using Complex.Collections;

namespace Complex.Ton
{
    public static class NftController
    {
        public static UInt128 Fee = Gram.FromValue(0.5m);

        public static MessageData CreateMintData(string colectionAddress, long queryId, UInt128 passAmount, long itemIndex, string itemOwnerAddress, string itemContent)
        {
            return new MessageData(colectionAddress, Fee, Mint(queryId, passAmount, itemIndex, itemOwnerAddress, itemContent));
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

        public static MessageData CreateBatchMint(string collectionAddress, long queryId, NftMintItem[] items)
        {
            return new MessageData(collectionAddress, Fee * (UInt128)items.Length, BatchMint(queryId, items));
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

        public static MessageData CreateItemChangeContent(string itemAddress, long queryId, string itenContent, RoyaltyParams royaltyParams)
        {
            return new MessageData(itemAddress, Fee, ItemChangeContent(queryId, itenContent, royaltyParams));
        }

        public static NftCollectionData GetCollectionData(AccountState state)
        {
            NftCollectionData data = null;
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
                        data = new NftCollectionData(nextItemId, content, owner);
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

        public static NftData GetNftData(AccountState state)
        {
            NftData data = null;
            if (state != null && (state.Type == WalletType.NftItem || state.Type == WalletType.NftSingle))
            {
                object[] stack = state.RunMethod("get_nft_data");
                if (stack != null)
                {
                    if (stack.Length >= 3)
                    {
                        bool inited = int.Parse(stack[0] as string) != 0;
                        long index = (long)(Int128)stack[1];
                        string collectionAddress = (stack[2] as Slice).LoadAddress();
                        if (inited)
                        {
                            string ownerAddress = (stack[3] as Slice).LoadAddress();
                            string content = Slice.LoadString(stack[4] as Slice);
                            data = new NftData(inited, index, collectionAddress, ownerAddress, content);
                        }
                        else
                        {
                            data = new NftData(inited, index, collectionAddress);
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

        private static Cell Mint(long queryId, UInt128 passAmount, long itemIndex, string itemOwnerAddress, string itemContent)
        {
            Cell content = CellBuilder.StoreBuffer(itemContent);
            Cell nftItemMessage = CellBuilder.Begin().StoreAddress(itemOwnerAddress).StoreRef(content).End();
            return CellBuilder.Begin()
                .Store(TransactionType.NftMint, 32)
                .Store(queryId, 64)
                .Store(itemIndex, 64)
                .StoreCoins(passAmount)
                .StoreRef(nftItemMessage)
                .End();
        }


        public static Cell BatchMint(long queryId, NftMintItem[] items)
        {
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


        private static Cell ChangeContent(long queryId, string collectionContent, string commonContent, RoyaltyParams royaltyParams)
        {
            Cell royaltyCell = CellBuilder.Begin()
                .Store(royaltyParams.royaltyFactor, 16)
                .Store(royaltyParams.royaltyBase, 16)
                .StoreAddress(royaltyParams.royaltyAddress)
                .End();

            Cell collContent = CellBuilder.Begin().Store(collectionContent).End();
            Cell comContent = CellBuilder.Begin().Store(commonContent).End();
            Cell contentCell = CellBuilder.Begin().StoreRef(collContent).StoreRef(comContent).End();

            return CellBuilder.Begin()
                .Store(TransactionType.ChangeContent, 32)
                .Store(queryId, 64)
                .StoreRef(contentCell)
                .StoreRef(royaltyCell)
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
            Cell royaltyCell = CellBuilder.Begin()
                .Store(royaltyParams.royaltyFactor, 16)
                .Store(royaltyParams.royaltyBase, 16)
                .StoreAddress(royaltyParams.royaltyAddress)
                .End();
            Cell contentCell = CellBuilder.Begin().Store(content).End();
            return CellBuilder.Begin()
                    .Store(TransactionType.ChangeContent, 32)
                    .Store(queryId, 64)
                    .StoreRef(contentCell)
                    .StoreRef(royaltyCell)
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
    }
}