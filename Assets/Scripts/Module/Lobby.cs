using Framework;
using Framework.Network;
using Framework.Message;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class Lobby : Singleton<Lobby>, IMessageListener
    {
        public List<network.RoomListResponse.RoomInfo> RoomInfo = null;
        public network.RoomListResponse.RoomInfo SelectRoom { get; private set; }

        public Lobby() : base()
        {
            MessageSystem.Regist(MessageType.ROOMLISTRESPONSE, this);
        }

        public void GetRoomList()
        {
            var proto = new network.RoomListRequest() { role_strategy = network.ROLE_STRATEGY.ROLE_STRATEGY_ALL };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.ROOMLISTREQUEST });
            RoomInfo = null;
            MessageSystem.Notify(MessageType.RoomList);
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

        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.ROOMLISTRESPONSE:
                    var proto = (network.RoomListResponse)parameters[0];
                    proto.rooms.Sort(
                        (x,y)=> 
                        {
                            if (x.playing && !y.playing)
                                return 1;
                            if (!x.playing && y.playing)
                                return -1;
                            if (x.room_id > y.room_id)
                                return 1;
                            else return -1;                            
                        });
                    RoomInfo = proto.rooms;                    
                    MessageSystem.Notify(MessageType.RoomList);
                    break;
            }
        }
    }
}


