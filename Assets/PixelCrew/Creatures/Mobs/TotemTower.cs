using PixelCrew.Components.Health;
using PixelCrew.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class TotemTower : MonoBehaviour
    {
        [SerializeField] private List<TotemFridayAI> _traps;
        [SerializeField] private Cooldown _cooldown;

        private int _currentTrap;

        private void Start()
        {
            foreach (var totemFridayAI in _traps)
            {
                totemFridayAI.enabled = false;
                var hp = totemFridayAI.GetComponent<HealthComponent>();
                hp._onDie.AddListener(() => OnTrapDead(totemFridayAI));
            }
        }

        private void OnTrapDead(TotemFridayAI totemFridayAI)
        {
            var index = _traps.IndexOf(totemFridayAI);
            _traps.Remove(totemFridayAI);
            if (index < _currentTrap)
            {
                _currentTrap--;
            }
        }

        private void Update()
        {
            if (_traps.Count == 0)
            {
                enabled = false;
                Destroy(gameObject, 1f);
            }

            var hasAnyTarget = _traps.Any(x => x._vision.IsTouchingLayer);

            if (hasAnyTarget)
            {
                if (_cooldown.IsReady)
                {
                    _traps[_currentTrap].Shoot();
                    _cooldown.Reset();
                    _currentTrap = (int) Mathf.Repeat(_currentTrap + 1, _traps.Count);
                }
            }
        }
    }
}

