using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Framework.Network
{
    public sealed class TCP
    {
        private ICoder coder;
        private IServerConfig serverConfig;
        private Socket socket;
        private bool startReconnect;
        private int reconnectDelay;
        private int reconnectDelayMin = 1000;
        private int reconnectDelayMax = 60000;
        private bool autoReconnect;
        private int reconnectTimes = 0;
        private int maxReconnectTimes = 3;

        private readonly ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

        public bool Connected { get { return null != socket && socket.Connected; } }

        public bool AutoReconnect
        {
            get { return autoReconnect; }
            set
            {
                autoReconnect = value;
                if (autoReconnect)
                {
                    if (!Connected)
                    {
                        reconnectDelay = 0;
                        startReconnect = true;
                        closeAndReconnect(socket);
                    }
                }
                else
                    startReconnect = false;
            }
        }

        public TCP(IServerConfig serverConfig, ICoder coder, bool reconnect = true, int maxReconnectTimes = 3)
        {
            this.serverConfig = serverConfig;
            this.coder = coder;
            autoReconnect = reconnect;
            this.maxReconnectTimes = maxReconnectTimes;
        }

        public void Connect()
        {
            if(socket != null)
            {
                UnityEngine.Debug.LogWarning("Socket has been established!");
                return;
            }

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendBufferSize = serverConfig.SendBufferSize,
                    ReceiveBufferSize = serverConfig.ReceiveBufferSize
                };
                socket.BeginConnect(serverConfig.ServerIP, serverConfig.Port,
                    (ar) =>
                    {
                        try
                        {
                            socket.EndConnect(ar);
                            onConnectSuccess();
                            StateObject state = new StateObject() { workSocket = (Socket)ar.AsyncState };
                            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, beginReceiveCallback, state);
                            reconnectDelay = 0;
                            startReconnect = false;
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.LogWarning("TCP.Connect failed:");
                            UnityEngine.Debug.LogException(ex);
                            closeAndReconnect(socket);
                        }
                    }, socket);
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogWarning("TCP.Connect failed :");
                UnityEngine.Debug.LogException(ex);
            }
        }

        public void Close()
        {
            if(socket!=null && socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            actions.clear();
            startReconnect = false;
            UnityEngine.Debug.Log("Disconnect.");
        }

        public void Send(Protobuf protobuf)
        {
            if(socket == null || !socket.Connected)
            {
                UnityEngine.Debug.LogError("Socket was not established!");
                return;
            }
            beforeSendProto(protobuf);
            var data = coder.Encode(protobuf);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, beginSendCallback, socket);
        }

        public void DoActions()
        {
            while (actions.Count > 0)
                actions.Dequeue()();
        }

        private void closeAndReconnect(Socket socket)
        {
            if (socket != this.socket)
                return;

            Close();
            if (autoReconnect && reconnectTimes < maxReconnectTimes)
            {
                if (reconnectDelay == 0)
                {
                    reconnectDelay = reconnectDelayMin;
                }
                else
                {
                    reconnectDelay *= 2;
                    if (reconnectDelay > reconnectDelayMax)
                        reconnectDelay = reconnectDelayMax;
                }
                startReconnect = true;
                reconnectTimes++;
                Connect();
                actions.Enqueue(onReconnect);
            }
            else
                actions.Enqueue(onConnectFail);
        }

        private void beginReceiveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket socket = state.workSocket;

            try
            {
                int readLength = socket.EndReceive(ar);
                if(readLength > 0)
                {
                    UnityEngine.Debug.Log("TCP Receive data length = " + readLength);
                    var data = new byte[readLength];
                    Array.Copy(state.buffer, 0, data, 0, readLength);
                    // 不处理粘包、分包等问题，在ICoder中处理
                    // 并且ICoder需要自己保留本次的数据
                    List<Protobuf> proto;
                    if (coder.Decode(data, out proto))
                    {
                        foreach (var v in proto)
                            afterReceiveProto(v);
                    }
                }
                // 继续接收
                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, beginReceiveCallback, state);
            }
            catch(SocketException ex)
            {
                UnityEngine.Debug.LogWarning("TCP.Receive failed :");
                UnityEngine.Debug.LogException(ex);
            }
        }

        private void beginSendCallback(IAsyncResult ar)
        {
            try
            {
                var sock = (Socket)ar.AsyncState;
                sock.EndSend(ar);
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.LogError("TCP.beginSend failed: ");
                UnityEngine.Debug.LogException(ex);
            }
        }

        private void onConnectSuccess()
        {
            actions.Enqueue(() => {
                UnityEngine.Debug.Log("OnConnectSuccess!");
                Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnConnect, true);
            });
        }

        private void onConnectFail()
        {
            actions.Enqueue(() =>
            {
                UnityEngine.Debug.Log("OnConnectFail!");
                Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnConnect, false);
            });
        }

        private void onDisconnect()
        {
            actions.Enqueue(() =>
            {
                UnityEngine.Debug.Log("OnDisconnect!");
                Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnDisconnect);
            });
        }

        private void onReconnect()
        {
            actions.Enqueue(() =>
            {
                UnityEngine.Debug.Log("OnDisconnect!");
                Message.MessageSystem<Message.MessageType>.Notify(Message.MessageType.OnReconnect);
            });
        }

        private void beforeSendProto(Protobuf protobuf)
        {
            UnityEngine.Debug.LogFormat("Send protobuf {0}", protobuf.ProtoID);
        }

        private void afterReceiveProto(Protobuf protobuf)
        {
            var msgType = (Message.MessageType)Enum.Parse(typeof(Message.MessageType), protobuf.ProtoID.ToString());
            actions.Enqueue(() =>
            {
                UnityEngine.Debug.LogFormat("Recv protobuf {0}", protobuf.ProtoID);
                Message.MessageSystem<Message.MessageType>.Notify(msgType, protobuf.Proto);
            });
        }
    }

    public class StateObject
    {
        // Client  socket.
        public Socket workSocket;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public MemoryStream stream = new MemoryStream();
    }

}


