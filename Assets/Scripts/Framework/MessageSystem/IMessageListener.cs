namespace Framework.Message
{
    public interface IMessageListener
    {
        void OnEventTrigger(string eventType, params object[] parameters);
    }

    public interface IMessageListener<T>
    {
        void OnEventTrigger(T eventType, params object[] parameters);
    }
}
