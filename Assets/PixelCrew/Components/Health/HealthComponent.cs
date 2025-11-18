using System;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _fullHealth;
        [SerializeField] private int _health;
        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        private bool _isDead;

        private void Awake()
        {
            _health = _fullHealth;
        }

        public void ModifyHealth(int healthDelta)
        {
            if (_isDead) return;

            _health += healthDelta;

            _onChange?.Invoke(_fullHealth, _health);

            if (healthDelta < 0)
            {
                _onDamage?.Invoke();
            }

            if (healthDelta > 0)
            {
                _onHeal?.Invoke();
            }

            if (_health <= 0)
            {
                _isDead = true;
                _onDie?.Invoke();
            }

            if (_health > _fullHealth)
            {
                _health = _fullHealth;
            }
        }

        public void ApplyDamage(int damageValue)
        {
            ModifyHealth(damageValue);
        }

        public void ApplyHealing(int healValue)
        {
            ModifyHealth(healValue);
        }

#if UNITY_EDITOR
        [ContextMenu("Update Health")]
        private void UpdateHealth()
        {
            _onChange?.Invoke(_fullHealth, _health);
        }
#endif
        public void SetHealth(int fullHealth, int currentHealth)
        {
            _fullHealth = fullHealth;
            _health = currentHealth;
        }

        [Serializable]
        public class HealthChangeEvent : UnityEvent<int, int>
        {

        }
    }
}

