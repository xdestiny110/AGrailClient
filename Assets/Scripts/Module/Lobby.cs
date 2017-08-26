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
            BattleData.Instance.Reset();
            //其实这么写不太合适，但后端没有合适的协议
            SelectRoom = roomInfo;
            //防止多次点击
            MessageSystem<MessageType>.Notify(MessageType.EnterRoom);
        }

        public void CreateRoom(network.CreateRoomRequest proto)
        {
            SelectRoom = new network.RoomListResponse.RoomInfo()
            {
                allow_guest = proto.allow_guest,
                first_extension = proto.first_extension,
                second_extension = proto.second_extension,
                now_player = 1,
                max_player = proto.max_player,
                role_strategy = proto.role_strategy,
                has_password = proto.passwordSpecified,
                room_name = proto.room_name,
                seat_mode = proto.seat_mode,
                playing = false,
            };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.CREATEROOMREQUEST });
        }

        public void LeaveRoom()
        {
            var proto = new network.LeaveRoomRequest();
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.LEAVEROOMREQUEST });
            SelectRoom = null;
            BattleData.Instance.Reset();
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


