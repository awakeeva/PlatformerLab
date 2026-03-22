using PixelCrew.Components.Health;
using PixelCrew.Model;
using PixelCrew.Model.Definitions;
using UnityEngine;

namespace PixelCrew.Creatures.Weapons
{
    public class BaseProjectile : MonoBehaviour
    {
        [SerializeField] protected float _speed;
        [SerializeField] protected bool _invertX;

        [SerializeField] protected bool _damageScaledByPlayerStats;

        protected Rigidbody2D Rigidbody;
        protected int Direction;

        private GameSession _session;

        protected virtual void Start()
        {
            _session = FindObjectOfType<GameSession>();

            var mod = _invertX ? -1 : 1;
            Direction = mod * transform.lossyScale.x > 0 ? 1 : -1;
            Rigidbody = GetComponent<Rigidbody2D>();

            if (_damageScaledByPlayerStats)
            {
                var modifyHealth = GetComponent<ModifyHealthComponent>();
                modifyHealth.HpDelta = -(int)_session.StatsModel.GetValue(StatId.RangeDamage);
            }
        }
    }
}

