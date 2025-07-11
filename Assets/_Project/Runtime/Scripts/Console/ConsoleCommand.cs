using System;
using System.Reflection;

namespace DeveloperConsole
{
    [Serializable]
    public class ConsoleCommand
    {
        #region Custom Types

        public struct Parameter
        {
            public struct Attributes
            {
                public ConsoleParameterInputAttribute consoleParameterInput;
                public ConsoleParameterOutputAttribute consoleParameterOutputAttribute;
            }
            
            
            public readonly ParameterInfo info;
            public readonly Attributes attributes;
            
            
            public Parameter(ParameterInfo info)
            {
                this.info = info;
                
                attributes = new Attributes
                {
                    consoleParameterInput = info.GetCustomAttribute<ConsoleParameterInputAttribute>(),
                    consoleParameterOutputAttribute = info.GetCustomAttribute<ConsoleParameterOutputAttribute>()
                };
            }
        }

        #endregion


        #region Variables

        public string Name { get; private set; }
        private MethodInfo _methodInfo;
        public Parameter[] Parameters { get; private set; }
        public uint ParametersWithDefaultValue { get; private set; }
        public string Description { get; private set; }

        #endregion


        #region Constructors

        private ConsoleCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }
        
        public ConsoleCommand(string name, string description, Action method) : this(name, description)
        {
            SetupFinalParameters(method.GetMethodInfo());
        }

        public ConsoleCommand(string name, string description, MethodInfo methodInfo) : this(name, description)
        {
            SetupFinalParameters(methodInfo);
        }

        #endregion


        #region Methods

        private void SetupFinalParameters(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            
            var parametersInfos = methodInfo.GetParameters();
            Parameters = new Parameter[parametersInfos.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameter parameter = new(parametersInfos[i]);
                Parameters[i] = parameter;
            }
            
            HasParametersInfoHaveDefaultValue();
        }

        public void InvokeMethod(object[] parameters)
        {
            _methodInfo.Invoke(null, parameters);
        }
        
        private void HasParametersInfoHaveDefaultValue()
        {
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (Parameters[i].info.HasDefaultValue)
                {
                    ParametersWithDefaultValue++;
                }
            }
        }
        
        public override string ToString()
        {
            return $"{Name} : {Description}";
        }

        #endregion
    }
}
