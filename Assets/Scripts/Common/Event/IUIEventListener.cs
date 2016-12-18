namespace EventSystem
{
    public interface IEventListener
    {
        void OnUIEventTrigger(EventType type, params object[] parameters);
    }
}