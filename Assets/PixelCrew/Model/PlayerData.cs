using System;

namespace PixelCrew.Model
{
    [Serializable]
    public class PlayerData
    {
        public int SilverCoinCount = 0;
        public int GoldCoinCount = 0;

        public bool isArmed;

        public int FullHealth;
        public int Health;
    }
}

