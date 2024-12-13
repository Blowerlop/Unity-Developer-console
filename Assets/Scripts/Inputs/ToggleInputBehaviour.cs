using System;
using DevelopperConsole;
using UnityEngine.InputSystem;

namespace Inputs
{
    [Serializable]
    public class ToggleInputBehaviour : BaseInputBehaviour
    {
        protected override void Callback(InputAction.CallbackContext context)
        {
            ConsoleBehaviour.instance.ToggleConsole();
        }
    }
}