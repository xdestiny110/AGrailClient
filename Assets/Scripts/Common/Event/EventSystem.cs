using System.Collections.Generic;

namespace EventSystem
{  
    public static class EventSystem
    {
        static Dictionary<EventType, List<IEventListener>> maps = new Dictionary<EventType, List<IEventListener>>();

        public static void Regist(EventType type, IEventListener listener)
        {
            if (!maps.ContainsKey(type))
                maps.Add(type, new List<IEventListener>());
            if (!maps[type].Contains(listener))
                maps[type].Add(listener);
        }

        public static void UnRegist(EventType type, IEventListener listener)
        {
            List<IEventListener> usrs;
            if(maps.TryGetValue(type, out usrs))
            {
                if (usrs.Contains(listener))
                    usrs.Remove(listener);
            }
        }

        public static void Notify(EventType type, params object[] parameters)
        {
            List<IEventListener> usrs;
            if (maps.TryGetValue(type, out usrs))
            {
                foreach (IEventListener usr in usrs)
                {
                    usr.OnEventTrigger(type, parameters);
                }
            }
        }
    }
}