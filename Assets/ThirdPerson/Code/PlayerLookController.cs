using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPerson.Code
{
    public class PlayerLookController : MonoBehaviour
    {
        [SerializeField] private Transform spine;
        [SerializeField] private float xSensitivity = 0.5f;
        [SerializeField] private float ySensitivity = 0.5f;
        [SerializeField] private Vector2 xRotation = new Vector2(-30, 30);
        [SerializeField] private Vector2 yRotation = new Vector2(-30, 30);
        private Vector2 _lookDirection;
        private Vector2 _lastLookDir;

        private void Start()
        {
            _lastLookDir = Vector3.zero;
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            _lookDirection = context.ReadValue<Vector2>();
        }

        private void LateUpdate()
        {
            _lastLookDir += new Vector2( -_lookDirection.y * ySensitivity,_lookDirection.x * xSensitivity);
            _lastLookDir.x = Mathf.Clamp(_lastLookDir.x, yRotation.x, yRotation.y);
            _lastLookDir.y = Mathf.Clamp(_lastLookDir.y, xRotation.x, xRotation.y);

            spine.localEulerAngles = _lastLookDir;

            // spine.Rotate(-_lastLookDir.y, _lastLookDir.x, 0);
        }
    }
}