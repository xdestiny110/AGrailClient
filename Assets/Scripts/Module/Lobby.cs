using Framework;
using Framework.Network;
using Framework.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AGrail
{
    public class Lobby : Singleton<Lobby>, IMessageListener<MessageType>
    {
        public List<network.RoomListResponse.RoomInfo> RoomInfo = null;
        public network.RoomListResponse.RoomInfo SelectRoom { get; private set; }

        public Lobby() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.ROOMLISTRESPONSE, this);
        }

        public void GetRoomList()
        {
            var proto = new network.RoomListRequest() { role_strategy = network.ROLE_STRATEGY.ROLE_STRATEGY_ALL };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.ROOMLISTREQUEST });
            RoomInfo = null;
            MessageSystem<MessageType>.Notify(MessageType.RoomList);
        }

        public void JoinRoom(network.RoomListResponse.RoomInfo roomInfo, string password = null)
        {
            var proto = new network.EnterRoomRequest() { room_id = roomInfo.room_id};
            if (!string.IsNullOrEmpty(password))
                proto.password = password;
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.ENTERROOMREQUEST });
            //其实这么写不太合适，但后端没有合适的协议
            SelectRoom = roomInfo;
        }
        
        public void CreateRoom()
        {

        }

        public void LeaveRoom()
        {
            var proto = new network.LeaveRoomRequest();
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.LEAVEROOMREQUEST });
            SelectRoom = null;
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.ROOMLISTRESPONSE:
                    var roomListProto = parameters[0] as network.RoomListResponse;
                    RoomInfo = roomListProto.rooms.OrderBy(t => t.playing).ThenBy(t => t.room_id).ToList();
                    MessageSystem<MessageType>.Notify(MessageType.RoomList);
                    break;
            }
        }
    }
}


