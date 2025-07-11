using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;

namespace DeveloperConsole
{
    [UsedImplicitly]
    public static class ConsoleNativeCommands
    {
        private static ConsoleBehaviour Instance => ConsoleBehaviour.instance;

        
        [ConsoleCommand("enable", "Enable the console")]
        [UsedImplicitly]
        private static void Show()
        {
            Instance.Show();
        }
        
        [ConsoleCommand("disable", "Disable the console")]
        [UsedImplicitly]
        private static void HideConsole()
        {
            Instance.Hide();
        }
        
        [ConsoleCommand("clear", "Wipe all the logs in the console")]
        [UsedImplicitly]
        private static void ClearLogs()
        {
            Instance.ClearLogs();
        }

        [ConsoleCommand(new[] {"help", "commands"}, "Display all the commands")]
        [UsedImplicitly]
        private static void DisplayAllCommands()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("Here the list of all available commands :\n");

            foreach (var kvp in Instance.commands)
            {
                stringBuilder.AppendLine(kvp.Value.ToString());
            }
            
            Debug.Log(stringBuilder.ToString());
        }

        [ConsoleCommand("find", "Find all commands related to search text")]
        [UsedImplicitly]
        private static void Find(string search)
        {
            // await Awaitable.BackgroundThreadAsync();
            
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (var kvp in Instance.commands.Where(kvp => kvp.Key.Contains(search) || kvp.Value.Description.Contains(search)))
            {
                stringBuilder.AppendLine(kvp.Value.ToString());
            }

            // await Awaitable.MainThreadAsync();
            
            Debug.Log(stringBuilder.ToString());
        }
    }
}