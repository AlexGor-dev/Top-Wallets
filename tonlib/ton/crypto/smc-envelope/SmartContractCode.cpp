/*
    This file is part of TON Blockchain Library.

    TON Blockchain Library is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 2 of the License, or
    (at your option) any later version.

    TON Blockchain Library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with TON Blockchain Library.  If not, see <http://www.gnu.org/licenses/>.

    Copyright 2017-2020 Telegram Systems LLP
*/
#include "SmartContractCode.h"

#include "vm/boc.h"
#include <map>

#include "td/utils/base64.h"
//#include "crypto/common/util.h"

namespace ton
{
    namespace
    {
        // WALLET_REVISION = 2;
        // WALLET2_REVISION = 2;
        // WALLET3_REVISION = 2;
        // HIGHLOAD_WALLET_REVISION = 2;
        // HIGHLOAD_WALLET2_REVISION = 2;
        // DNS_REVISION = 1;

        std::string parse_hex(std::string hex)
        {
            char* p;
            std::string res;
            size_t size = hex.size() / 2;
            for (unsigned i = 0; i < size; i++)
            {
                std::string s = "0x" + hex.substr(i * 2, 2);
                char ch = strtoul(s.c_str(), &p, 16);
                res += ch;
            }
            return res;
        }

        const auto& get_map()
        {
            static auto map = []
            {
                std::map<std::string, td::Ref<vm::Cell>, std::less<>> map;
                auto with_tvm_code = [&](auto name, td::Slice code_str)
                {
                    std::string s = td::base64_decode(code_str).move_as_ok();
                    map[name] = vm::std_boc_deserialize(s).move_as_ok();
                };
                auto with_tvm_codeHex = [&](auto name, std::string code_hex)
                {
                    std::string s = parse_hex(code_hex);
                    //std::string s = td::str_base64_decode(code_hex, true);
                    map[name] = vm::std_boc_deserialize(s).move_as_ok();
                };


#include "smartcont/auto/multisig-code.cpp"
#include "smartcont/auto/simple-wallet-ext-code.cpp"
#include "smartcont/auto/simple-wallet-code.cpp"
#include "smartcont/auto/wallet-code.cpp"
#include "smartcont/auto/highload-wallet-code.cpp"
#include "smartcont/auto/highload-wallet-v2-code.cpp"
#include "smartcont/auto/dns-manual-code.cpp"
#include "smartcont/auto/payment-channel-code.cpp"
#include "smartcont/auto/restricted-wallet3-code.cpp"

                with_tvm_code("highload-wallet-r1",
                              "te6ccgEBBgEAhgABFP8A9KQT9KDyyAsBAgEgAgMCAUgEBQC88oMI1xgg0x/TH9Mf+CMTu/Jj7UTQ0x/TH9P/"
                              "0VEyuvKhUUS68qIE+QFUEFX5EPKj9ATR+AB/jhghgBD0eG+hb6EgmALTB9QwAfsAkTLiAbPmWwGkyMsfyx/L/"
                              "8ntVAAE0DAAEaCZL9qJoa4WPw==");
                with_tvm_code("highload-wallet-r2",
                              "te6ccgEBCAEAlwABFP8A9KQT9LzyyAsBAgEgAgMCAUgEBQC48oMI1xgg0x/TH9Mf+CMTu/Jj7UTQ0x/TH9P/"
                              "0VEyuvKhUUS68qIE+QFUEFX5EPKj9ATR+AB/jhYhgBD0eG+lIJgC0wfUMAH7AJEy4gGz5lsBpMjLH8sfy//"
                              "J7VQABNAwAgFIBgcAF7s5ztRNDTPzHXC/+AARuMl+1E0NcLH4");
                with_tvm_code("highload-wallet-v2-r1",
                              "te6ccgEBBwEA1gABFP8A9KQT9KDyyAsBAgEgAgMCAUgEBQHu8oMI1xgg0x/TP/gjqh9TILnyY+1E0NMf0z/T//"
                              "QE0VNggED0Dm+hMfJgUXO68qIH+QFUEIf5EPKjAvQE0fgAf44YIYAQ9HhvoW+"
                              "hIJgC0wfUMAH7AJEy4gGz5luDJaHIQDSAQPRDiuYxyBLLHxPLP8v/9ADJ7VQGAATQMABBoZfl2omhpj5jpn+n/"
                              "mPoCaKkQQCB6BzfQmMktv8ld0fFADgggED0lm+hb6EyURCUMFMDud4gkzM2AZIyMOKz");
                with_tvm_code("highload-wallet-v2-r2",
                              "te6ccgEBCQEA5QABFP8A9KQT9LzyyAsBAgEgAgMCAUgEBQHq8oMI1xgg0x/TP/gjqh9TILnyY+1E0NMf0z/T//"
                              "QE0VNggED0Dm+hMfJgUXO68qIH+QFUEIf5EPKjAvQE0fgAf44WIYAQ9HhvpSCYAtMH1DAB+wCRMuIBs+"
                              "ZbgyWhyEA0gED0Q4rmMcgSyx8Tyz/L//QAye1UCAAE0DACASAGBwAXvZznaiaGmvmOuF/8AEG+X5dqJoaY+Y6Z/p/"
                              "5j6AmipEEAgegc30JjJLb/JXdHxQANCCAQPSWb6UyURCUMFMDud4gkzM2AZIyMOKz");
                with_tvm_code("simple-wallet-r1",
                              "te6ccgEEAQEAAAAAUwAAov8AIN0gggFMl7qXMO1E0NcLH+Ck8mCBAgDXGCDXCx/tRNDTH9P/"
                              "0VESuvKhIvkBVBBE+RDyovgAAdMfMSDXSpbTB9QC+wDe0aTIyx/L/8ntVA==");
                with_tvm_code("simple-wallet-r2",
                              "te6ccgEBAQEAXwAAuv8AIN0gggFMl7ohggEznLqxnHGw7UTQ0x/XC//jBOCk8mCBAgDXGCDXCx/tRNDTH9P/"
                              "0VESuvKhIvkBVBBE+RDyovgAAdMfMSDXSpbTB9QC+wDe0aTIyx/L/8ntVA==");
                with_tvm_code("wallet-r1",
                              "te6ccgEBAQEAVwAAqv8AIN0gggFMl7qXMO1E0NcLH+Ck8mCDCNcYINMf0x8B+CO78mPtRNDTH9P/0VExuvKhA/"
                              "kBVBBC+RDyovgAApMg10qW0wfUAvsA6NGkyMsfy//J7VQ=");
                with_tvm_code("wallet-r2",
                              "te6ccgEBAQEAYwAAwv8AIN0gggFMl7ohggEznLqxnHGw7UTQ0x/XC//jBOCk8mCDCNcYINMf0x8B+CO78mPtRNDTH9P/"
                              "0VExuvKhA/kBVBBC+RDyovgAApMg10qW0wfUAvsA6NGkyMsfy//J7VQ=");
                with_tvm_code("wallet3-r1",
                              "te6ccgEBAQEAYgAAwP8AIN0gggFMl7qXMO1E0NcLH+Ck8mCDCNcYINMf0x/TH/gjE7vyY+1E0NMf0x/T/"
                              "9FRMrryoVFEuvKiBPkBVBBV+RDyo/gAkyDXSpbTB9QC+wDo0QGkyMsfyx/L/8ntVA==");
                with_tvm_code("wallet3-r2",
                              "te6ccgEBAQEAcQAA3v8AIN0gggFMl7ohggEznLqxn3Gw7UTQ0x/THzHXC//jBOCk8mCDCNcYINMf0x/TH/gjE7vyY+1E0NMf0x/"
                              "T/9FRMrryoVFEuvKiBPkBVBBV+RDyo/gAkyDXSpbTB9QC+wDo0QGkyMsfyx/L/8ntVA==");
                with_tvm_code(
                    "dns-manual-r1",
                    "te6ccgECGAEAAtAAART/APSkE/S88sgLAQIBIAIDAgFIBAUC7PLbPAWDCNcYIPkBAdMf0z/"
                    "4I6ofUyC58mNTKoBA9A5voTHyYFKUuvKiVBNG+RDyo/gAItcLBcAzmDQBdtch0/"
                    "8wjoVa2zxAA+"
                    "IDgyWhyEAHgED0Q44aIIBA9JZvpTJREJQwUwe53iCTMzUBkjIw4rPmNVUD8AQREgICxQYHAgEgDA0CAc8ICQAIqoJfAwIBSAoLACHWQK5Y+"
                    "J5Z/l//oAegBk9qpAAFF8DgABcyPQAydBBM/Rw8qGAAF72c52omhpr5jrhf/"
                    "AIBIA4PABG7Nz7UTQ1wsfgD+"
                    "7owwh10kglF8DcG3hIHew8l4ieNci1wsHnnDIUATPFhPLB8nQAqYI3iDACJRfA3Bt4Ns8FF8EI3ADqwKY0wcBwAAToQLkIG2OnF8DIcjLBiTPF"
                    "snQhAlUQgHbPAWlFbIgwQEVQzDmMzUilF8FcG3hMgHHAJMxfwHfAtdJpvmBEVEAAYIcAAkjEB4AKAEPRqABztRNDTH9M/0//"
                    "0BPQE0QE2cFmOlNs8IMcBnCDXSpPUMNCTMn8C4t4i5jAxEwT20wUhwQqOLCGRMeEhwAGXMdMH1AL7AOABwAmOFNQh+wTtQwLQ7R7tU1RiA/"
                    "EGgvIA4PIt4HAiwRSUMNIPAd5tbSTBHoreJMEUjpElhAkj2zwzApUyxwDyo5Fb4t4kwAuOEzQC9ARQJIAQ9G4wECOECVnwAQHgJMAMiuAwFBUW"
                    "FwCEMQLTAAHAAZPUAdCY0wUBqgLXGAHiINdJwg/"
                    "ypiB41yLXCwfyaHBTEddJqTYCmNMHAcAAEqEB5DDIywYBzxbJ0FADACBZ9KhvpSCUAvQEMJIybeICACg0A4AQ9FqZECOECUBE8AEBkjAx4gBmM"
                    "SLAFZwy9AQQI4QJUELwAQHgIsAWmDIChAn0czAB4DAyIMAfkzD0BODAIJJtAeDyLG0B");
                with_tvm_code(
                    "restricted-wallet3-r1",
                    "te6ccgECEgEAAUsAART/APSkE/S88sgLAQIBIAIDAgFIBAUD+PKDCNcYINMf0x/THwL4I7vyY+1E0NMf0x/T/"
                    "1NDuvKhUWK68qIG+QFUEHb5EPKkAY4fMwHT/9EB0x/0BNH4AAOkyMsfFMsfy/8Syx/0AMntVOEC0x/"
                    "0BNH4ACH4I9s8IYAg9HtvpTGW+gAwcvsCkTDiApMg10qK6NECpMgPEBEABNAwAgEgBgcCASAICQIBSAwNAgFuCgsAEbjJftRNDXCx+"
                    "AAXrc52omhpn5jrhf/AABesePaiaGmPmOuFj8ABDbbYHwR7Z5AOAQm1B1tnkA4BTu1E0IEBQNch0x/"
                    "0BNEC2zz4J28QAoAg9HtvpTGX+gAwoXC2CZEw4g8AOiGOETGA8/gzIG6SMHCU0NcLH+IB3yGSAaGSW3/iAAzTB9QC+wAAHssfFMsfEsv/yx/"
                    "0AMntVA==");


                with_tvm_codeHex("wallet4-r1", "B5EE9C72410214010002D4000114FF00F4A413F4BCF2C80B010201200203020148040504F8F28308D71820D31FD31FD31F02F823BBF264ED44D0D31FD31FD3FFF404D15143BAF2A15151BAF2A205F901541064F910F2A3F80024A4C8CB1F5240CB1F5230CBFF5210F400C9ED54F80F01D30721C0009F6C519320D74A96D307D402FB00E830E021C001E30021C002E30001C0039130E30D03A4C8CB1F12CB1FCBFF1011121302E6D001D0D3032171B0925F04E022D749C120925F04E002D31F218210706C7567BD22821064737472BDB0925F05E003FA403020FA4401C8CA07CBFFC9D0ED44D0810140D721F404305C810108F40A6FA131B3925F07E005D33FC8258210706C7567BA923830E30D03821064737472BA925F06E30D06070201200809007801FA00F40430F8276F2230500AA121BEF2E0508210706C7567831EB17080185004CB0526CF1658FA0219F400CB6917CB1F5260CB3F20C98040FB0006008A5004810108F45930ED44D0810140D720C801CF16F400C9ED540172B08E23821064737472831EB17080185005CB055003CF1623FA0213CB6ACB1FCB3FC98040FB00925F03E20201200A0B0059BD242B6F6A2684080A06B90FA0218470D4080847A4937D29910CE6903E9FF9837812801B7810148987159F31840201580C0D0011B8C97ED44D0D70B1F8003DB29DFB513420405035C87D010C00B23281F2FFF274006040423D029BE84C600201200E0F0019ADCE76A26840206B90EB85FFC00019AF1DF6A26840106B90EB858FC0006ED207FA00D4D422F90005C8CA0715CBFFC9D077748018C8CB05CB0222CF165005FA0214CB6B12CCCCC973FB00C84014810108F451F2A7020070810108D718FA00D33FC8542047810108F451F2A782106E6F746570748018C8CB05CB025006CF165004FA0214CB6A12CB1FCB3FC973FB0002006C810108D718FA00D33F305224810108F459F2A782106473747270748018C8CB05CB025005CF165003FA0213CB6ACB1F12CB3FC973FB00000AF400C9ED54696225E5");
                with_tvm_code("wallet4-r2", "te6cckECFAEAAtQAART/APSkE/S88sgLAQIBIAIDAgFIBAUE+PKDCNcYINMf0x/THwL4I7vyZO1E0NMf0x/T//QE0VFDuvKhUVG68qIF+QFUEGT5EPKj+AAkpMjLH1JAyx9SMMv/UhD0AMntVPgPAdMHIcAAn2xRkyDXSpbTB9QC+wDoMOAhwAHjACHAAuMAAcADkTDjDQOkyMsfEssfy/8QERITAubQAdDTAyFxsJJfBOAi10nBIJJfBOAC0x8hghBwbHVnvSKCEGRzdHK9sJJfBeAD+kAwIPpEAcjKB8v/ydDtRNCBAUDXIfQEMFyBAQj0Cm+hMbOSXwfgBdM/yCWCEHBsdWe6kjgw4w0DghBkc3RyupJfBuMNBgcCASAICQB4AfoA9AQw+CdvIjBQCqEhvvLgUIIQcGx1Z4MesXCAGFAEywUmzxZY+gIZ9ADLaRfLH1Jgyz8gyYBA+wAGAIpQBIEBCPRZMO1E0IEBQNcgyAHPFvQAye1UAXKwjiOCEGRzdHKDHrFwgBhQBcsFUAPPFiP6AhPLassfyz/JgED7AJJfA+ICASAKCwBZvSQrb2omhAgKBrkPoCGEcNQICEekk30pkQzmkD6f+YN4EoAbeBAUiYcVnzGEAgFYDA0AEbjJftRNDXCx+AA9sp37UTQgQFA1yH0BDACyMoHy//J0AGBAQj0Cm+hMYAIBIA4PABmtznaiaEAga5Drhf/AABmvHfaiaEAQa5DrhY/AAG7SB/oA1NQi+QAFyMoHFcv/ydB3dIAYyMsFywIizxZQBfoCFMtrEszMyXP7AMhAFIEBCPRR8qcCAHCBAQjXGPoA0z/IVCBHgQEI9FHyp4IQbm90ZXB0gBjIywXLAlAGzxZQBPoCFMtqEssfyz/Jc/sAAgBsgQEI1xj6ANM/MFIkgQEI9Fnyp4IQZHN0cnB0gBjIywXLAlAFzxZQA/oCE8tqyx8Syz/Jc/sAAAr0AMntVGliJeU=");
                
                with_tvm_code("Nft-collection", "te6cckECFAEAAh8AART/APSkE/S88sgLAQIBYgIDAgLNBAUCASAODwTn0QY4BIrfAA6GmBgLjYSK3wfSAYAOmP6Z/2omh9IGmf6mpqGEEINJ6cqClAXUcUG6+CgOhBCFRlgFa4QAhkZYKoAueLEn0BCmW1CeWP5Z+A54tkwCB9gHAbKLnjgvlwyJLgAPGBEuABcYES4AHxgRgZgeACQGBwgJAgEgCgsAYDUC0z9TE7vy4ZJTE7oB+gDUMCgQNFnwBo4SAaRDQ8hQBc8WE8s/zMzMye1Ukl8F4gCmNXAD1DCON4BA9JZvpSCOKQakIIEA+r6T8sGP3oEBkyGgUyW78vQC+gDUMCJUSzDwBiO6kwKkAt4Ekmwh4rPmMDJQREMTyFAFzxYTyz/MzMzJ7VQALDI0AfpAMEFEyFAFzxYTyz/MzMzJ7VQAPI4V1NQwEDRBMMhQBc8WE8s/zMzMye1U4F8EhA/y8AIBIAwNAD1FrwBHAh8AV3gBjIywVYzxZQBPoCE8trEszMyXH7AIAC0AcjLP/gozxbJcCDIywET9AD0AMsAyYAAbPkAdMjLAhLKB8v/ydCACASAQEQAlvILfaiaH0gaZ/qamoYLehqGCxABDuLXTHtRND6QNM/1NTUMBAkXwTQ1DHUMNBxyMsHAc8WzMmAIBIBITAC+12v2omh9IGmf6mpqGDYg6GmH6Yf9IBhAALbT0faiaH0gaZ/qamoYCi+CeAI4APgCwGlAMbg==");
                with_tvm_code("Nft-item", "te6cckECDQEAAdAAART/APSkE/S88sgLAQIBYgIDAgLOBAUACaEfn+AFAgEgBgcCASALDALXDIhxwCSXwPg0NMDAXGwkl8D4PpA+kAx+gAxcdch+gAx+gAw8AIEs44UMGwiNFIyxwXy4ZUB+kDUMBAj8APgBtMf0z+CEF/MPRRSMLqOhzIQN14yQBPgMDQ0NTWCEC/LJqISuuMCXwSED/LwgCAkAET6RDBwuvLhTYAH2UTXHBfLhkfpAIfAB+kDSADH6AIIK+vCAG6EhlFMVoKHeItcLAcMAIJIGoZE24iDC//LhkiGOPoIQBRONkchQCc8WUAvPFnEkSRRURqBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7ABBHlBAqN1viCgBycIIQi3cXNQXIy/9QBM8WECSAQHCAEMjLBVAHzxZQBfoCFctqEssfyz8ibrOUWM8XAZEy4gHJAfsAAIICjjUm8AGCENUydtsQN0QAbXFwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7AJMwMjTiVQLwAwA7O1E0NM/+kAg10nCAJp/AfpA1DAQJBAj4DBwWW1tgAB0A8jLP1jPFgHPFszJ7VSC/dQQb");
                with_tvm_code("Nft-single", "te6cckECFQEAAwoAART/APSkE/S88sgLAQIBYgIDAgLOBAUCASAREgIBIAYHAgEgDxAEuQyIccAkl8D4NDTAwFxsJJfA+D6QPpAMfoAMXHXIfoAMfoAMPACBtMf0z+CEF/MPRRSMLqOhzIQRxA2QBXgghAvyyaiUjC64wKCEGk9OVBSMLrjAoIQHARBKlIwuoAgJCgsAET6RDBwuvLhTYAH2UTfHBfLhkfpAIfAB+kDSADH6AIIK+vCAG6EhlFMVoKHeItcLAcMAIJIGoZE24iDCAPLhkiGOPoIQBRONkchQC88WUAvPFnEkSxRURsBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7ABBnlBAqOVviDACGFl8GbCJwyMsByXCCEIt3FzUhyMv/A9ATzxYTgEBwgBDIywVQB88WUAX6AhXLahLLH8s/Im6zlFjPFwGRMuIByQH7AABUFl8GMwHQEoIQqMsArXCAEMjLBVAFzxYk+gIUy2oTyx/LPwHPFsmAQPsAAVyOhzIQRxA2QBXgMTI0NTWCEBoLnVESup9RE8cF8uGaAdTUMBAj8APgXwSED/LwDQCCAo41JvABghDVMnbbEDdGAG1xcIAQyMsFUAfPFlAF+gIVy2oSyx/LPyJus5RYzxcBkTLiAckB+wCTMDQ04lUC8AMB9lE2xwXy4ZH6QCHwAfpA0gAx+gCCCvrwgBuhIZRTFaCh3iLXCwHDACCSBqGRNuIgwv/y4ZIhjj6CEFEaRGPIUArPFlALzxZxJEoUVEawcIAQyMsFUAfPFlAF+gIVy2oSyx/LPyJus5RYzxcBkTLiAckB+wAQV5QQKjhb4g4AggKONSbwAYIQ1TJ22xA3RQBtcXCAEMjLBVAHzxZQBfoCFctqEssfyz8ibrOUWM8XAZEy4gHJAfsAkzAzNOJVAvADABU7UTQ+kD6QNTUMIAAbMhQBM8WWM8WzMzJ7VSACAVgTFAAjvH5/gBGBi4ZGWA5L+4AWggIcAB212v4ATYY6GmH6Yf9IBhAAEbQOngBCBGvgcOUAoqs=");
               
                with_tvm_codeHex("Nft-marketplace", "B5EE9C7241010401006D000114FF00F4A413F4BCF2C80B01020120020300AAD23221C700915BE0D0D3030171B0915BE0FA40ED44D0FA403012C705F2E19101D31F01C0018E2BFA003001D4D43021F90070C8CA07CBFFC9D077748018C8CB05CB0258CF165004FA0213CB6BCCCCC971FB00915BE20004F2308EF7CCE7");
                with_tvm_codeHex("Nft-sale", "B5EE9C7241020A010001B4000114FF00F4A413F4BCF2C80B01020120020302014804050004F2300202CD0607002FA03859DA89A1F481F481F481F401A861A1F401F481F4006101F7D00E8698180B8D8492F82707D201876A2687D207D207D207D006A18116BA4E10159C71D991B1B2990E382C92F837028916382F970FA01698FC1080289C6C8895D7970FAE99F98FD2018201A642802E78B2801E78B00E78B00FD016664F6AA701363804C9B081B2299823878027003698FE99F9810E000C92F857010C0801F5D41081DCD650029285029185F7970E101E87D007D207D0018384008646582A804E78B28B9D090D0A85AD08A500AFD010AE5B564B8FD80384008646582AC678B2803FD010B65B564B8FD80384008646582A802E78B00FD0109E5B564B8FD80381041082FE61E8A10C00C646582A802E78B117D010A65B509E58F8A40900C8C0029A3110471036454012F004E032363704C0038E4782103B9ACA0015BEF2E1C95312C70559C705B1F2E1CA702082105FCC3D14218010C8CB055006CF1622FA0215CB6A14CB1F14CB3F21CF1601CF16CA0021FA02CA00C98100A0FB00E05F06840FF2F0002ACB3F22CF1658CF16CA0021FA02CA00C98100A0FB00AECABAD1");
                with_tvm_code("Jetton-minter", "te6cckECDQEAApwAART/APSkE/S88sgLAQIBYgIDAgLMBAUCA3pgCwwC8dkGOASS+B8ADoaYGAuNhJL4HwfSB9IBj9ABi465D9ABj9ABgBaY/pn/aiaH0AfSBqahhACqk4XUcZmpqbGyiaY4L5cCSBfSB9AGoYEGhAMGuQ/QAYEogaKCF4BQpQKBnkKAJ9ASxni2ZmZPaqcEEIPe7L7yk4XXGBQGBwCTtfBQiAbgqEAmqCgHkKAJ9ASxniwDni2ZkkWRlgIl6AHoAZYBkkHyAODpkZYFlA+X/5Og7wAxkZYKsZ4soAn0BCeW1iWZmZLj9gEBwDY3NwH6APpA+ChUEgZwVCATVBQDyFAE+gJYzxYBzxbMySLIywES9AD0AMsAyfkAcHTIywLKB8v/ydBQBscF8uBKoQNFRchQBPoCWM8WzMzJ7VQB+kAwINcLAcMAkVvjDQgBpoIQLHa5c1JwuuMCNTc3I8ADjhozUDXHBfLgSQP6QDBZyFAE+gJYzxbMzMntVOA1AsAEjhhRJMcF8uBJ1DBDAMhQBPoCWM8WzMzJ7VTgXwWED/LwCQA+ghDVMnbbcIAQyMsFUAPPFiL6AhLLassfyz/JgEL7AAH+Nl8DggiYloAVoBW88uBLAvpA0wAwlcghzxbJkW3ighDRc1QAcIAYyMsFUAXPFiT6AhTLahPLHxTLPyP6RDBwuo4z+ChEA3BUIBNUFAPIUAT6AljPFgHPFszJIsjLARL0APQAywDJ+QBwdMjLAsoHy//J0M8WlmwicAHLAeL0AAoACsmAQPsAAH2tvPaiaH0AfSBqahg2GPwUALgqEAmqCgHkKAJ9ASxniwDni2ZkkWRlgIl6AHoAZYBk/IA4OmRlgWUD5f/k6EAAH68W9qJofQB9IGpqGD+qkEDvfJl9");
                with_tvm_code("Jetton-wallet", "te6cckECEQEAAx8AART/APSkE/S88sgLAQIBYgIDAgLMBAUAG6D2BdqJofQB9IH0gahhAgHUBgcCASAICQC7CDHAJJfBOAB0NMDAXGwlRNfA/AM4PpA+kAx+gAxcdch+gAx+gAwAtMfghAPin6lUiC6lTE0WfAJ4IIQF41FGVIgupYxREQD8ArgNYIQWV8HvLqTWfAL4F8EhA/y8IAARPpEMHC68uFNgAgEgCgsAg9QBBrkPaiaH0AfSB9IGoYAmmPwQgLxqKMqRBdQQg97svvCd0JWPlxYumfmP0AGAnQKBHkKAJ9ASxniwDni2Zk9qpAHxUD0z/6APpAIfAB7UTQ+gD6QPpA1DBRNqFSKscF8uLBKML/8uLCVDRCcFQgE1QUA8hQBPoCWM8WAc8WzMkiyMsBEvQA9ADLAMkg+QBwdMjLAsoHy//J0AT6QPQEMfoAINdJwgDy4sR3gBjIywVQCM8WcPoCF8trE8yAwCASANDgCeghAXjUUZyMsfGcs/UAf6AiLPFlAGzxYl+gJQA88WyVAFzCORcpFx4lAIqBOgggnJw4CgFLzy4sUEyYBA+wAQI8hQBPoCWM8WAc8WzMntVAL3O1E0PoA+kD6QNQwCNM/+gBRUaAF+kD6QFNbxwVUc21wVCATVBQDyFAE+gJYzxYBzxbMySLIywES9AD0AMsAyfkAcHTIywLKB8v/ydBQDccFHLHy4sMK+gBRqKGCCJiWgGa2CKGCCJiWgKAYoSeXEEkQODdfBOMNJdcLAYA8QANc7UTQ+gD6QPpA1DAH0z/6APpAMFFRoVJJxwXy4sEnwv/y4sIFggkxLQCgFrzy4sOCEHvdl97Iyx8Vyz9QA/oCIs8WAc8WyXGAGMjLBSTPFnD6AstqzMmAQPsAQBPIUAT6AljPFgHPFszJ7VSAAcFJ5oBihghBzYtCcyMsfUjDLP1j6AlAHzxZQB88WyXGAEMjLBSTPFlAG+gIVy2oUzMlx+wAQJBAjAHzDACPCALCOIYIQ1TJ223CAEMjLBVAIzxZQBPoCFstqEssfEss/yXL7AJM1bCHiA8hQBPoCWM8WAc8WzMntVLxoCgw=");

                return map;
            }();
            return map;
        }
    }  // namespace

