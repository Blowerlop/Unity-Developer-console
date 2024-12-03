using System;
using DeveloperConsole;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            DeveloperConsole.ConsoleBehaviour.instance.Toggle();
        }
    }

    [ConsoleCommand("test", "This is a test command.")]
    private static void CallMethod()
    {
        Debug.Log("Printed from CallMethod");
    }
}
