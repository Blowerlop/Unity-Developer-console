using System;
using DeveloperConsole.Extensions;
using DeveloperConsole.Inputs;
using UnityEngine.InputSystem;

namespace DeveloperConsole
{
    [Serializable]
    public class NavigateTroughPredictionInputBehaviour : BaseInputBehaviour
    {
        private ConsoleCommandPrediction _consoleCommandPrediction;
        private ConsoleCommandAdditionalPrediction _consoleCommandAdditionalPrediction;


        protected override void OnInit()
        {
            base.OnInit();
            
            _consoleCommandPrediction = consoleBehaviourInstance.GetComponentInChildren<ConsoleCommandPrediction>();
            _consoleCommandAdditionalPrediction = consoleBehaviourInstance.GetComponentInChildren<ConsoleCommandAdditionalPrediction>();
        }


        protected override void Callback(InputAction.CallbackContext context)
        {
            if (!consoleBehaviourInstance.isInputFieldFocus) return;
            if (!_consoleCommandPrediction.HasAPrediction()) return;

            var index = _consoleCommandAdditionalPrediction.index;
            index += (int)context.ReadValue<float>();
            // ReSharper disable once CompareOfFloatsByEqualityOperator

            if (index < 0) index = _consoleCommandAdditionalPrediction.GetButtonsCount() - 1;
            if (index >= _consoleCommandAdditionalPrediction.GetButtonsCount()) index = 0;
            
            _consoleCommandAdditionalPrediction.SelectButton(index);
        }
    }
}
