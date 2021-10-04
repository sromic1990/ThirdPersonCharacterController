using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float groundAcceleration = 5.0f;
    [SerializeField] private float groundDeceleration = 25.0f;
    [SerializeField] private float jumpSpeed = 30000f;
    
    private Vector2 _moveDirection;
    private float _jumpDirection; 
    private float _desiredSpeed;
    private float _forwardSpeed;
    
    private float _groundRayDist = 2f;

    private Animator _anim;
    private Rigidbody _rigidBody;

    private bool _onGround = true;
    private bool _readyJump = false;

    private readonly int _forwardSpeedHash = Animator.StringToHash("Speed");
    private readonly int _jumpBoolHash = Animator.StringToHash("ReadyJump");
    private readonly int _launchBoolHash = Animator.StringToHash("Launch");
    private readonly int _landBoolHash = Animator.StringToHash("Land");
    private bool _isMoveInput => !Mathf.Approximately(_moveDirection.sqrMagnitude, 0f);

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        
    }

    private void Update()
    {
        Move(_moveDirection);
        Jump(_jumpDirection);
        CheckLanding();
    }

    private void CheckLanding()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * _groundRayDist * 0.5f, -Vector3.up);
        if (Physics.Raycast(ray, out hit, _groundRayDist))
        {
            if (!_onGround)
            {
                _onGround = true;
                _anim.SetBool(_landBoolHash, true);
            }
        }
        else
        {
            _onGround = false;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        _jumpDirection = context.ReadValue<float>();
    }

    private void Move(Vector2 direction)
    {
        float turnAmount = direction.x;
        float fDirection = direction.y;
        
        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        _desiredSpeed = direction.magnitude * maxSpeed * Mathf.Sign(fDirection);
        float acceleration = _isMoveInput ? groundAcceleration : groundDeceleration;

        _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredSpeed, acceleration * Time.deltaTime);
        _anim.SetFloat(_forwardSpeedHash, _forwardSpeed);
        
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }
    
    private void Jump(float direction)
    {
        // Debug.Log($"jump = {direction}");
        if (direction > 0 && _onGround)
        {
            _anim.SetBool(_jumpBoolHash, true);
            _readyJump = true;
        }
        else if (_readyJump)
        {
            _anim.SetBool(_launchBoolHash, true);
            _readyJump = false;
            _anim.SetBool(_jumpBoolHash, false);
        }
    }

    public void Launch()
    {
        _anim.applyRootMotion = false;
        _rigidBody.AddForce(0, jumpSpeed, 0);
        _anim.SetBool(_launchBoolHash, false);
    }

    public void Land()
    {
        _anim.SetBool(_landBoolHash, false);
        _anim.applyRootMotion = true;
        _anim.SetBool(_launchBoolHash, false);
    }
}
