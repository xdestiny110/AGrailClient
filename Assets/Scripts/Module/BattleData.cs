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
        public int? RoomID { get; private set; }
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

        public PlayerAgent Agent { get; private set; }
        public network.SinglePlayerInfo MainPlayer { get; private set; }

        public BattleData() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.GAMEINFO, this);
            MessageSystem<MessageType>.Regist(MessageType.ROLEREQUEST, this);
            MessageSystem<MessageType>.Regist(MessageType.COMMANDREQUEST, this);
            MessageSystem<MessageType>.Regist(MessageType.ERROR, this);
            Reset();
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
                case MessageType.COMMANDREQUEST:
                    cmdReq = parameters[0] as network.CommandRequest;                    
                    break;
                case MessageType.ERROR:
                    var error = parameters[0] as network.Error;
                    if(error.id == 29)
                    {
                        //离开房间
                        var idx = PlayerInfos.FindIndex(u =>
                        {
                            return u != null && u.id == error.dst_id;
                        });
                        MessageSystem<MessageType>.Notify(MessageType.PlayerLeave, error.dst_id, idx);
                    }
                    break;
            }
        }

        public void Reset()
        {
            Morale = new uint[2];
            Gem = new uint[2];
            Crystal = new uint[2];
            Grail = new uint[2];
            PlayerInfos.Clear();            
            Agent = null;
            MainPlayer = null;
            RoomID = null;
        }

        private network.GameInfo gameInfo
        {
            set
            {                
                if (value.room_idSpecified && !RoomID.HasValue)
                {                    
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
                    MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Blue, (int)value.blue_gem - (int)Gem[(int)Team.Blue]);
                    Gem[(int)Team.Blue] = value.blue_gem;
                }
                if (value.red_gemSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.GemChange, Team.Red, (int)value.red_gem - (int)Gem[(int)Team.Red]);
                    Gem[(int)Team.Red] = value.red_gem;
                }
                if (value.blue_crystalSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Blue, (int)value.blue_crystal - (int)Crystal[(int)Team.Blue]);
                    Crystal[(int)Team.Blue] = value.blue_crystal;
                }
                if (value.red_crystalSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.CrystalChange, Team.Red, (int)value.red_crystal - (int)Crystal[(int)Team.Red]);
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
                    bool isInit = false;
                    if (player == null)
                    {
                        PlayerInfos.Add(v);
                        player = v;
                        player.max_hand = 6;                            
                        if (player.id == PlayerID)
                            MainPlayer = player;
                        isInit = true;
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
                        if(player.id == PlayerID)                        
                            Agent = new PlayerAgent(player.role_id);                            
                    }
                    if (v.nicknameSpecified)
                    {
                        player.nickname = v.nickname;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerNickName, idx, player.nickname);
                    }
                    if (v.is_kneltSpecified)
                    {
                        player.is_knelt = v.is_knelt;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerKneltChange, idx, player.is_knelt);
                    }
                    if (v.max_handSpecified)
                        player.max_hand = v.max_hand;                                            
                    if (v.hand_countSpecified)                    
                        player.hand_count = v.hand_count;
                    if (v.max_handSpecified || v.hand_countSpecified)
                    {
                        MessageSystem<MessageType>.Notify(MessageType.PlayerHandChange, idx, player.hand_count, player.max_hand);
                        if (MainPlayer != null && player.id == MainPlayer.id && v.hand_countSpecified)
                        {
                            //第一次的时候不用更新手牌
                            if (!isInit)
                            {
                                player.hands.Clear();
                                foreach (var u in v.hands)
                                    player.hands.Add(u);
                            }                            
                            MessageSystem<MessageType>.Notify(MessageType.AgentHandChange);
                        }                            
                    }                                   
                    if (v.heal_countSpecified)
                    {
                        player.heal_count = v.heal_count;
                        MessageSystem<MessageType>.Notify(MessageType.PlayerHealChange, idx, player.heal_count);
                    }
                    if (v.gemSpecified)                    
                        player.gem = v.gem;
                    if (v.crystalSpecified)                    
                        player.crystal = v.crystal;                        
                    if(v.gemSpecified || v.crystalSpecified)
                        MessageSystem<MessageType>.Notify(MessageType.PlayerEnergeChange, idx, player.gem, player.crystal);
                    if (v.yellow_tokenSpecified)
                        player.yellow_token = v.yellow_token;                        
                    if (v.blue_tokenSpecified)
                        player.blue_token = v.blue_token;
                    if (v.covered_countSpecified)
                        player.covered_count = v.covered_count;
                    if (v.yellow_tokenSpecified || v.blue_tokenSpecified || v.covered_countSpecified)
                    {
                        MessageSystem<MessageType>.Notify(MessageType.PlayerTokenChange, idx, player.yellow_token, player.blue_token, player.covered_count);
                        //UnityEngine.Debug.LogFormat("yellow token = {0}, blue token = {1}, cover = {2}, idx = {3}", player.yellow_token, player.blue_token, player.covered_count, idx);
                    }                        
                    if (v.basic_cards.Count > 0)
                        player.basic_cards = v.basic_cards;
                    if(v.ex_cards.Count > 0)
                        player.ex_cards = v.ex_cards;
                    if(v.delete_field.Count > 0)
                    {
                        foreach(var u in v.delete_field)
                        {
                            if (u == "ex_cards")
                                player.ex_cards = new List<uint>();
                            if (u == "basic_cards")
                                player.basic_cards = new List<uint>();
                        }
                    }
                    if(v.basic_cards.Count > 0 || v.ex_cards.Count > 0 || v.delete_field.Count > 0)
                        MessageSystem<MessageType>.Notify(MessageType.PlayerBasicAndExCardChange, idx, player.basic_cards, player.ex_cards);
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
                    //从proto上来看能够有多重响应
                    //但逻辑上说不通啊...
                    switch (v.respond_id)
                    {
                        case (uint)network.BasicRespondType.RESPOND_REPLY_ATTACK:
                            Agent.AgentState = (int)PlayerAgentState.Attacked;
                            Agent.RespCmd = v;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD:
                            Agent.AgentState = (int)PlayerAgentState.Discard;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD_COVER:
                            break;
                        case (uint)network.BasicRespondType.RESPOND_HEAL:
                            Agent.AgentState = (int)PlayerAgentState.HealCost;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_WEAKEN:
                            Agent.AgentState = (int)PlayerAgentState.Weaken;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_BULLET:
                            Agent.AgentState = (int)PlayerAgentState.MoDaned;
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


