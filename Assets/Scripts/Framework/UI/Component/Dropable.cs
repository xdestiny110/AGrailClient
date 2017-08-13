using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Framework.UI
{
    public class Dropable : UIBehaviour
    {
        [Serializable]
        public class OnDropEventTriiger : UnityEvent<GameObject, PointerEventData> { }

        [FormerlySerializedAs("onDrop")]
        [SerializeField]
        private OnDropEventTriiger onDrop = new OnDropEventTriiger();

        [SerializeField]
        private bool bindToButtonInteractable = false;
        [SerializeField]
        private Button btn;

        public OnDropEventTriiger OnDropEvent
        {
            get
            {
                return onDrop;
            }

            set
            {
                onDrop = value;
            }
        }

        public bool OnDrop(GameObject go, PointerEventData d)
        {
            if (isActiveAndEnabled && ((bindToButtonInteractable && btn.interactable) || !bindToButtonInteractable) && OnDropEvent != null)
            {
                OnDropEvent.Invoke(go, d);
                return true;
            }
            return false;
        }
    }
}