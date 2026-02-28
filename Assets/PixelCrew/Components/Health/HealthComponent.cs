using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace PixelCrew.Components.Health
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private int _fullHealth;
        [SerializeField] private int _health;

        [SerializeField] private bool _modeModularHealth;

        [SerializeField] private UnityEvent _onDamage;
        [SerializeField] private UnityEvent _onHeal;
        [SerializeField] public UnityEvent _onDie;
        [SerializeField] private HealthChangeEvent _onChange;

        private bool _isDead;

        private void Awake()
        {
            _health = _fullHealth;
        }

        public void ModifyHealth(int healthDelta)
        {
            if (_modeModularHealth)
            {
                ModifyHealthGroup(healthDelta);
            }
            else
            {
                ModifyHealthSingle(healthDelta);
            }
        }

        private void ModifyHealthGroup(int healthDelta)
        {
            var parentGO = this.gameObject.transform.parent.gameObject;
            List<GameObject> listOfChildren = new List<GameObject>();

            foreach (Transform child in parentGO.transform)
            {
                if (null == child)
                    continue;
                if (null == child.GetComponent<HealthComponent>())
                    continue;

                listOfChildren.Add(child.gameObject);
            }

            if (listOfChildren.Count() > 0)
            {
                var sortedListOfChildren = listOfChildren.OrderByDescending(go => go.transform.position.y).ToList();

                var healthComponent = sortedListOfChildren[0].GetComponent<HealthComponent>();
                
                healthComponent.ModifyHealthSingle(healthDelta);
            }
        }

        public void ModifyHealthSingle(int healthDelta)
        {
            if (_isDead) return;

            _health += healthDelta;

            if (_health > _fullHealth)
            {
                _health = _fullHealth;
            }

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

        private void OnDestroy()
        {
            _onDie.RemoveAllListeners();
        }

        [Serializable]
        public class HealthChangeEvent : UnityEvent<int, int>
        {

        }
    }
}

