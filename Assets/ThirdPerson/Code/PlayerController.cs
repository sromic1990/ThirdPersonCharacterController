using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float maxForwardSpeed = 8.0f;
    [SerializeField] private float groundAcceleration = 5.0f;
    [SerializeField] private float groundDeceleration = 25.0f;
    
    private Vector2 _moveDirection;
    private float _desiredForwardSpeed;
    private float _forwardSpeed;

    private Animator _anim;
    private readonly int _forwardSpeedHash = Animator.StringToHash("ForwardSpeed");
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
        if (direction.sqrMagnitude > 1)
        {
            direction.Normalize();
        }

        _desiredForwardSpeed = direction.magnitude * maxForwardSpeed;
        float acceleration = _isMoveInput ? groundAcceleration : groundDeceleration;

        _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredForwardSpeed, acceleration * Time.deltaTime);
        _anim.SetFloat(_forwardSpeedHash, _forwardSpeed);

        // transform.Translate(direction.x * moveSpeed * Time.deltaTime, 0, direction.y * moveSpeed * Time.deltaTime);
    }
}
