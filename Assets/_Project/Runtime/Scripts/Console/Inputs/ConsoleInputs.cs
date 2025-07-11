using UnityEngine;

namespace DeveloperConsole
{
    public class ConsoleInputs : MonoBehaviour
    {
        #region Variables

        [Tooltip("Inputs enable even when the console is disabled")]
        [SerializeReference, SubclassPicker] private BaseInputBehaviour[] _persistantInputs;
        [SerializeReference, SubclassPicker] private BaseInputBehaviour[] _nonPersistantInputs;

        #endregion


        #region Core Behaviours

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
            
            ConsoleBehaviour.instance.onShow += OnConsoleShow;
            ConsoleBehaviour.instance.onHide += OnConsoleHide;
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
            
            ConsoleBehaviour.instance.onShow -= OnConsoleShow;
            ConsoleBehaviour.instance.onHide -= OnConsoleHide;
        }

        #endregion

        
        #region Methods

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

        #endregion
    }
}