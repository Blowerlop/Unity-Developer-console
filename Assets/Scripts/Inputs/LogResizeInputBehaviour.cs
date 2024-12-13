using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class LogResizeInputBehaviour : BaseInputBehaviour
    {
        private ConsoleBehaviour _consoleBehaviourInstance => ConsoleBehaviour.instance;
        
        [Header("Parameters")]
        [SerializeField] private Vector2 _fontSizeRange = new(20, 60);
        
        
        protected override void Callback(InputAction.CallbackContext context)
        {
            GameObject currentCurrentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentCurrentSelectedGameObject == _consoleBehaviourInstance.logScrollRect.gameObject ||
                currentCurrentSelectedGameObject == _consoleBehaviourInstance.logInputField.gameObject)
            {
                IncreaseOrDecreaseLogTextSize();
            }
        }
        
        private void IncreaseOrDecreaseLogTextSize()
        {
            _consoleBehaviourInstance.logInputField.pointSize = Mathf.Clamp(_consoleBehaviourInstance.logInputField.pointSize + Input.mouseScrollDelta.y, _fontSizeRange.x,
                _fontSizeRange.y);
        }
    }
}