using System;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;


namespace DeveloperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        #region Custom Types

        public struct EventArgs
        {
            public string commandInput;
            public ConsoleCommand command;
        }

        #endregion


        #region Variables

        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        public ConsoleCommand CurrentPrediction { get; private set; }
        
        public Action<EventArgs> onPredictionStart;
        public Action<ConsoleCommand> onPredictionComplete;
        public Action onPredictionEnd;

        #endregion


        #region Core Behaviours

        private void OnEnable()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.AddListener(Predict);
        }

        private void OnDisable()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.RemoveListener(Predict);
        }

        #endregion
        

        #region Methods

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
            for (int i = 0; i < ConsoleBehaviour.instance.CommandsName.Count; i++)
            {
                var commandName = ConsoleBehaviour.instance.CommandsName[i];
                var commandNameSpan = commandName.AsSpan();
                
                if (commandNameSpan.StartsWith(commandInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConsoleBehaviour.instance.commands[commandName];
                }
            }

            return null;
        }


        private void PredictCommand(ReadOnlySpan<char> commandInput, ConsoleCommand consoleCommand)
        {
            CurrentPrediction = consoleCommand;
            
            int commandInputLength = commandInput.Length;
            string consoleCommandName = consoleCommand.Name;

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
                onPredictionStart?.Invoke(new EventArgs {commandInput = newCommandInput, command = CurrentPrediction});
            }
        }

        private void Clear()
        {
            CurrentPrediction = null;
            ClearInputFieldPrediction();
            
            onPredictionEnd?.Invoke();
        }
        
        public void ClearInputFieldPrediction()
        {
            _inputFieldPredictionPlaceHolder.text = string.Empty;
        }

        public bool HasAPrediction() => CurrentPrediction != null;

        #endregion
    }
}
