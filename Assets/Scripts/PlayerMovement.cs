using Sirenix.OdinInspector;
using UnityEngine;
using static UserInput.InputType;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private UserInput _input;
    
    private Rigidbody2D _rb;

    [SerializeField] private float _gravity;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _terminalVel;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _jumpForce;

    [ReadOnly, ShowInInspector]
    private Vector2 _velocity;

    private bool _queueJump;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
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
        var centerRay = Physics2D.Raycast(_rb.position, Vector2.down, .6f);
        var leftRay = Physics2D.Raycast(_rb.position + new Vector2(-.48f, 0), Vector2.down, .6f);
        var rightRay = Physics2D.Raycast(_rb.position + new Vector2(.48f, 0), Vector2.down, .6f);
        Debug.DrawRay(_rb.position, Vector2.down);
        Debug.DrawRay(_rb.position + new Vector2(-.48f, 0), Vector2.down);
        Debug.DrawRay(_rb.position + new Vector2(.48f, 0), Vector2.down);
        var grounded = leftRay.collider || rightRay.collider || centerRay.collider;
        
        if (!grounded) 
        {
            var upRay = Physics2D.Raycast(_rb.position, Vector2.up, .6f);
            
            var newVel = Mathf.Max(_velocity.y - _gravity, -_terminalVel);
            if (upRay.collider)
                _velocity.y = -_gravity;
            else
                _velocity.y = newVel;
        }
        
        
        
        if (_velocity.x != 0) 
            _velocity.x = Mathf.Max(_velocity.x - _acceleration, 0) * Time.deltaTime;
        
        var multiplier = 100;
        
        if (_input.Inputs[MoveLeft])
        {
            _velocity.x = Mathf.Clamp(-_acceleration, -_maxSpeed, _maxSpeed) * Time.deltaTime;
        }

        if (_input.Inputs[MoveRight])
        {
            _velocity.x = Mathf.Clamp(_acceleration, -_maxSpeed, _maxSpeed) * Time.deltaTime;
        }

        if (_queueJump)
        {
            _queueJump = false;    
            if (grounded)
            {
                _velocity.y = _jumpForce * Time.deltaTime;
            }
        }
        
        _rb.MovePosition(transform.position + new Vector3(_velocity.x, _velocity.y));
    }
}
