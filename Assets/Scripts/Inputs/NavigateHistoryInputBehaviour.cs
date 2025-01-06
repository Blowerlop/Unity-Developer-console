using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class NavigateHistoryInputBehaviour : BaseInputBehaviour
    {
        private ConsoleCommandPrediction _consoleCommandPrediction;


        protected override void OnInit()
        {
            base.OnInit();
            
            _consoleCommandPrediction = consoleBehaviourInstance.GetComponentInChildren<ConsoleCommandPrediction>();
        }

        protected override void Callback(InputAction.CallbackContext context)
        {
            if (!consoleBehaviourInstance.isInputFieldFocus) return;
            if (_consoleCommandPrediction != null && _consoleCommandPrediction.HasAPrediction()) return;
            
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