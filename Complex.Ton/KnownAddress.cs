using System;
using Complex.Collections;
using Complex.Remote;

namespace Complex.Ton
{
    public class KnownAddress
    {
        private const string url = "https://github.com/catchain/tonscan/blob/master/src/addrbook.json";
        private const string rawUrl = "https://raw.githubusercontent.com/catchain/tonscan/master/src/addrbook.json";
        private const string rawUrl2 = "https://catchain.github.io/tonscan/src/addrbook.json";

        private static Hashtable<string, string> hashtable;
        private static object async = new object();

        static KnownAddress()
        {
        }

        private static void InitHash()
        {
            if (hashtable == null)
            {
                lock (async)
                {
                    if (hashtable == null)
                    {
                        try
                        {
                            string data = Http.GetBrouser(rawUrl2);
                            if (data != null)
                            {
                                JsonArray array = Json.Parse2(data) as JsonArray;
                                if (array != null && array.Count > 0)
                                {
                                    hashtable = new Hashtable<string, string>();
                                    foreach (JsonValue value in array)
                                    {
                                        if (value.Value is string)
                                        {
                                            Add(value.name, value.Value as string);
                                        }
                                        else if (value.Value is JsonArray)
                                        {
                                            object res = (value.Value as JsonArray)["name"];
                                            if (res is string)
                                                Add(value.name, res as string);

                                        }
                                    }
                                }

                            }
                        }
                        catch (Exception e)
                        {

                        }
                        if (hashtable == null)
                            hashtable = new Hashtable<string, string>();
                        InitDefault();

                    }
                }
            }
        }

        public static string GetName(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                InitHash();
                return hashtable[address];
            }
            return null;
        }

        private static void Add(string addres, string name)
        {
            hashtable[addres] = name;
        }