    td::Result<td::Ref<vm::Cell>> SmartContractCode::load(td::CSlice name)
    {
        auto& map = get_map();
        auto it = map.find(name);
        if (it == map.end())
        {
            return td::Status::Error(PSLICE() << "Can't load td::Ref<vm::Cell> " << name);
        }
        return it->second;
    }

    td::Span<int> SmartContractCode::get_revisions(WalletType type)
    {
        switch (type)
        {
        case WalletType::WalletV3:
        {
            static int res[] = { 2, 1 };
            return res;
        }
        case WalletType::WalletV4:
        {
            static int res[] = { 2, 1 };
            return res;
        }
        case WalletType::NftCollection:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::NftItem:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::NftSingle:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::NftMarketplace:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::NftSale:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::JettonMinter:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::JettonWallet:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::WalletV1:
        {
            static int res[] = { 2, 1 };
            return res;
        }
        case WalletType::WalletV2:
        {
            static int res[] = { 2, 1 };
            return res;
        }
        case WalletType::WalletV1Ext:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::HighloadWalletV1:
        {
            static int res[] = { -1, 1, 2 };
            return res;
        }
        case WalletType::HighloadWalletV2:
        {
            static int res[] = { -1, 1, 2 };
            return res;
        }
        case WalletType::Multisig:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::ManualDns:
        {
            static int res[] = { -1, 1 };
            return res;
        }
        case WalletType::PaymentChannel:
        {
            static int res[] = { -1 };
            return res;
        }
        case WalletType::RestrictedWallet:
        {
            static int res[] = { 1 };
            return res;
        }
        }
        //UNREACHABLE();
        return {};
    }

