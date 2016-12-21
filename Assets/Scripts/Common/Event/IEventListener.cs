namespace EventSystem
{
    public interface IEventListener
    {
        void OnEventTrigger(EventType type, params object[] parameters);
    }
}