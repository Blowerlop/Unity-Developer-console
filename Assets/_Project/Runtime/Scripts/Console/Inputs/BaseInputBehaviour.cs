using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DeveloperConsole
{
    [Serializable]
    public abstract class BaseInputBehaviour
    {
        #region Variables

        [SerializeField] private InputAction _inputAction;
        protected ConsoleBehaviour ConsoleBehaviourInstance => ConsoleBehaviour.instance;

        #endregion


        #region Core Behaviours

        public void Enable()
        {
            _inputAction.Enable();
        }

        public virtual void OnEnable()
        {
        }
        
        public void Disable()
        {
            _inputAction.Disable();
        }
        
        public virtual void OnDisable()
        {
        }

        #endregion

        
        #region Methods

        public void RegisterListener()
        {
            _inputAction.performed += Callback;
        }
        
        public void UnRegisterListener()
        {
            _inputAction.performed -= Callback;
        }

        public void Init()
        {
            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        protected abstract void Callback(InputAction.CallbackContext context);

        #endregion
    }
}