    td::Result<int> SmartContractCode::validate_revision(WalletType type, int revision)
    {
        auto revisions = get_revisions(type);
        if (revision == -1)
        {
            if (revisions[0] == -1)
            {
                return -1;
            }
            return revisions[revisions.size() - 1];
        }
        if (revision == 0)
        {
            return revisions[revisions.size() - 1];
        }
        for (auto x : revisions)
        {
            if (x == revision)
            {
                return revision;
            }
        }
        return td::Status::Error("No such revision");
    }

    std::string SmartContractCode::getWalletName(WalletType type)
    {
        switch (type)
        {
        case WalletType::WalletV1:
            return "simple-wallet";
        case WalletType::WalletV2:
            return "wallet";
        case WalletType::WalletV3:
            return "wallet3";
        case WalletType::WalletV4:
            return "wallet4";
        case WalletType::WalletV1Ext:
            return "simple-wallet-ext";
        case WalletType::HighloadWalletV1:
            return "highload-wallet";
        case WalletType::HighloadWalletV2:
            return "highload-wallet-v2";
        case WalletType::Multisig:
            return "multisig";
        case WalletType::ManualDns:
            return "dns-manual";
        case WalletType::PaymentChannel:
            return "payment-channel";
        case WalletType::RestrictedWallet:
            return "restricted-wallet3";
        case WalletType::NftCollection:
            return "Nft-collection";
        case WalletType::NftItem:
            return "Nft-item";
        case WalletType::NftSingle:
            return "Nft-single";
        case WalletType::NftMarketplace:
            return "Nft-marketplace";
        case WalletType::NftSale:
            return "Nft-sale";
        case WalletType::JettonMinter:
            return "Jetton-minter";
        case WalletType::JettonWallet:
            return "Jetton-wallet";
        }
        //UNREACHABLE();
        return "Unknown";
    }

    td::Ref<vm::Cell> SmartContractCode::get_code(WalletType type, int ext_revision)
    {
        auto revision = validate_revision(type, ext_revision).move_as_ok();
        auto basename = getWalletName(type);
        if (revision == -1)
            return load(basename).move_as_ok();

        //mycode
        //return load(PSLICE() << basename << "-r" << revision).move_as_ok();
        return load(basename + "-r" + std::to_string(revision)).move_as_ok();
    }

    td::int32 SmartContractCode::guess_revision(WalletType type, const vm::Cell::Hash& code_hash)
    {
        for (auto i : get_revisions(type))
        {
            auto code = SmartContractCode::get_code(type, i);
            if (code->get_hash() == code_hash)
                return i;
        }
        return {};

    }
}  // namespace ton
