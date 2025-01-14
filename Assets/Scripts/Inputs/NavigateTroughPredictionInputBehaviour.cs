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
        

        protected override void OnInit()
        {
            base.OnInit();
            
            _consoleCommandPrediction = consoleBehaviourInstance.GetComponentInChildren<ConsoleCommandPrediction>();
        }


        protected override void Callback(InputAction.CallbackContext context)
        {
            // if (!consoleBehaviourInstance.isInputFieldFocus) return;
            // if (!_consoleCommandPrediction.HasAPrediction()) return;
            //
            // string predictionName;
            // var index = (int)_consoleCommandPrediction.index;
            //
            // // ReSharper disable once CompareOfFloatsByEqualityOperator
            // if (context.ReadValue<float>() == -1)
            // {
            //     predictionName = _consoleCommandPrediction.GetPredictionsName().Previous(ref index);
            // }
            // else
            // {
            //     predictionName = _consoleCommandPrediction.GetPredictionsName().Next(ref index);
            // }
            //
            // _consoleCommandPrediction.PredictCommand(predictionName, (uint)index);
        }
    }
}
