using Framework;
using Framework.Network;
using Framework.Message;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class Room : Singleton<Room>, IMessageListener
    {
        List<network.RoomListResponse.RoomInfo> roomInfo = new List<network.RoomListResponse.RoomInfo>();

        public Room() : base()
        {
            MessageSystem.Regist(MessageType.ROOMLISTRESPONSE, this);
        }

        public void GetRoomList()
        {

        }

        public void JoinRoom()
        {

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
                    roomInfo = proto.rooms;
                    MessageSystem.Notify(MessageType.RoomList);
                    break;
            }
        }
    }
}


