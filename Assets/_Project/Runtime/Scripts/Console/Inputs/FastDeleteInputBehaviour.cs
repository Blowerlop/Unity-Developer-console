using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole
{
    [Serializable]
    public class FastDeleteInputBehaviour : BaseInputBehaviour
    {
        protected override void Callback(InputAction.CallbackContext context)
        {
            int startWordPosition = 0;
            for (int i = ConsoleBehaviourInstance.inputInputField.caretPosition - 1; i >= 0; i--)
            { 
                if (ConsoleBehaviourInstance.inputInputField.text[i] == ' ')
                {
                    startWordPosition = i;
                    break;
                }
            }
            
            ConsoleBehaviourInstance.SetTextOfInputInputField(ConsoleBehaviourInstance.inputInputField.text.Remove(
                startWordPosition, ConsoleBehaviourInstance.inputInputField.caretPosition - startWordPosition));
        }
    }   
}