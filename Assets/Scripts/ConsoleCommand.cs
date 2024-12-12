using System;
using System.Reflection;

namespace DevelopperConsole
{
    [Serializable]
    public class ConsoleCommand
    {
        public string name { get; private set; }
        public ParameterInfo[] parametersInfo { get; private set; }
        private MethodInfo _methodInfo;
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
            parametersInfo = methodInfo.GetParameters();
            HasParametersInfoHaveDefaultValue();
        }

        public void InvokeMethod(object[] parameters)
        {
            _methodInfo.Invoke(null, parameters);
        }
        
        private void HasParametersInfoHaveDefaultValue()
        {
            for (int i = 0; i < parametersInfo.Length; i++)
            {
                if (parametersInfo[i].HasDefaultValue)
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
