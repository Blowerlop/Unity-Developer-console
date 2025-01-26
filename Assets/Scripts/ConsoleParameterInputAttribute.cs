using System;
using System.Reflection;
using UnityEngine;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConsoleParameterInputAttribute : Attribute
    {
        private readonly Type _correctType = typeof(string[]);
        private readonly Func<string[]> _func;    
        
        
        public ConsoleParameterInputAttribute(Type targetType, string target)
        {
            var memberInfo = targetType.GetMember(target, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)[0];

            _func = memberInfo switch
            {
                MethodInfo methodInfo when methodInfo.ReturnType != typeof(string) => throw new InvalidOperationException("The target method must return a string."),
                MethodInfo methodInfo => (Func<string[]>)Delegate.CreateDelegate(typeof(Func<string>), methodInfo),
                
                PropertyInfo propertyInfo when propertyInfo.PropertyType != typeof(string) => throw new InvalidOperationException("The target property must be a string."),
                PropertyInfo propertyInfo => () => (string[])propertyInfo.GetValue(null),
                
                FieldInfo fieldInfo when fieldInfo.FieldType != typeof(string) => throw new InvalidOperationException("The target field must be a string."),
                FieldInfo fieldInfo => () => (string[])fieldInfo.GetValue(null),
                
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public ConsoleParameterInputAttribute(Type type)
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