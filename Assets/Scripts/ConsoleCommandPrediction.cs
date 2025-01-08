using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace DeveloperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        private struct ParseInput
        {
            public string raw;
            public string command;
            public string[] split;
            public bool isEndingWithSpace => raw.EndsWith(' ');
        }
        
        
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        public string currentPrediction { get; private set; }
        private List<string> _allCommandsName = new(20);
        public uint index { get; private set; }
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Button _template;

        public event Action<ParameterInfo> onPredictParameter;
        public event Action onStopPredict;


        private void OnEnable()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.AddListener(Predict);
        }

        private void OnDisable()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.RemoveListener(Predict);
        }

        public bool HasAPrediction() => !string.IsNullOrEmpty(currentPrediction);

        private void Predict(string input)
        {
            ClearPrediction();

            if (string.IsNullOrEmpty(input)) return;
            
            ParseInput parseInput = new ParseInput()
            {
                raw = input,
                split = input.Split(" ", StringSplitOptions.RemoveEmptyEntries),
            };
            
            parseInput.command = parseInput.split[0];
            
            RetrieveCommandsNameThatStartWith(parseInput.command);

            if (_allCommandsName.Any() == false) return;
            
            PredictCommand(parseInput, _allCommandsName.First(), 0);

            if (parseInput.command == currentPrediction)
            {
                PredictParameters(parseInput);
                return;
            }

            if (!IsWritingParameter(parseInput.split))
            {
                GeneratePredictionButtons(_allCommandsName);
            }
        }

        private void RetrieveCommandsNameThatStartWith(string commandInput)
        {
            _allCommandsName = new List<string>();

            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                string commandName = ConsoleBehaviour.instance.commandsName[i];
                
                if (commandName.StartsWith(commandInput, true, CultureInfo.InvariantCulture))
                {
                    _allCommandsName.Add(ConsoleBehaviour.instance.commandsName[i]);
                }
            }
        }

        private bool IsWritingParameter(string[] splitInput)
        {
            return splitInput.Length > 1;
        }

        private void PredictParameters(ParseInput parseInput)
        {
            StringBuilder stringBuilder = new StringBuilder(currentPrediction);

            int currentParameterIndex;
            if (parseInput.split.Length == 1) currentParameterIndex = 0;
            else
            {
                currentParameterIndex = parseInput.split.Length - 2;
                if (parseInput.isEndingWithSpace) currentParameterIndex++;
            }
            
            ConsoleCommand currentCommand = ConsoleBehaviour.instance.commands[currentPrediction];
            
            if (currentParameterIndex >= currentCommand.parametersInfo.Length) return;
            
            for (int i = 0; i < currentCommand.parametersInfo.Length; i++)
            {
                ParameterInfo parameterInfo = ConsoleBehaviour.instance.commands[currentPrediction].parametersInfo[i];

                bool isThisParameterTheCurrentOne = currentParameterIndex == i;
                if (isThisParameterTheCurrentOne) stringBuilder.Append(Constants.Styles.Bold.START);
                
                stringBuilder.Append($" {parameterInfo.Name}({parameterInfo.ParameterType.Name})");
                
                if (parameterInfo.HasDefaultValue)
                {
                    stringBuilder.Append("(Optional)");
                }
                
                if (isThisParameterTheCurrentOne) stringBuilder.Append(Constants.Styles.Bold.END);
            }
            
            InstantiateButton(stringBuilder.ToString(), null);
            
            onPredictParameter?.Invoke(currentCommand.parametersInfo[currentParameterIndex]);
        }

        public void PredictCommand(string predictedCommandName, uint index)
        {
            ParseInput parseInput = new ParseInput()
            {
                raw = predictedCommandName,
                command = predictedCommandName
            };
            
            PredictCommand(parseInput, predictedCommandName, index);
            ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(predictedCommandName);
        }

        private void PredictCommand(ParseInput parseInput, string predictedCommandName, uint index)
        {
            currentPrediction = predictedCommandName;
            this.index = index;

            int commandInputLength = parseInput.command.Length;
            
            // Correct user input to match the command name
            if (parseInput.raw != currentPrediction)
            {
                string newText = parseInput.raw.Remove(0, commandInputLength).Insert(0, currentPrediction.Substring(0, commandInputLength));
                ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(newText);
            }
            
            string preWriteCommandName = currentPrediction.Substring(0, commandInputLength);
            string nonWriteCommandName = currentPrediction.Substring(commandInputLength);

            if (string.IsNullOrEmpty(nonWriteCommandName))
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{parseInput.raw}</color>";
            }
            else
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{preWriteCommandName}</color>{nonWriteCommandName}";
            }
        }
        
        private void GeneratePredictionButtons(List<string> allPredictionsName)
        {
            uint i = 0;
            foreach (string predictionsName in allPredictionsName)
            {
                GeneratePredictionButton(predictionsName, i++);
            }
        }
        
        private void GeneratePredictionButton(string predictionName, uint index)
        {
            Button instance = Instantiate(_template, _gameObject.transform);
            instance.onClick.AddListener(() =>
            {
                PredictCommand(predictionName, index);
            });
            instance.GetComponentInChildren<TMP_Text>().text = predictionName;
        }

        private Button InstantiateButton(string content, UnityAction onClick)
        {
            Button instance = Instantiate(_template, _gameObject.transform);
            instance.GetComponentInChildren<TMP_Text>().text = content;
            
            if (onClick != null) instance.onClick.AddListener(onClick);

            return instance;
        }  
        
        private void ClearPrediction()
        {
            if (!HasAPrediction()) return;
            
            currentPrediction = null;
            _inputFieldPredictionPlaceHolder.text = string.Empty;
            _allCommandsName.Clear();
            _gameObject.DestroyChildren();
            index = 0;
            onStopPredict?.Invoke();
        }

        public IReadOnlyList<string> GetPredictionsName()
        {
            return _allCommandsName;
        }
    }
}
