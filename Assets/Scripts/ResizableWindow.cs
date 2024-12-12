using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project._200_Dev.UI
{
    public enum HandlerType
    {
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        TopLeft,
        Top
    }
    
    public class ResizableWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
    {
        [Header("Target")]
        private Canvas _canvas;
        [SerializeField] private bool _selfTarget;
        [SerializeField] private RectTransform _target;

        [Header("Resizable Parameters")]
        [SerializeField] private HandlerType _handler;
        [SerializeField] private bool _clampSize;
        [SerializeField] private bool _clampMinimum = true;
        [SerializeField] private Vector2 _minimumDimensions = new(50, 50);
        [SerializeField] private bool _clampMaximum = true;
        [SerializeField] private Vector2 _maximumDimensions = new(800, 800);

        [Header("Hacks :(")]
        private RectTransform _rectTransform;
        private Vector2 _defaultAnchorPosition;
        private Vector2 _defaultSizeDelta;
        private static bool _dragged;
        private static ResizableWindow _draggedUser;

        
        private void Awake()
        {
            if (_selfTarget) _target = GetComponent<RectTransform>();
            
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            _defaultAnchorPosition = GetComponent<RectTransform>().anchoredPosition;
            _defaultSizeDelta = GetComponent<RectTransform>().sizeDelta;
            
            float originalWidth = _target.rect.width;
            float originalHeight = _target.rect.height;
            _minimumDimensions = new Vector2 (0.1f * originalWidth, 0.1f * originalHeight);
            _maximumDimensions = new Vector2 (10f * originalWidth, 10f * originalHeight);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_dragged && _draggedUser != this) return;
            
            _rectTransform.sizeDelta = _defaultSizeDelta;
            _rectTransform.anchoredPosition = _defaultAnchorPosition;

            OnPointerEnterEvent();
        }
        
        protected virtual void OnPointerEnterEvent()
        {
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_dragged && _draggedUser != this) return;
            
            _rectTransform.sizeDelta = _defaultSizeDelta;
            _rectTransform.anchoredPosition = _defaultAnchorPosition;

            OnPointerExit();
        }

        protected virtual void OnPointerExit()
        {
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            _draggedUser = this;
            _dragged = true;
            
            _rectTransform.sizeDelta = _defaultSizeDelta;
            _rectTransform.anchoredPosition = _defaultAnchorPosition;
            
            _rectTransform.sizeDelta *= 100;
            _rectTransform.anchoredPosition *= 50;
            
            RectTransform.Edge? horizontalEdge = null; 
            RectTransform.Edge? verticalEdge = null;
            
            switch (_handler)
            {
                case HandlerType.TopRight:
                    horizontalEdge = RectTransform.Edge.Left;
                    verticalEdge = RectTransform.Edge.Bottom;
                    break;
                case HandlerType.Right:
                    horizontalEdge = RectTransform.Edge.Left;
                    break;
                case HandlerType.BottomRight:
                    horizontalEdge = RectTransform.Edge.Left;
                    verticalEdge = RectTransform.Edge.Top;
                    break;
                case HandlerType.Bottom:
                    verticalEdge = RectTransform.Edge.Top;
                    break;
                case HandlerType.BottomLeft:
                    horizontalEdge = RectTransform.Edge.Right;
                    verticalEdge = RectTransform.Edge.Top;
                    break;
                case HandlerType.Left:
                    horizontalEdge = RectTransform.Edge.Right;
                    break;
                case HandlerType.TopLeft:
                    horizontalEdge = RectTransform.Edge.Right;
                    verticalEdge = RectTransform.Edge.Bottom;
                    break;
                case HandlerType.Top:
                    verticalEdge = RectTransform.Edge.Bottom;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            ResizeWindow();
            return;


            // --- Local methods ---
            void ResizeWindow()
            {
                Vector2 sizeDelta;
                Vector2 scaledEventDataDelta = (eventData.delta / _canvas.scaleFactor);
                
                
                if (horizontalEdge != null)
                {
                    sizeDelta = _target.sizeDelta;
                    float newWidth;
                    float deltaPosX;
                    
                    if (horizontalEdge == RectTransform.Edge.Right)
                    {
                        newWidth = sizeDelta.x - scaledEventDataDelta.x;
                        
                        deltaPosX = -(newWidth - sizeDelta.x) * _target.pivot.x;
                        
                        _target.sizeDelta = new Vector2(newWidth, _target.sizeDelta.y);
                        _target.anchoredPosition += new Vector2(deltaPosX, 0);
                    }
                    else
                    {
                        if (_clampSize)
                        {
                            if (_clampMinimum)
                            {
                                newWidth = Mathf.Clamp(sizeDelta.x + scaledEventDataDelta.x,  _minimumDimensions.x, Mathf.Infinity);
                            }
                            else if (_clampMaximum)
                            {
                                newWidth = Mathf.Clamp(sizeDelta.x + scaledEventDataDelta.x, Mathf.Infinity, _maximumDimensions.x);
                            }
                            else return;
                        }
                        else
                        {
                            newWidth = sizeDelta.x + scaledEventDataDelta.x;
                        }
                        
                        deltaPosX = (newWidth - sizeDelta.x) * _target.pivot.x;

                        _target.sizeDelta = new Vector2(newWidth, _target.sizeDelta.y);
                        _target.anchoredPosition += new Vector2(deltaPosX, 0);
                    }
                }
                if (verticalEdge != null)
                {
                    sizeDelta = _target.sizeDelta;
                    float newHeight;
                    float deltaPosY;
                    
                    if (verticalEdge == RectTransform.Edge.Top)
                    {
                        newHeight = sizeDelta.y - scaledEventDataDelta.y;
                        deltaPosY = -(newHeight - sizeDelta.y) * _target.pivot.y;
                
                        _target.sizeDelta = new Vector2(_target.sizeDelta.x, newHeight);
                        _target.anchoredPosition += new Vector2(0, deltaPosY);
                    }
                    else
                    {
                        newHeight = sizeDelta.y + scaledEventDataDelta.y;
                        deltaPosY = (newHeight - sizeDelta.y) * _target.pivot.y;
                
                        _target.sizeDelta = new Vector2(_target.sizeDelta.x, newHeight);
                        _target.anchoredPosition += new Vector2(0, deltaPosY);
                    }
                }
            }
        }
        
        public void OnEndDrag(PointerEventData eventData)
        {
            _rectTransform.sizeDelta = _defaultSizeDelta;
            _rectTransform.anchoredPosition = _defaultAnchorPosition;
            
            _draggedUser = null;
            _dragged = false;
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticFields()
        {
            _draggedUser = null;
            _dragged = false;
        }
#endif
    }
}