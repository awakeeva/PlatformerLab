using UnityEngine;
using UnityEngine.Animations;
using PixelCrew.Components;
using PixelCrew.Utils;
using PixelCrew.Model;

namespace PixelCrew.Creatures
{
    public class Hero : Creature
    {
        [Header("HERO")]
        [SerializeField] private float _dashSpeed;
        [SerializeField] private float _dashDuration;

        [SerializeField] private float _slamDownVelocity;
        [SerializeField] private float _heavyFallSpeed;
        
        [SerializeField] private CheckCircleOverlap _interactionCheck;

        [SerializeField] private LayerCheck _wallCheck;

        [SerializeField] private UnityEditor.Animations.AnimatorController _armed;
        [SerializeField] private UnityEditor.Animations.AnimatorController _disarmed;


        [Space] [Header("Particles")]
        [SerializeField] private ParticleSystem _hitParticles;

        private bool _allowDoubleJump;

        private bool _isOnWall;

        private bool _isDashOn;
        private float _dashTimer;

        [Header("Sword Effects")]
        [SerializeField] private GameObject _StabbingBlowEffect;

        private const int SilverCoinCost = 1;
        private const int GoldCoinCost = 10;

        private GameSession _session;
        private float _defaultGravityScale;

        protected override void Awake()
        {
            base.Awake();
            _defaultGravityScale = RigidbodyComp.gravityScale;
        }

        private void Start()
        {
            _session = FindObjectOfType<GameSession>();

            HealthComp.SetHealth(_session.Data.FullHealth, _session.Data.Health);

            UpdateHeroWeapon();
        }

        public void OnHealthChanged(int fullHealth, int currentHealth)
        {
            _session.Data.FullHealth = fullHealth;
            _session.Data.Health = currentHealth;
        }

        protected override void Update()
        {
            base.Update();

            if (_wallCheck.IsTouchingLayer && Direction.x == transform.localScale.x)
            {
                _isOnWall = true;
                RigidbodyComp.gravityScale = 0;
            }
            else
            {
                _isOnWall = false;
                RigidbodyComp.gravityScale = _defaultGravityScale;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.IsLayer(_groundLayer))
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
            if (!IsGrounded && _allowDoubleJump)
            {
                _allowDoubleJump = false;
                HasJustJumpedFlag = true;

                return _jumpSpeed;
            }

            return base.CalculateJumpVelocity(yVelocity);
        }

        public void AddSilverCoin(int coins)
        {
            _session.Data.SilverCoinCount += coins;
            LogCoins(coins > 0 ? $"[ADD +{coins}] " : $"[DEL {coins}]");
        }

        public void AddGoldCoin(int coins)
        {
            _session.Data.GoldCoinCount += coins;
            LogCoins();
        }

        private void LogCoins(string metka = "[ADD]")
        {
            var total = _session.Data.SilverCoinCount * SilverCoinCost + _session.Data.GoldCoinCount * GoldCoinCost;
            Debug.Log($"{metka} SilverCount ={_session.Data.SilverCoinCount} GoldCount ={_session.Data.GoldCoinCount} TotalMoney ={total}");
        }

        public override void TakeDamage()
        {
            base.TakeDamage();

            if (_session.Data.SilverCoinCount > 0)
            {
                SpawnCoins();
            }
        }

        public void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_session.Data.SilverCoinCount, 5);
            AddSilverCoin(-numCoinsToDispose);

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
            if (!_session.Data.isArmed) return;

            base.Attack();
        }

        public void ArmHero()
        {
            _session.Data.isArmed = true;
            UpdateHeroWeapon();
        }

        private void UpdateHeroWeapon()
        {
            AnimatorComp.runtimeAnimatorController =
                _session.Data.isArmed ? _armed : _disarmed;
        }

        public void SpawnStabbingBlowEffect()
        {
            _StabbingBlowEffect.SetActive(true);
        }

    }
}
