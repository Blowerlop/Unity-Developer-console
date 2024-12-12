using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DevelopperConsole.Extensions.GameObject;


namespace DevelopperConsole
{
    public class ConsoleCommandPrediction : MonoBehaviour
    {
        [SerializeField] private TMP_Text _inputFieldPredictionPlaceHolder;
        [CanBeNull] public string currentPrediction { get; private set; }
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Button _template;

            
        public bool HasAPrediction() => !string.IsNullOrEmpty(currentPrediction);
        
        public void Predict(string input)
        {
            ClearPrediction();

            if (string.IsNullOrEmpty(input)) return;
            
            string[] splitInput = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            string commandInput = splitInput[0];

            HashSet<string> allCommandsName = new HashSet<string>();

            for (int i = 0; i < Console.instance.commandsName.Length; i++)
            {
                string commandName = Console.instance.commandsName[i];
                
                if (commandName.StartsWith(commandInput, true, CultureInfo.InvariantCulture))
                {
                    allCommandsName.Add(Console.instance.commandsName[i]);
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

            for (int i = 0; i < Console.instance.commands[currentPrediction].parametersInfo.Length; i++)
            {
                if (splitInput.Count > i + 1) continue;

                ParameterInfo parameterInfo = Console.instance.commands[currentPrediction].parametersInfo[i];
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
        }

        
        private void ComputeAdditionalPrediction(HashSet<string> allPredictionsName)
        {
            foreach (string predictionsName in allPredictionsName)
            {
                Button instance = Instantiate(_template, _gameObject.transform);
                instance.onClick.AddListener(() =>
                {
                    Console.instance.SetTextOfInputInputFieldSilent(predictionsName);
                    ComputeFirstPrediction(predictionsName, predictionsName, predictionsName,
                        new[] { predictionsName });
                    Console.instance.FocusOnInputField();
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
        }
    }
}
