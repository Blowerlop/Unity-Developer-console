using System;
using System.Reflection;
using UnityEngine;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterResolverAttribute : Attribute
    {
        private readonly Type _correctType = typeof(string[]);
        private readonly Func<string[]> _func;    
        
        
        public ParameterResolverAttribute(Type targetType, string targetMethodName)
        {
            var methodInfo = targetType.GetMethod(targetMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (methodInfo == null || methodInfo.ReturnType != _correctType || !methodInfo.IsStatic)
            {
                throw new InvalidOperationException("The target method must be static and return a string array.");
            }
            
            _func = (Func<string[]>)Delegate.CreateDelegate(typeof(Func<string[]>), methodInfo);
        }

        public ParameterResolverAttribute(Type type)
        {
            GenerateFuncByType(out _func);
            return;

            
            // --- Local methods ---
            void GenerateFuncByType(out Func<string[]> func)
            {
                if (type.IsEnum)
                {
                    func = () => Enum.GetNames(type);
                }
                else
                {
                    Debug.LogError($"{type.Name} is not supported. Cannot generate function");
                    func = null;
                }
            }
        }

        public string[] Resolve()
        {
            return _func.Invoke();
        }
    }
}