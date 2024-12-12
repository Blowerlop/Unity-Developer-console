using System;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DevelopperConsole
{
    public class ConsoleInputs : MonoBehaviour
    {
        [Tooltip("Input enable even when the console is disabled")]
        [SerializeReference, SubclassPicker] private BaseInput[] _persistantInputs;
        [SerializeReference, SubclassPicker] private BaseInput[] _nonPersistantInputs;


        private void Start()
        {
            foreach (var input in _persistantInputs)
            {
                input.RegisterListener();
                
                input.Enable();
            }
            
            foreach (var input in _nonPersistantInputs)
            {
                input.RegisterListener();
            }
            
            ConsoleBehaviour.instance.OnShowEvent += OnConsoleShow;
            ConsoleBehaviour.instance.OnHideEvent += OnConsoleHide;
        }
        
        private void OnDestroy()
        {
            foreach (var input in _persistantInputs)
            {
                input.UnRegisterListener();
                
                input.Disable();
            }
            
            foreach (var input in _nonPersistantInputs)
            {
                input.UnRegisterListener();
            }
            
            ConsoleBehaviour.instance.OnShowEvent -= OnConsoleShow;
            ConsoleBehaviour.instance.OnHideEvent -= OnConsoleHide;
        }
        
        private void OnConsoleShow()
        {
            foreach (var input in _nonPersistantInputs)
            {
                input.Enable();
            }
        }
        
        private void OnConsoleHide()
        {
            foreach (var input in _nonPersistantInputs)
            {
                input.Disable();
            }
        }
    }
}