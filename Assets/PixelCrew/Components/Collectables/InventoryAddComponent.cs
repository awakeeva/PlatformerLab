using PixelCrew.Creatures.Hero;
using UnityEngine;

namespace PixelCrew.Components.Collectables
{
    public class InventoryAddComponent : MonoBehaviour
    {
        [SerializeField] private string _id;
        [SerializeField] private int _count;
        public void Add(GameObject target)
        {
            var hero = target.GetComponent<Hero>();
            if (hero != null)
            {
                hero.AddInInventory(_id, _count);
            }
        }
    }
}

