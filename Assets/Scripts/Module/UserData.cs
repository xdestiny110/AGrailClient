using Framework;
using Framework.Network;
using Framework.Message;
using System;

namespace AGrail
{
    public class UserData : Singleton<UserData>, IMessageListener
    {
        public string UserName { get; private set; }
        public int GameID { get; private set; }

        public void Login(string userName, string password)
        {
            var request = new network.LoginRequest() { asGuest = false, user_id = userName, user_password = password, version = GameManager.Version };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = request, ProtoID = ProtoNameIds.LOGINREQUEST });
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.Protobuf:
                    var proto = (Protobuf)parameters[0];
                    switch (proto.ProtoID)
                    {
                        case ProtoNameIds.LOGINRESPONSE:

                            break;
                    }
                    break;
            }
        }
    }
}


