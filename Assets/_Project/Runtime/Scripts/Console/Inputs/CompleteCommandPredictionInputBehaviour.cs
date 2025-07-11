using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole
{
    [Serializable]
    public class CompleteCommandPredictionInputBehaviour : BaseInputBehaviour
    {
        #region Variables

        private ConsoleCommandPrediction _commandPrediction;

        #endregion


        #region Methods

        protected override void OnInit()
        {
            base.OnInit();

            _commandPrediction = ConsoleBehaviourInstance.GetComponent<ConsoleCommandPrediction>();
            
            if (_commandPrediction == null)
            {
                throw new NullReferenceException(
                    $"Using {this} input behaviour depending on the command prediction without having the ConsoleCommandPrediction component on the ConsoleBehaviour object");
            }
        }

        protected override void Callback(InputAction.CallbackContext context)
        {
            if (!ConsoleBehaviourInstance.IsInputFieldFocus) return;
            if (!_commandPrediction.HasAPrediction()) return;
            
            AutoCompleteTextWithThePrediction();
        }
        
        private void AutoCompleteTextWithThePrediction()
        {
            ConsoleBehaviourInstance.SetTextOfInputInputField(_commandPrediction.CurrentPrediction.Name);
        }

        #endregion
    }
}