using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Framework.UI
{
    public class LongPress : UIBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [FormerlySerializedAs("onLongPress")]
        [SerializeField]
        private UnityEvent m_OnLongPress;
        [SerializeField, Range(0, 5)]
        private float LONG_PRESS_TIME = 0.5f;
        [SerializeField]
        private Button interceptBtn;

        public UnityEvent OnLongPress
        {
            get { return m_OnLongPress; }
            set { m_OnLongPress = value; }
        }

        private float pressTime = -1;
        void Update()
        {
            if (pressTime > 0 && Time.realtimeSinceStartup - pressTime > LONG_PRESS_TIME && m_OnLongPress != null)
            {
                Debug.Log("long press");
                pressTime = -1;
                m_OnLongPress.Invoke();
                if (interceptBtn != null)
                {
                    interceptBtn.enabled = false;
                    interceptBtn.enabled = true;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("on pointer down");
            pressTime = Time.realtimeSinceStartup;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("on pointer up");
            pressTime = -1;
        }
    }
}

