using System;
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
            
            _consoleCommandPrediction = ConsoleBehaviourInstance.GetComponentInChildren<ConsoleCommandPrediction>();
            _consoleCommandAdditionalPrediction = ConsoleBehaviourInstance.GetComponentInChildren<ConsoleCommandAdditionalPrediction>();
        }


        protected override void Callback(InputAction.CallbackContext context)
        {
            if (!ConsoleBehaviourInstance.IsInputFieldFocus) return;
            if (!_consoleCommandPrediction.HasAPrediction()) return;

            var index = _consoleCommandAdditionalPrediction.Index;
            index += (int)context.ReadValue<float>();
            // ReSharper disable once CompareOfFloatsByEqualityOperator

            if (index < 0) index = _consoleCommandAdditionalPrediction.GetButtonsCount() - 1;
            if (index >= _consoleCommandAdditionalPrediction.GetButtonsCount()) index = 0;
            
            _consoleCommandAdditionalPrediction.SelectButton(index);
        }
    }
}
