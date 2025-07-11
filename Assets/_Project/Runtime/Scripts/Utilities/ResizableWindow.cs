using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DeveloperConsole
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
    
    public struct Edges
    {
        public RectTransform.Edge? horizontalEdge;
        public RectTransform.Edge? verticalEdge;
    }
    
    public class ResizableWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
    {
        #region Variables

        [Header("Target")]
        private Canvas _canvas;
        [SerializeField] private bool _selfTarget;
        [SerializeField, HideIf(nameof(_selfTarget))] private RectTransform _target;

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

        #endregion


        #region Core Behaviours

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

        #endregion


        #region Methods

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
            
            Edges edges = GetEdgeByHandlerType();
            ResizeWindow();
            return;


            // --- Local methods ---
            Edges GetEdgeByHandlerType()
            {
                Edges edges = default;
                
                switch (_handler)
                {
                    case HandlerType.TopRight:
                        edges.horizontalEdge = RectTransform.Edge.Left;
                        edges.verticalEdge = RectTransform.Edge.Bottom;
                        break;
                
                    case HandlerType.Right:
                        edges.horizontalEdge = RectTransform.Edge.Left;
                        break;
                
                    case HandlerType.BottomRight:
                        edges.horizontalEdge = RectTransform.Edge.Left;
                        edges.verticalEdge = RectTransform.Edge.Top;
                        break;
                
                    case HandlerType.Bottom:
                        edges.verticalEdge = RectTransform.Edge.Top;
                        break;
                
                    case HandlerType.BottomLeft:
                        edges.horizontalEdge = RectTransform.Edge.Right;
                        edges.verticalEdge = RectTransform.Edge.Top;
                        break;
                
                    case HandlerType.Left:
                        edges.horizontalEdge = RectTransform.Edge.Right;
                        break;
                
                    case HandlerType.TopLeft:
                        edges.horizontalEdge = RectTransform.Edge.Right;
                        edges.verticalEdge = RectTransform.Edge.Bottom;
                        break;
                
                    case HandlerType.Top:
                        edges.verticalEdge = RectTransform.Edge.Bottom;
                        break;
                
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return edges;
            }
            
            void ResizeWindow()
            {
                Vector2 sizeDelta;
                Vector2 scaledEventDataDelta = eventData.delta / _canvas.scaleFactor;
                
                
                if (edges.horizontalEdge != null)
                {
                    sizeDelta = _target.sizeDelta;
                    float newWidth;
                    float deltaPosX;
                    
                    if (edges.horizontalEdge == RectTransform.Edge.Right)
                    {
                        newWidth = sizeDelta.x - scaledEventDataDelta.x;
                        
                        deltaPosX = -(newWidth - sizeDelta.x) * _target.pivot.x;
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
                    }

                    _target.sizeDelta = new Vector2(newWidth, _target.sizeDelta.y);
                    _target.anchoredPosition += new Vector2(deltaPosX, 0);
                }
                if (edges.verticalEdge != null)
                {
                    sizeDelta = _target.sizeDelta;
                    float newHeight;
                    float deltaPosY;
                    
                    if (edges.verticalEdge == RectTransform.Edge.Top)
                    {
                        newHeight = sizeDelta.y - scaledEventDataDelta.y;
                        deltaPosY = -(newHeight - sizeDelta.y) * _target.pivot.y;
                    }
                    else
                    {
                        newHeight = sizeDelta.y + scaledEventDataDelta.y;
                        deltaPosY = (newHeight - sizeDelta.y) * _target.pivot.y;
                    }

                    _target.sizeDelta = new Vector2(_target.sizeDelta.x, newHeight);
                    _target.anchoredPosition += new Vector2(0, deltaPosY);
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

        #endregion
    }
}