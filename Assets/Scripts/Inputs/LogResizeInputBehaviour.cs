using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class LogResizeInputBehaviour : BaseInputBehaviour
    {
        [Header("Parameters")]
        [SerializeField] private Vector2 _fontSizeRange = new(20, 60);
        
        
        protected override void Callback(InputAction.CallbackContext context)
        {
            GameObject currentCurrentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            if (currentCurrentSelectedGameObject == consoleBehaviourInstance.logScrollRect.gameObject ||
                currentCurrentSelectedGameObject == consoleBehaviourInstance.logInputField.gameObject)
            {
                IncreaseOrDecreaseLogTextSize();
            }
        }
        
        private void IncreaseOrDecreaseLogTextSize()
        {
            consoleBehaviourInstance.logInputField.pointSize = Mathf.Clamp(consoleBehaviourInstance.logInputField.pointSize + Input.mouseScrollDelta.y, _fontSizeRange.x,
                _fontSizeRange.y);
        }
    }
}