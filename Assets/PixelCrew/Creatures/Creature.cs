using PixelCrew.Components.ColliderBased;
using UnityEngine;
using PixelCrew.Components.GoBased;
using PixelCrew.Components.Health;
using PixelCrew.Components.Audio;

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
        [SerializeField] private ColliderCheck _groundCheck;
        [SerializeField] protected LayerMask _groundLayer;

        [SerializeField] private CheckCircleOverlap _attackRange;

        [SerializeField] protected SpawnListComponent _particles;

        [Header("Ability")]
        [SerializeField] private bool abilityDeadbodyBurst;

        protected HealthComponent HealthComp;
        protected Rigidbody2D RigidbodyComp;
        protected Vector2 Direction;
        protected Animator AnimatorComp;
        protected PlaySoundsComponent Sounds;
        protected bool IsGrounded;
        private bool _isJumping;
        //private bool _isHeavyFall;

        private static readonly int isGroundKey = Animator.StringToHash("is-ground");
        private static readonly int isRunningKey = Animator.StringToHash("is-running");
        //private static readonly int isHeavyFallKey = Animator.StringToHash("is-heavy-fall");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int HitKey = Animator.StringToHash("hit");
        private static readonly int AttackKey = Animator.StringToHash("attack");
        private static readonly int AbilityDeadbodyBurstKey = Animator.StringToHash("ability-deadbody-burst");

        protected virtual void Awake()
        {
            RigidbodyComp = GetComponent<Rigidbody2D>();
            AnimatorComp = GetComponent<Animator>();
            Sounds = GetComponent<PlaySoundsComponent>();
            HealthComp = GetComponent<HealthComponent>();
        }

        private void OnEnable()
        {
            AnimatorComp.SetBool(AbilityDeadbodyBurstKey, abilityDeadbodyBurst);
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

            float rigidBodyVelocityY = (float)System.Math.Round(RigidbodyComp.velocity.y, 2);

            AnimatorComp.SetBool(isGroundKey, IsGrounded);
            AnimatorComp.SetFloat(VerticalVelocityKey, rigidBodyVelocityY);
            AnimatorComp.SetBool(isRunningKey, Direction.x != 0);
            //_animator.SetBool(isHeavyFallKey, _isHeavyFall);

            UpdateSpriteDirection(Direction);
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
                var isFalling = RigidbodyComp.velocity.y <= 0.001f;
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
                yVelocity = _jumpSpeed;
                DoJumpVfx();
            }

            return yVelocity;
        }

        protected void DoJumpVfx()
        {
            _particles.Spawn("JumpDust");
            if (Sounds != null) Sounds.Play("Jump");
        }

        public void UpdateSpriteDirection(Vector2 direction)
        {
            var multiplier = _invertScale ? -1 : 1;

            if (direction.x > 0)
            {
                transform.localScale = new Vector3(multiplier, 1, 1);
            }
            else if (direction.x < 0)
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
            if (Sounds != null) Sounds.Play("Melee");
        }

        public void OnDoAttack()
        {
            _attackRange.Check();
            _particles.Spawn("Slash");
        }
    }
}

