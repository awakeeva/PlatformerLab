using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] private float _damageJumpSpeed;

    [SerializeField] private LayerCheck _groundCheck;

    private Rigidbody2D _rigidbody;
    private Vector2 _direction;
    private Animator _animator;
    private SpriteRenderer _sprite;
    private bool _isGrounded;
    private bool _allowDoubleJump;

    private static readonly int isGroundKey = Animator.StringToHash("is-ground");
    private static readonly int isRunningKey = Animator.StringToHash("is-running");
    private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
    private static readonly int Hit = Animator.StringToHash("hit");

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
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

        _animator.SetBool(isGroundKey, _isGrounded);
        _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
        _animator.SetBool(isRunningKey, _direction.x != 0);

        UpdateSpriteDirection();
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
        }
        else if (_allowDoubleJump)
        {
            yVelocity = _jumpSpeed;
            _allowDoubleJump = false;
        }

        return yVelocity;
    }

    private void UpdateSpriteDirection()
    {
        if (_direction.x > 0)
        {
            _sprite.flipX = false;
        }
        else if (_direction.x < 0)
        {
            _sprite.flipX = true;
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

    public void TakeDamage()
    {
        _animator.SetTrigger(Hit);
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _damageJumpSpeed);
    }
}
