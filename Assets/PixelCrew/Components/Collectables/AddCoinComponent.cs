using UnityEngine;
using PixelCrew.Creatures.Hero;

namespace PixelCrew.Components.Collectables
{
    public class AddCoinComponent : MonoBehaviour
    {
        public void AddSilverCoin(GameObject target)
        {
            var hero = target.GetComponent<Hero>();
            if (hero != null)
            {
                hero.AddSilverCoin(1);
            }
        }
        public void AddGoldCoin(GameObject target)
        {
            var hero = target.GetComponent<Hero>();
            if (hero != null)
            {
                hero.AddGoldCoin(1);
            }
        }

    }
}
