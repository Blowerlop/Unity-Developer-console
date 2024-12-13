using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class NavigateHistoryInputBehaviour : BaseInputBehaviour
    {
        protected override void Callback(InputAction.CallbackContext context)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (context.ReadValue<float>() == -1)
            {
                GoToTheRecentInHistory();
            }
            else
            {
                GoToTheOlderInHistory();
            }
        }
        
        private void GoToTheOlderInHistory()
        {
            if (consoleBehaviourInstance.currentHistoryIndex + 1 >= consoleBehaviourInstance.commandHistory.Count)
            {
                consoleBehaviourInstance.MoveCaretToTheEndOfTheText();
                return;
            }

            consoleBehaviourInstance.currentHistoryIndex++;
            
            consoleBehaviourInstance.SetTextOfInputInputFieldSilent(consoleBehaviourInstance.commandHistory[consoleBehaviourInstance.currentHistoryIndex]);
        }
        
        private void GoToTheRecentInHistory()
        {
            if (consoleBehaviourInstance.currentHistoryIndex <= -1)
            {
                return;
            }
            
            if (consoleBehaviourInstance.currentHistoryIndex <= 0)
            {
                consoleBehaviourInstance.SetTextOfInputInputFieldSilent(string.Empty);
                consoleBehaviourInstance.currentHistoryIndex = -1;
                return;
            }

            consoleBehaviourInstance.currentHistoryIndex--;
            
            consoleBehaviourInstance.SetTextOfInputInputFieldSilent(consoleBehaviourInstance.commandHistory[consoleBehaviourInstance.currentHistoryIndex]);
        }
    }
}