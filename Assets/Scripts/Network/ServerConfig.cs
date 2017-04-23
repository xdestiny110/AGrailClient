using Framework.Network;

namespace AGrail
{
    public class ServerConfig : IServerConfig
    {
        private const int port = 11116;
        private const string serverIP = "127.0.0.1";
            //"115.28.77.222";
        private const int receiveBufferSize = 8192;
        private const int sendBufferSize = 8192;
        private const int version = 20161210;

        public int Port
        {
            get
            {
                return port;
            }
        }

        public int ReceiveBufferSize
        {
            get
            {
                return receiveBufferSize;
            }
        }

        public int SendBufferSize
        {
            get
            {
                return sendBufferSize;
            }
        }

        public string ServerIP
        {
            get
            {
                return serverIP;
            }
        }
    }
}


