using UnityEngine;
using UnityEngine.EventSystems;

namespace Project
{
    public class DraggableWindow : MonoBehaviour, IDragHandler
    {
        private Canvas _canvas;
        [SerializeField] private RectTransform _target;
        [SerializeField] private bool _selfTarget;

        
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_selfTarget) _target = GetComponent<RectTransform>();
        }

        
        public void OnDrag(PointerEventData eventData)
        {
            _target.anchoredPosition += (eventData.delta / _canvas.scaleFactor);
        }
    }
}
