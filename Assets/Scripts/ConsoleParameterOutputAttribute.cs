using System;
using System.Reflection;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConsoleParameterOutputAttribute : Attribute
    {
        private readonly Func<string> _func;

        
        public ConsoleParameterOutputAttribute(Type targetType, string target)
        {
            var memberInfo = targetType.GetMember(target, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)[0];

            _func = memberInfo switch
            {
                MethodInfo methodInfo when methodInfo.ReturnType != typeof(string) => throw new InvalidOperationException("The target method must return a string."),
                MethodInfo methodInfo => (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), methodInfo),
                
                PropertyInfo propertyInfo when propertyInfo.PropertyType != typeof(string) => throw new InvalidOperationException("The target property must be a string."),
                PropertyInfo propertyInfo => () => (string)propertyInfo.GetValue(null),
                
                FieldInfo fieldInfo when fieldInfo.FieldType != typeof(string) => throw new InvalidOperationException("The target field must be a string."),
                FieldInfo fieldInfo => () => (string)fieldInfo.GetValue(null),
                
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public string Resolve()
        {
            return _func.Invoke();
        }
    }
}