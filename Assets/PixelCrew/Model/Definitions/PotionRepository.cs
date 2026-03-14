using System;
using UnityEngine;

namespace PixelCrew.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/Potions", fileName = "Potions")]

    public class PotionRepository : DefRepository<PotionDef>
    {

    }

    [Serializable]
    public struct PotionDef : IHaveId
    {
        [InventoryId][SerializeField] private string _id;
        [SerializeField] private float _value;
        [SerializeField] private float _time;

        public string Id => _id;

        public bool IsVoid => string.IsNullOrEmpty(_id);

        public float Value => _value;

        public float Time => _time;

    }
}

