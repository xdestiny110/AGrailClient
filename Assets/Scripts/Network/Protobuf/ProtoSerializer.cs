using System.IO;
using ProtoBuf;

public enum ProtoNameIds {
    REGISTERREQUEST = 1,
    REGISTERRESPONSE = 2,
    LOGINREQUEST = 3,
    LOGINRESPONSE = 4,
    LOGOUTREQUEST = 5,
    LOGOUTRESPONSE = 6,
    ROOMLISTREQUEST = 7,
    ROOMLISTRESPONSE = 8,
    CREATEROOMREQUEST = 9,
    ENTERROOMREQUEST = 10,
    LEAVEROOMREQUEST = 12,
    JOINTEAMREQUEST = 13,
    READYFORGAMEREQUEST = 14,
    SINGLEPLAYERINFO = 15,
    GAMEINFO = 16,
    TALK = 17,
    GOSSIP = 18,
    ERROR = 19,
    HEARTBEAT = 20,
    POLLINGREQUEST = 23,
    POLLINGRESPONSE = 24,
    ROLEREQUEST = 101,
    PICKBAN = 102,
    ACTION = 103,
    RESPOND = 104,
    COMMANDREQUEST = 106,
    ERRORINPUT = 107,
    HITMSG = 108,
    TURNBEGIN = 109,
    CARDMSG = 110,
    HURTMSG = 111,
    SKILLMSG = 112,
}

