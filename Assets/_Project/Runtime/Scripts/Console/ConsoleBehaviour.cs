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
        internal bool IsConsoleEnabled => _canvas.activeSelf;
        internal bool IsInputFieldFocus => inputInputField.isFocused;
        private int _currentNumberOfMessages;

        [Header("Parameters")]
        [SerializeField] private int _maxMessages = 100;
        [SerializeField] private int _maxCommandHistory = 50;
        [SerializeField, ColorUsage(false)] private Color _logColor;
        [SerializeField, ColorUsage(false)] private Color _logWarningColor;
        [SerializeField, ColorUsage(false)] private Color _logErrorColor;
        [SerializeField] private bool _logTimeStamp = true;
        [SerializeField] private bool _logFrame =  true;
        [SerializeField, TextArea] private string _welcomeMessage;
        
        internal readonly Dictionary<string, ConsoleCommand> commands = new();
        internal List<string> CommandsName { get; private set; }
        internal List<string> CommandHistory { get; private set; }
        private int _commandHistoryIndex;
        [NonSerialized] internal int currentHistoryIndex = -1;
        
        [Header("References")]
        [SerializeField] private GameObject _canvas;
        [field: SerializeField] internal ScrollRect logScrollRect { get; private set; }
        [field: SerializeField] internal TMP_InputField logInputField { get; private set; }
        [field: SerializeField] internal TMP_InputField inputInputField { get; private set; }
        
        // Events
        internal Action onShow;
        internal Action onHide;

        #endregion
        
        
        #region Core Behaviours

        protected override void Awake()
        {
            ClearLogs();
            LogWelcomeMessage();
            Application.logMessageReceived += LogConsole;
            RetrieveCommandAttribute();
        }

        private void Start()
        {
            InitializeCommandsName();
            
            CommandHistory = new List<string>(_maxCommandHistory);
            
            HideForced();
            ClearInputField();
        }

        private void OnEnable()
        {
            inputInputField.onSubmit.AddListener(ExecuteCommand);
        }

        private void OnDisable()
        {
            inputInputField.onSubmit.RemoveListener(ExecuteCommand);
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= LogConsole;
        }

        #endregion


        #region Methods

        #region Commands name

        private void InitializeCommandsName()
        {
            CommandsName = new List<string>(commands.Count);
            
            foreach (var kvp in commands)
            {
                CommandsName.Add(kvp.Key);
            }
            
            CommandsName.Sort(StringComparer.OrdinalIgnoreCase);
        }

        private void RegisterNewCommandName(ConsoleCommand consoleCommand)
        {
            CommandsName.Add(consoleCommand.Name);
            
            int index = CommandsName.BinarySearch(consoleCommand.Name, StringComparer.OrdinalIgnoreCase);
            if (index < 0)
            {
                index = ~index;
            }
            
            CommandsName.Insert(index, consoleCommand.Name);
        }

        #endregion

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
                object[] parameters = new object[command.Parameters.Length];

                int commandsSplitInputLength = splitInput.Length - 1;
                if (commandsSplitInputLength > command.Parameters.Length || (commandsSplitInputLength < command.Parameters.Length && commandsSplitInputLength < command.Parameters.Length - command.ParametersWithDefaultValue))
                {
                    int commandParametersLength = command.Parameters.Length;

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
                Type parameterType = command.Parameters[i].info.ParameterType;
                
                if (i + 1 >= splitInput.Length)
                {
                    parameterResult = command.Parameters[i].info.DefaultValue;
                    return true;
                }

                string inputParameter = splitInput[i + 1];

                try
                {
                    // 0 and 1 are not parse as boolean
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
                    else if (parameterType.IsEnum)
                    {
                        parameterResult = Enum.Parse(parameterType, inputParameter, true);
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
                CommandHistory.RemoveAt(_maxCommandHistory);
            }
            
            CommandHistory.Insert(0, input);
            _commandHistoryIndex++;

            currentHistoryIndex = -1;
        }

        internal void AddCommand(ConsoleCommand consoleCommand)
        {
            AddCommandWithoutNotify(consoleCommand);
            OnCommandAdded(consoleCommand);
        }

        private void AddCommandWithoutNotify(ConsoleCommand consoleCommand)
        {
            commands.Add(consoleCommand.Name, consoleCommand);
        }
        
        private void OnCommandAdded(ConsoleCommand consoleCommand)
        {
            RegisterNewCommandName(consoleCommand);
        }
        
        #endregion

        #region Utilities
        internal void SetTextOfInputInputField(string text)
        {
            if (string.Equals(inputInputField.text, text)) return;
            
            inputInputField.text = text;
            MoveCaretToTheEndOfTheText();
        }
        
        internal void SetTextOfInputInputFieldSilent(string text)
        {
            if (string.Equals(inputInputField.text, text)) return;
            
            inputInputField.SetTextWithoutNotify(text);
            MoveCaretToTheEndOfTheText();
        }

        internal void MoveCaretToTheEndOfTheText()
        {
            inputInputField.MoveTextEnd(false);
        }

        private void ClearInputField() => inputInputField.text = (string.Empty);
        
        internal void FocusOnInputField()
        {
            inputInputField.ActivateInputField();
            MoveCaretToTheEndOfTheText();
        }
        
        
        #endregion

        internal void Show()
        {
            if (IsConsoleEnabled) return;

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
            
            onShow?.Invoke();
        }

        internal void Hide()
        {
            if (IsConsoleEnabled == false) return;
            
            HideForced();
        }

        private void HideForced()
        {
            instance._canvas.SetActive(false);

            instance.OnHideConsole();
        }

        private void OnHideConsole()
        {
            onHide?.Invoke();
        }

        private void LogWelcomeMessage()
        {
            if (string.IsNullOrWhiteSpace(_welcomeMessage)) return;

            LogConsole(_welcomeMessage, string.Empty, LogType.Log);
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

            string message;
            if (_logTimeStamp || _logFrame)
            {
                string timeStamp = _logTimeStamp ? $"[{DateTime.Now:HH:mm:ss}]" : string.Empty;
                string frame = _logFrame ? $"[{Time.frameCount}" : string.Empty;
                message = $"{timeStamp} {frame} {condition}";
            }
            else
            {
                message = condition;
            }
            
            logInputField.text += $"<color=#{ColorUtility.ToHtmlStringRGB(logColor)}>{message}</color>\n";

            if (_currentNumberOfMessages >= _maxMessages)
            {
                // _logInputField.text = _logInputField.text.RemoveFirstLine();
            }
            else
            {
                _currentNumberOfMessages++;
            }
        }
        
        internal void ClearLogs()
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
                                    for (int i = 0; i < commandAttribute.CommandNames.Length; i++)
                                    {
                                        AddCommandWithoutNotify(new ConsoleCommand(commandAttribute.CommandNames[i], commandAttribute.Description, method));
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
                    Debug.LogError(
                        $"Error whilst searching for developer console attributes in assembly({assemblyName}): {e.Message}. " +
                        $"\nStackTrace: {e.StackTrace}");
                }
            }
            
            Profiler.EndSample();
        }

        #endregion
    }
}
