using System;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;


namespace DeveloperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        public struct EventArgs
        {
            public string commandInput;
            public ConsoleCommand command;
        }
        
        
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        public ConsoleCommand currentPrediction { get; private set; }
        
        public Action<EventArgs> onPredictionStart;
        public Action<ConsoleCommand> onPredictionComplete;
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

            if (input.Count(' ') >= 1) return;
            
            ReadOnlySpan<char> commandInput = input.AsSpan();
            
            ConsoleCommand predictedCommandName = RetrieveCommandThatStartWith(commandInput);
            if (predictedCommandName == null)
            {
                if (HasAPrediction()) Clear();
                return;
            }

            PredictCommand(commandInput, predictedCommandName);
        }

        private ConsoleCommand RetrieveCommandThatStartWith(ReadOnlySpan<char> commandInput)
        {
            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                var commandName = ConsoleBehaviour.instance.commandsName[i].AsSpan();
                
                if (commandName.StartsWith(commandInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConsoleBehaviour.instance.commands[commandName.ToString()];
                }
            }

            return null;
        }


        private void PredictCommand(ReadOnlySpan<char> commandInput, ConsoleCommand consoleCommand)
        {
            currentPrediction = consoleCommand;
            
            int commandInputLength = commandInput.Length;
            string consoleCommandName = consoleCommand.name;

            string newCommandInput;
            // Correct user input to match the command name
            if (!commandInput.SequenceEqual(consoleCommandName))
            {
                newCommandInput = consoleCommandName[..commandInputLength];
                ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(newCommandInput);
            }
            else newCommandInput = commandInput.ToString();
            
            string preWriteCommandName = consoleCommandName[..commandInputLength];
            string nonWriteCommandName = consoleCommandName[commandInputLength..];

            if (string.IsNullOrEmpty(nonWriteCommandName))
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{newCommandInput}</color>";
            }
            else
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{preWriteCommandName}</color>{nonWriteCommandName}";
            }
            
            if (newCommandInput == consoleCommandName)
            {
                onPredictionComplete?.Invoke(consoleCommand);
            }
            else
            {
                onPredictionStart?.Invoke(new EventArgs {commandInput = newCommandInput, command = currentPrediction});
            }
        }

        private void Clear()
        {
            currentPrediction = null;
            ClearInputFieldPrediction();
            
            onPredictionEnd?.Invoke();
        }
        
        public void ClearInputFieldPrediction()
        {
            _inputFieldPredictionPlaceHolder.text = string.Empty;
        }

        public bool HasAPrediction() => currentPrediction != null;
    }
}
