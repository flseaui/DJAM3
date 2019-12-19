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

    [SerializeField] private float _initialGravity;
    [SerializeField] private float _initialTerminalVel;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _wallJumpHorizontal;

    [ReadOnly, ShowInInspector]
    private Vector2 _velocity;

    private bool _queueJump;
    private bool _grounded;
    private bool _wallSliding;
    private bool _direction; // true = right, false = left

    [ReadOnly, ShowInInspector]
    private float _gravity;
    [ReadOnly, ShowInInspector]
    private float _terminalVel;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
                var sideRay = Physics2D.Raycast(_rb.position, _direction ? Vector2.right : Vector2.left, .6f);
                if (sideRay.collider && (_direction ? _input.Inputs[MoveRight] : _input.Inputs[MoveLeft]))
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
        
        if (_input.Inputs[MoveLeft])
        {
            _direction = false;
            _velocity.x = Mathf.Clamp(_acceleration, -_maxSpeed, _maxSpeed) * Time.deltaTime;
        }

        if (_input.Inputs[MoveRight])
        {
            _direction = true;
            _velocity.x = Mathf.Clamp(_acceleration, -_maxSpeed, _maxSpeed) * Time.deltaTime;
        }

        if (_queueJump)
        {
            _queueJump = false;    
            if (_grounded || _wallSliding)
            {
                if (_wallSliding)
                {
                    _terminalVel = _initialTerminalVel;
                    _velocity.x = _wallJumpHorizontal * Time.deltaTime;
                    _direction = !_direction;
                }
                _velocity.y = _jumpForce * Time.deltaTime;
            }
        }
        
        //deceleration
        if (_velocity.x != 0)
            _velocity.x = Mathf.Max(_velocity.x - _deceleration * Time.deltaTime, 0);
        
        if (_velocity.y < -_terminalVel) _velocity.y = -_terminalVel;
        if (_velocity.y > _terminalVel) _velocity.y = _terminalVel;
        
        var dirVel = _direction ? _velocity.x : -_velocity.x;

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
        }
    }
}
