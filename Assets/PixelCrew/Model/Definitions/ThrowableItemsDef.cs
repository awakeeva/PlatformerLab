using PixelCrew.Model.Definitions;
using System;
using UnityEngine;

namespace PixelCrew.Model.Definitions
{
    [CreateAssetMenu(menuName = "Defs/ThrowableItems", fileName = "ThrowableItems")]

    public class ThrowableItemsDef : DefRepository<ThrowableDef>
    {

    }

    [Serializable]
    public struct ThrowableDef : IHaveId
    {
        [InventoryId] [SerializeField] private string _id;
        [SerializeField] private GameObject _projectile;

        public string Id => _id;

        public bool IsVoid => string.IsNullOrEmpty(_id);

        public GameObject Projectile => _projectile;

    }
}
