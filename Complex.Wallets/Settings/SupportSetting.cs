using System;

namespace Complex.Wallets
{
    [Serializable]
    public class SupportSetting
    {
        private bool enabled;
        public bool Enabled => enabled;

        private int maxTops;
        public int MaxTops => maxTops;

        private int maxLasts;
        public int MaxLasts => maxLasts;

        private int maxDays;
        public int MaxDays => maxDays;

        private int maxNameLenght;
        public int MaxNameLenght => maxNameLenght;

        private int maxMessageLenght;
        public int MaxMessageLenght => maxMessageLenght;

    }
}
