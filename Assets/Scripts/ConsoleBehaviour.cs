using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace DeveloperConsole
{
    [DefaultExecutionOrder(-1)]
    public class ConsoleBehaviour : MonoSingleton<ConsoleBehaviour>
    {
        #region Variables

        // Console State
        public bool isConsoleEnabled => _canvas.activeSelf;
        public bool isInputFieldFocus => inputInputField.isFocused;
        private int _currentNumberOfMessages;

        [Header("Parameters")]
        [SerializeField] private int _maxMessages = 100;
        [SerializeField] private int _maxCommandHistory = 50;
        
        public readonly Dictionary<string, ConsoleCommand> commands = new();
        public string[] commandsName { get; private set; }
        public List<string> commandHistory { get; private set; }
        private int _commandHistoryIndex;
        public int currentHistoryIndex = -1;
        
        [Header("References")]
        [SerializeField] private GameObject _canvas;
        [field: SerializeField] public ScrollRect logScrollRect { get; private set; }
        [field: SerializeField] public TMP_InputField logInputField { get; private set; }
        [field: SerializeField] public TMP_InputField inputInputField { get; private set; }
        [SerializeField] private ConsoleCommandPrediction _commandPrediction;

        [SerializeField, ColorUsage(false)] private Color _logColor;
        [SerializeField, ColorUsage(false)] private Color _logWarningColor;
        [SerializeField, ColorUsage(false)] private Color _logErrorColor;

        public Action OnShowEvent;
        public Action OnHideEvent;

        #endregion
        
        
        #region Updates

        protected override void Awake()
        {
            ClearLogs();
            Application.logMessageReceived += LogConsole;
            RetrieveCommandAttribute();
        }

        private void Start()
        {
            commandsName = new string[commands.Count];
            int index = 0;
            foreach (var kvp in commands)
            {
                commandsName[index] = kvp.Key;
                index++;
            }
            
            Array.Sort(commandsName);

            commandHistory = new List<string>(_maxCommandHistory);
            
            HideForced();
            ClearInputField();
        }

        private void OnEnable()
        {
            inputInputField.onSubmit.AddListener(ExecuteCommand);
            inputInputField.onValueChanged.AddListener(_commandPrediction.Predict);
        }

        private void OnDisable()
        {
            inputInputField.onSubmit.RemoveListener(ExecuteCommand);
            inputInputField.onValueChanged.RemoveListener(_commandPrediction.Predict);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= LogConsole;
        }

        #endregion


        #region Methods

        #region Command Relative

        private void ExecuteCommand(string rawInput)
        {
            if (string.IsNullOrEmpty(rawInput)) return;

            string trimString = rawInput.TrimEnd();
            string[] splitInput = trimString.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            Debug.Log($">> {trimString}");
            AddToCommandHistory(trimString);

            // Check if the command exist
            if (commands.TryGetValue(splitInput[0], out ConsoleCommand command))
            {
                // Check if the command have the same number of parameters that the player input
                object[] parameters = new object[command.parametersInfo.Length];

                int commandsSplitInputLength = splitInput.Length - 1;
                if (commandsSplitInputLength > command.parametersInfo.Length || (commandsSplitInputLength < command.parametersInfo.Length && commandsSplitInputLength < command.parametersInfo.Length - command.parametersWithDefaultValue))
                {
                    int commandParametersLength = command.parametersInfo.Length;

                    switch (commandParametersLength)
                    {
                        case 0:
                            Debug.LogError($"This command has no parameter, you pass {commandsSplitInputLength}");
                            break;
                        case 1:
                            Debug.LogError($"This command has {commandParametersLength} parameter, you pass {commandsSplitInputLength}");
                            break;
                        default:
                            Debug.LogError($"This command has {commandParametersLength} parameters, you pass {commandsSplitInputLength}");
                            break;
                    }

                    goto end;
                }
                
                
                // Loop through the player input and try parse the parameters
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (TryParseParameter(i, out object parameterResult) == false) goto end;

                    parameters[i] = parameterResult;
                }

                try
                {
                    command.InvokeMethod(parameters);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Unexpected behaviour : {e}");
                }
            }
            else
            {
                Debug.LogError($"Unknown command '{splitInput[0]}'");
            }
            
            end:
            ClearInputField();
            FocusOnInputField();

            
            
            bool TryParseParameter(int i, out object parameterResult)
            {
                Type parameterType = command.parametersInfo[i].ParameterType;
                
                if (i + 1 >= splitInput.Length)
                {
                    parameterResult = command.parametersInfo[i].DefaultValue;
                    return true;
                }

                string inputParameter = splitInput[i + 1];

                try
                {
                    // The is the only parameter type that is force check because C# does not Parse 0 and 1 as boolean
                    if (parameterType == typeof(bool))
                    {
                        if (bool.TryParse(inputParameter, out bool boolValue))
                        {
                            parameterResult = boolValue;
                        }
                        else
                        {
                            int valueInt = int.Parse(inputParameter);
                            if (valueInt == 0) parameterResult = false;
                            else if (valueInt == 1) parameterResult = true;
                            else throw new FormatException();
                        }
                    }
                    else
                    {
                        parameterResult = Convert.ChangeType(inputParameter, parameterType, CultureInfo.CurrentCulture);
                    }
                }
                catch
                {
                    Debug.LogError($"Unknown parameter '{inputParameter}'. Parameter need to be a {parameterType.Name}");
                    parameterResult = null;
                    return false;
                }

                return true;
            }
        }
        
        private void AddToCommandHistory(string input)
        {
            if (_commandHistoryIndex >= _maxCommandHistory)
            {
                commandHistory.RemoveAt(_maxCommandHistory);
            }
            
            commandHistory.Insert(0, input);
            _commandHistoryIndex++;

            currentHistoryIndex = -1;
        }

        public static void AddCommand(ConsoleCommand consoleCommand)
        {
            instance.commands.Add(consoleCommand.name, consoleCommand);
        }

        #endregion
        
        #region Used by shortcut
        public void ToggleConsole()
        {
            if (isConsoleEnabled) Hide();
            else Show();
        }
        
        #endregion
        
        #region Utilities
        public void SetTextOfInputInputField(string text)
        {
            if (string.Equals(inputInputField.text, text)) return;
            
            inputInputField.text = text;
            MoveCaretToTheEndOfTheText();
        }
        
        public void SetTextOfInputInputFieldSilent(string text)
        {
            if (string.Equals(inputInputField.text, text)) return;
            
            inputInputField.SetTextWithoutNotify(text);
            MoveCaretToTheEndOfTheText();
        }

        public void MoveCaretToTheEndOfTheText()
        {
            inputInputField.MoveTextEnd(false);
        }

        private void ClearInputField() => inputInputField.text = (string.Empty);
        
        public void FocusOnInputField()
        {
            inputInputField.ActivateInputField();
        }
        
        
        #endregion

        public void Show()
        {
            if (isConsoleEnabled) return;

            ShowForced();
        }

        private void ShowForced()
        {
            _canvas.SetActive(true);

            OnShow();
        }

        private void OnShow()
        {
            FocusOnInputField();
            
            OnShowEvent?.Invoke();
        }

        public void Hide()
        {
            if (isConsoleEnabled == false) return;
            
            HideForced();
        }

        private void HideForced()
        {
            instance._canvas.SetActive(false);

            instance.OnHideConsole();
        }

        private void OnHideConsole()
        {
            OnHideEvent?.Invoke();
        }
        
        private void LogConsole(string condition, string stacktrace, LogType logType)
        {
            bool setAtBottom = logScrollRect.verticalNormalizedPosition <= 0.01f;

            Color logColor;
            switch (logType)
            {
                case LogType.Log:
                    logColor = _logColor;
                    break;
                
                case LogType.Warning:
                    logColor = _logWarningColor;
                    break;
                
                // All the other LogType
                default:
                    logColor = _logErrorColor;
                    break;
            }
            
            logInputField.text += $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>{condition}</color>\n";

            if (_currentNumberOfMessages >= _maxMessages)
            {
                // _logInputField.text = _logInputField.text.RemoveFirstLine();
            }
            else
            {
                _currentNumberOfMessages++;
            }
        }
        
        public void ClearLogs()
        {
            Debug.Log("Console cleared");
            logInputField.text = string.Empty;
            _currentNumberOfMessages = 0;
        }

        
        
        
        // https://github.com/yasirkula/UnityIngameDebugConsole/blob/master/Plugins/IngameDebugConsole/Scripts/DebugLogConsole.cs
        // Implementation of finding attributes sourced from yasirkula's code
        private void RetrieveCommandAttribute()
        {
            Profiler.BeginSample("ConsoleAttributeRetrieving");

#if UNITY_EDITOR || !NETFX_CORE
            string[] ignoredAssemblies = new string[]
            {
                "Unity",
                "System",
                "Mono.",
                "mscorlib",
                "netstandard",
                "TextMeshPro",
                "Microsoft.GeneratedCode",
                "I18N",
                "Boo.",
                "UnityScript.",
                "ICSharpCode.",
                "ExCSS.Unity",
#if UNITY_EDITOR
				"Assembly-CSharp-Editor",
                "Assembly-UnityScript-Editor",
                "nunit.",
                "SyntaxTree.",
                "AssetStoreTools"
#endif
            };
#endif
#if UNITY_EDITOR || !NETFX_CORE
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
#else
            foreach (Assembly assembly in new Assembly[] { GetType().Assembly })
#endif
            {
#if (NET_4_6 || NET_STANDARD_2_0) && (UNITY_EDITOR || !NETFX_CORE)
                if (assembly.IsDynamic)
                    continue;
#endif

                string assemblyName = assembly.GetName().Name;

#if UNITY_EDITOR || !NETFX_CORE
                if (ignoredAssemblies.Any(a => assemblyName.ToLower().StartsWith(a.ToLower())))
                {
                    continue;
                }
#endif

                try
                {
                    foreach (Type type in assembly.GetExportedTypes())
                    {
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                        {
                            foreach (object attribute in method.GetCustomAttributes(typeof(ConsoleCommandAttribute), false))
                            {
                                ConsoleCommandAttribute commandAttribute = (ConsoleCommandAttribute)attribute;
                                if (commandAttribute != null)
                                {
                                    for (int i = 0; i < commandAttribute.commandNames.Length; i++)
                                    {
                                        AddCommand(new ConsoleCommand(commandAttribute.commandNames[i], commandAttribute.description, method));
                                    }
                                    
                                }
                            }
                        }
                    }
                }
                catch (NotSupportedException) { }
                catch (System.IO.FileNotFoundException) { }
                catch (Exception e)
                {
                    Debug.LogError("Error whilst searching for developer console attributes in assembly(" + assemblyName + "): " + e.Message + ".");
                }
            }
            
            Profiler.EndSample();
        }

        #endregion
    }
}
