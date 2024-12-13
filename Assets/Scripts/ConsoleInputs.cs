using DeveloperConsole.Inputs;
using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleInputs : MonoBehaviour
    {
        [Tooltip("Inputs enable even when the console is disabled")]
        [SerializeReference, SubclassPicker] private BaseInputBehaviour[] _persistantInputs;
        [SerializeReference, SubclassPicker] private BaseInputBehaviour[] _nonPersistantInputs;


        private void Start()
        {
            foreach (var input in _persistantInputs)
            {
                input.Init();
                input.RegisterListener();
                input.Enable();
            }
            
            foreach (var input in _nonPersistantInputs)
            {
                input.Init();
                input.RegisterListener();
            }
            
            ConsoleBehaviour.instance.onShowEvent += OnConsoleShow;
            ConsoleBehaviour.instance.onHideEvent += OnConsoleHide;
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
            
            ConsoleBehaviour.instance.onShowEvent -= OnConsoleShow;
            ConsoleBehaviour.instance.onHideEvent -= OnConsoleHide;
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