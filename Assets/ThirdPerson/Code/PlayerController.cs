using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPerson.Code
{
    public class PlayerController : MonoBehaviour
    {
        #region FIELDS AND MEMBER VARIABLES

        #region SERIALIZED MEMBERS

        [SerializeField] private float moveSpeed = 2.0f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float turnSpeed = 100f;
        [SerializeField] private float groundAcceleration = 5.0f;
        [SerializeField] private float groundDeceleration = 25.0f;
        [SerializeField] private float jumpSpeed = 30000f;
        [SerializeField] private float groundRayDist = 2.0f;

        #endregion

        #region FLOAT MEMBERS

        private Vector2 _moveDirection;
        private float _jumpDirection;
        private float _desiredSpeed;
        private float _forwardSpeed;

        private float _jumpEffort = 0.0f;

        #endregion

        #region UNITY COMPONENT CACHES

        private Animator _anim;
        private Rigidbody _rigidBody;

        #endregion

        #region BOOL MEMBERS

        private bool _onGround = true;
        private bool _readyJump = false;

        #endregion

        #region ANIMATOR HASHES

        private readonly int _forwardSpeedHash = Animator.StringToHash("Speed");
        private readonly int _jumpBoolHash = Animator.StringToHash("ReadyJump");
        private readonly int _launchBoolHash = Animator.StringToHash("Launch");
        private readonly int _landBoolHash = Animator.StringToHash("Land");
        private readonly int _landVelocityHash = Animator.StringToHash("LandingVelocity");
        private readonly int _fallingBoolHash = Animator.StringToHash("Falling");

        #endregion

        #region PROPERTIES

        private bool IsMoveInput => !Mathf.Approximately(_moveDirection.sqrMagnitude, 0f);

        #endregion

        #endregion

        #region METHODS

        #region UNITY EVENT METHODS

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _rigidBody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            Move(_moveDirection);
            Jump(_jumpDirection);
            CheckLanding();
        }

        #endregion

        #region INPUT SYSTEM EVENTS

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            _jumpDirection = context.ReadValue<float>();
        }

        #endregion

        #region PRIVATE METHODS

        private void Move(Vector2 direction)
        {
            float turnAmount = direction.x;
            float fDirection = direction.y;

            if (direction.sqrMagnitude > 1)
            {
                direction.Normalize();
            }

            _desiredSpeed = direction.magnitude * maxSpeed * Mathf.Sign(fDirection);
            float acceleration = IsMoveInput ? groundAcceleration : groundDeceleration;

            _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, _desiredSpeed, acceleration * Time.deltaTime);
            _anim.SetFloat(_forwardSpeedHash, _forwardSpeed);

            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void Jump(float direction)
        {
            if (direction > 0 && _onGround)
            {
                _anim.SetBool(_jumpBoolHash, true);
                _readyJump = true;
                _jumpEffort += Time.deltaTime;
            }
            else if (_readyJump)
            {
                _anim.SetBool(_launchBoolHash, true);
                _readyJump = false;
                _anim.SetBool(_jumpBoolHash, false);
            }
        }

        private void CheckLanding()
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up);
            Debug.DrawRay(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up);
            if (Physics.Raycast(ray, out hit, groundRayDist))
            {
                if (!_onGround)
                {
                    _onGround = true;
                    _anim.SetFloat(_landVelocityHash, _rigidBody.velocity.magnitude);
                    _anim.SetBool(_landBoolHash, true);
                    _anim.SetBool(_fallingBoolHash, false);
                }
            }
            else
            {
                _onGround = false;
                _anim.SetBool(_fallingBoolHash, true);
                _anim.applyRootMotion = false;
            }
        }

        #endregion

        #region ANIMATION DRIVEN CODE

        public void Launch()
        {
            _anim.applyRootMotion = false;
            _rigidBody.AddForce(0, jumpSpeed * Mathf.Clamp(_jumpEffort, 1, 3), 0);
            _anim.SetBool(_launchBoolHash, false);
        }

        public void Land()
        {
            _anim.SetBool(_landBoolHash, false);
            _anim.applyRootMotion = true;
            _anim.SetBool(_launchBoolHash, false);
            _jumpEffort = 0;
        }

        #endregion

        #endregion
    }
}