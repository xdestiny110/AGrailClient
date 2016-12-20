using System;
using System.Collections;
using EventSystem;

namespace Network
{
    public class NetworkManager : IEventListener
    {
        protected static object locker = new object();
        protected static NetworkManager instance;
        public static NetworkManager Instance
        {
            get
            {
                if (null == instance)
                {
                    lock (locker)
                    {
                        if (null == instance)
                            instance = new NetworkManager();
                    }
                }
                return instance;
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

        public void Send(ProtoBuf.IExtensible protobuf, ProtoNameIds id)
        {
            var serializeStream = new System.IO.MemoryStream();
            ProtoSerializer.Serialize(id, serializeStream, protobuf);
            var sendStream = Utils.SerializeStreamToSendStream(serializeStream, id);

            byte[] bytes = new byte[sendStream.Length];
            Array.Copy((sendStream as System.IO.MemoryStream).GetBuffer(), bytes, sendStream.Length);
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


