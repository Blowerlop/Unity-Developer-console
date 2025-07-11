using UnityEngine;
using UnityEngine.EventSystems;

namespace DeveloperConsole
{
    public class DraggableWindow : MonoBehaviour, IDragHandler
    {
        #region Variables

        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _target;
        [SerializeField] private bool _selfTarget;

        #endregion


        #region Core Behaviours

        private void Awake()
        {
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
            if (_selfTarget) _target = GetComponent<RectTransform>();
        }

        #endregion


        #region Methods

        public void OnDrag(PointerEventData eventData)
        {
            _target.anchoredPosition += eventData.delta / _canvas.scaleFactor;
        }

        #endregion
    }
}
