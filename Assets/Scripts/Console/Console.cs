using System;

namespace DeveloperConsole
{
    public static class Console
    {
        public static void AddCommand(ConsoleCommand command)
        {
            ConsoleBehaviour.instance.AddCommand(command);
        }
        
        public static void Show()
        {
            ConsoleBehaviour.instance.Show();
        }
        
        public static void Hide()
        {
            ConsoleBehaviour.instance.Hide();
        }
    }
}