using System;

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
        public static void MethodWithStringParameter([ParameterResolver(typeof(TEST), nameof(GetSaves))] string parameter)
        {
        }
        
        [ConsoleCommand("MethodEnum", "")]
        public static void MethodWithEnumParameter([ParameterResolver(typeof(ETESTEnum)), ParameterGetter(typeof(TEST), nameof(GetSave))]  ETESTEnum parameter)
        {
        }
        
        [ConsoleCommand("MethodAll", "")]
        public static void MethodWillAllParameters([ParameterResolver(typeof(TEST), nameof(GetSaves))] string parameter1, [ParameterResolver(typeof(ETESTEnum))] ETESTEnum parameter2)
        {
        }
        
        [ConsoleCommand("MethodEverything", "")]
        public static void MethodWithEVERYTHING([ParameterResolver(typeof(ETESTEnum)), ParameterGetter(typeof(TEST), nameof(GetSave))]  ETESTEnum parameter, [ParameterResolver(typeof(ETESTEnum)), ParameterGetter(typeof(TEST), nameof(Save))]  ETESTEnum parameter2, [ParameterResolver(typeof(ETESTEnum)), ParameterGetter(typeof(TEST), nameof(SaveProperty))]  ETESTEnum parameter3)
        {
        }
        
        public static string[] GetSaves()
        {
            return new []{"Save1", "Save2", "Save3"};
        }
        
        public static string GetSave()
        {
            return "Save";
        }

        public static string Save = "Save field";
        public static string SaveProperty => "Save property";
    }
}