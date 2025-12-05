using System;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Components.GoBased;
using PixelCrew.Utils;
using UnityEngine;

namespace PixelCrew.Creatures.Mobs
{
    public class ShooterTrapAI : MonoBehaviour
    {
        [SerializeField] private ColliderCheck _vision;

        [Header("Melee")]
        [SerializeField] private Cooldown _meleeCooldown;
        [SerializeField] private CheckCircleOverlap _meleeAttack;
        [SerializeField] private ColliderCheck _meleeCanAttack;

        [Header("Range")]
        [SerializeField] private Cooldown _rangeCooldown;
        [SerializeField] private SpawnComponent _rangeAttack;
        
        private static readonly int MeleeKey = Animator.StringToHash("melee");
        private static readonly int RangeKey = Animator.StringToHash("range");

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (_vision.IsTouchingLayer)
            {
                if (_meleeCanAttack.IsTouchingLayer)
                {
                    if (_meleeCooldown.IsReady)
                    {
                        MeleeAttack();
                    }
                    return;
                }

                if (_rangeCooldown.IsReady)
                {
                    RangeAttack();
                }
            }
        }

        private void RangeAttack()
        {
            _rangeCooldown.Reset();
            _animator.SetTrigger(RangeKey);
        }

        private void OnRangeAttack()
        {
            _rangeAttack.Spawn();
        }

        private void MeleeAttack()
        {
            _meleeCooldown.Reset();
            _animator.SetTrigger(MeleeKey);
        }

        private void OnMeleeAttack()
        {
            _meleeAttack.Check();
        }

    }
}
