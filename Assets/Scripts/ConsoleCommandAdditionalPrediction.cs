using System;
using System.Collections.Generic;
using System.Text;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeveloperConsole.Inputs
{
    [RequireComponent(typeof(ConsoleCommandPrediction))]
    public class ConsoleCommandAdditionalPrediction : MonoBehaviour
    {
        private ConsoleCommandPrediction _commandPrediction;
        
        private List<ConsoleCommand> _commandsName;
        public int index { get; private set; } = -1;
        
        [SerializeField] private GameObject _commandButtonsContainer;
        [SerializeField] private Button _commandButtonTemplate;


        private void Awake()
        {
            _commandPrediction = GetComponent<ConsoleCommandPrediction>();
        }

        private void Start()
        {
            _commandsName = new List<ConsoleCommand>(ConsoleBehaviour.instance.commandsName.Length / 4);
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
            index = -1;
        }
        
        private void OnPredictionEnd()
        {
            Clear();
        }
        
        private void RetrieveCommandsNameThatStartWith(ReadOnlySpan<char> commandInput)
        {
            for (int i = 0; i < ConsoleBehaviour.instance.commandsName.Length; i++)
            {
                string commandName = ConsoleBehaviour.instance.commandsName[i];
                var commandNameSpan = ConsoleBehaviour.instance.commandsName[i].AsSpan();
                
                if (commandNameSpan.StartsWith(commandInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    _commandsName.Add(ConsoleBehaviour.instance.commands[commandName]);
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

        private void CreateCommandButton(ConsoleCommand consoleCommand)
        {
            index++;
            int localIndexCopy = index;
            
            var button = Instantiate(_commandButtonTemplate, _commandButtonsContainer.transform);
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(consoleCommand.name);
            
            for (int i = 0; i < consoleCommand.parameters.Length; i++)
            {
                if (consoleCommand.parameters[i].attributes.consoleParameterOutputAttribute == null) continue;
                
                stringBuilder.Append(" ");
                stringBuilder.Append(consoleCommand.parameters[i].attributes.consoleParameterOutputAttribute.Resolve());
            }
            
            button.GetComponentInChildren<TMP_Text>().text = stringBuilder.ToString();
            button.onClick.AddListener(() =>
            {
                _commandPrediction.ClearInputFieldPrediction();
                ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(consoleCommand.name);
                ConsoleBehaviour.instance.FocusOnInputField();
                index = localIndexCopy;
            });
        }

        public void SelectButton(int index)
        {
            _commandButtonsContainer.transform.GetChild(index).GetComponent<Button>().OnPointerClick(new PointerEventData(EventSystem.current));
        }
        
        public int GetButtonsCount()
        {
            return _commandButtonsContainer.transform.childCount;
        }

        private void Clear()
        {
            _commandsName.Clear();
            _commandButtonsContainer.DestroyChildren();
            index = -1;
        }
    }
}