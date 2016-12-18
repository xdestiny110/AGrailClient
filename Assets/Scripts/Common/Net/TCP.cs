using System.Net.Sockets;
using System.Net;
using System;
using System.IO;
using UnityEngine;
using network;
using System.Collections;

namespace Network
{
    public class TCP
    {
        protected Socket _socket;

        public void Connect(string ip, int port)  //连接服务器
        {
            if (_socket != null && _socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 30);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30);
            _socket.BeginConnect(ip, port, new AsyncCallback(ConnectCallback), _socket);
        }

        public void Disconnect()
        {
            if (_socket == null || !_socket.Connected)
                return;
            Debug.Log("关闭连接");
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                socket.EndConnect(ar);
            }
            catch (SocketException e)
            {
                _socket = null;
                Debug.LogError("服务器连接失败:" + e.ToString());
                EventSystem.EventSystem.NotifyAsync(EventSystem.EventType.SocketResponse, false);
                return;
            }
            Debug.Log("服务器连接成功!");
            //注册消息接收
            StateObject state = new StateObject();
            state.workSocket = socket;
            socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            EventSystem.EventSystem.NotifyAsync(EventSystem.EventType.SocketResponse, true);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket socket = state.workSocket;

            int readLength = 0;
            try
            {
                readLength = socket.EndReceive(ar);
            }
            catch (SocketException e)
            {
                _socket = null;
                Debug.LogError("接收消息失败.");
                Debug.LogException(e);
                return;
            }
            //Debug.Log("接收消息成功");
            if (readLength > 0)
            {
                // 将新接收的数据写入stream尾
                state.stream.Position = state.stream.Length;
                state.stream.Write(state.buffer, 0, readLength);

                // 两个字节文件头表示长度
                if (state.stream.Length > 2)
                {
                    state.stream.Position = 0;

                    // 4个字节协议长度
                    byte[] b = state.stream.GetBuffer();
                    int sumLen = Utils.ReadBytes(state.stream, 4);

                    //读取完成 解析
                    //Debug.Log("sum len = " + sumLen + " stream len = " + state.stream.Length);
                    while (sumLen + 4 <= state.stream.Length)
                    {
                        state.stream.Position = 6;
                        int protoId = Utils.ReadBytes(state.stream, 2);
                        state.stream.Position = 8;
                        MemoryStream protoStream = new MemoryStream();
                        //开始位置，长度                   
                        protoStream.Write(state.stream.ToArray(), 8, sumLen - 4);
                        protoStream.Position = 0;

                        //姑且在这里依据类型调用回调函数，先把整套流程走通
                        switch ((ProtoNameIds)protoId)
                        {
                            //case ProtoNameIds.LOGINRESPONSE:
                            //    Debug.Log("login response");
                            //    LoginResponse loginResponse = (LoginResponse)ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream);
                            //    Debug.Log("set login callback timer");
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.LOGINRESPONSE, loginResponse); });
                            //    Debug.Log("wait timer to trigger");
                            //    break;
                            //case ProtoNameIds.LOGOUTRESPONSE:
                            //    break;
                            //case ProtoNameIds.ROOMLISTRESPONSE:
                            //    //Debug.Log("room list response");
                            //    RoomListResponse roomListResponse = (RoomListResponse)ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream);
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.ROOMLISTRESPONSE, roomListResponse); });
                            //    break;
                            //case ProtoNameIds.GAMEINFO:
                            //    //获取房间已有的信息，有人加入时、战局变化时也会触发
                            //    //Debug.Log("get game info");
                            //    try
                            //    {
                            //        GameInfo gameInfo = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as GameInfo;
                            //        new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.GAMEINFO, gameInfo); });
                            //    }catch(Exception ex)
                            //    {
                            //        Debug.Log(ex);
                            //    }                            
                            //    break;
                            //case ProtoNameIds.ROLEREQUEST:
                            //    //选择角色
                            //    //Debug.Log("choose role");
                            //    RoleRequest roleRequest = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as RoleRequest;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.ROLEREQUEST, roleRequest); });
                            //    break;
                            //case ProtoNameIds.TURNBEGIN:
                            //    //回合开始
                            //    //Debug.Log("turn begin");
                            //    TurnBegin turnBegin = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as TurnBegin;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.TURNBEGIN, turnBegin); });
                            //    break;
                            //case ProtoNameIds.COMMANDREQUEST:
                            //    //Debug.Log("command request");
                            //    CommandRequest cmdReq = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as CommandRequest;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.COMMANDREQUEST, cmdReq); });
                            //    break;
                            //case ProtoNameIds.CARDMSG:
                            //    var cardMsg = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as CardMsg;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.CARDMSG, cardMsg); });
                            //    break;
                            //case ProtoNameIds.ACTION:
                            //    break;
                            //case ProtoNameIds.RESPOND:
                            //    break;
                            //case ProtoNameIds.HITMSG:
                            //    //命中
                            //    var hitMsg = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as HitMsg;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.HITMSG, hitMsg); });
                            //    break;
                            //case ProtoNameIds.HURTMSG:
                            //    //最终伤害
                            //    var hurtMsg = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as HurtMsg;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.HURTMSG, hurtMsg); });
                            //    break;
                            //case ProtoNameIds.SKILLMSG:
                            //    //技能消息
                            //    var skillMsg = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as SkillMsg;
                            //    new Timer(0.01f, () => { EventSystem.TriggerEvent(ProtoNameIds.SKILLMSG, skillMsg); });
                            //    break;
                            //case ProtoNameIds.GOSSIP:
                            //    //聊天
                            //    break;
                            //case ProtoNameIds.ERROR:
                            //    Error error = ProtoSerializer.ParseFrom((ProtoNameIds)protoId, protoStream) as Error;
                            //    switch (error.id)
                            //    {
                            //        case 29:
                            //            Debug.Log("玩家" + error.dst_id + "离开房间");
                            //            new Timer(0.01f, () => { EventSystem.TriggerEvent(EventSystemEnum.PLAYER_LEAVE, error.dst_id); });
                            //            break;
                            //        case 31:
                            //            Debug.Log("密码不对");
                            //            new Timer(0.01f, () => { EventSystem.TriggerEvent(EventSystemEnum.WRONG_PASSWARD, null); });                                    
                            //            break;
                            //        default:
                            //            Debug.Log("错误代码" + error.id);
                            //            break;
                            //    }
                            //    break;
                            //default:
                            //    Debug.LogError("返回类型错误, 错误类型代码: " + (ProtoNameIds)protoId);
                            //    break;
                        }

                        state.stream.Position = sumLen + 4;
                        //读取剩下的字节信息
                        if (state.stream.Length - state.stream.Position > 0)
                        {
                            MemoryStream newStream = new MemoryStream();
                            newStream.Write(state.stream.ToArray(), (int)state.stream.Position, (int)(state.stream.Length - state.stream.Position));
                            state.stream = newStream;

                            if (state.stream.Length > 2)
                            {
                                state.stream.Position = 0;
                                sumLen = Utils.ReadBytes(state.stream, 4);
                            }
                            else break;
                        }
                        else
                        {
                            state.stream = new MemoryStream();
                            break;
                        }
                    }
                }
                // 继续接收
                socket.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
        }

        public void Send(byte[] msgString)
        {
            if (_socket == null || !_socket.Connected) return;
            _socket.BeginSend(msgString, 0, msgString.Length, 0, new AsyncCallback(SendCallback), _socket);
        }

        protected void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                handler.EndSend(ar);
                Debug.Log("消息推送成功");
            }
            catch (SocketException e)
            {
                Debug.LogError("消息推送失败.");
                Debug.LogException(e);
                return;
            }
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
