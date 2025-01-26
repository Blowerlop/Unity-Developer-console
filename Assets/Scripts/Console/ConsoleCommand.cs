using System;
using System.Reflection;

namespace DeveloperConsole
{
    [Serializable]
    public class ConsoleCommand
    {
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
        
        
        public string name { get; private set; }
        private MethodInfo _methodInfo;
        public Parameter[] parameters { get; private set; }
        public uint parametersWithDefaultValue { get; private set; }
        public string description { get; private set; }
        

        private ConsoleCommand(string name, string description)
        {
            this.name = name;
            this.description = description;
        }
        
        public ConsoleCommand(string name, string description, Action method) : this(name, description)
        {
            SetupFinalParameters(method.GetMethodInfo());
        }

        public ConsoleCommand(string name, string description, MethodInfo methodInfo) : this(name, description)
        {
            SetupFinalParameters(methodInfo);
        }


        private void SetupFinalParameters(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
            
            var parametersInfos = methodInfo.GetParameters();
            parameters = new Parameter[parametersInfos.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameter parameter = new(parametersInfos[i]);
                parameters[i] = parameter;
            }
            
            HasParametersInfoHaveDefaultValue();
        }

        public void InvokeMethod(object[] parameters)
        {
            _methodInfo.Invoke(null, parameters);
        }
        
        private void HasParametersInfoHaveDefaultValue()
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].info.HasDefaultValue)
                {
                    parametersWithDefaultValue++;
                }
            }
        }
        
        public override string ToString()
        {
            return $"{name} : {description}";
        }
    }
}
