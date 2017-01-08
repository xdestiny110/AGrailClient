namespace Framework.Message
{
    public interface IMessageListener
    {
        void OnEventTrigger(MessageType eventType, params object[] parameters);
    }
}


