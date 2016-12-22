namespace Framework
{
    namespace EventSystem
    {
        public interface IEventListener
        {
            void OnEventTrigger(EventType eventType, params object[] parameters);
        }
    }
}


