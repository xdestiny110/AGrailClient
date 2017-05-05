using UnityEngine;
using System.Collections;
using System;

namespace Framework.Network
{
    public abstract class SocketBase
    {
        protected readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();
        protected ICoder coder;
        protected IServerConfig serverConfig;

        public abstract bool Connected { get; }
        public abstract bool AutoReconnect { get; set; }
        public abstract void Connect();
        public abstract void Send(Protobuf protobuf);
        public abstract void Close();

        public SocketBase(IServerConfig serverConfig, ICoder coder)
        {
            this.serverConfig = serverConfig;
            this.coder = coder;
        }

        public void DoActions()
        {
            while (actions.Count > 0)
                actions.Dequeue()();
        }

        protected void onConnectSuccess()
        {
            UnityEngine.Debug.Log("OnConnectSuccess!");
            actions.Enqueue(() => { Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnConnect, true); });
        }

        protected void onConnectFail()
        {
            UnityEngine.Debug.Log("OnConnectFail!");
            actions.Enqueue(() => { Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnConnect, false); });
        }

        protected void onDisconnect()
        {
            UnityEngine.Debug.Log("OnDisconnect!");
            actions.Enqueue(() => { Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnDisconnect); });
        }

        protected void onReconnect()
        {
            UnityEngine.Debug.Log("OnDisconnect!");
            actions.Enqueue(() => { Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnReconnect); });
        }

        protected void beforeSendProto(Protobuf protobuf)
        {
            UnityEngine.Debug.LogFormat("Send protobuf {0}", protobuf.ProtoID);
        }

        protected void afterReceiveProto(Protobuf protobuf)
        {
            UnityEngine.Debug.LogFormat("Recv protobuf {0}", protobuf.ProtoID);
            var msgType = (Message.MessageType)Enum.Parse(typeof(Message.MessageType), protobuf.ProtoID.ToString());
            actions.Enqueue(() => { Message.MessageSystem<Message.MessageType>.Notify(msgType, protobuf.Proto); });
        }

    }
}


