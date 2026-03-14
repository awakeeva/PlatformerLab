using UnityEngine;
using UnityEngine.Animations;
using PixelCrew.Components;
using PixelCrew.Utils;
using PixelCrew.Model;
using System;
using PixelCrew.Components.ColliderBased;
using PixelCrew.Creatures;
using PixelCrew.Model.Data;
using System.Collections;
using PixelCrew.Components.GoBased;
using PixelCrew.Model.Definitions;

namespace PixelCrew.Creatures.Hero
{
    public class Hero : Creature, ICanAddInInventory
    {
        [Header("HERO")]
        [SerializeField] private float _dashSpeed;
        [SerializeField] private float _dashDuration;

        [SerializeField] private Cooldown _throwCooldown;

        [SerializeField] private float _slamDownVelocity;
        [SerializeField] private float _heavyFallSpeed;

        [SerializeField] private CheckCircleOverlap _interactionCheck;

        [SerializeField] private ColliderCheck _wallCheck;

        [SerializeField] private UnityEditor.Animations.AnimatorController _armed;
        [SerializeField] private UnityEditor.Animations.AnimatorController _disarmed;

        [Space]
        [Header("Super throw")]
        [SerializeField] private Cooldown _superThrowCooldown;
        [SerializeField] private int _superThrowParticles;
        [SerializeField] private float _superThrowDelay;

        [Space]
        [Header("HitDrop")]
        [SerializeField] private ProbabilityDropComponent _hitDrop;

        [SerializeField] private SpawnComponent _throwSpawner;

        private static readonly int ThrowKey = Animator.StringToHash("throw");
        private static readonly int IsOnWallKey = Animator.StringToHash("is-on-wall");
        private static readonly int HealKey = Animator.StringToHash("heal");

        private bool _allowDoubleJump;

        private bool _isOnWall;
        private bool _superThrow;

        private bool _isDashOn;
        private float _dashTimer;

        private GameSession _session;
        private float _defaultGravityScale;


        private const string SwordId = "Sword";
        private int SwordCount => _session.Data.Inventory.Count(SwordId);
        private int SilverCoinCount => _session.Data.Inventory.Count("SilverCoin");
        private int GoldCoinCount => _session.Data.Inventory.Count("GoldCoin");
        private int HealthPotionCount => _session.Data.Inventory.Count("HealthPotion");

        private string SelectedItemID => _session.QuickInventory.SelectedItem.Id;

        private bool CanThrow
        {
            get
            {
                if (_session.QuickInventory.Inventory.Length < 1)
                    return false;

                if (SelectedItemID == SwordId)
                    return SwordCount > 1;

                //var def = DefsFacade.I.Items.Get(SelectedItemID);

                //return def.HasTag(ItemTag.Throwable);

                return _session.QuickInventory.SelectedDef.HasTag(ItemTag.Throwable);
            }
        }

        private bool CanHeal
        {
            get
            {
                if (_session.QuickInventory.Inventory.Length < 1)
                    return false; 

                var def = DefsFacade.I.Items.Get(SelectedItemID);
                return def.HasTag(ItemTag.Potion);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            _defaultGravityScale = RigidbodyComp.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            HealthComp.SetHealth(_session.Data.FullHealth, _session.Data.Hp.Value);

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
            if (id == SwordId)
            {
                UpdateHeroWeapon();
            }
        }

        public void OnHealthChanged(int fullHealth, int currentHealth)
        {
            _session.Data.FullHealth = fullHealth;
            _session.Data.Hp.Value = currentHealth;
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
                DoJumpVfx();

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

            _hitDrop.SetCount(numCoinsToDispose);
            _hitDrop.CalculateDrop();
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
            if (_superThrow)
            {
                var throwableCount = _session.Data.Inventory.Count(SelectedItemID);
                var possibleCount = SelectedItemID == SwordId ? throwableCount - 1 : throwableCount;
                
                var numThrows = Mathf.Min(_superThrowParticles, possibleCount);
                StartCoroutine(DoSuperThrow(numThrows));
            }
            else
            {
                ThrowAndRemoveFromInventory();
            }

            _superThrow = false;
        }

        private IEnumerator DoSuperThrow(int numThrows)
        {
            for (int i = 0; i < numThrows; i++)
            {
                ThrowAndRemoveFromInventory();
                yield return new WaitForSeconds(_superThrowDelay);
            };
        }

        private void ThrowAndRemoveFromInventory()
        {
            if (Sounds != null) Sounds.Play("Range");

            var throwableId = _session.QuickInventory.SelectedItem.Id;
            var throwableDef = DefsFacade.I.Throwable.Get(throwableId);
            _throwSpawner.SetPrefab(throwableDef.Projectile);
            _throwSpawner.Spawn();

            //_particles.Spawn("ThrowSword");
            _session.Data.Inventory.Remove(throwableId, 1);
        }

        public void StartUsing()
        {
            if (CanThrow)
                _superThrowCooldown.Reset();
        }

        public void PerformUsing()
        {
            if (CanThrow)
            {
                if (!_throwCooldown.IsReady) return;

                if (_superThrowCooldown.IsReady) _superThrow = true;

                AnimatorComp.SetTrigger(ThrowKey);
                _throwCooldown.Reset();
            }

            if (CanHeal)
            {
                UsePotion();
            }

        }

        private void UsePotion()
        {
            var potion = DefsFacade.I.Potions.Get(SelectedItemID);

            switch (potion.Effect)
            {
                case Effect.AddHp:
                    HealthComp.ModifyHealth((int)potion.Value);
                    break;
                case Effect.SpeedUp:
                    _speedUpCooldown.Value = potion.Time + _speedUpCooldown.TimeLasts;
                    _additionalSpeed = Mathf.Max(potion.Value, _additionalSpeed);
                    _speedUpCooldown.Reset();
                    break;
            }
            
            AnimatorComp.SetTrigger(HealKey);
            _session.Data.Inventory.Remove(SelectedItemID, 1);
        }

        private Cooldown _speedUpCooldown = new Cooldown();
        private float _additionalSpeed;

        protected override float CalculateSpeed()
        {
            if (_speedUpCooldown.IsReady)
            {
                _additionalSpeed = 0f;
            }

            return base.CalculateSpeed() + _additionalSpeed;
        }

        public void NextItem()
        {
            _session.QuickInventory.SetNextItem();
        }
    }
}
