using System;
using System.Globalization;
using System.Linq;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DeveloperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        public struct EventArgs
        {
            public string commandInput;
            public string commandName;
        }
        
        
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        public string currentPrediction { get; private set; }
        
        public Action<EventArgs> onPredictionStart;
        public Action onPredictionEnd;
        
        
        private void OnEnable()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.AddListener(Predict);
        }

        private void OnDisable()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.RemoveListener(Predict);
        }

        private void Predict(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                if (HasAPrediction()) Clear();
                return;
            }

            if (input.Count(' ') > 1)
            {
                if (HasAPrediction()) Clear();
                return;
            }
            
            
            ReadOnlySpan<char> commandInput = input.AsSpan();
            
            ReadOnlySpan<char> predictedCommandName = RetrieveCommandNameThatStartWith(commandInput);
            if (predictedCommandName.IsEmpty)
            {
                if (HasAPrediction()) Clear();
                return;
            }

            PredictCommand(commandInput, predictedCommandName);
        }

        private ReadOnlySpan<char> RetrieveCommandNameThatStartWith(ReadOnlySpan<char> commandInput)
        {
            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                var commandName = ConsoleBehaviour.instance.commandsName[i].AsSpan();
                
                if (commandName.StartsWith(commandInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    return commandName;
                }
            }

            return ReadOnlySpan<char>.Empty;
        }


        private void PredictCommand(ReadOnlySpan<char> commandInput, ReadOnlySpan<char> predictedCommandName)
        {
            int commandInputLength = commandInput.Length;

            string newCommandInput;
            // Correct user input to match the command name
            if (!commandInput.SequenceEqual(predictedCommandName))
            {
                newCommandInput = predictedCommandName[..commandInputLength].ToString();
                ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(newCommandInput);
            }
            else newCommandInput = commandInput.ToString();
            
            string preWriteCommandName = predictedCommandName[..commandInputLength].ToString();
            string nonWriteCommandName = predictedCommandName[commandInputLength..].ToString();

            if (string.IsNullOrEmpty(nonWriteCommandName))
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{newCommandInput}</color>";
            }
            else
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{preWriteCommandName}</color>{nonWriteCommandName}";
            }
            
            currentPrediction = predictedCommandName.ToString();
            
            onPredictionStart?.Invoke(new EventArgs {commandInput = newCommandInput, commandName = currentPrediction});
        }

        private void Clear()
        {
            currentPrediction = string.Empty;
            ClearInputFieldPrediction();
            
            onPredictionEnd?.Invoke();
        }
        
        public void ClearInputFieldPrediction()
        {
            _inputFieldPredictionPlaceHolder.text = string.Empty;
        }

        public bool HasAPrediction() => currentPrediction != string.Empty;
    }
}
