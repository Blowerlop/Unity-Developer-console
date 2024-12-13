using System;
using UnityEngine;

namespace DeveloperConsole
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterResolverAttribute : Attribute
    {
        private Type _enumT;    
        
        public ParameterResolverAttribute(Type enumT)
        {
            _enumT = enumT;
        }

        public string[] Resolve()
        {
            return Enum.GetNames(_enumT);
        }
    }
}