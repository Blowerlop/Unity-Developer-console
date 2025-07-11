using System;
using System.Collections.Generic;
using System.Text;
using DeveloperConsole.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeveloperConsole
{
    [RequireComponent(typeof(ConsoleCommandPrediction))]
    public class ConsoleCommandAdditionalPrediction : MonoBehaviour
    {
        #region Variables

        private ConsoleCommandPrediction _commandPrediction;
        
        private List<ConsoleCommand> _commandsName;
        public int Index { get; private set; } = -1;
        
        [SerializeField] private GameObject _commandButtonsContainer;
        [SerializeField] private Button _commandButtonTemplate;

        #endregion


        #region Core Behaviours

        private void Awake()
        {
            _commandPrediction = GetComponent<ConsoleCommandPrediction>();
        }

        private void Start()
        {
            _commandsName = new List<ConsoleCommand>(ConsoleBehaviour.instance.CommandsName.Count / 4);
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

        #endregion


        #region Methods

        private void OnStartPrediction(ConsoleCommandPrediction.EventArgs obj)
        {
            Clear();
            RetrieveCommandsNameThatStartWith(obj.commandInput);
            
            if (_commandsName.Count == 0) return;
            
            CreateCommandButtons();
            Index = -1;
        }
        
        private void OnPredictionEnd()
        {
            Clear();
        }
        
        private void RetrieveCommandsNameThatStartWith(ReadOnlySpan<char> commandInput)
        {
            bool hasFindAnyStartsWith = false;
            
            for (int i = 0; i < ConsoleBehaviour.instance.CommandsName.Count; i++)
            {
                string commandName = ConsoleBehaviour.instance.CommandsName[i];
                var commandNameSpan = ConsoleBehaviour.instance.CommandsName[i].AsSpan();
                
                if (commandNameSpan.StartsWith(commandInput, StringComparison.InvariantCultureIgnoreCase))
                {
                    _commandsName.Add(ConsoleBehaviour.instance.commands[commandName]);
                    hasFindAnyStartsWith = true;
                }
                // Because the commandsName is sorted,
                // if we find any command that starts with the input and failed later,
                // it means that we have already found all the commands that start with the input.
                else if (hasFindAnyStartsWith) return;
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
            Index++;
            int localIndexCopy = Index;
            
            var button = Instantiate(_commandButtonTemplate, _commandButtonsContainer.transform);
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(consoleCommand.Name);
            
            for (int i = 0; i < consoleCommand.Parameters.Length; i++)
            {
                if (consoleCommand.Parameters[i].attributes.consoleParameterOutputAttribute == null) continue;
                
                stringBuilder.Append(" ");
                stringBuilder.Append(consoleCommand.Parameters[i].attributes.consoleParameterOutputAttribute.Resolve());
            }
            
            button.GetComponentInChildren<TMP_Text>().text = stringBuilder.ToString();
            button.onClick.AddListener(() =>
            {
                _commandPrediction.ClearInputFieldPrediction();
                ConsoleBehaviour.instance.SetTextOfInputInputFieldSilent(consoleCommand.Name);
                ConsoleBehaviour.instance.FocusOnInputField();
                Index = localIndexCopy;
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
            Index = -1;
        }

        #endregion
    }
}