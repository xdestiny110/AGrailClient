using UnityEngine;
using System.Collections;
using Framework;
using Framework.Message;
using System;
using System.Linq;
using Framework.Network;

namespace AGrail
{
    public class Dialog : Singleton<Dialog>, IMessageListener<MessageType>
    {
        private string log;
        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                MessageSystem<MessageType>.Notify(MessageType.LogChange);
            }
        }

        public Dialog() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.ACTION, this);
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.HITMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.HURTMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.GOSSIP, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerLeave, this);
            MessageSystem<MessageType>.Regist(MessageType.TURNBEGIN, this);            
            Reset();
        }

        public void Reset()
        {
            Log = string.Empty;
        }

        public void SendTalk(string str)
        {
            Debug.LogFormat("talk str = {0}", str);
            var proto = new network.Talk() { txt = str };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.TALK });
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            network.SinglePlayerInfo srcPlayer, dstPlayer;
            RoleBase r1, r2;
            switch (eventType)
            {
                case MessageType.HITMSG:
                    var hitMsg = parameters[0] as network.HitMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(hitMsg.src_id);
                    dstPlayer = BattleData.Instance.GetPlayerInfo(hitMsg.dst_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    r2 = RoleFactory.Create(dstPlayer.role_id);
                    Log += string.Format("{0}攻击了{1}" + Environment.NewLine, r1.RoleName, r2.RoleName);
                    break;
                case MessageType.HURTMSG:
                    var hurtMsg = parameters[0] as network.HurtMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(hurtMsg.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    Log += r1.RoleName;
                    if (hurtMsg.dst_idSpecified)
                    {
                        dstPlayer = BattleData.Instance.GetPlayerInfo(hurtMsg.dst_id);
                        r2 = RoleFactory.Create(dstPlayer.role_id);
                        Log += "对" + r2.RoleName;
                    }
                    Log += string.Format("造成{0}点伤害" + Environment.NewLine, hurtMsg.hurt);
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(cardMsg.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    Log += r1.RoleName;
                    if (cardMsg.dst_idSpecified)
                    {
                        dstPlayer = BattleData.Instance.GetPlayerInfo(cardMsg.dst_id);
                        r2 = RoleFactory.Create(dstPlayer.role_id);
                        Log += "对" + r2.RoleName + "使用了";
                    }
                    else
                        Log += "展示了";                       
                    foreach (var v in cardMsg.card_ids)
                    {
                        var c = Card.GetCard(v);
                        Log += c.Name + ",";
                    }                        
                    Log += Environment.NewLine;
                    break;
                case MessageType.SKILLMSG:
                    var skillMsg = parameters[0] as network.SkillMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(skillMsg.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    Log += r1.RoleName;
                    if (skillMsg.dst_ids.Count > 0) Log += "对";
                    foreach (var v in skillMsg.dst_ids)
                    {
                        dstPlayer = BattleData.Instance.GetPlayerInfo(v);
                        if(dstPlayer != null)
                        {
                            r2 = RoleFactory.Create(dstPlayer.role_id);
                            Log += r2.RoleName + ",";
                        }
                    }
                    var s = Skill.GetSkill(skillMsg.skill_id);
                    if(s != null)
                        Log += string.Format("使用了技能{0}" + Environment.NewLine, s.SkillName);
                    else                    
                        Log += string.Format("使用了技能{0}" + Environment.NewLine, skillMsg.skill_id);
                    break;
                case MessageType.GOSSIP:
                    var gossip = parameters[0] as network.Gossip;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(gossip.id);
                    if (srcPlayer.role_idSpecified)
                    {
                        r1 = RoleFactory.Create(srcPlayer.role_id);
                        Log += string.Format("<color=#0000FFFF>[{0}]: {1}</color>" + Environment.NewLine, r1.RoleName, gossip.txt);
                    }
                    else
                        Log += string.Format("<color=#0000FFFF>[{0}]: {1}</color>" + Environment.NewLine, srcPlayer.nickname, gossip.txt);
                    break;
                case MessageType.PlayerLeave:
                    Log += string.Format("<color=#FF0000FF>玩家[{0}]离开房间</color>" + Environment.NewLine, (int)parameters[0]);
                    break;
                case MessageType.TURNBEGIN:
                    Log += "<color=#00FF00FF>=======================</color>" + Environment.NewLine;
                    break;
                case MessageType.ACTION:
                    var act = parameters[0] as network.Action;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(act.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    Log += string.Format("{0}进行了", r1.RoleName);
                    switch (act.action_id)
                    {
                        case 0:
                            Log += "购买" + Environment.NewLine;
                            break;
                        case 1:
                            Log += "合成" + Environment.NewLine;
                            break;
                        case 2:
                            Log += "提炼" + Environment.NewLine;
                            break;
                        default:
                            Log += "奇怪的行动" + Environment.NewLine;
                            break;
                    }
                    break;
            }
        }
    }
}

