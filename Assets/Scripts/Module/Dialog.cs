using UnityEngine;
using Framework;
using Framework.Message;
using System;
using Framework.Network;
using System.Collections.Generic;
using System.Text;

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

        public class ChatPerson
        {
            public uint ID;
            public uint? RoleID = null;
            public string msg;
        }
        public List<ChatPerson> Chat = new List<ChatPerson>();

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
            Chat.Clear();
        }

        public void SendTalk(string str)
        {
            Debug.LogFormat("talk str = {0}", str);
            var proto = new network.Talk() { txt = str };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.TALK });
        }

        StringBuilder str = new StringBuilder(1024);
        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            network.SinglePlayerInfo srcPlayer, dstPlayer;
            RoleBase r1, r2;
            str.Remove(0, str.Length);
            switch (eventType)
            {
                case MessageType.HITMSG:
                    var hitMsg = parameters[0] as network.HitMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(hitMsg.src_id);
                    dstPlayer = BattleData.Instance.GetPlayerInfo(hitMsg.dst_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    r2 = RoleFactory.Create(dstPlayer.role_id);
                    str.Append(string.Format("{0}命中了{1}" + Environment.NewLine, r1.RoleName, r2.RoleName));
                    Log += "<color=#ffffff>"+str.ToString() + "</color>";
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, str.ToString());
                    break;
                case MessageType.HURTMSG:
                    var hurtMsg = parameters[0] as network.HurtMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(hurtMsg.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    str.Append(r1.RoleName);
                    if (hurtMsg.dst_idSpecified)
                    {
                        dstPlayer = BattleData.Instance.GetPlayerInfo(hurtMsg.dst_id);
                        r2 = RoleFactory.Create(dstPlayer.role_id);
                        str.Append("对" + r2.RoleName);
                    }
                    str.Append(string.Format("造成{0}点伤害" + Environment.NewLine, hurtMsg.hurt));
                    Log += string.Format("<color=#ffffff>" + str.ToString() + "</color>");
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, str.ToString());
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(cardMsg.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    str.Append(r1.RoleName);
                    if (cardMsg.dst_idSpecified)
                    {
                        dstPlayer = BattleData.Instance.GetPlayerInfo(cardMsg.dst_id);
                        r2 = RoleFactory.Create(dstPlayer.role_id);
                        str.Append("对" + r2.RoleName + "使用了");
                    }
                    else
                        str.Append("展示了");
                    foreach (var v in cardMsg.card_ids)
                    {
                        var c = Card.GetCard(v);
                        str.Append(c.Name + "-" + c.Property.ToString() +"　");
                    }
                    str.Append(Environment.NewLine);
                    Log += str.ToString();
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, str.ToString());
                    break;
                case MessageType.SKILLMSG:
                    var skillMsg = parameters[0] as network.SkillMsg;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(skillMsg.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    str.Append(r1.RoleName);
                    if (skillMsg.dst_ids.Count > 0) str.Append("对");
                    foreach (var v in skillMsg.dst_ids)
                    {
                        dstPlayer = BattleData.Instance.GetPlayerInfo(v);
                        if(dstPlayer != null)
                        {
                            r2 = RoleFactory.Create(dstPlayer.role_id);
                            str.Append(r2.RoleName + ",");
                        }
                    }
                    var s = Skill.GetSkill(skillMsg.skill_id);
                    if (s != null)
                        str.Append(string.Format("使用了技能{0}" + Environment.NewLine, s.SkillName));
                    else
                        str.Append(string.Format("使用了技能{0}" + Environment.NewLine, skillMsg.skill_id));
                    log += string.Format("<color=#ffffff>" + str.ToString()+"</color>");
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, str.ToString());
                    break;
                case MessageType.GOSSIP:
                    var gossip = parameters[0] as network.Gossip;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(gossip.id);
                    if (srcPlayer.role_idSpecified)
                        Chat.Add(new ChatPerson() { ID = srcPlayer.id, RoleID = srcPlayer.role_id, msg = gossip.txt.Replace(" ", "\u00a0") });
                    else
                        Chat.Add(new ChatPerson() { ID = srcPlayer.id, msg = gossip.txt });
                    MessageSystem<MessageType>.Notify(MessageType.ChatChange);
                    break;
                case MessageType.PlayerLeave:
                    Log += string.Format("<color=#FF0000FF>玩家[{0}]离开房间</color>" + Environment.NewLine,
                        BattleData.instance.GetPlayerInfo((uint)(int)parameters[0]).nickname);
                    break;
                case MessageType.TURNBEGIN:
                    Log += "<color=#606060>---------------------------------</color>" + Environment.NewLine;
                    break;
                case MessageType.ACTION:
                    var act = parameters[0] as network.Action;
                    srcPlayer = BattleData.Instance.GetPlayerInfo(act.src_id);
                    r1 = RoleFactory.Create(srcPlayer.role_id);
                    str.Append(string.Format("{0}进行了", r1.RoleName));
                    switch (act.action_id)
                    {
                        case 0:
                            str.Append("购买" + Environment.NewLine);
                            break;
                        case 1:
                            str.Append("合成" + Environment.NewLine);
                            break;
                        case 2:
                            str.Append("提炼" + Environment.NewLine);
                            break;
                        default:
                            str.Append("奇怪的行动" + Environment.NewLine);
                            break;
                    }
                    log += str.ToString();
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, str.ToString());
                    break;
            }
        }
    }
}

