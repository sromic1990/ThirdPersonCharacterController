using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float turnSpeed = 100f;
    [SerializeField] private float groundAcceleration = 5.0f;
    [SerializeField] private float groundDeceleration = 25.0f;
    
    private Vector2 _moveDirection;
    private float _desiredSpeed;
    private float _forwardSpeed;

    private Animator _anim;
    private readonly int _forwardSpeedHash = Animator.StringToHash("Speed");
    private bool _isMoveInput => !Mathf.Approximately(_moveDirection.sqrMagnitude, 0f);

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    
    private void Start()
    {
        
    }
    
    private void Update()
    {
        Move(_moveDirection);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
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
}
