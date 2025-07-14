using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DeveloperConsole
{
    [Serializable]
    public class LogResizeInputBehaviour : BaseInputBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private Vector2 _fontSizeRange = new(20, 60);
        
        
        protected override void Callback(InputAction.CallbackContext context)
        {
            GameObject currentCurrentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentCurrentSelectedGameObject == ConsoleBehaviourInstance.logScrollRect.gameObject ||
                currentCurrentSelectedGameObject == ConsoleBehaviourInstance.logInputField.gameObject)
            {
                IncreaseOrDecreaseLogTextSize();
            }
        }
        
        private void IncreaseOrDecreaseLogTextSize()
        {
            ConsoleBehaviourInstance.logInputField.pointSize = Mathf.Clamp(ConsoleBehaviourInstance.logInputField.pointSize + Input.mouseScrollDelta.y, _fontSizeRange.x,
                _fontSizeRange.y);
        }
    }
}