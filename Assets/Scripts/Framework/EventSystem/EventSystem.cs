using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    namespace EventSystem
    {
        public static class EventSystem
        {
            private static Dictionary<EventType, List<IEventListener>> maps = new Dictionary<EventType, List<IEventListener>>();

            public static void Regist(EventType eventType, IEventListener listener)
            {
                if (!maps.ContainsKey(eventType))
                    maps.Add(eventType, new List<IEventListener>() { listener });
                else
                    maps[eventType].Add(listener);
            }

            public static void UnRegist(EventType eventType, IEventListener listener)
            {
                if (maps.ContainsKey(eventType))
                    maps[eventType].Remove(listener);
            }

            public static void UnRegist(IEventListener listener)
            {
                foreach (var v in maps.Keys)
                    UnRegist(v, listener);
            }

            public static void Notify(EventType eventType, params object[] parameters)
            {
                if (maps.ContainsKey(eventType))
                {
                    var listeners = new List<IEventListener>(maps[eventType]);
                    foreach (var v in listeners)
                        v.OnEventTrigger(eventType, parameters);
                }
            }
        }
    }
}