public class ProtoSerializer
{
    public static object ParseFrom(ProtoNameIds protoType, Stream stream)
    {
        switch (protoType) {
            case ProtoNameIds.REGISTERREQUEST: return Serializer.Deserialize<network.RegisterRequest>(stream);
            case ProtoNameIds.REGISTERRESPONSE: return Serializer.Deserialize<network.RegisterResponse>(stream);
            case ProtoNameIds.LOGINREQUEST: return Serializer.Deserialize<network.LoginRequest>(stream);
            case ProtoNameIds.LOGINRESPONSE: return Serializer.Deserialize<network.LoginResponse>(stream);
            case ProtoNameIds.LOGOUTREQUEST: return Serializer.Deserialize<network.LogoutRequest>(stream);
            case ProtoNameIds.LOGOUTRESPONSE: return Serializer.Deserialize<network.LogoutResponse>(stream);
            case ProtoNameIds.ROOMLISTREQUEST: return Serializer.Deserialize<network.RoomListRequest>(stream);
            case ProtoNameIds.ROOMLISTRESPONSE: return Serializer.Deserialize<network.RoomListResponse>(stream);
            case ProtoNameIds.CREATEROOMREQUEST: return Serializer.Deserialize<network.CreateRoomRequest>(stream);
            case ProtoNameIds.ENTERROOMREQUEST: return Serializer.Deserialize<network.EnterRoomRequest>(stream);
            case ProtoNameIds.LEAVEROOMREQUEST: return Serializer.Deserialize<network.LeaveRoomRequest>(stream);
            case ProtoNameIds.JOINTEAMREQUEST: return Serializer.Deserialize<network.JoinTeamRequest>(stream);
            case ProtoNameIds.READYFORGAMEREQUEST: return Serializer.Deserialize<network.ReadyForGameRequest>(stream);
            case ProtoNameIds.SINGLEPLAYERINFO: return Serializer.Deserialize<network.SinglePlayerInfo>(stream);
            case ProtoNameIds.GAMEINFO: return Serializer.Deserialize<network.GameInfo>(stream);
            case ProtoNameIds.TALK: return Serializer.Deserialize<network.Talk>(stream);
            case ProtoNameIds.GOSSIP: return Serializer.Deserialize<network.Gossip>(stream);
            case ProtoNameIds.ERROR: return Serializer.Deserialize<network.Error>(stream);
            case ProtoNameIds.POLLINGREQUEST: return Serializer.Deserialize<network.PollingRequest>(stream);
            case ProtoNameIds.POLLINGRESPONSE: return Serializer.Deserialize<network.PollingResponse>(stream);
            case ProtoNameIds.ROLEREQUEST: return Serializer.Deserialize<network.RoleRequest>(stream);
            case ProtoNameIds.PICKBAN: return Serializer.Deserialize<network.PickBan>(stream);
            case ProtoNameIds.ACTION: return Serializer.Deserialize<network.Action>(stream);
            case ProtoNameIds.RESPOND: return Serializer.Deserialize<network.Respond>(stream);
            case ProtoNameIds.COMMANDREQUEST: return Serializer.Deserialize<network.CommandRequest>(stream);
            case ProtoNameIds.ERRORINPUT: return Serializer.Deserialize<network.ErrorInput>(stream);
            case ProtoNameIds.HITMSG: return Serializer.Deserialize<network.HitMsg>(stream);
            case ProtoNameIds.TURNBEGIN: return Serializer.Deserialize<network.TurnBegin>(stream);
            case ProtoNameIds.CARDMSG: return Serializer.Deserialize<network.CardMsg>(stream);
            case ProtoNameIds.HURTMSG: return Serializer.Deserialize<network.HurtMsg>(stream);
            case ProtoNameIds.SKILLMSG: return Serializer.Deserialize<network.SkillMsg>(stream);
            default: break;
        }
        return null;
    }
    public static void Serialize(ProtoNameIds protoType, Stream stream, object proto)
    {
        switch (protoType)
        {
            case ProtoNameIds.REGISTERREQUEST: Serializer.Serialize(stream, (network.RegisterRequest)proto); break;
            case ProtoNameIds.REGISTERRESPONSE: Serializer.Serialize(stream, (network.RegisterResponse)proto); break;
            case ProtoNameIds.LOGINREQUEST: Serializer.Serialize(stream, (network.LoginRequest)proto); break;
            case ProtoNameIds.LOGINRESPONSE: Serializer.Serialize(stream, (network.LoginResponse)proto); break;
            case ProtoNameIds.LOGOUTREQUEST: Serializer.Serialize(stream, (network.LogoutRequest)proto); break;
            case ProtoNameIds.LOGOUTRESPONSE: Serializer.Serialize(stream, (network.LogoutResponse)proto); break;
            case ProtoNameIds.ROOMLISTREQUEST: Serializer.Serialize(stream, (network.RoomListRequest)proto); break;
            case ProtoNameIds.ROOMLISTRESPONSE: Serializer.Serialize(stream, (network.RoomListResponse)proto); break;
            case ProtoNameIds.CREATEROOMREQUEST: Serializer.Serialize(stream, (network.CreateRoomRequest)proto); break;
            case ProtoNameIds.ENTERROOMREQUEST: Serializer.Serialize(stream, (network.EnterRoomRequest)proto); break;
            case ProtoNameIds.LEAVEROOMREQUEST: Serializer.Serialize(stream, (network.LeaveRoomRequest)proto); break;
            case ProtoNameIds.JOINTEAMREQUEST: Serializer.Serialize(stream, (network.JoinTeamRequest)proto); break;
            case ProtoNameIds.READYFORGAMEREQUEST: Serializer.Serialize(stream, (network.ReadyForGameRequest)proto); break;
            case ProtoNameIds.SINGLEPLAYERINFO: Serializer.Serialize(stream, (network.SinglePlayerInfo)proto); break;
            case ProtoNameIds.GAMEINFO: Serializer.Serialize(stream, (network.GameInfo)proto); break;
            case ProtoNameIds.TALK: Serializer.Serialize(stream, (network.Talk)proto); break;
            case ProtoNameIds.GOSSIP: Serializer.Serialize(stream, (network.Gossip)proto); break;
            case ProtoNameIds.ERROR: Serializer.Serialize(stream, (network.Error)proto); break;
            case ProtoNameIds.POLLINGREQUEST: Serializer.Serialize(stream, (network.PollingRequest)proto); break;
            case ProtoNameIds.POLLINGRESPONSE: Serializer.Serialize(stream, (network.PollingResponse)proto); break;
            case ProtoNameIds.ROLEREQUEST: Serializer.Serialize(stream, (network.RoleRequest)proto); break;
            case ProtoNameIds.PICKBAN: Serializer.Serialize(stream, (network.PickBan)proto); break;
            case ProtoNameIds.ACTION: Serializer.Serialize(stream, (network.Action)proto); break;
            case ProtoNameIds.RESPOND: Serializer.Serialize(stream, (network.Respond)proto); break;
            case ProtoNameIds.COMMANDREQUEST: Serializer.Serialize(stream, (network.CommandRequest)proto); break;
            case ProtoNameIds.ERRORINPUT: Serializer.Serialize(stream, (network.ErrorInput)proto); break;
            case ProtoNameIds.HITMSG: Serializer.Serialize(stream, (network.HitMsg)proto); break;
            case ProtoNameIds.TURNBEGIN: Serializer.Serialize(stream, (network.TurnBegin)proto); break;
            case ProtoNameIds.CARDMSG: Serializer.Serialize(stream, (network.CardMsg)proto); break;
            case ProtoNameIds.HURTMSG: Serializer.Serialize(stream, (network.HurtMsg)proto); break;
            case ProtoNameIds.SKILLMSG: Serializer.Serialize(stream, (network.SkillMsg)proto); break;
            default: break;
        }
    }
}
