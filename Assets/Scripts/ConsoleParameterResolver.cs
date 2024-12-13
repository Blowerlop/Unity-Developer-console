using System;
using System.Reflection;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeveloperConsole
{
    [RequireComponent(typeof(ConsoleCommandPrediction))]
    public class ConsoleParameterResolver : MonoBehaviour
    {
        private ConsoleCommandPrediction _consoleCommandPrediction;
        
        [SerializeField] private GameObject _gameObject;
        [SerializeField] private Button _template;


        private void Awake()
        {
            _consoleCommandPrediction = ConsoleBehaviour.instance.GetComponent<ConsoleCommandPrediction>();  
        }

        private void OnEnable()
        {
            _consoleCommandPrediction.onPredict += OnPredict;
            _consoleCommandPrediction.onStopPredict += OnStopPredict;
        }

        private void OnDisable()
        {
            _consoleCommandPrediction.onPredict -= OnPredict;
            _consoleCommandPrediction.onStopPredict -= OnStopPredict;
        }
        
        
        private void OnPredict(ConsoleCommand consoleCommand, int parameterIndex)
        {
            if (consoleCommand.parametersInfo.Length == 0) return;
            if (parameterIndex >= consoleCommand.parametersInfo.Length) return;
            
            var parameterResolverAttribute = consoleCommand.parametersInfo[parameterIndex].GetCustomAttribute<ParameterResolverAttribute>();

            var values = parameterResolverAttribute.Resolve();
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                
                Button instance = Instantiate(_template, _gameObject.transform);
                instance.onClick.AddListener(() =>
                {
                    // ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(predictionsName);
                    // ComputeFirstPrediction(predictionsName, predictionsName, predictionsName,
                    //     new[] { predictionsName });
                    // ConsoleBehaviour.instance.FocusOnInputField();
                });
                instance.GetComponentInChildren<TMP_Text>().text = value;
            }

            
        }
        
        private void OnStopPredict()
        {
            _gameObject.DestroyChildren();
        }
    }
}