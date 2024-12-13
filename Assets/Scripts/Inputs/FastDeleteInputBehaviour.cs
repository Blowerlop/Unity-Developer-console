using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class FastDeleteInputBehaviour : BaseInputBehaviour
    {
        protected override void Callback(InputAction.CallbackContext context)
        {
            int startWordPosition = 0;
            for (int i = consoleBehaviourInstance.inputInputField.caretPosition - 1; i >= 0; i--)
            { 
                if (consoleBehaviourInstance.inputInputField.text[i] == ' ')
                {
                    startWordPosition = i;
                    break;
                }
            }
            
            consoleBehaviourInstance.SetTextOfInputInputField(consoleBehaviourInstance.inputInputField.text.Remove(
                startWordPosition, consoleBehaviourInstance.inputInputField.caretPosition - startWordPosition));
        }
    }   
}