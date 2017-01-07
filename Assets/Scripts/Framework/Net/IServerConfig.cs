namespace Framework.Network
{
    public interface IServerConfig
    {
        string ServerIP { get; }
        int Port { get; }
        int SendBufferSize { get; }
        int ReceiveBufferSize { get; }
    }
}
