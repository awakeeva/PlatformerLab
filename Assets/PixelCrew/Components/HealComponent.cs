using UnityEngine;

namespace PixelCrew.Components
{
    public class HealComponent : MonoBehaviour
    {
        [SerializeField] private int _heal;

        public void ApplyHealing(GameObject target)
        {
            var healthComponent = target.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.ApplyHealing(_heal);
            }
        }
    }
}

