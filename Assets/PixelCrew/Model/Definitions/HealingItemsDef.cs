using System;
using UnityEngine;

namespace PixelCrew.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/HealingItems", fileName = "HealingItems")]

    public class HealingItemsDef : ScriptableObject
    {
        [SerializeField] private HealingDef[] _items;
#if UNITY_EDITOR
        public HealingDef[] ItemsForEditor => _items;
#endif
        public HealingDef Get(string id)
        {
            foreach (var itemDef in _items)
            {
                if (itemDef.Id == id)
                    return itemDef;
            }

            return default;
        }
    }

    [Serializable]
    public struct HealingDef
    {
        [InventoryId][SerializeField] private string _id;
        [SerializeField] private int _hp;

        public string Id => _id;

        public bool IsVoid => string.IsNullOrEmpty(_id);

        public int Hp => _hp;

    }
}

