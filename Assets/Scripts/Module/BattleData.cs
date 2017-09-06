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
        public uint StartPlayerID = 0;//第一个行动玩家的ID
        public uint NowPlayerID = 0;//当前回合行动玩家的ID
        public List<uint> DisConnectedPlayer = new List<uint>();

        public PlayerAgent Agent { get; private set; }
        public network.SinglePlayerInfo MainPlayer { get; private set; }

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

        public void ChooseTeam(Team team)
        {
            var proto = new network.JoinTeamRequest() { team = (network.JoinTeamRequest.Team)team };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.JOINTEAMREQUEST });
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.TURNBEGIN:
                    turnBegin = parameters[0] as network.TurnBegin;
                    break;
                case MessageType.GAMEINFO:
                    gameInfo = parameters[0] as network.GameInfo;
                    break;
                case MessageType.COMMANDREQUEST:
                    cmdReq = parameters[0] as network.CommandRequest;
                    break;
                case MessageType.ERROR:
                    var error = parameters[0] as network.Error;
                    if(error.id == 30)
                    {
                        //离开房间
                        var idx = PlayerInfos.FindIndex(u =>
                        {
                            return u != null && u.id == error.dst_id;
                        });
                        if(!DisConnectedPlayer.Contains((uint)error.dst_id))
                        {
                        DisConnectedPlayer.Add((uint)error.dst_id);
                        MessageSystem<MessageType>.Notify(MessageType.PlayerLeave, error.dst_id, idx);
                        }
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

        private network.TurnBegin turnBegin
        {
            set
            {
                NowPlayerID = (uint)value.id;
                if (NowPlayerID == MainPlayer.id)
                    //if (MainPlayer != null && MainPlayer.id != 9 )
                {
                    Agent.PlayerRole.IsStart = false;
                    Agent.PlayerRole.attackable = true;
                }
            }
        }

        private network.GameInfo gameInfo
        {
            set
            {
                if (value.room_idSpecified && !RoomID.HasValue)
                {
                    RoomID = value.room_id;
                    Lobby.Instance.SelectRoom.room_id = value.room_id;
                    MessageSystem<MessageType>.Notify(MessageType.RoomIDChange);
                }
                if (value.player_idSpecified)
                {
                    PlayerID = value.player_id;
                }
                Pile = value.pileSpecified ? value.pile : Pile;
                Discard = value.discardSpecified ? value.discard : Discard;
                if (value.blue_moraleSpecified)
                {
                    if (Morale[(int)Team.Blue] != value.blue_morale)
                        Dialog.instance.Log += "<color=#E61E19>蓝方士气由" + Morale[(int)Team.Blue] + (Morale[(int)Team.Blue] > value.blue_morale ? "下降至" : "上升至") + value.blue_morale + "</color>"+"\n";
                    Morale[(int)Team.Blue] = value.blue_morale;
                    MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Blue, Morale[(int)Team.Blue]);
                    if(value.blue_morale == 0)
                        MessageSystem<MessageType>.Notify((MainPlayer.team == (uint)Team.Blue) ? MessageType.Lose : MessageType.Win);
                }
                if (value.red_moraleSpecified)
                {
                    if (Morale[(int)Team.Red] != value.red_morale)
                        Dialog.instance.Log += "<color=#E61E19>红方士气由" + Morale[(int)Team.Red] + (Morale[(int)Team.Red] > value.red_morale ? "下降至" : "上升至") + value.red_morale + "</color>"+"\n";
                    Morale[(int)Team.Red] = value.red_morale;
                    MessageSystem<MessageType>.Notify(MessageType.MoraleChange, Team.Red, Morale[(int)Team.Red]);
                    if (value.red_morale == 0)
                        MessageSystem<MessageType>.Notify((MainPlayer.team == (uint)Team.Red) ? MessageType.Lose : MessageType.Win);
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
                    if (value.blue_grail == 5)
                        MessageSystem<MessageType>.Notify((MainPlayer.team == (uint)Team.Red) ? MessageType.Lose : MessageType.Win);
                }
                if (value.red_grailSpecified)
                {
                    MessageSystem<MessageType>.Notify(MessageType.GrailChange, Team.Red, value.red_grail - Grail[(int)Team.Red]);
                    Grail[(int)Team.Red] = value.red_grail;
                    if (value.red_grail == 5)
                        MessageSystem<MessageType>.Notify((MainPlayer.team == (uint)Team.Red) ? MessageType.Win : MessageType.Lose);
                }
                if (value.is_startedSpecified)
                {
                    //游戏开始，可能需要重新定位玩家位置
                    if (!IsStarted && value.is_started)
                    {
                        PlayerIdxOrder.Clear();
                        int t = -1;
                        StartPlayerID = value.player_infos[0].id;
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
                            MainPlayer = new network.SinglePlayerInfo() { id = 9, team = (uint)Team.Other };
                        //if(RoleChoose.Instance.RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_31)
                      //  MessageSystem<MessageType>.Notify(MessageType.GameStart);
                    }
                    //需要再发一次准备
                    //以前是不用的...不知道现在改成这样的目的是什么
                  //  if (RoleChoose.Instance.RoleStrategy == network.ROLE_STRATEGY.ROLE_STRATEGY_31)
                  //      MessageSystem<MessageType>.Notify(MessageType.GameStart);
                    IsStarted = value.is_started;
                }
                int count = 0;
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
                        count++;
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
                    {
                        int offset = (int)v.hand_count - (int)player.hand_count;
                        if (offset != 0)
                        {
                            Dialog.instance.Log += "<color=#FFF000>" + RoleFactory.Create(player.role_id).RoleName + (player.hand_count > v.hand_count ? "失去了" : "获得了") + Math.Abs((int)player.hand_count - (int)v.hand_count) + "张手牌" + "</color>\n";

                        }
                        player.hand_count = v.hand_count;
                    }
                
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
                        Dialog.instance.Log += RoleFactory.Create(player.role_id).RoleName + (player.heal_count > v.heal_count ? "失去了" : "获得了") + Math.Abs((int)player.heal_count - (int)v.heal_count) + "点治疗" + "\n";
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
                            //加入盖牌改变的log
                            // UnityEngine.Debug.Log();
                            Dialog.instance.Log += "<color=#FFF000>" + RoleFactory.Create(player.role_id).RoleName + "的盖牌变为" + v.covered_count+"</color>" + Environment.NewLine;

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
                    {
                        player.basic_cards.Clear();
                        player.basic_cards.AddRange(v.basic_cards);
                    }
                    if (v.ex_cards.Count > 0)
                    {
                        if(player != v)
                        {
                            player.ex_cards.Clear();
                            player.ex_cards.AddRange(v.ex_cards);
                        }                            
                        //为了进行卡牌编号的区分, 专有牌的序号都+1000
                        for (int i = 0; i < player.ex_cards.Count; i++)
                            player.ex_cards[i] += 1000;
                    }
                    if(v.delete_field.Count > 0)
                    {
                        foreach(var u in v.delete_field)
                        {
                            if (u == "ex_cards")
                                player.ex_cards.Clear();
                            if (u == "basic_cards")
                                player.basic_cards.Clear();
                        }
                    }
                    if(v.basic_cards.Count > 0 || v.ex_cards.Count > 0 || v.delete_field.Count > 0)
                        MessageSystem<MessageType>.Notify(MessageType.PlayerBasicAndExCardChange, idx, player.basic_cards, player.ex_cards);
                    if(v.onlineSpecified)
                    {
                        if(!v.online && !DisConnectedPlayer.Contains(v.id) )
                            DisConnectedPlayer.Add(v.id);
                        if (v.online && DisConnectedPlayer.Contains(v.id))
                            DisConnectedPlayer.Remove(v.id);
                        if(DisConnectedPlayer.Count == 0 && GameManager.UIInstance.PeekWindow() == Framework.UI.WindowType.DisConnectedPoll)
                            GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.None);
                    }
                }
                if(count == BattleData.Instance.PlayerInfos.Count)
                    MessageSystem<MessageType>.Notify(MessageType.GameStart);

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
                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange);
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
                            if (v.args[2] != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.args[2], "等待应战响应");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                           RoleFactory.Create( BattleData.instance.GetPlayerInfo(v.args[3]).role_id).RoleName+"对你使用了"+Card.GetCard(v.args[1]).Name);
                            Agent.AgentState = (int)PlayerAgentState.Attacked;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD:
                            if (v.dst_ids[0] != MainPlayer.id)
                            {
                                if (v.args[0] == 1)
                                {
                                    // MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.dst_ids[0], v.args[2],v.args[1]);
                                    string heroName = RoleFactory.Create(GetPlayerInfo(v.dst_ids[0]).role_id).RoleName;
                                    //   UnityEngine.Debug.Log("名字:"+heroName);
                                    string isShow = v.args[2] == 1 ? "明弃" : "暗弃";
                                    //   UnityEngine.Debug.Log("方式:" + isShow);
                                    int numberOfDiscard = (int)v.args[1];
                                    //  UnityEngine.Debug.Log("数量:" + numberOfDiscard);
                                    string msg = heroName + "需要" + isShow + numberOfDiscard + "张牌" + Environment.NewLine;
                                    Dialog.instance.Log += "<color=#FFF000>" + msg+"</color>";
                                    //  Dialog.instance.Log +=string.Format("玩家[0]需要[1][2]张牌",v.dst_ids[0], v.args[2] == 1 ? "明弃" : "暗弃", v.args[1]);
                                    Agent.AgentState = (int)PlayerAgentState.Idle;
                                    continue;
                                }
                                else
                                {
                                 MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.dst_ids[0], "等待弃牌响应");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                                }
                                 
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.Discard;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_DISCARD_COVER:
                            if (v.dst_ids[0] != MainPlayer.id)
                            {
                                 MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.dst_ids[0], "等待弃盖牌响应");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.DiscardCovered;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_HEAL:
                            if (v.args[0] != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.args[0], "等待治疗响应");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.HealCost;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_WEAKEN:
                            if (v.args[0] != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.args[0], "等待虚弱响应");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.Weaken;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_BULLET:
                            if (v.args[0] != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.args[0], "等待魔弹响应");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.MoDaned;
                            break;
                        case (uint)network.BasicRespondType.RESPOND_ADDITIONAL_ACTION:
                            if (v.src_id != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待响应额外行动");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.AdditionAction;
                            break;
                        case (uint)network.BasicActionType.ACTION_ANY:
                            if (v.src_id != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待行动");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState =
                                (int)PlayerAgentState.CanAttack | (int)PlayerAgentState.CanMagic | (int)PlayerAgentState.CanSpecial;
                            break;
                        case (uint)network.BasicActionType.ACTION_ATTACK_MAGIC:
                            if (v.src_id != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待行动");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanMagic | (int)PlayerAgentState.CanAttack;
                            break;
                        case (uint)network.BasicActionType.ACTION_ATTACK:
                            if (v.src_id != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待攻击行动");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanAttack;
                            break;
                        case (uint)network.BasicActionType.ACTION_MAGIC:
                            if (v.src_id != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待法术行动");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanMagic;
                            break;
                        case (uint)network.BasicActionType.ACTION_NONE:
                            //无法行动
                            if (v.src_id != MainPlayer.id)
                            {
                                MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待响应无法行动");
                                Agent.AgentState = (int)PlayerAgentState.Idle;
                                continue;
                            }
                            //Agent.Cmd = v;
                            Agent.AgentState = (int)PlayerAgentState.CanResign;
                            break;
                        default:
                            //技能响应
                            if (v.src_id != MainPlayer.id)
                            {
                                r = RoleFactory.Create(GetPlayerInfo(v.src_id).role_id);
                                if (r.Skills.ContainsKey(v.respond_id))
                                    MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待响应" + r.Skills[v.respond_id].SkillName);
                                else if (Skill.GetSkill(v.respond_id) != null)
                                    MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待响应" + Skill.GetSkill(v.respond_id).SkillName);
                                else
                                    MessageSystem<MessageType>.Notify(MessageType.PlayerActionChange, v.src_id, "等待响应技能" + v.respond_id.ToString());
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


