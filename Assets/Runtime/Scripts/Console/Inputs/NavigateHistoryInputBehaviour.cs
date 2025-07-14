using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole
{
    [Serializable]
    public class NavigateHistoryInputBehaviour : BaseInputBehaviour
    {
        #region Variables

        private ConsoleCommandPrediction _consoleCommandPrediction;

        #endregion


        #region Methods

        protected override void OnInit()
        {
            base.OnInit();
            
            _consoleCommandPrediction = ConsoleBehaviourInstance.GetComponentInChildren<ConsoleCommandPrediction>();
        }

        protected override void Callback(InputAction.CallbackContext context)
        {
            if (!ConsoleBehaviourInstance.IsInputFieldFocus) return;
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
            if (ConsoleBehaviourInstance.currentHistoryIndex + 1 >= ConsoleBehaviourInstance.CommandHistory.Count)
            {
                ConsoleBehaviourInstance.MoveCaretToTheEndOfTheText();
                return;
            }

            ConsoleBehaviourInstance.currentHistoryIndex++;
            
            ConsoleBehaviourInstance.SetTextOfInputInputFieldSilent(ConsoleBehaviourInstance.CommandHistory[ConsoleBehaviourInstance.currentHistoryIndex]);
        }
        
        private void GoToTheRecentInHistory()
        {
            if (ConsoleBehaviourInstance.currentHistoryIndex <= -1)
            {
                return;
            }
            
            if (ConsoleBehaviourInstance.currentHistoryIndex <= 0)
            {
                ConsoleBehaviourInstance.SetTextOfInputInputFieldSilent(string.Empty);
                ConsoleBehaviourInstance.currentHistoryIndex = -1;
                return;
            }

            ConsoleBehaviourInstance.currentHistoryIndex--;
            
            ConsoleBehaviourInstance.SetTextOfInputInputFieldSilent(ConsoleBehaviourInstance.CommandHistory[ConsoleBehaviourInstance.currentHistoryIndex]);
        }

        #endregion
    }
}