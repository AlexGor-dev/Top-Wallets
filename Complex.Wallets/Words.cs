using System;
using System.IO;
using System.Reflection;
using Complex.Collections;

namespace Complex.Wallets
{
    public class Words
    {
        static Words()
        {
            using (Stream s = Assembly.GetAssembly(typeof(Words)).GetManifestResourceStream("Complex.Wallets.Resource.words.txt"))
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string word = line.Trim();
                        hash.Add(word, word);
                    }
                }
            }
        }

        private static Hashtable<string, string> hash = new Hashtable<string, string>();

        public static bool Contains(string word)
        {
            return hash.ContainsKey(word);
        }

        public static string[] GetWords(string prefix, int maxcount)
        {
            Collection<string> res = new Collection<string>();
            prefix = prefix.Trim().ToLower();
            foreach (string word in hash)
            {
                if (word.StartsWith(prefix))
                {
                    res.Add(word);
                    if (maxcount > 0 && res.Count > maxcount)
                        return new string[0];
                }
            }
            return res.ToArray();
        }
    }
}
