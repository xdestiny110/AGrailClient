using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Framework.UI
{
    public class Dragable : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, ICanvasRaycastFilter, IPointerDownHandler
    {
        private Canvas rootCanvas;
        private bool isRaycastValid = true;

        [Serializable]
        public class OnBeginDragEventTriiger : UnityEvent<GameObject, PointerEventData> { }
        [Serializable]
        public class OnEndDragEventTrigger : UnityEvent<GameObject, PointerEventData, bool> { }

        [FormerlySerializedAs("onBeginDrag")]
        [SerializeField]
        private OnBeginDragEventTriiger onBeginDrag;
        public OnBeginDragEventTriiger OnBeginDragEvent
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
        private OnBeginDragEventTriiger onDraging;
        public OnBeginDragEventTriiger OnDragingEvent
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
        private OnEndDragEventTrigger onEndDrag;
        public OnEndDragEventTrigger OnEndDragEvent
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

        [SerializeField]
        private bool bindToButtonInteractable = false;
        [SerializeField]
        private Button btn;

        protected override void Start()
        {
            rootCanvas = transform.root.GetComponent<Canvas>();
            if (rootCanvas == null)
                Debug.LogErrorFormat("Cannot find root canvas! name = {0}", gameObject.name);
            base.Start();
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
            if (isActiveAndEnabled && ((bindToButtonInteractable && btn.interactable) || !bindToButtonInteractable))
            {
                transform.parent = rootCanvas.transform;
                transform.SetAsLastSibling();
                isRaycastValid = false;
                if (OnBeginDragEvent != null)
                    OnBeginDragEvent.Invoke(gameObject, eventData);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isActiveAndEnabled && ((bindToButtonInteractable && btn.interactable) || !bindToButtonInteractable))
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
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (isActiveAndEnabled && ((bindToButtonInteractable && btn.interactable) || !bindToButtonInteractable))
            {
                var go = eventData.pointerCurrentRaycast.gameObject;
                bool flag = false;
                if (go != null && go.GetComponentInParent<Dropable>() != null)
                    flag = go.GetComponentInParent<Dropable>().OnDrop(gameObject, eventData);
                isRaycastValid = true;
                if (OnEndDragEvent != null)
                    OnEndDragEvent.Invoke(go, eventData, flag);
            }
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return isRaycastValid;
        }
    }
}


