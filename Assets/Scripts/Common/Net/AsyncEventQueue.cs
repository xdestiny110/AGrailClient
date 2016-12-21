using UnityEngine;
using System.Collections.Generic;

namespace EventSystem
{
    public class AsyncEventQueue : MonoBehaviour
    {
        private ConcurrentQueue<EventType> eventTypes = new ConcurrentQueue<EventType>();
        private ConcurrentQueue<object[]> eventParms = new ConcurrentQueue<object[]>();

        public static AsyncEventQueue Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            if(eventTypes.Count > 0)
            {
                EventSystem.Notify(eventTypes.Dequeue(), eventParms.Dequeue());
            }
        }

        public void Enqueue(EventType type, params object[] parameters)
        {
            eventTypes.Enqueue(type);
            eventParms.Enqueue(parameters);
        }
    }
}
