using System;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StringExtensions = DeveloperConsole.Extensions.StringExtensions;

namespace DeveloperConsole
{
    [RequireComponent(typeof(ConsoleCommandPrediction))]
    public class ConsoleCommandParameterPrediction : MonoBehaviour
    {
        #region Variables

        private ConsoleCommandPrediction _commandPrediction;
        
        [SerializeField] private GameObject _commandButtonsContainer;
        [SerializeField] private Button _commandButtonTemplate;
        
        private int _parameterIndexInText;

        #endregion


        #region Core Behaviours

        private void Awake()
        {
            _commandPrediction = GetComponent<ConsoleCommandPrediction>();
        }
        
        private void OnEnable()
        {
            _commandPrediction.onPredictionStart += OnPredictionStart;
            _commandPrediction.onPredictionComplete += OnPredictionComplete;
            _commandPrediction.onPredictionEnd += OnPredictionEnd;
        }
        
        private void OnDisable()
        {
            _commandPrediction.onPredictionStart -= OnPredictionStart;
            _commandPrediction.onPredictionComplete -= OnPredictionComplete;
            _commandPrediction.onPredictionEnd -= OnPredictionEnd;
        }

        #endregion

        
        #region Methods

        private void OnPredictionStart(ConsoleCommandPrediction.EventArgs obj)
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.RemoveListener(Predict);
            Clear();
        }

        private void OnPredictionComplete(ConsoleCommand consoleCommand)
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.RemoveListener(Predict);
            ConsoleBehaviour.instance.inputInputField.onValueChanged.AddListener(Predict);
            Clear();
        }
        
        private void OnPredictionEnd()
        {
            ConsoleBehaviour.instance.inputInputField.onValueChanged.RemoveListener(Predict);
            Clear();
        }

        private void Predict(string input)
        {
            Clear();
            
            var parameterCount = input.Count(' ');
            if (parameterCount == 0) return;

            ConsoleCommand.Parameter[] parameters = _commandPrediction.CurrentPrediction.Parameters;
            if (parameterCount > parameters.Length) return;

            var parameterIndex = input.Find(StringExtensions.ESearchOrder.End, ' ');
            if (parameterIndex == _parameterIndexInText) return;
            
            _parameterIndexInText = parameterIndex;
            
            ConsoleCommand.Parameter parameter = parameters[parameterCount - 1];
            if (parameter.attributes.consoleParameterInput != null)
            {
                CreateParameterButtons(parameter.attributes.consoleParameterInput.Resolve());
            }
            else if (parameter.info.ParameterType.IsEnum)
            {
                CreateParameterButtons(Enum.GetNames(parameter.info.ParameterType));
            }
        }
        
        private void CreateParameterButtons(string[] parametersValue)
        {
            foreach (var parameterValue in parametersValue)
            {
                CreateCommandButton(parameterValue);
            }
        }

        private void CreateCommandButton(string parameterValue)
        {
            var button = Instantiate(_commandButtonTemplate, _commandButtonsContainer.transform);
            
            button.GetComponentInChildren<TMP_Text>().text = parameterValue;
            button.onClick.AddListener(() =>
            {
                var newText = ConsoleBehaviour.instance.inputInputField.text;
                var textToKeepLength = _parameterIndexInText + 1;

                Span<char> span = stackalloc char[textToKeepLength + parameterValue.Length];
                newText.AsSpan(0, textToKeepLength).CopyTo(span);
                parameterValue.AsSpan().CopyTo(span[textToKeepLength..]);
                
                ConsoleBehaviour.instance.SetTextOfInputInputField(span.ToString());
                ConsoleBehaviour.instance.FocusOnInputField();
            });
        }

        private void Clear()
        {
            _commandButtonsContainer.DestroyChildren();
            _parameterIndexInText = -1;
        }

        #endregion
    }
}