using System.Text;
using UnityEngine;

namespace DeveloperConsole
{
    public static class ConsoleNativeCommands
    {
        private static ConsoleBehaviour instance => ConsoleBehaviour.instance;

        
        [ConsoleCommand("enable", "Enable the console")]
        private static void Show()
        {
            instance.Show();
        }
        
        [ConsoleCommand("disable", "Disable the console")]
        private static void HideConsole()
        {
            instance.Hide();
        }
        
        [ConsoleCommand("clear", "Wipe all the logs in the console")]
        private static void ClearLogs()
        {
            instance.ClearLogs();
        }

        [ConsoleCommand(new string[] {"help", "commands"}, "Display all the commands")]
        private static void DisplayAllCommands()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Here the list of all available commands :\n");

            foreach (var kvp in instance.commands)
            {
                stringBuilder.AppendLine(kvp.Value.ToString());
            }
            
            Debug.Log(stringBuilder.ToString());
        }

        [ConsoleCommand("find", "Find all commands related to search text")]
        private static void Find(string search)
        {
            // await Awaitable.BackgroundThreadAsync();
            
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (var kvp in instance.commands)
            {
                if (kvp.Key.Contains(search) || kvp.Value.description.Contains(search))
                {
                    stringBuilder.AppendLine(kvp.Value.ToString());
                }
            }

            // await Awaitable.MainThreadAsync();
            
            Debug.Log(stringBuilder.ToString());
        }
    }
}