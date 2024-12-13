using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DeveloperConsole.Extensions;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DeveloperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        [CanBeNull] public string currentPrediction { get; private set; }
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

            HashSet<string> allCommandsName = new HashSet<string>();

            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                string commandName = ConsoleBehaviour.instance.commandsName[i];
                
                if (commandName.StartsWith(commandInput, true, CultureInfo.InvariantCulture))
                {
                    allCommandsName.Add(ConsoleBehaviour.instance.commandsName[i]);
                }
                
            }

            if (allCommandsName.Any() == false)
            {
                ClearPrediction();
                return;
            }
            
            ComputeFirstPrediction(input, allCommandsName.First(), commandInput, splitInput);
            ComputeAdditionalPrediction(allCommandsName);
        }

        private void ComputeFirstPrediction(string input, string firstPredictionName, string commandInput, IReadOnlyCollection<string> splitInput)
        {
            currentPrediction = firstPredictionName;
#if UNITY_EDITOR
            // Just to make Rider happy :)
            if (currentPrediction == null)
            {
                Debug.LogError("Current prediction is null, it should never happen");
                ClearPrediction();
                return;
            }
#endif

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

        
        private void ComputeAdditionalPrediction(HashSet<string> allPredictionsName)
        {
            foreach (string predictionsName in allPredictionsName)
            {
                Button instance = Instantiate(_template, _gameObject.transform);
                instance.onClick.AddListener(() =>
                {
                    ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(predictionsName);
                    ComputeFirstPrediction(predictionsName, predictionsName, predictionsName,
                        new[] { predictionsName });
                    ConsoleBehaviour.instance.FocusOnInputField();
                });
                instance.GetComponentInChildren<TMP_Text>().text = predictionsName;
            }
        }

        private void ClearPrediction()
        {
            if (currentPrediction == null) return;
            
            currentPrediction = null;
            _inputFieldPredictionPlaceHolder.text = string.Empty;
            _gameObject.DestroyChildren();
            onStopPredict?.Invoke();
        }
    }
}
