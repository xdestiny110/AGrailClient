using System;
using System.Collections;
using EventSystem;

namespace Network
{
    public class NetworkManager : IEventListener
    {
        protected static NetworkManager _instance;
        protected static object locker = new object();
        public static NetworkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                            _instance = new NetworkManager();
                    }
                }
                return _instance;
            }
        }

        public enum ConnectState
        {
            NotStart,
            Connecting,
            Connected,
            Disconected,
        }

        protected TCP _tcp;
        private ConnectState _state;

        public ConnectState State
        {
            get { return _state; }
        }

        protected NetworkManager()
        {
            _tcp = new TCP();
            _state = ConnectState.NotStart;

            //懒得写unregist了
            EventSystem.EventSystem.Regist(EventType.SocketResponse, this);
        }

        public void Connect()
        {
            _state = ConnectState.Connecting;
            _tcp.Connect(ServerConfig.Ip, ServerConfig.Port);
        }

        public void Disconnect()
        {
            _tcp.Disconnect();
            _state = ConnectState.Disconected;
        }

        public void Send(System.IO.Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            Array.Copy((stream as System.IO.MemoryStream).GetBuffer(), bytes, stream.Length);
            _tcp.Send(bytes);
        }

        public void OnUIEventTrigger(EventType type, params object[] parameters)
        {
            if (type == EventType.SocketResponse)
            {
                if ((bool)parameters[0])
                    _state = ConnectState.Connected;
                else
                    _state = ConnectState.Disconected;
            }
        }
    }
}


