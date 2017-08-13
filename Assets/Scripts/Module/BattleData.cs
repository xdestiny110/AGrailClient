using System;
using System.Linq;
using Framework;
using Framework.Message;
using Framework.Network;
using System.Collections.Generic;
using UnityEngine;

namespace AGrail
{   
    public class BattleData : Singleton<BattleData>, IMessageListener<MessageType>
    {        
        public int? RoomID { get; private set; }
        public int? PlayerID { get; private set; }
        public uint Pile { get; private set; }
        public uint Discard { get; private set; }
        public bool IsStarted { get; private set; }
        public uint Round { get; private set; }
        public uint[] Morale = new uint[2];
        public uint[] Gem = new uint[2];
        public uint[] Crystal = new uint[2];
        public uint[] Grail = new uint[2];        
        public List<network.SinglePlayerInfo> PlayerInfos = new List<network.SinglePlayerInfo>();
        public List<int> PlayerIdxOrder = new List<int>();//按照顺序排列玩家ID, 第一个一定是主玩家

        public PlayerAgent Agent { get; private set; }
        public network.SinglePlayerInfo MainPlayer { get; private set; }

        private uint currentPlayerID = 9;
        public uint CurrentPlayerID
        {
            set
            {
                currentPlayerID = value;
                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, currentPlayerID);
            }
            get { return currentPlayerID; }
        }

