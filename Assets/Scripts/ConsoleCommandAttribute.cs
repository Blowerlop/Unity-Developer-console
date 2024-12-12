using System;

namespace DevelopperConsole
{
    /// <summary>
    /// This Attribute only works on static method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ConsoleCommandAttribute : Attribute
    {
        public string[] commandNames { get; private set; }
        public string description { get; private set; }
        

        public ConsoleCommandAttribute(string commandName, string description)
        {
            commandNames = new[] { commandName };
            this.description = description;
        }

        public ConsoleCommandAttribute(string[] commandNames, string description)
        {
            this.commandNames = commandNames;
            this.description = description;
        }
    }
}
