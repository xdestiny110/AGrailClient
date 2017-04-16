namespace Framework.Message
{
    public interface IMessageListener<T> where T : struct
    {
        void OnEventTrigger(T eventType, params object[] parameters);
    }
}


