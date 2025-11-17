using System;
using UnityEngine;

namespace PixelCrew.Model
{
    [Serializable]
    public class PlayerData
    {
        public int SilverCoinCount;
        public int GoldCoinCount;

        public bool isArmed;
        public int SwordProjectileCount;

        public int FullHealth;
        public int Health;

        public PlayerData Clone()
        {
            var json = JsonUtility.ToJson(this);
            return JsonUtility.FromJson<PlayerData>(json);
        }
    }
}

