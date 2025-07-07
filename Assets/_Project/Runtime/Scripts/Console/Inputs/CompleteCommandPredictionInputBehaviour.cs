using System;
using UnityEngine.InputSystem;

namespace DeveloperConsole.Inputs
{
    [Serializable]
    public class CompleteCommandPredictionInputBehaviour : BaseInputBehaviour
    {
        private ConsoleCommandPrediction _commandPrediction;


        protected override void OnInit()
        {
            base.OnInit();

            _commandPrediction = consoleBehaviourInstance.GetComponent<ConsoleCommandPrediction>();
            
            if (_commandPrediction == null)
            {
                throw new NullReferenceException(
                    $"Using {this} input behaviour depending on the command prediction without having the ConsoleCommandPrediction component on the ConsoleBehaviour object");
            }
        }

        protected override void Callback(InputAction.CallbackContext context)
        {
            if (!consoleBehaviourInstance.isInputFieldFocus) return;
            if (!_commandPrediction.HasAPrediction()) return;
            
            AutoCompleteTextWithThePrediction();
        }
        
        private void AutoCompleteTextWithThePrediction()
        {
            consoleBehaviourInstance.SetTextOfInputInputField(_commandPrediction.currentPrediction.name);
        }
    }
}