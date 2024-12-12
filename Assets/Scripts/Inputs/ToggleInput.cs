using System;
using DevelopperConsole;
using UnityEngine.InputSystem;

namespace Inputs
{
    [Serializable]
    public class ToggleInput : BaseInput
    {
        protected override void Listener(InputAction.CallbackContext context)
        {
            ConsoleBehaviour.instance.ToggleConsole();
        }
    }
}