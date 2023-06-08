using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Complex.Collections;

namespace Complex.Ton
{
    internal unsafe class TonLib
    {
//#if DEBUG
//        private const string Dll = @"E:\Complex\Ton\Top-Wallets\tonlib_win\x64\Debug\tonlib.dll";
//#else
        private const string Dll = "tonlib.dll";
//#endif


        [DllImport(Dll)]
        public static extern void LiteClientGetServerTime(IntPtr handle, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern IntPtr LiteClientCreate(string name, string directory, string config, string validConfig, IntPtr connectedHandler);

        [DllImport(Dll)]
        public static extern void LiteClientDestroy(IntPtr handle);

        [DllImport(Dll)]
        public static extern void LiteClientConnect(IntPtr handle);

        [DllImport(Dll)]
        public static extern int LiteClientGetServerIndex(IntPtr handle);

        [DllImport(Dll)]
        public static extern bool LiteClientIsConnected(IntPtr handle);

        [DllImport(Dll)]
        public static extern void LiteClientSend(IntPtr handle, long cellHandle, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientLast(IntPtr handle, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientGetTransactions(IntPtr handle, string address, long lt, byte[] lt_hash, IntPtr resultHandler, int count, IntPtr handler);



        [DllImport(Dll)]
        private static extern bool LiteClientGetImportKey(IntPtr handle, byte[] mnemonicPassword, string[] worlds, byte[] publicKey, byte[] password, byte[] secret, byte[] builder, out int len, IntPtr resultHandler);

        [DllImport(Dll)]
        private static extern bool LiteClientCreateWalletAddress(IntPtr handle, byte[] mnemonicPassword, byte[] publicKey, byte[] password, byte[] secret, byte[] seed, byte[] builder, out int len, IntPtr wordsHandler, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern bool LiteClientGetWords(IntPtr handle, byte[] publicKey, byte[] password, byte[] secret, IntPtr wordsHandler, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientCreateWallet(IntPtr handle, byte[] publicKey, byte[] password, byte[] secret, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientSendMessage(IntPtr handle, string srcAddress, byte[] publicKey, byte[] password, byte[] secret, MessageInfo[] messages, int messagesLen, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientCreateSendMessageCell(IntPtr handle, string srcAddress, byte[] publicKey, byte[] password, byte[] secret, MessageInfo[] messages, int messagesLen, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientCalcFee(IntPtr handle, string srcAddress, MessageInfo[] messages, int messagesLen, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void LiteClientGetAccountState(IntPtr handle, string address, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern bool LiteClientExportKey(IntPtr handle, string dataPassword, byte[] publicKey, byte[] password, byte[] secret, byte[] keyData, out int keyDataLen, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern bool LiteClientImportKey(IntPtr handle, string dataPassword, byte[] password, byte[] outSecret, byte[] keyData, int keyDataLen, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern bool LiteClientDeleteKey(IntPtr handle, byte[] publicKey, byte[] secret, IntPtr resultHandler);

        [DllImport(Dll)]
        private static extern void LiteClientGetStateInit(IntPtr client, IntPtr account, byte[] publicKey, StringBuilder builder, out int len);

        [DllImport(Dll)]
        public static extern bool LiteClientSign(IntPtr client, byte[] publicKey, byte[] password, byte[] secret, byte[] message, int messageLen, byte[] resSign, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern bool LiteClientGetPrivateKey(IntPtr client, byte[] publicKey, byte[] password, byte[] secret, byte[] outPrivateKey, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern bool LiteClientGetSeed(IntPtr client, byte[] publicKey, byte[] password, byte[] secret, byte[] outSeed, IntPtr resultHandler);

        [DllImport(Dll)]
        public static extern void AccountDestroy(IntPtr handle);
        [DllImport(Dll)]
        public static extern long AccountBalance(IntPtr handle);
        [DllImport(Dll)]
        public static extern long AccountTransaction(IntPtr handle);
        [DllImport(Dll)]
        public static extern long AccountTime(IntPtr handle);

        [DllImport(Dll)]
        public static extern void AccountTransactionHash(IntPtr handle, byte[] hash);

        [DllImport(Dll)]
        public static extern long AccountSeqno(IntPtr handle);
        [DllImport(Dll)]
        public static extern ContractState AccountGetState(IntPtr handle);

        [DllImport(Dll)]
        private static extern void AccountVersion(IntPtr handle, byte[] chs, out int len);
        [DllImport(Dll)]
        private static extern void AccountVersion(IntPtr handle, StringBuilder builder, out int len);

        [DllImport(Dll)]
        private static extern void AccountAddress(IntPtr handle, StringBuilder builder, out int len);

        [DllImport(Dll)]
        public static extern void AccountGetMsgHash(IntPtr handle, byte[] res);

        [DllImport(Dll)]
        public static extern WalletType AccountType(IntPtr handle);



        [DllImport(Dll)]
        private static extern void AccountRunMethod(IntPtr handle, string methodName, IntPtr[] param, int paramsLen, IntPtr vmHandler);

        [DllImport(Dll)]
        private static extern void DeleteVmObject(IntPtr handle);

        [DllImport(Dll)]
        public static extern IntPtr AddressCreate(int wc, byte[] rdata, bool bounceable, bool testnet);
        [DllImport(Dll)]
        public static extern IntPtr AddressParse(string address);
        [DllImport(Dll)]
        public static extern void AddressDestroy(IntPtr handle);

        [DllImport(Dll)]
        public static extern bool AddressIsValid(string address);

        [DllImport(Dll)]
        public static extern int AddressWorkchain(IntPtr handle);

        [DllImport(Dll)]
        private static extern void AddressToString(IntPtr handle, StringBuilder builder, out int len);

        [DllImport(Dll)]
        private static extern void AddressFromDataToString(int wc, byte[] rdata, bool bounceable, bool testnet, StringBuilder builder, out int len);

        [DllImport(Dll)]
        private static extern void AddressFromHex(string hexAddress, StringBuilder builder, out int len);

        [DllImport(Dll)]
        private static extern void AddressToHex(string hexAddress, StringBuilder builder, out int len);

        [DllImport(Dll)]
        public static extern bool AddressCompareTo(IntPtr handle, long to);
        [DllImport(Dll)]
        public static extern void AddressData(IntPtr handle, byte[] data);
        [DllImport(Dll)]
        public static extern int AddressGetData(string address, byte[] data);


        [DllImport(Dll)]
        public static extern IntPtr CellBuilderCreate();
        [DllImport(Dll)]
        public static extern void CellBuilderDestroy(IntPtr handle);
        [DllImport(Dll)]
        public static extern bool CellBuilderStoreLong(IntPtr handle, long value, int bits);

        [DllImport(Dll)]
        public static extern bool CellBuilderStoreBigInt(IntPtr builder, string dec, int bits);

        [DllImport(Dll)]
        public static extern bool CellBuilderStoreAddress(IntPtr builder, string address);

        [DllImport(Dll)]
        public static extern bool CellBuilderStoreRef(IntPtr handle, IntPtr cell);
        [DllImport(Dll)]
        public static extern bool CellBuilderStoreBytes(IntPtr handle, byte[] data, int offset, int len);
        [DllImport(Dll)]
        public static extern bool CellBuilderStoreSlice(IntPtr handle, IntPtr slice);

        [DllImport(Dll)]
        public static extern bool CellBuilderStoreDict(IntPtr builder, IntPtr dict);

        [DllImport(Dll)]
        public static extern bool CellBuilderStoreDictFromCell(IntPtr builder, IntPtr rootCell);

        [DllImport(Dll)]
        public static extern IntPtr CellBuilderFinalize(IntPtr handle);
        [DllImport(Dll)]
        public static extern int CellBuilderBits(IntPtr handle);
        [DllImport(Dll)]
        public static extern int CellBuilderRefs(IntPtr handle);

        [DllImport(Dll)]
        public static extern void CellDestroy(IntPtr handle);
        [DllImport(Dll)]
        public static extern int CellBits(IntPtr handle);
        [DllImport(Dll)]
        public static extern int CellRefs(IntPtr handle);
        [DllImport(Dll)]
        public static extern IntPtr CellFromBoc(byte[] bocData, int len);

        [DllImport(Dll)]
        private static extern void CellGetHash(IntPtr handle, byte[] data);

        [DllImport(Dll)]
        private static extern void CellGetData(IntPtr handle, byte** data, int* len);

        [DllImport(Dll)]
        public static extern IntPtr CellSliceCreate(IntPtr cell);
        [DllImport(Dll)]
        public static extern void CellSliceDestroy(IntPtr handle);
        [DllImport(Dll)]
        public static extern int CellSliceBits(IntPtr handle);
        [DllImport(Dll)]
        public static extern int CellSliceRefs(IntPtr handle);
        [DllImport(Dll)]
        public static extern bool CellSliceIsValid(IntPtr handle);
        [DllImport(Dll)]
        public static extern ulong CellSliceLoadLong(IntPtr handle, int bits);
        [DllImport(Dll)]
        private static extern void CellSliceLoadBigInt(IntPtr handle, int bits, StringBuilder builder, out int len);

        [DllImport(Dll)]
        public static extern ulong CellSlicePreLoadLong(IntPtr handle, int bits);

        [DllImport(Dll)]
        private static extern void CellSliceLoadBytes(IntPtr handle, byte[] data, int length);

        [DllImport(Dll)]
        public static extern IntPtr CellSliceLoadRef(IntPtr handle);

        [DllImport(Dll)]
        public static extern IntPtr CellSliceLoadRefCell(IntPtr handle);

        [DllImport(Dll)]
        public static extern IntPtr CellSliceLoadSlice(IntPtr handle);

        [DllImport(Dll)]
        public static extern IntPtr CellSliceLoadDict(IntPtr slice, int keySizeBits);

        [DllImport(Dll)]
        public static extern int CellSliceBSelect(IntPtr handle, int bits, int mask);

        //[DllImport(Dll)]
        //public static extern bool CellUnpackMessage(long cellHandle, MessageRecord message);

        [DllImport(Dll)]
        private static extern void CellSliceLoadAddress(IntPtr handle, StringBuilder builder, out int len);

        [DllImport(Dll)]
        private static extern void CellSliceGetAddressExt(IntPtr handle, StringBuilder builder, out int len);

        [DllImport(Dll)]
        public static extern long CellSliceLoadBalance(IntPtr handle);

        [DllImport(Dll)]
        public static extern long CellSliceGetFee(IntPtr handle);

        [DllImport(Dll)]
        public static extern long CellSliceGetStorageFeeRef(IntPtr handle);
        [DllImport(Dll)]
        public static extern IntPtr CellSliceFromBoc(byte[] bocData, int len);


        [DllImport(Dll)]
        private static extern bool PrivateKeyGenerate(byte[] res);

        [DllImport(Dll)]
        private static extern bool PrivateKeyGetPublicKey(byte[] privKey, byte[] res);

        [DllImport(Dll)]
        private static extern bool PrivateKeySign(byte[] privKey, IntPtr cell, byte** res, int* resLen);

        [DllImport(Dll)]
        private static extern bool PublicKeyEncrypt(byte[] pubKey, byte[] data, int dataLen, byte** res, int* resLen);

        [DllImport(Dll)]
        private static extern bool PrivateKeyDecrypt(byte[] privKey, byte[] data, int dataLen, byte** res, int* resLen);

        [DllImport(Dll)]
        public static extern void KeyEncryptDecrypt(byte[] pubKey, byte[] privKey, byte* data, int dataLen, byte** res, int* resLen);

        [DllImport(Dll)]
        public static extern void PrivateKeyToString(byte[] data, byte[] builder, out int len);

        [DllImport(Dll)]
        public static extern void PublicKeyToString(byte[] data, byte[] builder, out int len);

        [DllImport(Dll)]
        public static extern bool PrivateKeyParse(string data, byte[] res);

        [DllImport(Dll)]
        public static extern bool PublicKeyParse(string data, byte[] res);

        [DllImport(Dll)]
        public static extern bool PublicKeyVerify(byte[] publicKey, byte[] message, int messageLen, byte[] signature);
        [DllImport(Dll)]
        public static extern IntPtr DictionaryCreate(int keySizeBits);
        [DllImport(Dll)]
        public static extern void DictionaryDestroy(IntPtr handle);

        [DllImport(Dll)]
        public static extern void DictionaryStoreRef(IntPtr dict, byte[] key, int keyLen, IntPtr cell);

        [DllImport(Dll)]
        public static extern IntPtr DictionaryFindRef(IntPtr dict, byte[] key, int keyLen);

        [DllImport(Dll)]
        public static extern IntPtr DictionaryGetRootCell(IntPtr dict);

        [DllImport(Dll)]
        private static extern void BigIntegerGetData(long value, byte[] res, out int resLen);

        [DllImport(Dll)]
        private static extern void BigIntegerGetDataFromString(string value, byte[] res, out int resLen);

        [DllImport(Dll)]
        public static extern void UtilsAesIgeEncryption(byte[] buffer, byte[] key, byte[] iv, bool encrypt, int offset, int length);

        [DllImport(Dll)]
        public static extern void UtilsAesIgeEncryptionByteArray(byte[] buffer, byte[] key, byte[] iv, bool encrypt, int length);

        [DllImport(Dll)]
        public static extern int UtilsPbkdf2(byte[] password, int passwordLength, byte[] salt, int saltLength, byte[] dst, int dstLength, int iterations);

        [DllImport(Dll)]
        private static extern void RandomSecureBytes(byte[] data, int size);

        [DllImport(Dll)]
        private static extern void Sha256ComputeHash(byte[] data, int dataLen, byte[] buff);

        [DllImport(Dll)]
        public static extern bool EncryptData(byte[] data, byte[] public_key, byte[] private_key, byte[] salt, ref IntPtr res, ref int resLen);
        [DllImport(Dll)]
        public static extern bool DecryptData(byte[] data, byte[] public_key, byte[] private_key, byte[] salt, ref IntPtr res, ref int resLen);


        //#else

        //        //private const string Dll = @"E:\Complex\Ton\Top-Wallet\x64\Debug\tonlib_win.dll";

        //        private static object async = new object();

        //        static TonLib()
        //        {
        //            Disposable.Exited += delegate ()
        //            {
        //                if (loader != null)
        //                    loader.Dispose();
        //            };
        //        }


        //        private static IDllLoader loader;
        //        private static IDllLoader Loader
        //        {
        //            get
        //            {
        //                if (loader == null)
        //                {
        //                    lock (async)
        //                    {
        //                        if (loader == null)
        //                        {
        //                            string path = System.IO.Path.GetTempFileName();
        //                            using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate))
        //                            {
        //                                using (System.IO.Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("Complex.Ton.tonlib_win.dll"))
        //                                {
        //                                    byte[] data = new byte[s.Length];
        //                                    s.Read(data, 0, data.Length);
        //                                    fs.Write(data, 0, data.Length);
        //                                }
        //                            }

        //                            loader = new DllLoader(path);
        //                        }
        //                    //loader = DllMemoryLoader.FromResource(typeof(TonLib).Assembly, "Complex.Ton.tonlib_win.dll");
        //                    }
        //                }
        //                return loader;
        //            }
        //        }

        //        public delegate long LiteClientCreateDelegate(string name, string directory, string config, IntPtr connectedHandler);
        //        private static LiteClientCreateDelegate liteClientCreateDelegate;
        //        public static LiteClientCreateDelegate LiteClientCreate => Loader.LoadDelegate(ref liteClientCreateDelegate);

        //        public delegate void LiteClientDestroyDelegate(long handle);
        //        private static LiteClientDestroyDelegate liteClientDestroyDelegate;
        //        public static LiteClientDestroyDelegate LiteClientDestroy => Loader.LoadDelegate(ref liteClientDestroyDelegate);


        //        public delegate void LiteClientConnectDelegate(long handle);
        //        private static LiteClientConnectDelegate liteClientConnectDelegate;
        //        public static LiteClientConnectDelegate LiteClientConnect => Loader.LoadDelegate(ref liteClientConnectDelegate);

        //        public delegate int LiteClientGetServerIndexDelegate(long handle);
        //        private static LiteClientGetServerIndexDelegate liteClientGetServerIndexDelegate;
        //        public static LiteClientGetServerIndexDelegate LiteClientGetServerIndex => Loader.LoadDelegate(ref liteClientGetServerIndexDelegate);

        //        public delegate bool LiteClientIsConnectedDelegate(long handle);
        //        private static LiteClientIsConnectedDelegate liteClientIsConnectedDelegate;
        //        public static LiteClientIsConnectedDelegate LiteClientIsConnected  => Loader.LoadDelegate(ref liteClientIsConnectedDelegate);

        //        private delegate bool LiteClientGetImportKeyDelegate(long handle, byte[] mnemonicPassword, string[] worlds, byte[] publicKey, byte[] password, byte[] secret, byte[] builder, out int len, IntPtr resultHandler);
        //        private static LiteClientGetImportKeyDelegate liteClientGetImportKeyDelegate;
        //        private static LiteClientGetImportKeyDelegate LiteClientGetImportKey => Loader.LoadDelegate(ref liteClientGetImportKeyDelegate);

        //        public delegate void LiteClientLastDelegate(long handle, IntPtr resultHandler);
        //        private static LiteClientLastDelegate liteClientLastDelegate;
        //        public static LiteClientLastDelegate LiteClientLast => Loader.LoadDelegate(ref liteClientLastDelegate);

        //        public delegate void LiteClientGetTransactionsDelegate(long handle, string address, long lt, byte[] lt_hash, IntPtr resultHandler, int count, IntPtr handler);
        //        private static LiteClientGetTransactionsDelegate liteClientGetTransactionsDelegate;
        //        public static LiteClientGetTransactionsDelegate LiteClientGetTransactions => Loader.LoadDelegate(ref liteClientGetTransactionsDelegate);

        //        private delegate bool LiteClientCreateWalletAddressDelegate(long handle, byte[] mnemonicPassword, byte[] publicKey, byte[] password, byte[] secret, byte[] seed, byte[] builder, out int len, IntPtr wordsHandler, IntPtr resultHandler);
        //        private static LiteClientCreateWalletAddressDelegate liteClientCreateWalletAddressDelegate;
        //        private static LiteClientCreateWalletAddressDelegate LiteClientCreateWalletAddress => Loader.LoadDelegate(ref liteClientCreateWalletAddressDelegate);

        //        public delegate bool LiteClientGetWordsDelegate(long handle, byte[] publicKey, byte[] password, byte[] secret, IntPtr wordsHandler, IntPtr resultHandler);
        //        private static LiteClientGetWordsDelegate liteClientGetWordsDelegate;
        //        public static LiteClientGetWordsDelegate LiteClientGetWords => Loader.LoadDelegate(ref liteClientGetWordsDelegate);

        //        public delegate void LiteClientCalcFeeDelegate(long handle, string srcAddress, string destAddress, long gram, string message, IntPtr resultHandler);
        //        private static LiteClientCalcFeeDelegate liteClientCalcFeeDelegate;
        //        public static LiteClientCalcFeeDelegate LiteClientCalcFee => Loader.LoadDelegate(ref liteClientCalcFeeDelegate);

        //        public delegate void LiteClientGetAccountStateDelegate(long handle, string address, IntPtr resultHandler);
        //        private static LiteClientGetAccountStateDelegate liteClientGetAccountStateDelegate;
        //        public static LiteClientGetAccountStateDelegate LiteClientGetAccountState => Loader.LoadDelegate(ref liteClientGetAccountStateDelegate);

        //        private delegate bool LiteClientExportKeyDelegate(long handle, string dataPassword, byte[] publicKey, byte[] password, byte[] secret, byte[] keyData, out int keyDataLen, QueryResultHandler resultHandler);
        //        private static LiteClientExportKeyDelegate liteClientExportKeyDelegate;
        //        private static LiteClientExportKeyDelegate LiteClientExportKey => Loader.LoadDelegate(ref liteClientExportKeyDelegate);

        //        private delegate bool LiteClientImportKeyDelegate(long handle, string dataPassword, byte[] password, byte[] outSecret, byte[] keyData, int keyDataLen, QueryResultHandler resultHandler);
        //        private static LiteClientImportKeyDelegate liteClientImportKeyDelegate;
        //        private static LiteClientImportKeyDelegate LiteClientImportKey => Loader.LoadDelegate(ref liteClientImportKeyDelegate);

        //        public delegate bool LiteClientDeleteKeyDelegate(long handle, byte[] publicKey, byte[] secret, QueryResultHandler resultHandler);
        //        private static LiteClientDeleteKeyDelegate liteClientDeleteKeyDelegate;
        //        public static LiteClientDeleteKeyDelegate LiteClientDeleteKey => Loader.LoadDelegate(ref liteClientDeleteKeyDelegate);

        //        public delegate void LiteClientSendGramDelegate(long handle, string srcAddress, string destAddress, byte[] publicKey, byte[] password, byte[] secret, long gram, string message, IntPtr resultHandler);
        //        private static LiteClientSendGramDelegate liteClientSendGramDelegate;
        //        public static LiteClientSendGramDelegate LiteClientSendGram => Loader.LoadDelegate(ref liteClientSendGramDelegate);

        //        public delegate void LiteClientCreateWalletDelegate(long handle, byte[] publicKey, byte[] password, byte[] secret, IntPtr resultHandler);
        //        private static LiteClientCreateWalletDelegate liteClientCreateWalletDelegate;
        //        public static LiteClientCreateWalletDelegate LiteClientCreateWallet => Loader.LoadDelegate(ref liteClientCreateWalletDelegate);


        //        public delegate void AccountDestroyDelegate(long handle);
        //        private static AccountDestroyDelegate accountDestroyDelegate;
        //        public static AccountDestroyDelegate AccountDestroy => Loader.LoadDelegate(ref accountDestroyDelegate);

        //        public delegate long AccountBalanceDelegate(long handle);
        //        private static AccountBalanceDelegate accountBalanceDelegate;
        //        public static AccountBalanceDelegate AccountBalance => Loader.LoadDelegate(ref accountBalanceDelegate);


        //        public delegate long AccountTransactionDelegate(long handle);
        //        private static AccountTransactionDelegate accountTransactionDelegate;
        //        public static AccountTransactionDelegate AccountTransaction => Loader.LoadDelegate(ref accountTransactionDelegate);

        //        public delegate long AccountTimeDelegate(long handle);
        //        private static AccountTimeDelegate accountTimeDelegate;
        //        public static AccountTimeDelegate AccountTime => Loader.LoadDelegate(ref accountTimeDelegate);

        //        public delegate void AccountTransactionHashDelegate(long handle, byte[] hash);
        //        private static AccountTransactionHashDelegate accountTransactionHashDelegate;
        //        public static AccountTransactionHashDelegate AccountTransactionHash => Loader.LoadDelegate(ref accountTransactionHashDelegate);

        //        public delegate long AccountSeqnoDelegate(long handle);
        //        private static AccountSeqnoDelegate accountSeqnoDelegate;
        //        public static AccountSeqnoDelegate AccountSeqno => Loader.LoadDelegate(ref accountSeqnoDelegate);

        //        public delegate ContractState AccountGetStateDelegate(long handle);
        //        private static AccountGetStateDelegate accountGetStateDelegate;
        //        public static AccountGetStateDelegate AccountGetState => Loader.LoadDelegate(ref accountGetStateDelegate);

        //        public delegate void AccountVersionDelegate(long handle, byte[] chs, out int len);
        //        private static AccountVersionDelegate accountVersionDelegate;
        //        public static AccountVersionDelegate AccountVersion => Loader.LoadDelegate(ref accountVersionDelegate);

        //        public delegate void AccountGetMsgHashDelegate(long handle, byte[] res);
        //        private static AccountGetMsgHashDelegate accountGetMsgHashDelegate;
        //        public static AccountGetMsgHashDelegate AccountGetMsgHash => Loader.LoadDelegate(ref accountGetMsgHashDelegate);

        //        public delegate WalletType AccountTypeDelegate(long handle);
        //        private static AccountTypeDelegate accountTypeDelegate;
        //        public static AccountTypeDelegate AccountType => Loader.LoadDelegate(ref accountTypeDelegate);

        //        private delegate void AccountRunMethodDelegate(long handle, string methodName, IntPtr[] param, int paramsLen, IntPtr** res, int* resLen);
        //        private static AccountRunMethodDelegate accountRunMethodDelegate;
        //        private static AccountRunMethodDelegate AccountRunMethod => Loader.LoadDelegate(ref accountRunMethodDelegate);


        //        public delegate long AddressCreateDelegate(int wc, byte[] rdata, bool bounceable, bool testnet);
        //        private static AddressCreateDelegate addressCreateDelegate;
        //        public static AddressCreateDelegate AddressCreate => Loader.LoadDelegate(ref addressCreateDelegate);

        //        public delegate long AddressParseDelegate(string address);
        //        private static AddressParseDelegate addressParseDelegate;
        //        public static AddressParseDelegate AddressParse => Loader.LoadDelegate(ref addressParseDelegate);

        //        public delegate void AddressDestroyDelegate(long handle);
        //        private static AddressDestroyDelegate addressDestroyDelegate;
        //        public static AddressDestroyDelegate AddressDestroy => Loader.LoadDelegate(ref addressDestroyDelegate);

        //        public delegate bool AddressIsValidDelegate(string address);
        //        private static AddressIsValidDelegate addressIsValidDelegate;
        //        public static AddressIsValidDelegate AddressIsValid => Loader.LoadDelegate(ref addressIsValidDelegate);

        //        private delegate void AddressToStringDelegate(long handle, StringBuilder builder, out int len);
        //        private static AddressToStringDelegate addressToStringDelegate;
        //        private static AddressToStringDelegate AddressToString => Loader.LoadDelegate(ref addressToStringDelegate);

        //        public delegate bool AddressCompareToDelegate(long handle, long to);
        //        private static AddressCompareToDelegate addressCompareToDelegate;
        //        public static AddressCompareToDelegate AddressCompareTo => Loader.LoadDelegate(ref addressCompareToDelegate);

        //        public delegate void AddressDataDelegate(long handle, byte[] data);
        //        private static AddressDataDelegate addressDataDelegate;
        //        public static AddressDataDelegate AddressData => Loader.LoadDelegate(ref addressDataDelegate);

        //        public delegate long CellBuilderCreateDelegate();
        //        private static CellBuilderCreateDelegate cellBuilderCreateDelegate;
        //        public static CellBuilderCreateDelegate CellBuilderCreate => Loader.LoadDelegate(ref cellBuilderCreateDelegate);

        //        public delegate void CellBuilderDestroyDelegate(long handle);
        //        private static CellBuilderDestroyDelegate cellBuilderDestroyDelegate;
        //        public static CellBuilderDestroyDelegate CellBuilderDestroy => Loader.LoadDelegate(ref cellBuilderDestroyDelegate);

        //        public delegate bool CellBuilderStoreLongDelegate(long handle, long value, byte bits);
        //        private static CellBuilderStoreLongDelegate cellBuilderStoreLongDelegate;
        //        public static CellBuilderStoreLongDelegate CellBuilderStoreLong => Loader.LoadDelegate(ref cellBuilderStoreLongDelegate);

        //        public delegate bool CellBuilderStoreRefDelegate(long handle, long cell);
        //        private static CellBuilderStoreRefDelegate cellBuilderStoreRefDelegate;
        //        public static CellBuilderStoreRefDelegate CellBuilderStoreRef => Loader.LoadDelegate(ref cellBuilderStoreRefDelegate);

        //        public delegate bool CellBuilderStoreBytesDelegate(long handle, byte[] data, int offset, int len);
        //        private static CellBuilderStoreBytesDelegate cellBuilderStoreBytesDelegate;
        //        public static CellBuilderStoreBytesDelegate CellBuilderStoreBytes => Loader.LoadDelegate(ref cellBuilderStoreBytesDelegate);

        //        public delegate bool CellBuilderStoreSliceDelegate(long handle, long slice);
        //        private static CellBuilderStoreSliceDelegate cellBuilderStoreSliceDelegate;
        //        public static CellBuilderStoreSliceDelegate CellBuilderStoreSlice => Loader.LoadDelegate(ref cellBuilderStoreSliceDelegate);

        //        public delegate long CellBuilderFinalizeDelegate(long handle);
        //        private static CellBuilderFinalizeDelegate cellBuilderFinalizeDelegate;
        //        public static CellBuilderFinalizeDelegate CellBuilderFinalize => Loader.LoadDelegate(ref cellBuilderFinalizeDelegate);

        //        public delegate int CellBuilderBitsDelegate(long handle);
        //        private static CellBuilderBitsDelegate cellBuilderBitsDelegate;
        //        public static CellBuilderBitsDelegate CellBuilderBits => Loader.LoadDelegate(ref cellBuilderBitsDelegate);

        //        public delegate int CellBuilderRefsDelegate(long handle);
        //        private static CellBuilderRefsDelegate cellBuilderRefsDelegate;
        //        public static CellBuilderRefsDelegate CellBuilderRefs => Loader.LoadDelegate(ref cellBuilderRefsDelegate);

        //        public delegate void CellDestroyDelegate(long handle);
        //        private static CellDestroyDelegate cellDestroyDelegate;
        //        public static CellDestroyDelegate CellDestroy => Loader.LoadDelegate(ref cellDestroyDelegate);

        //        public delegate int CellBitsDelegate(long handle);
        //        private static CellBitsDelegate cellBitsDelegate;
        //        public static CellBitsDelegate CellBits => Loader.LoadDelegate(ref cellBitsDelegate);

        //        public delegate int CellRefsDelegate(long handle);
        //        private static CellRefsDelegate cellRefsDelegate;
        //        public static CellRefsDelegate CellRefs => Loader.LoadDelegate(ref cellRefsDelegate);

        //        private delegate long CellFromBocDelegate(byte[] bocData, int len);
        //        private static CellFromBocDelegate cellFromBocDelegate;
        //        private static CellFromBocDelegate CellFromBoc => Loader.LoadDelegate(ref cellFromBocDelegate);

        //        private delegate void CellGetHashDelegate(long handle, byte** data, int* len);
        //        private static CellGetHashDelegate cellGetHashDelegate;
        //        private static CellGetHashDelegate CellGetHash => Loader.LoadDelegate(ref cellGetHashDelegate);

        //        private delegate void CellGetDataDelegate(long handle, byte** data, int* len);
        //        private static CellGetDataDelegate cellGetDataDelegate;
        //        private static CellGetDataDelegate CellGetData => Loader.LoadDelegate(ref cellGetDataDelegate);

        //        public delegate long CellSliceCreateDelegate(long cell);
        //        private static CellSliceCreateDelegate cellSliceCreateDelegate;
        //        public static CellSliceCreateDelegate CellSliceCreate => Loader.LoadDelegate(ref cellSliceCreateDelegate);

        //        public delegate void CellSliceDestroyDelegate(long handle);
        //        private static CellSliceDestroyDelegate cellSliceDestroyDelegate;
        //        public static CellSliceDestroyDelegate CellSliceDestroy => Loader.LoadDelegate(ref cellSliceDestroyDelegate);

        //        public delegate int CellSliceBitsDelegate(long handle);
        //        private static CellSliceBitsDelegate cellSliceBitsDelegate;
        //        public static CellSliceBitsDelegate CellSliceBits => Loader.LoadDelegate(ref cellSliceBitsDelegate);

        //        public delegate int CellSliceRefsDelegate(long handle);
        //        private static CellSliceRefsDelegate cellSliceRefsDelegate;
        //        public static CellSliceRefsDelegate CellSliceRefs => Loader.LoadDelegate(ref cellSliceRefsDelegate);

        //        public delegate bool CellSliceIsValidDelegate(long handle);
        //        private static CellSliceIsValidDelegate cellSliceIsValidDelegate;
        //        public static CellSliceIsValidDelegate CellSliceIsValid => Loader.LoadDelegate(ref cellSliceIsValidDelegate);

        //        public delegate long CellSliceLoadLongDelegate(long handle, int bits);
        //        private static CellSliceLoadLongDelegate cellSliceLoadLongDelegate;
        //        public static CellSliceLoadLongDelegate CellSliceLoadLong => Loader.LoadDelegate(ref cellSliceLoadLongDelegate);

        //        public delegate long CellSlicePreLoadLongDelegate(long handle, int bits);
        //        private static CellSlicePreLoadLongDelegate cellSlicePreLoadLongDelegate;
        //        public static CellSlicePreLoadLongDelegate CellSlicePreLoadLong => Loader.LoadDelegate(ref cellSlicePreLoadLongDelegate);

        //        private delegate void CellSliceLoadBytesDelegate(long handle, byte[] data, int length);
        //        private static CellSliceLoadBytesDelegate cellSliceLoadBytesDelegate;
        //        private static CellSliceLoadBytesDelegate CellSliceLoadBytes => Loader.LoadDelegate(ref cellSliceLoadBytesDelegate);

        //        public delegate long CellSliceLoadRefDelegate(long handle);
        //        private static CellSliceLoadRefDelegate cellSliceLoadRefDelegate;
        //        public static CellSliceLoadRefDelegate CellSliceLoadRef => Loader.LoadDelegate(ref cellSliceLoadRefDelegate);

        //        public delegate long CellSliceLoadRefCellDelegate(long handle);
        //        private static CellSliceLoadRefCellDelegate cellSliceLoadRefCellDelegate;
        //        public static CellSliceLoadRefCellDelegate CellSliceLoadRefCell => Loader.LoadDelegate(ref cellSliceLoadRefCellDelegate);

        //        public delegate long CellSliceLoadSliceDelegate(long handle);
        //        private static CellSliceLoadSliceDelegate cellSliceLoadSliceDelegate;
        //        public static CellSliceLoadSliceDelegate CellSliceLoadSlice => Loader.LoadDelegate(ref cellSliceLoadSliceDelegate);

        //        public delegate int CellSliceBSelectDelegate(long handle, int bits, int mask);
        //        private static CellSliceBSelectDelegate cellSliceBSelectDelegate;
        //        public static CellSliceBSelectDelegate CellSliceBSelect => Loader.LoadDelegate(ref cellSliceBSelectDelegate);

        //        private delegate void CellSliceGetAddressDelegate(long handle, StringBuilder builder, out int len);
        //        private static CellSliceGetAddressDelegate cellSliceGetAddressDelegate;
        //        private static CellSliceGetAddressDelegate CellSliceGetAddress => Loader.LoadDelegate(ref cellSliceGetAddressDelegate);

        //        private delegate void CellSliceGetAddressExtDelegate(long handle, StringBuilder builder, out int len);
        //        private static CellSliceGetAddressExtDelegate cellSliceGetAddressExtDelegate;
        //        private static CellSliceGetAddressExtDelegate CellSliceGetAddressExt => Loader.LoadDelegate(ref cellSliceGetAddressExtDelegate);

        //        public delegate long CellSliceGetBalanceDelegate(long handle);
        //        private static CellSliceGetBalanceDelegate cellSliceGetBalanceDelegate;
        //        public static CellSliceGetBalanceDelegate CellSliceGetBalance => Loader.LoadDelegate(ref cellSliceGetBalanceDelegate);

        //        public delegate long CellSliceGetFeeDelegate(long handle);
        //        private static CellSliceGetFeeDelegate cellSliceGetFeeDelegate;
        //        public static CellSliceGetFeeDelegate CellSliceGetFee => Loader.LoadDelegate(ref cellSliceGetFeeDelegate);

        //        public delegate long CellSliceGetStorageFeeRefDelegate(long handle);
        //        private static CellSliceGetStorageFeeRefDelegate cellSliceGetStorageFeeRefDelegate;
        //        public static CellSliceGetStorageFeeRefDelegate CellSliceGetStorageFeeRef => Loader.LoadDelegate(ref cellSliceGetStorageFeeRefDelegate);


        //        private delegate bool PrivateKeyGenerateDelegate(byte[] res);
        //        private static PrivateKeyGenerateDelegate privateKeyGenerateDelegate;
        //        private static PrivateKeyGenerateDelegate PrivateKeyGenerate => Loader.LoadDelegate(ref privateKeyGenerateDelegate);

        //        private delegate bool PrivateKeyGetPublicKeyDelegate(byte[] privKey, byte[] res);
        //        private static PrivateKeyGetPublicKeyDelegate privateKeyGetPublicKeyDelegate;
        //        private static PrivateKeyGetPublicKeyDelegate PrivateKeyGetPublicKey=> Loader.LoadDelegate(ref privateKeyGetPublicKeyDelegate);

        //        private delegate bool PrivateKeySignDelegate(byte[] privKey, long cell, byte** res, int* resLen);
        //        private static PrivateKeySignDelegate privateKeySignDelegate;
        //        private static PrivateKeySignDelegate PrivateKeySign => Loader.LoadDelegate(ref privateKeySignDelegate);

        //        private delegate bool PublicKeyEncryptDelegate(byte[] pubKey, byte[] data, int dataLen, byte** res, int* resLen);
        //        private static PublicKeyEncryptDelegate publicKeyEncryptDelegate;
        //        private static PublicKeyEncryptDelegate PublicKeyEncrypt => Loader.LoadDelegate(ref publicKeyEncryptDelegate);

        //        private delegate bool PrivateKeyDecryptDelegate(byte[] privKey, byte[] data, int dataLen, byte** res, int* resLen);
        //        private static PrivateKeyDecryptDelegate privateKeyDecryptDelegate;
        //        private static PrivateKeyDecryptDelegate PrivateKeyDecrypt => Loader.LoadDelegate(ref privateKeyDecryptDelegate);

        //        public delegate void KeyEncryptDecryptDelegate(byte[] pubKey, byte[] privKey, byte* data, int dataLen, byte** res, int* resLen);
        //        private static KeyEncryptDecryptDelegate keyEncryptDecryptDelegate;
        //        public static KeyEncryptDecryptDelegate KeyEncryptDecrypt => Loader.LoadDelegate(ref keyEncryptDecryptDelegate);

        //        private delegate void PrivateKeyToStringDelegate(byte[] data, byte[] builder, out int len);
        //        private static PrivateKeyToStringDelegate privateKeyToStringDelegate;
        //        private static PrivateKeyToStringDelegate PrivateKeyToString => Loader.LoadDelegate(ref privateKeyToStringDelegate);

        //        private delegate void PublicKeyToStringDelegate(byte[] data, byte[] builder, out int len);
        //        private static PublicKeyToStringDelegate publicKeyToStringDelegate;
        //        private static PublicKeyToStringDelegate PublicKeyToString => Loader.LoadDelegate(ref publicKeyToStringDelegate);

        //        public delegate bool PrivateKeyParseDelegate(string data, byte[] res);
        //        private static PrivateKeyParseDelegate privateKeyParseDelegate;
        //        public static PrivateKeyParseDelegate PrivateKeyParse => Loader.LoadDelegate(ref privateKeyParseDelegate);

        //        public delegate bool PublicKeyParseDelegate(string data, byte[] res);
        //        private static PublicKeyParseDelegate publicKeyParseDelegate;
        //        public static PublicKeyParseDelegate PublicKeyParse => Loader.LoadDelegate(ref publicKeyParseDelegate);

        //        public delegate long DictionaryCreateDelegate(int bits);
        //        private static DictionaryCreateDelegate dictionaryCreateDelegate;
        //        public static DictionaryCreateDelegate DictionaryCreate => Loader.LoadDelegate(ref dictionaryCreateDelegate);

        //        public delegate void DictionaryDestroyDelegate(long handle);
        //        private static DictionaryDestroyDelegate dictionaryDestroyDelegate;
        //        public static DictionaryDestroyDelegate DictionaryDestroy => Loader.LoadDelegate(ref dictionaryDestroyDelegate);


        //        private delegate void BigIntegerGetDataDelegate(long value, byte** res, int* resLen);
        //        private static BigIntegerGetDataDelegate bigIntegerGetDataDelegate;
        //        private static BigIntegerGetDataDelegate BigIntegerGetData => Loader.LoadDelegate(ref bigIntegerGetDataDelegate);

        //        public delegate void UtilsAesIgeEncryptionDelegate(byte[] buffer, byte[] key, byte[] iv, bool encrypt, int offset, int length);
        //        private static UtilsAesIgeEncryptionDelegate utilsAesIgeEncryptionDelegate;
        //        public static UtilsAesIgeEncryptionDelegate UtilsAesIgeEncryption => Loader.LoadDelegate(ref utilsAesIgeEncryptionDelegate);

        //        public delegate void UtilsAesIgeEncryptionByteArrayDelegate(byte[] buffer, byte[] key, byte[] iv, bool encrypt, int length);
        //        private static UtilsAesIgeEncryptionByteArrayDelegate utilsAesIgeEncryptionByteArrayDelegate;
        //        public static UtilsAesIgeEncryptionByteArrayDelegate UtilsAesIgeEncryptionByteArray=> Loader.LoadDelegate(ref utilsAesIgeEncryptionByteArrayDelegate);

        //        public delegate int UtilsPbkdf2Delegate(byte[] password, int passwordLength, byte[] salt, int saltLength, byte[] dst, int dstLength, int iterations);
        //        private static UtilsPbkdf2Delegate utilsPbkdf2Delegate;
        //        public static UtilsPbkdf2Delegate UtilsPbkdf2 => Loader.LoadDelegate(ref utilsPbkdf2Delegate);

        //        private delegate void RandomSecureBytesDelegate(byte[] data, int size);
        //        private static RandomSecureBytesDelegate randomSecureBytesDelegate;
        //        private static RandomSecureBytesDelegate RandomSecureBytes => Loader.LoadDelegate(ref randomSecureBytesDelegate);

        //#endif

        public static byte[] EncryptData(byte[] data, byte[] public_key, byte[] private_key, byte[] salt)
        {
            int resLen = 0;
            IntPtr res = IntPtr.Zero;
            if (EncryptData(data, public_key, private_key, salt, ref res, ref resLen))
            {
                byte[] buff = new byte[resLen];
                Marshal.Copy((IntPtr)res, buff, 0, buff.Length);
                Marshal.FreeHGlobal((IntPtr)res);
                return buff;
            }
            return null;
        }

        public static byte[] EncryptData(string data, byte[] public_key, byte[] private_key, byte[] salt)
        {
            return EncryptData(Util.GetBytes(data), public_key, private_key, salt);
        }

        private static byte[] DecryptDataIn(byte[] data, byte[] public_key, byte[] private_key, byte[] salt)
        {
            int resLen = 0;
            IntPtr res = IntPtr.Zero;
            if (DecryptData(data, public_key, private_key, salt, ref res, ref resLen))
            {
                byte[] buff = new byte[resLen];
                Marshal.Copy((IntPtr)res, buff, 0, buff.Length);
                Marshal.FreeHGlobal((IntPtr)res);
                return buff;
            }
            return null;
        }

        public static string DecryptData(byte[] data, byte[] public_key, byte[] private_key, byte[] salt)
        {
            return DecryptDataIn(data, public_key, private_key, salt).GetString();
        }

        public static string LiteClientGetImportKey(IntPtr handle, byte[] mnemonicPassword, string[] worlds, byte[] publicKey, byte[] password, byte[] secret, IntPtr resultHandler)
        {
            int len;
            byte[] chs = new byte[256];
            if (LiteClientGetImportKey(handle, mnemonicPassword, worlds, publicKey, password, secret, chs, out len, resultHandler))
                return Encoding.UTF8.GetString(chs, 0, len);
            return null;
        }


        public static string LiteClientCreateWalletAddress(IntPtr handle, byte[] mnemonicPassword, byte[] publicKey, byte[] password, byte[] secret, byte[] seed, IntPtr wordsHandler, IntPtr resultHandler)
        {
            int len;
            byte[] chs = new byte[256];
            if (LiteClientCreateWalletAddress(handle, mnemonicPassword, publicKey, password, secret, seed, chs, out len, wordsHandler, resultHandler))
                return Encoding.UTF8.GetString(chs, 0, len);
            return null;
        }

        public static string GetAccountVersion(IntPtr handle)
        {
            int len;
            byte[] chs = new byte[16];
            AccountVersion(handle, chs, out len);
            return Encoding.UTF8.GetString(chs, 0, len);
        }

        public static object[] AccountRunMethod(IntPtr handle, string methodName, params object[] args)
        {
            IntPtr[] res = new IntPtr[10];
            IntPtr[] param = new IntPtr[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                object vm = null;
                if (args[i] is Cell)
                    vm = new VmCell((Cell)args[i]);
                else if (args[i] is Slice)
                    vm = new VmSlice((Slice)args[i]);
                else
                    vm = new VmNumber(args[i].ToString());
                param[i] = Marshal.AllocHGlobal(Marshal.SizeOf(vm));
                Marshal.StructureToPtr(vm, param[i], false);
            }
            Array<object> array = new Array<object>();
            QueryLongHandler vmHandler = (result) =>
            {
                VmObjectType type = (VmObjectType)Marshal.ReadInt32((IntPtr)result);
                switch (type)
                {
                    case VmObjectType.Number:
                        VmNumber number = (VmNumber)Marshal.PtrToStructure((IntPtr)result, typeof(VmNumber));
                        array.Add(number.value);
                        break;
                    case VmObjectType.Cell:
                        VmCell cell = (VmCell)Marshal.PtrToStructure((IntPtr)result, typeof(VmCell));
                        array.Add(new Cell(cell.value));
                        break;
                    case VmObjectType.Slice:
                        VmSlice slice = (VmSlice)Marshal.PtrToStructure((IntPtr)result, typeof(VmSlice));
                        array.Add(new Slice(slice.value));
                        break;
                }

            };
            using (Fixed f = Fixed.Normal(vmHandler))
                AccountRunMethod(handle, methodName, param, param.Length, f);
            foreach (IntPtr ptr in param)
                Marshal.FreeHGlobal(ptr);
            return array.ToArray();
        }

        public static string AddressToString(Address address)
        {
            int len;
            StringBuilder builder = new StringBuilder(256);
            AddressToString(address, builder, out len);
            return builder.ToString(0, len);
        }

        public static string AddressFromDataToString(int wc, byte[] rdata, bool bounceable, bool testnet)
        {
            int len;
            StringBuilder builder = new StringBuilder(256);
            AddressFromDataToString(wc, rdata, bounceable, testnet, builder, out len);
            return builder.ToString(0, len);
        }

        public static string AddressFromHex(string hexAddress)
        {
            if (!string.IsNullOrEmpty(hexAddress))
            {
                int len;
                StringBuilder builder = new StringBuilder(256);
                AddressFromHex(hexAddress, builder, out len);
                return builder.ToString(0, len);
            }
            return null;
        }

        public static string AddressToHex(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                int len;
                StringBuilder builder = new StringBuilder(256);
                AddressToHex(address, builder, out len);
                return builder.ToString(0, len);
            }
            return null;
        }

        public static string AccountAddress(IntPtr handle)
        {
            int len;
            StringBuilder builder = new StringBuilder(256);
            AccountAddress(handle, builder, out len);
            return builder.ToString(0, len);
        }

        public static string LiteClientGetStateInit(LiteClient client, AccountState state, byte[] publicKey)
        {
            int len;
            StringBuilder builder = new StringBuilder(1024 * 10);
            LiteClientGetStateInit(client, state, publicKey, builder, out len);
            return builder.ToString(0, len);
        }


        public static IntPtr CellFromBoc(byte[] bocData)
        {
            return CellFromBoc(bocData, bocData.Length);
        }

        public static byte[] CellGetHash(IntPtr handle)
        {
            byte[] res = new byte[32];
            CellGetHash(handle, res);
            return res;
        }

        public static byte[] CellGetData(IntPtr handle)
        {
            byte* res = null;
            int resLen = 0;
            CellGetData(handle, &res, &resLen);
            byte[] buff = new byte[resLen];
            Marshal.Copy((IntPtr)res, buff, 0, buff.Length);
            Marshal.FreeHGlobal((IntPtr)res);
            return buff;
        }

        public static byte[] CellSliceLoadBytes(IntPtr handle, int length) 
        {
            byte[] data = new byte[length];
            CellSliceLoadBytes(handle, data, length);
            return data;
        }

        public static UInt128 CellSliceLoadBigInt(IntPtr handle, int bits)
        {
            int len;
            StringBuilder builder = new StringBuilder(256);
            CellSliceLoadBigInt(handle, bits, builder, out len);
            Int128 value = Int128.Parse(builder.ToString(0, len));
            if (value > 0)
                return (UInt128)value;
            return 0;
        }

        public static string CellSliceLoadAddress(IntPtr handle)
        {
            int len;
            StringBuilder builder = new StringBuilder(256);
            CellSliceLoadAddress(handle, builder, out len);
            return builder.ToString(0, len);
        }

        public static string CellSliceGetAddressExt(IntPtr handle)
        {
            int len;
            StringBuilder builder = new StringBuilder(256);
            CellSliceGetAddressExt(handle, builder, out len);
            return builder.ToString(0, len);
        }

        public static IntPtr CellSliceFromBoc(byte[] bocData)
        {
            return CellSliceFromBoc(bocData, bocData.Length);
        }

        public static byte[] PrivateKeyGenerate()
        {
            byte[] res = new byte[32];
            if (PrivateKeyGenerate(res))
                return res;
            return null;
        }

        public static byte[] PrivateKeyGetPublicKey(byte[] privKey)
        {
            byte[] res = new byte[32];
            if (PrivateKeyGetPublicKey(privKey, res))
                return res;
            return null;
        }

        public static byte[] PrivateKeySign(byte[] privKey, IntPtr cell)
        {
            byte* res = null;
            int resLen = 0;
            if (PrivateKeySign(privKey, cell, &res, &resLen))
            {
                byte[] buff = new byte[resLen];
                Marshal.Copy((IntPtr)res, buff, 0, buff.Length);
                Marshal.FreeHGlobal((IntPtr)res);
                return buff;
            }
            return null;
        }

        public static byte[] PublicKeyEncrypt(byte[] pubKey, byte[] data)
        {
            byte* res = null;
            int resLen = 0;
            if (PublicKeyEncrypt(pubKey, data, data.Length, &res, &resLen))
            {
                byte[] buff = new byte[resLen];
                Marshal.Copy((IntPtr)res, buff, 0, buff.Length);
                Marshal.FreeHGlobal((IntPtr)res);
                return buff;
            }
            return null;
        }

        public static byte[] PrivateKeyDecrypt(byte[] pubKey, byte[] data)
        {
            byte* res = null;
            int resLen = 0;
            if (PrivateKeyDecrypt(pubKey, data, data.Length, &res, &resLen))
            {
                byte[] buff = new byte[resLen];
                Marshal.Copy((IntPtr)res, buff, 0, buff.Length);
                Marshal.FreeHGlobal((IntPtr)res);
                return buff;
            }
            return null;
        }

        public static string PrivateKeyToString(PrivateKey privateKey)
        {
            int len;
            byte[] chs = new byte[256];
            PrivateKeyToString(privateKey.keyData, chs, out len);
            return Encoding.UTF8.GetString(chs, 0, len);
        }

        public static string PublicKeyToString(PublicKey publicKey)
        {
            int len;
            byte[] chs = new byte[256];
            PublicKeyToString(publicKey.keyData, chs, out len);
            return Encoding.UTF8.GetString(chs, 0, len);
        }


        public static byte[] BigIntegerGetData(string value)
        {
            int resLen;
            byte[] buff = new byte[32];
            BigIntegerGetDataFromString(value, buff, out resLen);
            byte[] res = new byte[resLen];
            Array.Copy(buff, res, resLen);
            return res;
        }

        public static byte[] BigIntegerGetData(UInt128 value)
        {
            return BigIntegerGetData(value.ToString());
        }

        public static byte[] ComputePBKDF2(string password, byte[] salt)
        {
            byte[] dst = new byte[64];
            byte[] psw = Encoding.UTF8.GetBytes(password);
            UtilsPbkdf2(psw, psw.Length, salt, salt.Length, dst, dst.Length, 100000);
            return dst;
        }

        public static byte[] RandomSecureBytes(int size)
        {
            byte[] data = new byte[size];
            RandomSecureBytes(data, size);
            return data;
        }

        public static void RandomSecureBytes(byte[] data)
        {
            RandomSecureBytes(data, data.Length);
        }

        public static byte[] Sha256ComputeHash(byte[] data)
        {
            byte[] buff = new byte[32];
            Sha256ComputeHash(data, data.Length, buff);
            return buff;
        }

        public static byte[] Sha256ComputeHashText(string text)
        {
            return Sha256ComputeHash(text.ToBytes());
        }
    }
}
