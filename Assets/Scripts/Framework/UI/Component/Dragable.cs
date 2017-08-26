using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.UI
{
    public class Dragable : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, ICanvasRaycastFilter, IPointerDownHandler
    {
        private Canvas rootCanvas;
        private bool isRaycastValid = true;

        [Serializable]
        public class OnDragEndEventTriiger : UnityEvent<GameObject, PointerEventData> { }

        [FormerlySerializedAs("onBeginDrag")]
        [SerializeField]
        private OnDragEndEventTriiger onBeginDrag;
        public OnDragEndEventTriiger OnBeginDragEvent
        {
            get
            {
                return onBeginDrag;
            }

            set
            {
                onBeginDrag = value;
            }
        }

        [FormerlySerializedAs("onDraging")]
        [SerializeField]
        private OnDragEndEventTriiger onDraging;
        public OnDragEndEventTriiger OnDragingEvent
        {
            get
            {
                return onDraging;
            }

            set
            {
                onDraging = value;
            }
        }

        [FormerlySerializedAs("onEndDrag")]
        [SerializeField]
        private OnDragEndEventTriiger onEndDrag;
        public OnDragEndEventTriiger OnEndDragEvent
        {
            get
            {
                return onEndDrag;
            }

            set
            {
                onEndDrag = value;
            }
        }

        protected override void Awake()
        {
            rootCanvas = transform.root.GetComponent<Canvas>();
            if (rootCanvas == null)
                Debug.LogErrorFormat("Cannot find root canvas! name = {0}", gameObject.name);
            base.Awake();
        }

        private Vector3 lastMousePos = Vector3.zero;
        public void OnPointerDown(PointerEventData eventData)
        {
            Vector3 mouseWorldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out mouseWorldPosition))
                lastMousePos = mouseWorldPosition;
            else
                lastMousePos = Vector3.zero;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.parent = rootCanvas.transform;
            transform.SetAsLastSibling();
            isRaycastValid = false;
            if (OnBeginDragEvent != null)
                OnBeginDragEvent.Invoke(gameObject, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mouseWorldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position,
                eventData.pressEventCamera, out mouseWorldPosition))
            {
                if (lastMousePos != Vector3.zero)
                    gameObject.transform.position += mouseWorldPosition - lastMousePos;
                lastMousePos = mouseWorldPosition;
            }
            if (OnDragingEvent != null)
                OnDragingEvent.Invoke(gameObject, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            var go = eventData.pointerCurrentRaycast.gameObject;
            if (go != null && go.GetComponentInParent<Dropable>() != null)
                go.GetComponentInParent<Dropable>().OnDrop(gameObject, eventData);
            isRaycastValid = true;
            if (OnEndDragEvent != null)
                OnEndDragEvent.Invoke(go, eventData);
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return isRaycastValid;
        }
    }
}


