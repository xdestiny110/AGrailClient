using System;
using EventSystem;
using System.IO;

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
            var serializeStream = new MemoryStream();
            ProtoSerializer.Serialize(id, serializeStream, protobuf);
            var sendStream = serializeStreamToSendStream(serializeStream, id);

            byte[] bytes = new byte[sendStream.Length];
            Array.Copy((sendStream as MemoryStream).GetBuffer(), bytes, sendStream.Length);
            _tcp.Send(bytes);
        }

        public void OnEventTrigger(EventType type, params object[] parameters)
        {
            if (type == EventType.SocketResponse)
            {
                if ((bool)parameters[0])
                    _state = ConnectState.Connected;
                else
                    _state = ConnectState.Disconected;
            }
        }

        public void Process(long maxMiliSecond)
        {
            _tcp.Process(maxMiliSecond);
        }

        private MemoryStream serializeStreamToSendStream(MemoryStream serializeStream, ProtoNameIds type)
        {
            MemoryStream sendStream = new MemoryStream();
            sendStream.SetLength(serializeStream.Length + 8);
            sendStream.Position = 0;
            sendStream.Write(BitConverter.GetBytes((int)sendStream.Length - 4), 0, 4);
            sendStream.Write(BitConverter.GetBytes((short)(sendStream.Length - 4)), 0, 2);
            sendStream.Write(BitConverter.GetBytes((short)type), 0, 2);
            sendStream.Write(serializeStream.GetBuffer(), 0, (int)serializeStream.Length);
            return sendStream;
        }
    }
}


