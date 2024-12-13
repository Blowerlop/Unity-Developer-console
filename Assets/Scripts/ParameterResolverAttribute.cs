using System;
using UnityEngine;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterResolverAttribute : Attribute
    {
        private Func<string[]> _func;    
        
        
        public ParameterResolverAttribute(Type targetType, string targetMethodName)
        {
            _func = () => (string[]) targetType.GetMethod(targetMethodName)?.Invoke(null, null);
        }

        public ParameterResolverAttribute(Type type)
        {
            GenerateFuncByType(type);
        }

        private void GenerateFuncByType(Type type)
        {
            if (type.IsEnum)
            {
                _func = () => Enum.GetNames(type);
            }
            else
            {
                Debug.LogError($"{type.Name} is not supported. Cannot generate function");
            }
        }

        public string[] Resolve()
        {
            return _func.Invoke();
        }
    }
}