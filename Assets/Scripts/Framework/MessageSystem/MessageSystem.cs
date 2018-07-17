using System.Collections.Generic;

namespace Framework
{
    namespace Message
    {
        public static class MessageSystem
        {
            private static Dictionary<string, List<IMessageListener>> maps = new Dictionary<string, List<IMessageListener>>();

            public static void Regist(string eventType, IMessageListener listener)
            {
                if (!maps.ContainsKey(eventType))
                    maps.Add(eventType, new List<IMessageListener>() { listener });
                else
                    maps[eventType].Add(listener);
            }

            public static void UnRegist(string eventType, IMessageListener listener)
            {
                if (maps.ContainsKey(eventType))
                    maps[eventType].Remove(listener);
            }

            public static void UnRegist(IMessageListener listener)
            {
                foreach (var v in maps.Keys)
                    UnRegist(v, listener);
            }

            public static void Notify(string eventType, params object[] parameters)
            {
                if (maps.ContainsKey(eventType))
                {
                    var listeners = new List<IMessageListener>(maps[eventType]);
                    foreach (var v in listeners)
                    {
                        v.OnEventTrigger(eventType, parameters);
                    }
                }
            }
        }

        public static class MessageSystem<T> where T : struct
        {
            public static void Regist(T eventType, IMessageListener listener)
            {
                MessageSystem.Regist(eventType.ToString(), listener);
            }

            public static void UnRegist(T eventType, IMessageListener listener)
            {
                MessageSystem.UnRegist(eventType.ToString(), listener);
            }

            public static void UnRegist(IMessageListener listener)
            {
                MessageSystem.UnRegist(listener);
            }

            public static void Notify(T eventType, params object[] parameters)
            {
                MessageSystem.Notify(eventType.ToString(), parameters);
            }
        }
    }
}
