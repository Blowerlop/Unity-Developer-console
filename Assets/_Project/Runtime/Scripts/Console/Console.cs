using System;

namespace DeveloperConsole
{
    public static class Console
    {
        #region Variables
        
        private static ConsoleBehaviour ConsoleBehaviourInstance => ConsoleBehaviour.instance;

        public static bool IsConsoleEnabled => ConsoleBehaviourInstance.IsConsoleEnabled;
        public static event Action onShow
        {
            add => ConsoleBehaviourInstance.onShow += value;
            remove => ConsoleBehaviourInstance.onShow -= value;
        }
        public static event Action onHide
        {
            add => ConsoleBehaviourInstance.onHide += value;
            remove => ConsoleBehaviourInstance.onHide -= value;
        }

        #endregion
        
        
        #region Methods

        public static void AddCommand(ConsoleCommand command)
        {
            ConsoleBehaviourInstance.AddCommand(command);
        }
        
        public static void Show()
        {
            ConsoleBehaviourInstance.Show();
        }
        
        public static void Hide()
        {
            ConsoleBehaviourInstance.Hide();
        }
        
        public static void ClearLogs()
        {
            ConsoleBehaviourInstance.ClearLogs();
        }

        #endregion
    }
}