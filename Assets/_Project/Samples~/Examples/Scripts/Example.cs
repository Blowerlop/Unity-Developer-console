using UnityEngine;

namespace DeveloperConsole
{
    public class Example : MonoBehaviour
    {
        private enum EExampleEnum
        {
            First,
            Second,
            Third
        }
        
        private static string[] _exampleStringArray = { "StringOne", "StringTwo", "StringThree" };
        private static string _result = "Change the result";
        
        
        
        [ConsoleCommand(nameof(ExampleCommand_NoArgs), "Example command with no args")]
        private static void ExampleCommand_NoArgs()
        {
            Debug.Log($"This is the {nameof(ExampleCommand_NoArgs)}");
        }
        
        [ConsoleCommand(nameof(ExampleCommand_SingleArg), "Example command with a single arg")]
        private static void ExampleCommand_SingleArg(int arg1)
        {
            Debug.Log($"This is the {nameof(ExampleCommand_SingleArg)} with arg1: {arg1}");
        }
        
        [ConsoleCommand(nameof(ExampleCommand_TwoArgs), "Example command with a two args")]
        private static void ExampleCommand_TwoArgs(int arg1, string arg2)
        {
            Debug.Log($"This is the {nameof(ExampleCommand_TwoArgs)} with arg1: {arg1} and arg2: {arg2}");
        }
        
        [ConsoleCommand(nameof(ExampleCommand_ImplicitInputParameter), "Example command with an implicit input parameter")]
        private static void ExampleCommand_ImplicitInputParameter(EExampleEnum arg1)
        {
            Debug.Log($"This is the {nameof(ExampleCommand_ImplicitInputParameter)} with arg: {arg1}");
        }
        
        [ConsoleCommand(nameof(ExampleCommand_ExplicitInputParameter), "Example command with an explicit input parameter")]
        private static void ExampleCommand_ExplicitInputParameter([ConsoleParameterInput(typeof(Example), nameof(_exampleStringArray))] string arg1)
        {
            Debug.Log($"This is the {nameof(ExampleCommand_ImplicitInputParameter)} with arg: {arg1}");
        }
        
        [ConsoleCommand(nameof(ExampleCommand_OutParameter), "Example command with an out parameter")]
        private static void ExampleCommand_OutParameter([ConsoleParameterOutput(typeof(Example), nameof(_result))] string arg1)
        {
            Debug.Log($"This is the {nameof(ExampleCommand_OutParameter)} with arg: {arg1}");
            _result = arg1;
        }
    }
}
