using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole
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
            if (ConsoleBehaviourInstance.IsConsoleEnabled) Console.Hide();
            else Console.Show();
        }
    }
}