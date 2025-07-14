using UnityEngine;

namespace DeveloperConsole
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region Variables

        [SerializeField] private bool _dontDestroyOnLoad = true;
        
        private static T _instance;
        public static T instance 
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false) return null;
#endif
                
                if (_instance == null) _instance = FindAnyObjectByType<T>();
                
                return _instance;
            }
        }

        #endregion

        
        #region Core Behaviours

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogError($"There is more than one instance of {this}");
                Destroy(this);
                return;
            }
            
            _instance = GetComponent<T>();
            
            if(_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        
        #region Methods

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticVariables()
        {
            _instance = null;
        }
#endif

        #endregion
    }
}
