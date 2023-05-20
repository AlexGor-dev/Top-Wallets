using System;
using System.Collections.Generic;
using System.Text;

namespace Complex.Ton
{
    public enum ContractState
    {
        Invalid = -1,
        None,
        Initialized,
        Active,
        Frozen
    }

    public enum VmObjectType
    {
        Unknown,
        Number,
        Cell,
        Slice,
    }

    public enum WalletType
    {
        Empty,
        Unknown,
        Giver,
        WalletV1,
        WalletV1Ext,
        WalletV2,
        WalletV3,
        WalletV4,
        HighloadWalletV1,
        HighloadWalletV2,
        ManualDns,
        Multisig,
        PaymentChannel,
        RestrictedWallet,
        NftCollection,
        NftItem,
        NftSingle,
        NftMarketplace,
        NftSale,
        JettonMinter,
        JettonWallet,
    }

    public enum AccountStatus
    {
        Uninit, Frozen, Active, Nonexist
    }

    public enum MessageType : int
    {
        None,
        Text,
        Raw,
        DecryptedText,
        EncryptedText
    }
    public enum ContentLayout
    {
        ONCHAIN = 0x00,
        OFFCHAIN = 0x01,
    }

    public enum DataFormat
    {
        SNAKE = 0x00,
        CHUNK = 0x01,
    }

    public enum TransactionType : uint
    {
        None = 0,
        NftMint = 1,
        NftBatchMint = 2,

        ChangeOwner = 3,
        ChangeContent = 4,

        Mint = 21,
        Transfer = 0xf8a7ea5,
        TransferNotification = 0x7362d09c,
        InternalTransfer = 0x178d4519,
        Excesses = 0xd53276db,
        Burn = 0x595f07bc,
        BurnNotification = 0x7bdd97de,


        Increment = 0x37491f2f,
        Deposit = 0x47d54391,
        Withdraw = 0x41836980,
        TransferOwnership = 0x2da38aaf,
        ProvideWalletAddress = 0x2c76b973,
        TakeWalletAddress = 0xd1735400,

        DeployNewNft = 1,
        BatchDeployOfNewNfts = 2,
        NftTransfer = 0x5fcc3d14,
        NftOwnershipAssigned = 0x05138d91,
        NftGetStaticData = 0x2fcb26a2,
        NftReportStaticData = 0x8b771735,
        NftGetRoyaltyParams = 0x693d3950,
        NftReportRoyaltyParams = 0xa8cb00ad,

        NftEditContent = 0x1a0b9d51,
        NftTransferEditorship = 0x1c04412a,
        NftEditorshipAssigned = 0x511a4463,

        NftRequestOwner = 0xd0c3bfea,
        NftOwnerInfo = 0x0dd607e3,

        NftProveOwnership = 0x04ded148,
        NftOwnershipProof = 0x0524c7ae,
        NftOwnershipProofBounced = 0xc18e86d2,

        NftDestroy = 0x1f04537a,
        NftRevoke = 0x6f89f5e3,
        NftTakeExcess = 0xd136d3b3,
    }


    public static class Ex
    {
        public static string ToString2(this TransactionType type)
        {
            string name = Enum.GetName(type.GetType(), type);
            if (name != null)
                return name;
            return null;
            //return "UnknownTr";
        }
    }
}
