using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using static UserInput.InputType;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private UserInput _input;
    
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _renderer;
    
    [SerializeField] private float _initialGravity;
    [SerializeField] private float _initialTerminalVel;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _wallJumpForce;
    [SerializeField] private float _wallJumpHorizontal;

    [ReadOnly, ShowInInspector]
    private Vector2 _velocity;

    private bool _queueJump;
    private bool _grounded;
    private bool _wallSliding;
    private bool _direction; // true = right, false = left
    private bool _noLeft, _noRight;
    private bool _canLeft, _canRight;
    private bool _wallJumping;

    [ReadOnly, ShowInInspector]
    private float _gravity;
    [ReadOnly, ShowInInspector]
    private float _terminalVel;

    private static readonly int Walking = Animator.StringToHash("walking");

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
        _gravity = _initialGravity;
        _terminalVel = _initialTerminalVel;
    }

    private void Update()
    {
        if (_input.Inputs[Jump])
        {
            _queueJump = true;
        }
    }
    
    private void FixedUpdate()
    {
        var centerDownRay = Physics2D.Raycast(_rb.position, Vector2.down, .6f);
        var leftDownRay = Physics2D.Raycast(_rb.position + new Vector2(-.48f, 0), Vector2.down, .6f);
        var rightDownRay = Physics2D.Raycast(_rb.position + new Vector2(.48f, 0), Vector2.down, .6f);
        
        _grounded = leftDownRay.collider || rightDownRay.collider || centerDownRay.collider;

        if (!_grounded) 
        {
            var upRay = Physics2D.Raycast(_rb.position, Vector2.up, .6f);
            var leftUpRay = Physics2D.Raycast(_rb.position + new Vector2(-.48f, 0), Vector2.up, .6f);
            var rightUpRay = Physics2D.Raycast(_rb.position + new Vector2(.48f, 0), Vector2.up, .6f);

            if (_velocity.y < 0)
            {
                var centerSideRay = Physics2D.Raycast(_rb.position, _direction ? Vector2.right : Vector2.left, .6f);
                var topSideRay = Physics2D.Raycast(_rb.position + new Vector2(0, .48f), _direction ? Vector2.right : Vector2.left, .6f);
                var bottomSideRay = Physics2D.Raycast(_rb.position + new Vector2(0, -.48f), _direction ? Vector2.right : Vector2.left, .6f);
                
                if ((centerSideRay.collider || topSideRay.collider || bottomSideRay.collider) && (_direction ? _input.Inputs[MoveRight] : _input.Inputs[MoveLeft]))
                {
                    _terminalVel = _initialTerminalVel / 8;
                    _wallSliding = true;
                }
                else
                {
                    _terminalVel = _initialTerminalVel;
                    _wallSliding = false;
                }
            }
            else
            {
                _terminalVel = _initialTerminalVel;
                _wallSliding = false;
            }

            if (upRay.collider || leftUpRay.collider || rightUpRay.collider)
                _velocity.y = -_gravity * Time.deltaTime;
            else
            {
                _velocity.y += (_velocity.y - _gravity) * Time.deltaTime;
            }
        }
        else
        {
            _wallJumping = false;
            _velocity.y = -_gravity * Time.deltaTime / 2;
        }

        if (_noLeft)
            _canLeft = true;
        if (_noRight)
            _canRight = true;
        
        _animator.SetBool(Walking, false);
        
        if (_wallJumping && _canLeft && _input.Inputs[MoveLeft]
            || !_wallJumping && _input.Inputs[MoveLeft])
        {
            _noLeft = false;
            _wallJumping = false;
            _direction = false;
            _velocity.x = Mathf.Clamp(_velocity.x + _acceleration * Time.deltaTime, -_maxSpeed * Time.deltaTime, _maxSpeed * Time.deltaTime);
            
            if (_grounded) 
                _animator.SetBool(Walking, true);
        }

        if (_wallJumping && _canRight && _input.Inputs[MoveRight]
            || !_wallJumping && _input.Inputs[MoveRight])
        {
            _noRight = false;
            _wallJumping = false;
            _direction = true;
            _velocity.x = Mathf.Clamp(_velocity.x + _acceleration * Time.deltaTime, -_maxSpeed * Time.deltaTime, _maxSpeed * Time.deltaTime);
            
            if (_grounded) 
                _animator.SetBool(Walking, true);
        }

        if (_wallJumping)
        {
            var holdingOut = _direction ? _input.Inputs[MoveRight] : _input.Inputs[MoveLeft];
            if (holdingOut) 
                _velocity.x =_wallJumpHorizontal * Time.deltaTime;
        }
        
        if (_queueJump)
        {
            _queueJump = false;    
            if (_grounded || _wallSliding)
            {
                if (_wallSliding)
                {
                    _terminalVel = _initialTerminalVel;
                    var holdingIn = _direction ? _input.Inputs[MoveRight] : _input.Inputs[MoveLeft];
                    _velocity.x = (holdingIn ? _wallJumpHorizontal / 2 : _wallJumpHorizontal) * Time.deltaTime;
                    _velocity.y = _wallJumpForce * Time.deltaTime;
                    _direction = !_direction;
                    _wallJumping = true;
                }
                else
                {
                    _velocity.y = _jumpForce * Time.deltaTime;
                }
            }
        }
        
        //deceleration
        if (!_wallJumping)
        {
            if (_velocity.x != 0)
                _velocity.x = Mathf.Max(_velocity.x - _deceleration * Time.deltaTime, 0);
        }

        if (_velocity.y < -_terminalVel) _velocity.y = -_terminalVel;
        if (_velocity.y > _terminalVel) _velocity.y = _terminalVel;
        
        float dirVel;
        if (_direction)
        {
            dirVel = _velocity.x;
            _renderer.flipX = false;
        }
        else
        {
            dirVel = -_velocity.x;
            _renderer.flipX = true;
        }

        _rb.MovePosition(transform.position + new Vector3(dirVel, _velocity.y) * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        Gizmos.DrawRay(_rb.position, Vector2.down);
        Gizmos.DrawRay(_rb.position + new Vector2(-.48f, 0), Vector2.down);
        Gizmos.DrawRay(_rb.position + new Vector2(.48f, 0), Vector2.down);

        if (!_grounded)
        {
            Debug.DrawRay(_rb.position, Vector2.up);
            Debug.DrawRay(_rb.position + new Vector2(-.48f, 0), Vector2.up);
            Debug.DrawRay(_rb.position + new Vector2(.48f, 0), Vector2.up);

            Debug.DrawRay(_rb.position, _direction ? Vector2.right : Vector2.left);
            Debug.DrawRay(_rb.position + new Vector2(0, .48f), _direction ? Vector2.right : Vector2.left);
            Debug.DrawRay(_rb.position + new Vector2(0, -.48f), _direction ? Vector2.right : Vector2.left);
        }
    }
}
