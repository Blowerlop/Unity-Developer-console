using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class ToggleInputBehaviour : BaseInputBehaviour
    {
        protected override void Callback(InputAction.CallbackContext context)
        {
            ToggleConsole();
        }
        
        public void ToggleConsole()
        {
            if (consoleBehaviourInstance.isConsoleEnabled) consoleBehaviourInstance.Hide();
            else consoleBehaviourInstance.Show();
        }
    }
}