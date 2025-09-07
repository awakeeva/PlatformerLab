using UnityEngine;
using PixelCrew.Components;

namespace PixelCrew
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpSpeed;
        [SerializeField] private float _damageJumpSpeed;
        [SerializeField] private float _heavyFallSpeed;

        [SerializeField] private LayerCheck _groundCheck;

        [SerializeField] private float _interactionRadius;
        [SerializeField] private LayerMask _interactionLayer;

        [SerializeField] private SpawnComponent _footStepParticles;
        [SerializeField] private SpawnComponent _jumpDustParticles;
        [SerializeField] private SpawnComponent _fallDustParticles;
        [SerializeField] private ParticleSystem _hitParticles;

        private Collider2D[] _interactionResult = new Collider2D[1];
        private Rigidbody2D _rigidbody;
        private Vector2 _direction;
        private Animator _animator;
        private bool _isGrounded;
        private bool _allowDoubleJump;
        private bool _hasJustJumpedFlag;
        private bool _isHeavyFall;

        private static readonly int isGroundKey = Animator.StringToHash("is-ground");
        private static readonly int isRunningKey = Animator.StringToHash("is-running");
        private static readonly int isHeavyFallKey = Animator.StringToHash("is-heavy-fall");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");

        private const int SilverCoinCost = 1;
        private const int GoldCoinCost = 10;
        private int _silverCoinCount = 0;
        private int _goldCoinCount = 0;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }

        private void Update()
        {
            _isGrounded = IsGrounded();
        }

        private void FixedUpdate()
        {
            var xVelocity = _direction.x * _speed;
            var yVelocity = CalculateYVelocity();
            _rigidbody.velocity = new Vector2(xVelocity, yVelocity);

            if (!_allowDoubleJump || yVelocity <= -_heavyFallSpeed)
            {
                _isHeavyFall = true;
            }
            else if (!_isGrounded)
            {
                _isHeavyFall = false;
            }

            _animator.SetBool(isGroundKey, _isGrounded);
            _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
            _animator.SetBool(isRunningKey, _direction.x != 0);
            _animator.SetBool(isHeavyFallKey, _isHeavyFall);

            UpdateSpriteDirection();

            if (_hasJustJumpedFlag)
            {
                _hasJustJumpedFlag = false;
                SpawnJumpDust();
            }
        }

        private float CalculateYVelocity()
        {
            var yVelocity = _rigidbody.velocity.y;
            var isJumpingPressing = _direction.y > 0;

            if (_isGrounded) _allowDoubleJump = true;

            if (isJumpingPressing)
            {
                yVelocity = CalculateJumpVelocity(yVelocity);
            }
            else if (_rigidbody.velocity.y > 0)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }

        private float CalculateJumpVelocity(float yVelocity)
        {
            var isFalling = _rigidbody.velocity.y <= 0.001f;
            if (!isFalling) return yVelocity;

            if (_isGrounded)
            {
                yVelocity += _jumpSpeed;
                _hasJustJumpedFlag = true;
            }
            else if (_allowDoubleJump)
            {
                yVelocity = _jumpSpeed;
                _allowDoubleJump = false;
                _hasJustJumpedFlag = true;
            }

            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
            {
                transform.localScale = Vector3.one;
            }
            else if (_direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        private bool IsGrounded()
        {
            return _groundCheck.IsTouchingLayer;
        }

        public void SaySomething()
        {
            Debug.Log("Something!");
        }

        public void AddSilverCoin(int coins)
        {
            _silverCoinCount += coins;
            LogCoins(coins > 0 ? $"[ADD +{coins}] " : $"[DEL {coins}]");
        }

        public void AddGoldCoin(int coins)
        {
            _goldCoinCount += coins;
            LogCoins();
        }

        private void LogCoins(string metka = "[ADD]")
        {
            var total = _silverCoinCount * SilverCoinCost + _goldCoinCount * GoldCoinCost;
            Debug.Log($"{metka} SilverCount ={_silverCoinCount} GoldCount ={_goldCoinCount} TotalMoney ={total}");
        }

        public void TakeDamage()
        {
            _animator.SetTrigger(HitKey);
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damageJumpSpeed);

            if (_silverCoinCount > 0)
            {
                SpawnCoins();
            }
        }

        public void SpawnCoins()
        {
            var numCoinsToDispose = Mathf.Min(_silverCoinCount, 5);
            AddSilverCoin(-numCoinsToDispose);

            var burst = _hitParticles.emission.GetBurst(0);
            burst.count = numCoinsToDispose;
            _hitParticles.emission.SetBurst(0, burst);

            _hitParticles.gameObject.SetActive(true);
            _hitParticles.Play();
        }

        public void Interact()
        {
            var size = Physics2D.OverlapCircleNonAlloc(
                transform.position,
                _interactionRadius,
                _interactionResult,
                _interactionLayer);

            for (int i = 0; i < size; i++)
            {
                var interactable =_interactionResult[i].GetComponent<InteractableComponent>();

                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }

        public void SpawnFootDust()
        {
            _footStepParticles.Spawn();
        }

        public void SpawnJumpDust()
        {
            _jumpDustParticles.Spawn();
        }
        public void SpawnFallDust()
        {
            _fallDustParticles.Spawn();
        }

    }
}
