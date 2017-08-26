using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.UI
{
    public class Dropable : UIBehaviour
    {
        [Serializable]
        public class OnDropEventTriiger : UnityEvent<GameObject, PointerEventData> { }

        [FormerlySerializedAs("onDrop")]
        [SerializeField]
        private OnDropEventTriiger onDrop = new OnDropEventTriiger();

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

        public void OnDrop(GameObject go, PointerEventData d)
        {
            if (OnDropEvent != null)
                OnDropEvent.Invoke(go, d);
        }
    }
}