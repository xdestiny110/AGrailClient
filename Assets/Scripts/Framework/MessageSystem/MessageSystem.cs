using System;
using System.Collections.Generic;
using AGrail;

namespace Framework
{
    namespace Message
    {
        public static class MessageSystem<T> where T : struct
        {
            private static Dictionary<T, List<IMessageListener<T>>> maps = new Dictionary<T, List<IMessageListener<T>>>();

            public static void Regist(T eventType, IMessageListener<T> listener)
            {
                if (!maps.ContainsKey(eventType))
                    maps.Add(eventType, new List<IMessageListener<T>>() { listener });
                else
                    maps[eventType].Add(listener);
            }

            public static void UnRegist(T eventType, IMessageListener<T> listener)
            {
                if (maps.ContainsKey(eventType))
                    maps[eventType].Remove(listener);
            }

            public static void UnRegist(IMessageListener<T> listener)
            {
                foreach (var v in maps.Keys)
                    UnRegist(v, listener);
            }

            public static void Notify(T eventType, params object[] parameters)
            {
                //UnityEngine.Debug.LogFormat("MessageSystem notify {0}", eventType.ToString());
                if (maps.ContainsKey(eventType))
                {
                    var listeners = new List<IMessageListener<T>>(maps[eventType]);
                    foreach (var v in listeners)
                    {
                        v.OnEventTrigger(eventType, parameters);
                    }
                }
            }
        }
    }
}


