using System;
using System.Reflection;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConsoleParameterInputAttribute : Attribute
    {
        private readonly Func<string[]> _func;    
        
        
        /// <summary>
        /// Find any member (static method, property or field) in the <b><paramref name="targetType"/></b> class that returns a string array.
        /// </summary>
        /// <param name="targetType">Class type where the <b><paramref name="target"/></b> is located</param>
        /// <param name="target">Member name</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        
        /// <param name="type">Enum type required</param>
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
                else throw new InvalidOperationException("{type.Name} is not an enum.");
            }
        }

        public string[] Resolve()
        {
            return _func.Invoke();
        }
    }
}