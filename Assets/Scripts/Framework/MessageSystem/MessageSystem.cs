using System.Collections.Generic;

namespace Framework
{
    namespace Message
    {
        public static class MessageSystem
        {
            private static Dictionary<MessageType, List<IMessageListener>> maps = new Dictionary<MessageType, List<IMessageListener>>();

            public static void Regist(MessageType eventType, IMessageListener listener)
            {
                if (!maps.ContainsKey(eventType))
                    maps.Add(eventType, new List<IMessageListener>() { listener });
                else
                    maps[eventType].Add(listener);
            }

            public static void UnRegist(MessageType eventType, IMessageListener listener)
            {
                if (maps.ContainsKey(eventType))
                    maps[eventType].Remove(listener);
            }

            public static void UnRegist(IMessageListener listener)
            {
                foreach (var v in maps.Keys)
                    UnRegist(v, listener);
            }

            public static void Notify(MessageType eventType, params object[] parameters)
            {
                if (maps.ContainsKey(eventType))
                {
                    var listeners = new List<IMessageListener>(maps[eventType]);
                    foreach (var v in listeners)
                        v.OnEventTrigger(eventType, parameters);
                }
            }
        }
    }
}


