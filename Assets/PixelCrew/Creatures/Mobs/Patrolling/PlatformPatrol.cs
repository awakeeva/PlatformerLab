using System.Collections;
using PixelCrew.Components.ColliderBased;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs.Patrolling
{
    public class PlatformPatrol : Patrol
    {
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] private float _direction;
        [SerializeField] private Creature _creature;
        public override IEnumerator DoPatrol()
        {
            while (enabled)
            {
                if (_groundCheck.IsTouchingLayer)
                {
                    _creature.SetDirection(new Vector2(_direction, 0f));
                }
                else
                {
                    _direction = -_direction;
                    _creature.SetDirection(new Vector2(_direction, 0f));
                }

                yield return null;
            }
        }
    }
}

