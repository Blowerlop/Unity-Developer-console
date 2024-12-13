namespace DeveloperConsole
{
    public enum ETESTEnum
    {
        TEST1,
        TEST2,
        TEST3
    }
    
    public static class TEST
    {
        [ConsoleCommand("MethodString", "")]
        public static void MethodWithStringParameter(string parameter)
        {
        }
        
        [ConsoleCommand("MethodEnum", "")]
        public static void MethodWithEnumParameter([ParameterResolver(typeof(ETESTEnum))] ETESTEnum parameter)
        {
        }
    }
}