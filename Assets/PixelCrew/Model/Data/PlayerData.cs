using System;
using UnityEngine;

namespace PixelCrew.Model.Data
{
    [Serializable]
    public class PlayerData
    {
        [SerializeField] private InventoryData _inventory;

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

