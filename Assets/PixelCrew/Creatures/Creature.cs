using PixelCrew.Components;
using UnityEngine;

namespace PixelCrew.Creatures
{
    public class Creature : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private bool _invertScale;
        [SerializeField] private float _speed;
        [SerializeField] protected float _jumpSpeed;

        [SerializeField] private float _takeDamageJumpSpeed;
        [SerializeField] protected float _fallDamageSpeed;

        [Header("Checkers")]
        [SerializeField] private LayerCheck _groundCheck;
        [SerializeField] protected LayerMask _groundLayer;

        [SerializeField] private CheckCircleOverlap _attackRange;

        [SerializeField] protected SpawnListComponent _particles;

        protected HealthComponent HealthComp;
        protected Rigidbody2D RigidbodyComp;
        protected Vector2 Direction;
        protected Animator AnimatorComp;
        protected bool IsGrounded;
        private bool _isJumping;
        protected bool HasJustJumpedFlag;
        //private bool _isHeavyFall;

        private static readonly int isGroundKey = Animator.StringToHash("is-ground");
        private static readonly int isRunningKey = Animator.StringToHash("is-running");
        //private static readonly int isHeavyFallKey = Animator.StringToHash("is-heavy-fall");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");

        protected virtual void Awake()
        {
            RigidbodyComp = GetComponent<Rigidbody2D>();
            AnimatorComp = GetComponent<Animator>();
            HealthComp = GetComponent<HealthComponent>();
        }

        public void SetDirection(Vector2 direction)
        {
            Direction = direction;
        }

        protected virtual void Update()
        {
            IsGrounded = _groundCheck.IsTouchingLayer;
        }

        private void FixedUpdate()
        {
            var xVelocity = CalculateXVelocity();
            var yVelocity = CalculateYVelocity();
            RigidbodyComp.velocity = new Vector2(xVelocity, yVelocity);

            //if (!_allowDoubleJump || yVelocity <= -_heavyFallSpeed)
            //{
            //    _isHeavyFall = true;
            //}
            //else if (!_isGrounded)
            //{
            //    _isHeavyFall = false;
            //}

            AnimatorComp.SetBool(isGroundKey, IsGrounded);
            AnimatorComp.SetFloat(VerticalVelocityKey, RigidbodyComp.velocity.y);
            AnimatorComp.SetBool(isRunningKey, Direction.x != 0);
            //_animator.SetBool(isHeavyFallKey, _isHeavyFall);

            UpdateSpriteDirection();

            if (HasJustJumpedFlag)
            {
                HasJustJumpedFlag = false;
                _particles.Spawn("JumpDust");
            }
        }

        protected virtual float CalculateXVelocity()
        {
            return Direction.x * _speed;
        }

        protected virtual float CalculateYVelocity()
        {
            var yVelocity = RigidbodyComp.velocity.y;
            var isJumpingPressing = Direction.y > 0;

            if (IsGrounded)
            {
                _isJumping = false;
            }

            if (isJumpingPressing)
            {
                _isJumping = true;
                var isFalling = RigidbodyComp.velocity.y <= 0.01f;
                yVelocity = isFalling ? CalculateJumpVelocity(yVelocity) : yVelocity;
            }
            else if (RigidbodyComp.velocity.y > 0 && _isJumping)
            {
                yVelocity *= 0.5f;
            }

            return yVelocity;
        }

        protected virtual float CalculateJumpVelocity(float yVelocity)
        {
            if (IsGrounded)
            {
                yVelocity += _jumpSpeed;
                HasJustJumpedFlag = true;
            }

            return yVelocity;
        }

        private void UpdateSpriteDirection()
        {
            var multiplier = _invertScale ? -1 : 1;

            if (Direction.x > 0)
            {
                transform.localScale = new Vector3(multiplier, 1, 1);
            }
            else if (Direction.x < 0)
            {
                transform.localScale = new Vector3(-1 * multiplier, 1, 1);
            }
        }

        public virtual void TakeDamage()
        {
            _isJumping = false;
            AnimatorComp.SetTrigger(HitKey);
            RigidbodyComp.velocity = new Vector2(RigidbodyComp.velocity.x, _takeDamageJumpSpeed);
        }

        public virtual void Attack()
        {
            AnimatorComp.SetTrigger(AttackKey);
        }
        
        public void OnDoAttack()
        {
            _attackRange.Check();
        }
    }
}

