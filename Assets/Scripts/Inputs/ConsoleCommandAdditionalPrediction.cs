using System;
using System.Collections.Generic;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeveloperConsole.Inputs
{
    [RequireComponent(typeof(ConsoleCommandPrediction))]
    public class ConsoleCommandAdditionalPrediction : MonoBehaviour
    {
        private ConsoleCommandPrediction _commandPrediction;
        
        private List<string> _commandsName;
        [SerializeField] private GameObject _commandButtonsContainer;
        [SerializeField] private Button _commandButtonTemplate;


        private void Awake()
        {
            _commandPrediction = GetComponent<ConsoleCommandPrediction>();
        }

        private void Start()
        {
            _commandsName = new List<string>(ConsoleBehaviour.instance.commandsName.Length / 4);
        }

        private void OnEnable()
        {
            _commandPrediction.onPredictionStart += OnStartPrediction;
            _commandPrediction.onPredictionEnd += OnPredictionEnd;
        }
        
        private void OnDisable()
        {
            _commandPrediction.onPredictionStart -= OnStartPrediction;
            _commandPrediction.onPredictionEnd -= OnPredictionEnd;
        }
        

        private void OnStartPrediction(ConsoleCommandPrediction.EventArgs obj)
        {
            Clear();
            RetrieveCommandsNameThatStartWith(obj.commandInput);
            
            if (_commandsName.Count == 0) return;
            
            CreateCommandButtons();
        }
        
        private void OnPredictionEnd()
        {
            Clear();
        }
        
        private void RetrieveCommandsNameThatStartWith(ReadOnlySpan<char> commandInput)
        {
            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                var commandName = ConsoleBehaviour.instance.commandsName[i].AsSpan();
                
                if (commandName.StartsWith(commandInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    _commandsName.Add(commandName.ToString());
                }
            }
        }
        
        private void CreateCommandButtons()
        {
            foreach (var commandName in _commandsName)
            {
                CreateCommandButton(commandName);
            }
        }

        private void CreateCommandButton(string commandName)
        {
            var button = Instantiate(_commandButtonTemplate, _commandButtonsContainer.transform);
            button.GetComponentInChildren<TMP_Text>().text = commandName;
            button.onClick.AddListener(() =>
            {
                _commandPrediction.ClearInputFieldPrediction();
                ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(commandName);
                ConsoleBehaviour.instance.FocusOnInputField();
            });
        }

        private void Clear()
        {
            _commandsName.Clear();
            _commandButtonsContainer.DestroyChildren();
        }
    }
}