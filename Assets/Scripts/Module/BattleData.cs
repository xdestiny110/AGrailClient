using System;
using System.Linq;
using Framework;
using Framework.Message;
using Framework.Network;
using System.Collections.Generic;

namespace AGrail
{
    public class BattleData : Singleton<BattleData>, IMessageListener<MessageType>
    {
        public int RoomID { get; private set; }
        public int PlayerID { get; private set; }
        public uint Pile { get; private set; }
        public uint Discard { get; private set; }
        public bool IsStarted { get; private set; }
        public uint Round { get; private set; }
        public uint[] Morale = new uint[2];
        public uint[] Gem = new uint[2];
        public uint[] Crystal = new uint[2];
        public uint[] Grail = new uint[2];
        public List<network.SinglePlayerInfo> PlayerInfos = new List<network.SinglePlayerInfo>();

        public BattleData() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.GAMEINFO, this);
            MessageSystem<MessageType>.Regist(MessageType.COMMANDREQUEST, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);
            reset();
            var inst = RoleChoose.Instance;
        }

        public void Ready(bool flag)
        {
            var proto = new network.ReadyForGameRequest() { type = flag ? network.ReadyForGameRequest.Type.START_READY : network.ReadyForGameRequest.Type.CANCEL_START_REDAY };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.READYFORGAMEREQUEST });
        }

        public void ChooseTeam()
        {

        }        

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.GAMEINFO:
                    gameInfo = parameters[0] as network.GameInfo;
                    break;
                case MessageType.TURNBEGIN:
                    break;
                case MessageType.COMMANDREQUEST:
                    cmdReq = parameters[0] as network.CommandRequest;                    
                    break;
            }
        }

        private void reset()
        {
            Morale = new uint[2];
            Gem = new uint[2];
            Crystal = new uint[2];
            Grail = new uint[2];
            PlayerInfos.Clear();
        }

        

        private network.GameInfo gameInfo
        {
            set
            {
                if (value.room_idSpecified)
                {
                    reset();
                    RoomID = value.room_id;
                    MessageSystem<MessageType>.Notify(MessageType.EnterRoom);
                }                
                PlayerID = value.player_idSpecified ? value.player_id : PlayerID;
                Pile = value.pileSpecified ? value.pile : Pile;
                Discard = value.discardSpecified ? value.discard : Discard;
                IsStarted = value.is_startedSpecified ? value.is_started : IsStarted;
                if (value.blue_moraleSpecified)
                {
                    Morale[(int)Team.Blue] = value.blue_morale;
                    MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Blue, Morale[(int)Team.Blue]);
                }
                if (value.red_moraleSpecified)
                {
                    Morale[(int)Team.Red] = value.red_morale;
                    MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Red, Morale[(int)Team.Red]);
                }
                if (value.blue_gemSpecified)
                {                    
                    MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Blue, value.blue_gem - Gem[(int)Team.Blue]);
                    Gem[(int)Team.Blue] = value.blue_gem;
                }
                if (value.red_gemSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Red, value.red_gem - Gem[(int)Team.Red]);
                    Gem[(int)Team.Red] = value.red_gem;
                }
                if (value.blue_crystalSpecified)
                {                    
                    MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Blue, value.blue_crystal-Crystal[(int)Team.Blue]);
                    Crystal[(int)Team.Blue] = value.blue_crystal;
                }
                if (value.red_crystalSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Red, value.red_crystal - Crystal[(int)Team.Red]);
                    Crystal[(int)Team.Red] = value.red_crystal;
                }
                if (value.blue_grailSpecified)
                {                    
                    MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Blue, value.blue_grail - Grail[(int)Team.Blue]);
                    Grail[(int)Team.Blue] = value.blue_grail;
                }
                if (value.red_grailSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Red, value.red_crystal - Grail[(int)Team.Red]);
                    Grail[(int)Team.Red] = value.red_grail;
                }                

                foreach(var v in value.player_infos)
                {
                    var player = PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => {
                            return u != null && u.id == v.id;
                        });
                    if(player == null)
                    {
                        PlayerInfos.Add(v);
                        player = v;
                    }
                    var idx = PlayerInfos.IndexOf(player);
                    if (v.readySpecified)
                    {
                        player.ready = v.ready;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerIsReady, idx, player.ready);
                    }
                    if (v.teamSpecified)
                    {
                        player.team = v.team;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerTeamChange, idx, player.team);
                    }
                    if (v.role_idSpecified)
                    {
                        player.role_id = v.role_id;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerRoleChange, idx, player.role_id);
                    }
                    if (v.nicknameSpecified)
                    {
                        player.nickname = v.nickname;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerNickName, idx, player.nickname);
                    }
                    if (v.hand_countSpecified)
                    {
                        player.hand_count = v.hand_count;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerHandChange, idx, player.hand_count);
                    }
                    if (v.max_handSpecified)
                    {
                        player.max_hand = v.max_hand;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerHandMaxChange, idx, player.max_hand);
                    }
                    if (v.heal_countSpecified)
                    {
                        player.heal_count = v.heal_count;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerHealChange, idx, player.heal_count);
                    }
                    if (v.gemSpecified)
                    {
                        player.gem = v.gem;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerGemChange, idx);
                    }
                    if (v.crystalSpecified)
                    {
                        player.crystal = v.crystal;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerCrystalChange, idx);
                    }
                }
            }
        }

        private network.CommandRequest cmdReq
        {
            set
            {
                UnityEngine.Debug.Log(string.Format("cmd request call back: {0}", (value.cmd_type == network.CmdType.CMD_ACTION) ?
                    ((network.BasicActionType)value.commands[0].respond_id).ToString() : ((network.BasicRespondType)value.commands[0].respond_id).ToString()));
                foreach(var v in value.commands)
                {
                    switch (v.respond_id)
                    {
                        case (uint)network.BasicRespondType.RESPOND_REPLY_ATTACK:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD_COVER:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_HEAL:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_WEAKEN:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_BULLET:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_ADDITIONAL_ACTION:
                            break;
                        case (uint)network.BasicActionType.ACTION_ANY:
                            break;
                        case (uint)network.BasicActionType.ACTION_ATTACK_MAGIC:
                            break;
                        case (uint)network.BasicActionType.ACTION_ATTACK:
                            break;
                        case (uint)network.BasicActionType.ACTION_MAGIC:
                            break;
                        case (uint)network.BasicActionType.ACTION_NONE:
                            break;
                    }
                }
            }
        }
    }

    public enum Team
    {
        Blue = 0,
        Red,
        Other,
    }
}