        public BattleData() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);
            MessageSystem<MessageType>.Regist(MessageType.GAMEINFO, this);
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
                case MessageType.TURNBEGIN:
                    if (MainPlayer != null && MainPlayer.id != 9)
                        Agent.PlayerRole.IsStart = false;
                    break;
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
            Morale = new uint[2] { 15, 15 };
            Gem = new uint[2];
            Crystal = new uint[2];
            Grail = new uint[2];
            PlayerInfos.Clear();
            PlayerIdxOrder.Clear();
            Agent = new PlayerAgent(0);
            MainPlayer = null;
            RoomID = null;
            PlayerID = null;
            IsStarted = false;
            CurrentPlayerID = 9;
        }

        public network.SinglePlayerInfo GetPlayerInfo(uint playerID)
        {
            var player = PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
               u =>
               {
                  return u != null && u.id == playerID;
               });
            return player;
        }

        private network.GameInfo gameInfo
        {
            set
            {
                if (value.room_idSpecified && !RoomID.HasValue)
                {                    
                    RoomID = value.room_id;
                    //MessageSystem<MessageType>.Notify(MessageType.EnterRoom);
                }
                if (value.player_idSpecified)
                {
                    PlayerID = value.player_id;
                }
                Pile = value.pileSpecified ? value.pile : Pile;
                Discard = value.discardSpecified ? value.discard : Discard;                
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
                    MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Red, value.red_grail - Grail[(int)Team.Red]);
                    Grail[(int)Team.Red] = value.red_grail;
                }
                if (value.is_startedSpecified)
                {
                    //游戏开始，可能需要重新定位玩家位置
                    UnityEngine.Debug.Log("game start");
                    if (!IsStarted && value.is_started)
                    {
                        PlayerIdxOrder.Clear();
                        int t = -1;
                        for (int i = 0; i < value.player_infos.Count; i++)
                        {
                            PlayerIdxOrder.Add((int)value.player_infos[i].id);
                            if (value.player_infos[i].id == PlayerID)
                            {
                                MainPlayer = value.player_infos[i];
                                t = i;
                            }
                        }
                        if (t != -1)
                        {
                            PlayerIdxOrder.AddRange(PlayerIdxOrder.GetRange(0, t));
                            PlayerIdxOrder.RemoveRange(0, t);
                        }
                        if (value.player_id == 9)
                            MainPlayer = new network.SinglePlayerInfo() { id = 9 };                            
                        MessageSystem<MessageType>.Notify(MessageType.GameStart);
                    }
                    //需要再发一次准备
                    //以前是不用的...不知道现在改成这样的目的是什么
                    var proto = new network.ReadyForGameRequest() { type = network.ReadyForGameRequest.Type.SEAT_READY };
                    GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.READYFORGAMEREQUEST });
                    IsStarted = value.is_started;
                }
                foreach (var v in value.player_infos)
                {
                    var player = GetPlayerInfo(v.id);
                    if (player == null)
                    {
                        PlayerInfos.Add(v);
                        player = v;
                        if (!player.max_handSpecified)
                            player.max_hand = 6;
                    }
                    if (player.id == PlayerID && MainPlayer != player)
                        MainPlayer = player;
                    //这里可能有些乱...以后再整理吧
                    var idx = PlayerInfos.IndexOf(player);
                    if(PlayerIdxOrder.Count > 0)
                        idx = PlayerIdxOrder.IndexOf((int)player.id);
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
                        if (player.id == PlayerID)
                        {
                            Agent = new PlayerAgent(player.role_id);
                            Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, false);
                            MessageSystem<MessageType>.Notify(MessageType.AgentUpdate);
                        }
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
                            //如果两个是同一个引用则不清空
                            if(player != v)
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
                    if (v.gemSpecified || v.crystalSpecified)
                        MessageSystem<MessageType>.Notify(MessageType.PlayerEnergeChange, idx, player.gem, player.crystal);
                    if (v.yellow_tokenSpecified)
                        player.yellow_token = v.yellow_token;
                    if (v.blue_tokenSpecified)
                        player.blue_token = v.blue_token;
                    if (v.covered_countSpecified)
                    {
                        if (player != v)
                        {
                            player.covered_count = v.covered_count;
                            player.covereds.Clear();
                            foreach (var u in v.covereds)
                                player.covereds.Add(u);
                        }
                        if(MainPlayer != null && v.id == MainPlayer.id)
                            MessageSystem<MessageType>.Notify(MessageType.AgentHandChange);
                    }
                    if (v.yellow_tokenSpecified || v.blue_tokenSpecified || v.covered_countSpecified)
                        MessageSystem<MessageType>.Notify(MessageType.PlayerTokenChange,
                            idx, player.yellow_token, player.blue_token, player.covered_count);
                    if (v.basic_cards.Count > 0 && player != v)
                            player.basic_cards = v.basic_cards;
                    if (v.ex_cards.Count > 0)
                    {
                        if(player != v)
                            player.ex_cards = v.ex_cards;
                        //为了进行卡牌编号的区分, 专有牌的序号都+1000
                        for (int i = 0; i < player.ex_cards.Count; i++)
                            player.ex_cards[i] += 1000;
                    }
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
                for (int i = 0; i < value.commands.Count; i++)
                    UnityEngine.Debug.Log(string.Format("cmd request call back: {0}", (value.cmd_type == network.CmdType.CMD_ACTION) ?
                        ((network.BasicActionType)value.commands[i].respond_id).ToString() : ((network.BasicRespondType)value.commands[i].respond_id).ToString()));
                if (MainPlayer == null)
                    return;
                foreach (var v in value.commands)
                {
                    RoleBase r = null;
                    //能够有多重响应
                    switch (v.respond_id)
                    {
                        case 0:
                            //本次行动能够放弃
                            if (v.dst_ids[0] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.AgentState |= (int)PlayerAgentState.CanResign;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_REPLY_ATTACK:
                            r = RoleFactory.Create(GetPlayerInfo(v.args[2]).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "应战响应" + Environment.NewLine;
                            CurrentPlayerID = v.args[2];
                            if (v.args[2] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.Attacked;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD:
                            r = RoleFactory.Create(GetPlayerInfo(v.dst_ids[0]).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "弃牌响应" + Environment.NewLine;
                            CurrentPlayerID = v.dst_ids[0];
                            if (v.dst_ids[0] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.Discard;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD_COVER:
                            r = RoleFactory.Create(GetPlayerInfo(v.dst_ids[0]).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "弃盖牌响应" + Environment.NewLine;
                            CurrentPlayerID = v.dst_ids[0];
                            if (v.dst_ids[0] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.DiscardCovered;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_HEAL:
                            r = RoleFactory.Create(GetPlayerInfo(v.args[0]).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "治疗响应" + Environment.NewLine;
                            CurrentPlayerID = v.args[0];
                            if (v.args[0] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.HealCost;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_WEAKEN:
                            r = RoleFactory.Create(GetPlayerInfo(v.args[0]).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "虚弱响应" + Environment.NewLine;
                            CurrentPlayerID = v.args[0];
                            if (v.args[0] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.Weaken;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_BULLET:
                            r = RoleFactory.Create(GetPlayerInfo(v.args[0]).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "魔弹响应" + Environment.NewLine;
                            CurrentPlayerID = v.args[0];
                            if (v.args[0] != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.MoDaned;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_ADDITIONAL_ACTION:
                            r = RoleFactory.Create(GetPlayerInfo(v.src_id).role_id);
                            Dialog.Instance.Log += "等待" + r.RoleName + "额外行动响应" + Environment.NewLine;
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.AdditionAction;
                            break;
                        case (uint)network.BasicActionType.ACTION_ANY:
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState =
                                (int)PlayerAgentState.CanAttack | (int)PlayerAgentState.CanMagic | (int)PlayerAgentState.CanSpecial;
                            break;
                        case (uint)network.BasicActionType.ACTION_ATTACK_MAGIC:
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanMagic | (int)PlayerAgentState.CanAttack;
                            break;
                        case (uint)network.BasicActionType.ACTION_ATTACK:
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanAttack;
                            break;
                        case (uint)network.BasicActionType.ACTION_MAGIC:
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanMagic;
                            break;
                        case (uint)network.BasicActionType.ACTION_NONE:
                            //无法行动
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            //Agent.Cmd = v;                            
                            Agent.AgentState = (int)PlayerAgentState.CanResign;
                            break;
                        default:
                            //技能响应
                            r = RoleFactory.Create(GetPlayerInfo(v.src_id).role_id);
                            if (r.Skills.ContainsKey(v.respond_id))
                                Dialog.Instance.Log += "等待" + r.RoleName + "响应技能" + r.Skills[v.respond_id].SkillName + Environment.NewLine;
                            else if (Skill.GetSkill(v.respond_id) != null)
                                Dialog.Instance.Log += "等待" + r.RoleName + "响应技能" + Skill.GetSkill(v.respond_id).SkillName + Environment.NewLine;
                            else
                                Dialog.Instance.Log += "等待" + r.RoleName + "响应技能" + v.respond_id.ToString() + Environment.NewLine;
                            CurrentPlayerID = v.src_id;
                            if (v.src_id != MainPlayer.id)
                            {
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.SkillResponse;
                            break;
                    }
                }
            }
        }        
    }
}


