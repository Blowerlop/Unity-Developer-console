using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public abstract class BaseInputBehaviour
    {
        [SerializeField] private InputAction _inputAction;

        protected ConsoleBehaviour consoleBehaviourInstance => ConsoleBehaviour.instance;
        
        
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
            _inputAction.performed += Callback;
        }
        
        public void UnRegisterListener()
        {
            _inputAction.performed -= Callback;
        }

        protected abstract void Callback(InputAction.CallbackContext context);
    }
}