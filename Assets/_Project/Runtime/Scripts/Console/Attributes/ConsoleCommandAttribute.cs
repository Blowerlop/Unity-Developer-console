using System;

namespace DeveloperConsole
{
    /// <summary>
    /// This Attribute only works on static method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        public string[] CommandNames { get; private set; }
        public string Description { get; private set; }
        

        public ConsoleCommandAttribute(string commandName, string description)
        {
            CommandNames = new[] { commandName };
            Description = description;
        }

        public ConsoleCommandAttribute(string[] commandNames, string description)
        {
            CommandNames = commandNames;
            Description = description;
        }
    }
}
