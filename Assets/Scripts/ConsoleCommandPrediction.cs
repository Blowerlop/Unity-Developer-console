using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DeveloperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        public string currentPrediction { get; private set; }
        private List<string> _allCommandsName = new(20);
        public uint index { get; private set; }
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Button _template;

        public event Action<ConsoleCommand, int> onPredict;
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
            
            string[] splitInput = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string commandInput = splitInput[0];

            _allCommandsName = new List<string>();

            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                string commandName = ConsoleBehaviour.instance.commandsName[i];
                
                if (commandName.StartsWith(commandInput, true, CultureInfo.InvariantCulture))
                {
                    _allCommandsName.Add(ConsoleBehaviour.instance.commandsName[i]);
                }
                
            }

            if (_allCommandsName.Any() == false)
            {
                ClearPrediction();
                return;
            }
            
            ComputePrediction(input, _allCommandsName.First(), 0, commandInput, splitInput);
            GeneratePredictionButtons(_allCommandsName);
        }

        private void ComputePrediction(string input, string predictionName, uint index, string commandInput, IReadOnlyCollection<string> splitInput)
        {
            currentPrediction = predictionName;
            
            if (currentPrediction == null)
            {
                Debug.LogError("Current prediction is null, it should never happen");
                ClearPrediction();
                return;
            }

            this.index = index;

            int inputLength = commandInput.Length;

            string preWriteCommandName = currentPrediction.Substring(0, inputLength);
            string nonWriteCommandName = currentPrediction.Substring(inputLength);

            if (string.IsNullOrEmpty(nonWriteCommandName))
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{input}</color>";
            }
            else
            {
                _inputFieldPredictionPlaceHolder.text = $"<color=#00000000>{preWriteCommandName}</color>{nonWriteCommandName}";
            }

            for (int i = splitInput.Count - 1; i < ConsoleBehaviour.instance.commands[currentPrediction].parametersInfo.Length; i++)
            {
                ParameterInfo parameterInfo = ConsoleBehaviour.instance.commands[currentPrediction].parametersInfo[i];
                if (parameterInfo.HasDefaultValue)
                {
                    // _inputFieldPredictionPlaceHolder.text += $" <{parameterType.Name}>(Optional)";
                    _inputFieldPredictionPlaceHolder.text += $" {parameterInfo.Name}(Optional)";
                }
                else
                {
                    // _inputFieldPredictionPlaceHolder.text += $" <{parameterType.Name}>";
                    _inputFieldPredictionPlaceHolder.text += $" {parameterInfo.Name}";
                }
            }
            
            onPredict?.Invoke(ConsoleBehaviour.instance.commands[currentPrediction], splitInput.Count - 1);
        }

        public void ComputePrediction(string predictionName, uint index)
        {
            ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(predictionName);
            ComputePrediction(predictionName, predictionName, index, predictionName,
                new[] { predictionName });
            ConsoleBehaviour.instance.FocusOnInputField();
            ConsoleBehaviour.instance.MoveCaretToTheEndOfTheText();
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
                ComputePrediction(predictionName, index);
            });
            instance.GetComponentInChildren<TMP_Text>().text = predictionName;
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
