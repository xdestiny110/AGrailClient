namespace Framework.Message
{
    public enum MessageType
    {
        // Const Message Type
        Null = 0,
        OnConnect,
        OnDisconnect,
        OnReconnect,

        // Protobuf Message Type
        REGISTERREQUEST,
        REGISTERRESPONSE,
        LOGINREQUEST,
        LOGINRESPONSE,
        LOGOUTREQUEST,
        LOGOUTRESPONSE,
        ROOMLISTREQUEST,
        ROOMLISTRESPONSE,
        CREATEROOMREQUEST,
        ENTERROOMREQUEST,
        LEAVEROOMREQUEST,
        JOINTEAMREQUEST,
        READYFORGAMEREQUEST,
        SINGLEPLAYERINFO,
        GAMEINFO,
        TALK,
        GOSSIP,
        ERROR,
        ROLEREQUEST,
        PICKBAN,
        ACTION,
        RESPOND,
        COMMANDREQUEST,
        ERRORINPUT,
        HITMSG,
        TURNBEGIN,
        CARDMSG,
        HURTMSG,
        SKILLMSG,

        // Custom Message Type
        LoginState,
        RoomList,
    }
}
