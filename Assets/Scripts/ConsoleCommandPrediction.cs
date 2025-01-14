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
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Button _template;

        public string currentPrediction { get; private set; }
        
        
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
            ClearPrediction();

            if (string.IsNullOrEmpty(input)) return;
            
            if (input.Count(' ') > 1) return;
            
            
            ReadOnlySpan<char> commandInput = input.AsSpan();
            
            ReadOnlySpan<char> predictedCommandName = RetrieveCommandNameThatStartWith(commandInput);
            if (predictedCommandName.IsEmpty) return;

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
        }

        private void ClearPrediction()
        {
            currentPrediction = string.Empty;
            _inputFieldPredictionPlaceHolder.text = string.Empty;
            _gameObject.DestroyChildren();
        }

        public bool HasAPrediction() => currentPrediction != string.Empty;
    }
}
