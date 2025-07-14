using System.Diagnostics;
using UnityEngine;

namespace DeveloperConsole
{
    [Conditional("UNITY_EDITOR")]
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string memberName;

        
        public HideIfAttribute(string memberName)
        {
            this.memberName = memberName;
        }
    }
}