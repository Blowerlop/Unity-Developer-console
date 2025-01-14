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
            // _consoleCommandPrediction.onPredictParameter += OnPredictParameter;
            // _consoleCommandPrediction.onStopPredict += OnStopPredict;
        }

        private void OnDisable()
        {
            // _consoleCommandPrediction.onPredictParameter -= OnPredictParameter;
            // _consoleCommandPrediction.onStopPredict -= OnStopPredict;
        }
        
        
        private void OnPredictParameter(ParameterInfo parameterInfo)
        {
            var parameterResolverAttribute = parameterInfo.GetCustomAttribute<ParameterResolverAttribute>();

            var values = parameterResolverAttribute.Resolve();
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                
                Button instance = Instantiate(_template, _gameObject.transform);
                instance.onClick.AddListener(() =>
                {
                    string currentInputFieldText = ConsoleBehaviour.instance.inputInputField.text;
                    ConsoleBehaviour.instance.SetTextOfInputInputField(currentInputFieldText + " " + value);
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