        private static void InitDefault()
        {
            Add("EQDCH6vT0MvVp0bBYNjoONpkgb51NMPNOJXFQWG54XoIAs5Y", "CAT Services");
            Add("EQCD39VS5jcptHL8vMjEXrzGaRcCVYto7HUn4bpAOg8xqB2N", "TON Foundation");
            Add("EQAhE3sLxHZpsyZ_HecMuwzvXHKLjYx4kEUehhOy2JmCcHCT", "TON Ecosystem Reserve");
            Add("Ef8zMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzMzM0vF", "Elector Contract");
            Add("Ef9VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVbxn", "Config Contract");
            Add("Ef8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADAU", "System");
            Add("Ef80UXx731GHxVr0-LYf3DIViMerdo3uJLAG3ykQZFjXz2kW", "Log tests Contract");
            Add("EQC3dNlesgVD8YbAazcauIrXBPfiVhMMr5YYk2in0Mtsz0Bz", ".ton DNS");

            Add("EQCtiv7PrMJImWiF2L5oJCgPnzp-VML2CAt5cbn1VsKAxLiE", "CryptoBot");
            Add("EQBYivdc0GAk-nnczaMnYNuSjpeXu2nJS3DZ4KqLjosX5sVC", "Testgiver TON Bot");
            Add("EQDd3NPNrWCvTA1pOJ9WetUdDCY_pJaNZVq0JMaara-TIp90", "Wallet Bot");
            Add("EQBDanbCeUqI4_v-xrnAN0_I2wRvEIaLg1Qg2ZN5c6Zl1KOh", "Wallet Bot");
            Add("EQBfAN7LfaUYgXZNw5Wc7GBgkEX2yhuJ5ka95J1JJwXXf4a8", "OKX");
            Add("EQCzFTXpNNsFu8IgJnRnkDyBCL2ry8KgZYiDi3Jt31ie8EIQ", "FTX");
            Add("EQBX63RAdgShn34EAFMV73Cut7Z15lUZd1hnVva68SEl7sxi", "MEXC");
            Add("EQB5lISMH8vLxXpqWph7ZutCS4tU4QdZtrUUpmtgDCsO73JR", "EXMO");
            Add("EQCNGVeTuq2aCMRtw1OuvpmTQdq9B3IblyXxnhirw9ENkhLa", "EXMO Cold Storage 1");
            Add("EQAmq4rnY6OnwwZ9iCt7Ac1dNyVMuHaPV7akfAACjv_HuO5H", "EXMO Cold Storage 2");
            Add("EQABMMdzRuntgt9nfRB61qd1wR-cGPagXA3ReQazVYUNrT7p", "EXMO Deposit");
            Add("EQA0KjWeODV8CDloEp_d3fBJ71xHMVv77ydQWjVr-fAtZSqw", "CoinEx");
            Add("EQCFr3jo0DXpIBF82mVGFc3zcdRkSAtinhENPFMQ2FqzYqDB", "Huobi Widthdrawal");
            Add("EQBVXzBT4lcTA3S7gxrg4hnl5fnsDKj4oNEzNp09aQxkwj1f", "Huobi Deposit");
            Add("EQCzflcDPbIdELlQ5hQ7ZYwQw79CW9GTAllgrvfyLbz0_OZs", "KuCoin Widthdrawal");
            Add("EQCA1BI4QRZ8qYmskSRDzJmkucGodYRTZCf_b9hckjla6dZl", "KuCoin Deposit");

            Add("Ef9NXAIQs12t2qIZ-sRZ26D977H65Ol6DQeXc5_gUNaUys5r", "BSC Bridge");
            Add("EQAHI1vGuw7d4WG-CtfDrWqEPNtmUuKjKFEFeJmZaqqfWTvW", "BSC Bridge Collector");
            Add("Ef8OvX_5ynDgbp4iqJIvWudSEanWo0qAlOjhWHtga9u2YjVp", "BSC Bridge Governance");
            Add("Ef_dJMSh8riPi3BTUTtcxsWjG8RLKnLctNjAM4rw8NN-xWdr", "ETH Bridge");
            Add("EQCuzvIOXLjH2tv35gY4tzhIvXCqZWDuK9kUhFGXKLImgxT5", "ETH Bridge Collector");
            Add("Ef87m7_QrVM4uXAPCDM4DuF9Rj5Rwa5nHubwiQG96JmyAjQY", "ETH Bridge Governance");
            Add("Ef_P2CJw784O1qVd8Qbn8RCQc4EgxAs8Ra-M3bDhZn3OfzRb", "Bridge Oracle 0");
            Add("Ef8DfObDUrNqz66pr_7xMbUYckUFbIIvRh1FSNeVSLWrvo1M", "Bridge Oracle 1");
            Add("Ef8JKqx4I-XECLuVhTqeY1WMgbgTp8Ld3mzN-JUogBF4ZEW-", "Bridge Oracle 2");
            Add("Ef8voAFh-ByCeKD3SZhjMNzioqCmDOK6S6IaeefTwYmRhgsn", "Bridge Oracle 3");
            Add("Ef_uJVTTToU8b3o7-Jr5pcUqenxWzDNYpyklvhl73KSIA17M", "Bridge Oracle 4");
            Add("Ef93olLWqh1OuBSTOnJKWZ4NwxNq_ELK55_h_laNPVwxcEro", "Bridge Oracle 5");
            Add("Ef_iUPZdKLOCrqcNpDuFGNEmiuBwMB18TBXNjDimewpDExgn", "Bridge Oracle 6");
            Add("Ef_tTGGToGmONePskH_Y6ZG-QLV9Kcg5DIXeKwBvCX4YifKa", "Bridge Oracle 7");
            Add("Ef94L53akPw-4gOk2uQOenUyDYLOaif2g2uRoiu1nv0cWYMC", "Bridge Oracle 8");

            Add("Ef-VAFf1Wd3fXd-mQhDw5lNsVdIZv2_H1yhbdzXCFfIe9p95", "CAT Validator 1");
            Add("Ef-p4N7wkBQcce3Awcm06a1EV2VsFPYR7GtRczlYP0G2C1Pm", "CAT Validator 2");
            Add("Ef86ziqX4uPh-ZcrOK8bWszUzfNhHg_SPnvf9CQOnElFINEE", "CAT Validator 3");
            Add("Ef8vk8p6nogM_JKMhpqXnffFrzikOzGjIUNLP8sdIasIb8DV", "CAT Validator 4");

            Add("Ef9wm_whwjPFe7H4jvP-ODhluiZFm0Tb2Gj-67zqS31hCaWC", "CAT Staking Pool 1");
            Add("Ef-BHO0nH49EnLUetZIAkLkssgCyDwcXBnbp22--naUWz8VY", "CAT Staking Pool 2");
            Add("Ef8iu8EiNOP2MczVvHseFi-CrGO1C4v6MkSSOgVZcESNGfT7", "CAT Staking Pool 3");
            Add("Ef8AeKBMQKW-PB8-RDeyJQSsxQQr5oEwbQeBEwE2BKDiFA_U", "CAT Staking Pool 4");
            Add("Ef-DxWkExr12iOSi0vJfT5TKCUG9W3-eWInm1yT5oEJISJkl", "CAT Staking Pool 5");
            Add("Ef8z0gek-K888pl61tyErw96TfnthjV9lZ7UDVFXlad9HEGS", "CAT Staking Pool 6");
            Add("Ef98be2ASo4xA_t2Q2VIH0cdpYKylDyfhL3nL7Byjz180VaU", "CAT Staking Pool 7");
            Add("Ef8h5pd9_ZuWJOlilBH6a_LOACxl_R6DJbWHhut7QLDLWSgf", "CAT Staking Pool 8");
            Add("Ef8gQpp7pKD9GzBrcr3ju9faPjEWHPerhZ4tFpSiDoDUINxn", "Very First Pool #1");
            Add("Ef_dodh2I8BjpvxJIrKG5owo5_C1RZlzkZtXC3HGLuNZE0Sa", "Very First Pool #2");
            Add("Ef9Qhifu_o6WiS3JzJEailMuDtlDqmy55eCGDJ5cYSTGD4BW", "Global Net AC Pool #1");
            Add("Ef-pcGkDL4qjf44vN8iD8-yrjs-wVWOi5LVQudbDKbukHQvb", "Global Net AC Pool #2");
            Add("Ef9IJWfn0qDrh4S8CBJsZUzAPrxQkH3C_JanVrQWjOZ5LndS", "Fastnet Pool A #1");
            Add("Ef__38zm-M_kTn6OgIs5DKVQy2qnjJIF50xZ203regWrI-yh", "Fastnet Pool A #2");
            Add("Ef8_6eSCSeJyCeLM7uCxYjeirtQ4Zo8OEp0W4HJYJY4IC0zK", "Fastnet Pool B #1");
            Add("Ef_nHR5IUKCBf_qHlIjCsUWuo6bbrh174f_aJfN6zIAnBF9n", "Fastnet Pool B #2");
            Add("Ef9qXoe6qX5kboeTbXXdxNOcqCA53Oi9LYsY66l4FuNLZRWx", "TonStake Validator #1");
            Add("Ef_L2qAIJ3Xe3hQqGdzG19gmbkteqYTqSZUkYWnzWH734Sm-", "TonStake Validator #2");
            Add("Ef_AhqqheA-GkmwN4uAg7j5qjHffA2VL-kzFuXoYs9fD7kJL", "TonStake Validator #3");
            Add("EQAUgVXUBJC7c72oEaQGowLvWhnf-nKghL7zBX8FSqrSXkp4", "TonStake Deposit");
            Add("EQAOCN7KlgGzTp6YjW6d_fm_ibJEUe0VwFyKNnZVzlL4Jda3", "TonStake Withdrawal");

            Add("EQAAFhjXzKuQ5N0c96nsdZQWATcJm909LYSaCAvWFxVJP80D", "Whales Pool");
            Add("EQBeNwQShukLyOWjKWZ0Oxoe5U3ET-ApQIWYeC4VLZ4tmeTm", "Whales Withdraw 1");
            Add("EQAQwQc4N7k_2q1ZQoTOi47_e5zyVCdEDrL8aCdi4UcTZef4", "Whales Withdraw 2");
            Add("EQDQA68_iHZrDEdkqjJpXcVqEM3qQC9u0w4nAhYJ4Ddsjttc", "Whales Withdraw 3");
            Add("EQCr1U4EVmSWpx2sunO1jhtHveatorjfDpttMCCkoa0JyD1P", "Whales Withdraw 4");
            Add("EQAB_3oC0MH1r4fz1kztk6Nhq9GFQnrBUgObzrhyAXjzzjrc", "Whales Withdraw 5");
            Add("EQCz4NlftqOJlDZFerutRjy8bpDuNkLuLFn9pHsnK-mfXuZ0", "TON Coin Pool Fund");
            Add("EQCUp88072pLUGNQCXXXDFJM3C5v9GXTjV7ou33Mj3r0Xv2W", "TON Coin Pool Rewards");
            Add("EQCUp88072pLUGNQCXXXDFJM3C5v9GXTjV7ou33Mj3r0Xv2W", "TON Coin Pool Withdraw 1");
            Add("EQAW6gzsWc-zqY7Z9rquxxeOA4Y6QMB09skcBXDnnuL3EK8L", "TON Coin Pool Withdraw 2");

            Add("EQCjk1hh952vWaE9bRguFkAhDAL5jj3xj9p0uPWrFBq_GEMS", "Getgems Marketplace");
            Add("EQBYTuYbLf8INxFtD8tQeNk5ZLy-nAX9ahQbG_yl1qQ-GEMS", "Getgems Sales");
            Add("EQDrLq-X6jKZNHAScgghh0h1iog3StK71zn8dcmrOj8jPWRA", "Disintar Marketplace");
            Add("EQA8sc2WlFb7VIpK_777JIX9vrYMz3FvPogd2OdhxR18e-Hg", "Rich Cats Fund");
            Add("EQDe1lrwD7d5ntSQuAPtQ2kUp_BSa8a4tNMMNak21zQXPSUa", "Rich Cats");
            Add("EQCJTkhd1W2wztkVNp_dsKBpv2SIoUWoIyzI7mQrbSrj_NSh", "TON Diamonds");
            Add("EQANKN8ZnM0OzYOENTkOEg7VVgFog5fBWdCtqQro1MRmU5_2", "Animals Red List NFT");
            Add("EQCRMjhmUVkjiYvj9d-Yotr4OT2ekPoKU9Hmq0EHTokRO6EK", "TON Earth");
            Add("EQASOUQL3Pok0fuUHvc_d0lY4K7Z9VnGE_VLot3GAeSNVeF5", "TON Guys NFT");

            Add("EQA5Pxp_EC9pTlxrvO59D1iqBqodajojullgf07ENKa22oSN", "TelePay");
            Add("EQBNaV2nd9-OGDmWXNip6SizsygyGrhd7CQ-hkJ6xm7b6NhC", "OTC Market");
            Add("EQCtBAFC02qgf2jKf6SrLNiRxTZaHut7pRpbXZoasOx2EnXs", "Tonometr Bot");
            Add("EQCR1zBW4DUjLwmq-CQqHVHuqYtqW-u_isDJ5SHQKhpL2wQV", "Morgenshtern");
            Add("EQCpDaCVY7Z0Ckt_aMoJ-9t2sANcwQFFChbi55uYXruzilrn", "Morgenshtern Private");
            Add("EQBd3OeCL1nRhTVSWlKFzYDO_u8_Xf-N1CmaiVfhKdh_wUzR", "Subscriptions");

            Add("Ef-kkdY_B7p-77TLn2hUhM6QidWrrsl8FYWCIvBMpZKprKDH", "PoW Giver 1");
            Add("Ef8SYc83pm5JkGt0p3TQRkuiM58O9Cr3waUtR9OoFq716uj0", "PoW Giver 2");
            Add("Ef-FV4QTxLl-7Ct3E6MqOtMt-RGXMxi27g4I645lw6MTWg0f", "PoW Giver 3");
            Add("Ef_NSzfDJI1A3rOM0GQm7xsoUXHTgmdhN5-OrGD8uwL2JHBa", "PoW Giver 4");
            Add("Ef8gf1PQy4u2kURl-Gz4LbS29eaN4sVdrVQkPO-JL80VhFww", "PoW Giver 5");
            Add("Ef8kO6K6Qh6YM4ddjRYYlvVAK7IgyW8Zet-4ZvNrVsmQ4PgP", "PoW Giver 6");
            Add("Ef-P_TOdwcCh0AXHhBpICDMxStxHenWdLCDLNH5QcNpwMMn2", "PoW Giver 7");
            Add("Ef91o4NNTryJ-Cw3sDGt9OTiafmETdVFUMvylQdFPoOxInls", "PoW Giver 8");
            Add("Ef9iWhwk9GwAXjtwKG-vN7rmXT3hLIT23RBY6KhVaynRrDkx", "PoW Giver 9");
            Add("Ef8JfFUEJhhpRW80_jqD7zzQteH6EBHOzxiOhygRhBdt44YH", "PoW Giver 10");
            Add("kf8guqdIbY6kpMykR8WFeVGbZcP2iuBagXfnQuq0rGrxgE04", "Large Giver 1");
            Add("kf9CxReRyaGj0vpSH0gRZkOAitm_yDHvgiMGtmvG-ZTirrMC", "Large Giver 2");
            Add("kf-WXA4CX4lqyVlN4qItlQSWPFIy00NvO2BAydgC4CTeIUme", "Large Giver 3");
            Add("kf8yF4oXfIj7BZgkqXM6VsmDEgCqWVSKECO1pC0LXWl399Vx", "Large Giver 4");
            Add("kf9nNY69S3_heBBSUtpHRhIzjjqY0ChugeqbWcQGtGj-gQxO", "Large Giver 5");
            Add("kf_wUXx-l1Ehw0kfQRgFtWKO07B6WhSqcUQZNyh4Jmj8R4zL", "Large Giver 6");
            Add("kf_6keW5RniwNQYeq3DNWGcohKOwI85p-V2MsPk4v23tyO3I", "Large Giver 7");
            Add("kf_NSPpF4ZQ7mrPylwk-8XQQ1qFD5evLnx5_oZVNywzOjSfh", "Large Giver 8");
            Add("kf-uNWj4JmTJefr7IfjBSYQhFbd3JqtQ6cxuNIsJqDQ8SiEA", "Large Giver 9");
            Add("kf8mO4l6ZB_eaMn1OqjLRrrkiBcSt7kYTvJC_dzJLdpEDKxn", "Large Giver 10");

        }
    }
}
