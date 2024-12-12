using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    [Serializable]
    public abstract class BaseInput
    {
        [SerializeField] private InputAction _inputAction;
        
        
        public void Enable()
        {
            _inputAction.Enable();
        }
        
        public void Disable()
        {
            _inputAction.Disable();
        }
        
        public void RegisterListener()
        {
            _inputAction.performed += Listener;
        }
        
        public void UnRegisterListener()
        {
            _inputAction.performed -= Listener;
        }

        protected abstract void Listener(InputAction.CallbackContext context);
    }
}