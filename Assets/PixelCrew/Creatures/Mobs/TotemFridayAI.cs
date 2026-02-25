using PixelCrew.Animations;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Utils;
using System;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class TotemFridayAI : MonoBehaviour
    {
        [SerializeField] public ColliderCheck _vision;
        [SerializeField] private Cooldown _cooldown;
        [SerializeField] private SpriteAnimationState _animation;

        private void Update()
        {
            if (_vision.IsTouchingLayer && _cooldown.IsReady)
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            _cooldown.Reset();
            _animation.SetClip("start-attack");
        }
    }
}

