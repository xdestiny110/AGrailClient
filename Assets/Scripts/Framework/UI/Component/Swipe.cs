using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.UI
{
    public class Swipe : UIBehaviour, IPointerDownHandler
    {
        [Serializable]
        public class OnSwipeEventTriiger : UnityEvent<GameObject, SwipeDirection> { }

        [SerializeField]
        private float SwipeThreshold = 200;
        [FormerlySerializedAs("onSwipe")]
        [SerializeField]
        private OnSwipeEventTriiger m_OnSwipe = null;

        public OnSwipeEventTriiger OnSwipe
        {
            get { return m_OnSwipe; }
            set { m_OnSwipe = value; }
        }

        private bool isSwipeStart = false;
        private Vector2 startPos = Vector2.zero;
        private Vector2 currentPos = Vector2.zero;

        public void OnPointerDown(PointerEventData eventData)
        {
            isSwipeStart = true;
            startPos = eventData.position;
            currentPos = Vector2.zero;
        }

        void Update()
        {
            if (isSwipeStart && Input.GetButton("Fire1"))
            {
                currentPos = Input.mousePosition;
            }
            if (isSwipeStart && Input.GetButtonUp("Fire1"))
            {
                var diff = currentPos - startPos;
                Debug.LogFormat(diff.magnitude.ToString());
                if (diff.magnitude > SwipeThreshold && OnSwipe != null)
                {
                    var angle = Math.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                    Debug.LogFormat(angle.ToString());
                    if (Math.Abs(angle) <= 181 && Math.Abs(angle) >= 165)
                        OnSwipe.Invoke(gameObject, SwipeDirection.Left);
                    else if ((Math.Abs(angle) <= 15))
                        OnSwipe.Invoke(gameObject, SwipeDirection.Right);
                    else if (angle <= 105 && angle >= 75)
                        OnSwipe.Invoke(gameObject, SwipeDirection.Up);
                    else if (angle >= -105 && angle <= -75)
                        OnSwipe.Invoke(gameObject, SwipeDirection.Down);
                }
                isSwipeStart = false;
            }
        }

        [Serializable]
        public enum SwipeDirection
        {
            None,
            Up,
            Down,
            Left,
            Right
        }
    }
}
