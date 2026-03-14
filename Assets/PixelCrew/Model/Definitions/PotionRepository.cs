using System;
using UnityEngine;

namespace PixelCrew.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/HealingItems", fileName = "HealingItems")]

    public class PotionRepository : DefRepository<HealingDef>
    {

    }

    [Serializable]
    public struct HealingDef : IHaveId
    {
        [InventoryId][SerializeField] private string _id;
        [SerializeField] private int _hp;

        public string Id => _id;

        public bool IsVoid => string.IsNullOrEmpty(_id);

        public int Hp => _hp;

    }
}

