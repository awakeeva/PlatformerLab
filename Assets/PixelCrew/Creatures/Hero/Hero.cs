using UnityEngine;
using UnityEngine.Animations;
using PixelCrew.Components;
using PixelCrew.Utils;
using PixelCrew.Model;
using System;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Creatures;

namespace PixelCrew.Creatures.Hero
{
    public class Hero : Creature
    {
        [Header("HERO")]
        [SerializeField] private float _dashSpeed;
        [SerializeField] private float _dashDuration;

        [SerializeField] private Cooldown _throwCooldown;

        [SerializeField] private float _slamDownVelocity;
        [SerializeField] private float _heavyFallSpeed;

        [SerializeField] private CheckCircleOverlap _interactionCheck;

        [SerializeField] private LayerCheck _wallCheck;

        [SerializeField] private UnityEditor.Animations.AnimatorController _armed;
        [SerializeField] private UnityEditor.Animations.AnimatorController _disarmed;


        [Space]
        [Header("Particles")]
        [SerializeField] private ParticleSystem _hitParticles;

        private static readonly int ThrowKey = Animator.StringToHash("throw");
        private static readonly int IsOnWallKey = Animator.StringToHash("is-on-wall");

        private bool _allowDoubleJump;

        private bool _isOnWall;

        private bool _isDashOn;
        private float _dashTimer;

        private const int SilverCoinCost = 1;
        private const int GoldCoinCost = 10;

        private GameSession _session;
        private float _defaultGravityScale;

        private int SwordCount => _session.Data.Inventory.Count("Sword");
        private int SilverCoinCount => _session.Data.Inventory.Count("SilverCoin");
        private int GoldCoinCount => _session.Data.Inventory.Count("GoldCoin");

        protected override void Awake()
        {
            base.Awake();
            _defaultGravityScale = RigidbodyComp.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            HealthComp.SetHealth(_session.Data.FullHealth, _session.Data.Health);

            _session.Data.Inventory.onChanged += OnInventoryChanged;
            _session.Data.Inventory.onChanged += OnInventoryChangedLog;

            UpdateHeroWeapon();
        }

        private void OnDestroy()
        {
            _session.Data.Inventory.onChanged -= OnInventoryChanged;
            _session.Data.Inventory.onChanged -= OnInventoryChangedLog;
        }

        private void OnInventoryChangedLog(string id, int value)
        {
            Debug.Log($"Inventory changed: {id}: {value}");
        }

        private void OnInventoryChanged(string id, int value)
        {
            if (id == "Sword")
            {
                UpdateHeroWeapon();
            }
        }

        public void OnHealthChanged(int fullHealth, int currentHealth)
        {
            _session.Data.FullHealth = fullHealth;
            _session.Data.Health = currentHealth;
        }

        protected override void Update()
        {
            base.Update();

            var moveToSameDirection = Direction.x * transform.localScale.x > 0;
            if (_wallCheck.IsTouchingLayer && moveToSameDirection)
            {
                _isOnWall = true;
                RigidbodyComp.gravityScale = 0;
            }
            else
            {
                _isOnWall = false;
                RigidbodyComp.gravityScale = _defaultGravityScale;
            }

            AnimatorComp.SetBool(IsOnWallKey, _isOnWall);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.IsInLayer(_groundLayer))
            {
                var contact = other.contacts[0];
                if (contact.relativeVelocity.y >= _slamDownVelocity)
                {
                    _particles.Spawn("FallDust");
                }

                if (contact.relativeVelocity.y >= _fallDamageSpeed)
                {
                    HealthComp.ModifyHealth(-1);
                }
            }
        }

        protected override float CalculateXVelocity()
        {
            if (_dashTimer > 0)
            {
                _dashTimer -= Time.deltaTime;
            }
            else
            {
                _dashTimer = 0;
                _isDashOn = false;
            }

            if (_isDashOn)
            {
                return transform.localScale.x * _dashSpeed;
            }
            else
            {
                return base.CalculateXVelocity();
            }
        }

        protected override float CalculateYVelocity()
        {
            var isJumpingPressing = Direction.y > 0;

            if (IsGrounded || _isOnWall)
            {
                _allowDoubleJump = true;
            }

            if (!isJumpingPressing && _isOnWall)
            {
                return 0f;
            }

            return base.CalculateYVelocity();
        }

        protected override float CalculateJumpVelocity(float yVelocity)
        {
            if (!IsGrounded && _allowDoubleJump && !_isOnWall)
            {
                _allowDoubleJump = false;
                HasJustJumpedFlag = true;

                return _jumpSpeed;
            }

            return base.CalculateJumpVelocity(yVelocity);
        }

        public void AddInInventory(string id, int value)
        {
            _session.Data.Inventory.Add(id, value);
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            _allowDoubleJump = true;

            if (SilverCoinCount > 0)
            {
                SpawnCoins();
            }
        }

        public void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(SilverCoinCount, 5);
            _session.Data.Inventory.Remove("SilverCoin", numCoinsToDispose);

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Interact()
        {
            _interactionCheck.Check();
        }

        public void Dash()
        {
            _isDashOn = true;
            _dashTimer = _dashDuration;
        }


        public override void Attack()
        {
            if (SwordCount <= 0) return;

            base.Attack();
        }

        private void UpdateHeroWeapon()
        {
            AnimatorComp.runtimeAnimatorController =
                SwordCount > 0 ? _armed : _disarmed;
        }

        public void OnDoThrow()
        {
            _particles.Spawn("ThrowSword");
        }

        public void Throw()
        {
            if (_throwCooldown.IsReady && SwordCount > 1)
            {
                AnimatorComp.SetTrigger(ThrowKey);
                _session.Data.Inventory.Remove("Sword", 1);
                _throwCooldown.Reset();
            }
        }

    }
}
