using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.UI
{
    public class Dragable : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, ICanvasRaycastFilter
    {
        private Canvas rootCanvas;
        private bool isRaycastValid = true;

        [SerializeField]
        public class OnDragEndEventTriiger : UnityEvent<GameObject, PointerEventData> { }

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

        public void OnBeginDrag(PointerEventData eventData)
        {
            transform.parent = rootCanvas.transform;
            transform.SetAsLastSibling();
            isRaycastValid = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 mouseWorldPosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, 
                eventData.pressEventCamera, out mouseWorldPosition))
            {
                gameObject.transform.position = mouseWorldPosition;
            }
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


