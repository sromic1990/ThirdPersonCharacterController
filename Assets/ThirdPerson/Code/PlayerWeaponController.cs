using UnityEngine;
using UnityEngine.InputSystem;

namespace ThirdPerson.Code
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [SerializeField] private Transform weapon;
        [SerializeField] private Transform hand;
        [SerializeField] private Transform hip;
        
        private readonly int _armedBoolHash = Animator.StringToHash("Armed");
        private readonly int _fireTriggerHash = Animator.StringToHash("Fire");
        
        private Animator _anim;

        private void Awake()
        {
            _anim = GetComponent<Animator>();
        }
        
        public void PickupGun()
        {
            weapon.SetParent(hand);
            weapon.localPosition = new Vector3(0.054f, -0.029f, -0.011f);
            weapon.localRotation = Quaternion.Euler(new Vector3(-20.26f, 76.985f, 125.45f));
            weapon.localScale = Vector3.one;
        }

        public void PutDownGun()
        {
            weapon.SetParent(hip);
            weapon.localPosition = new Vector3(0.0853f, 0.0509f, -0.0775f);
            weapon.localRotation = Quaternion.Euler(new Vector3(94.578f, 187.022f, 172.451f));
            weapon.localScale = Vector3.one;
        }

        public void OnArmed(InputAction.CallbackContext context)
        {
            _anim.SetBool(_armedBoolHash, !_anim.GetBool(_armedBoolHash));
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if ((int)context.ReadValue<float>() == 1 && _anim.GetBool(_armedBoolHash))
            {
                _anim.SetTrigger(_fireTriggerHash);
            }
        }
    }
}