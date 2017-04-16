using UnityEngine;
using System.Collections;
using Framework;
using Framework.Message;
using System;
using System.Linq;

namespace AGrail
{
    public class Dialog : Singleton<Dialog>, IMessageListener<MessageType>
    {
        private string log;
        public string Log
        {
            get { return log; }
            private set
            {
                log = value;
                MessageSystem<MessageType>.Notify(MessageType.LogChange);
            }
        }

        public Dialog() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.SKILLMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.HITMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.HURTMSG, this);
            MessageSystem<MessageType>.Regist(MessageType.GOSSIP, this);
            Reset();
        }

        public void Reset()
        {
            Log = string.Empty;
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            network.SinglePlayerInfo srcPlayer, dstPlayer;
            switch (eventType)
            {
                case MessageType.HITMSG:
                    var hitMsg = parameters[0] as network.HitMsg;
                    srcPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => { return u != null && u.id == hitMsg.src_id; });
                    dstPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => { return u != null && u.id == hitMsg.dst_id; });
                    Log += string.Format("{0}攻击了{1}" + Environment.NewLine, srcPlayer.nickname, dstPlayer.nickname);
                    break;
                case MessageType.HURTMSG:
                    var hurtMsg = parameters[0] as network.HurtMsg;
                    srcPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => { return u != null && u.id == hurtMsg.src_id; });
                    Log += srcPlayer.nickname;
                    if (hurtMsg.dst_idSpecified)
                    {
                        dstPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                            u => { return u != null && u.id == hurtMsg.dst_id; });
                        Log += "对" + dstPlayer.nickname;
                    }
                    Log += string.Format("造成{0}点伤害" + Environment.NewLine, hurtMsg.hurt);
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    srcPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => { return u != null && u.id == cardMsg.src_id; });
                    Log += srcPlayer.nickname;
                    if (cardMsg.dst_idSpecified)
                    {
                        dstPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                            u => { return u != null && u.id == cardMsg.dst_id; });
                        Log += "对" + dstPlayer.nickname + "使用了";
                    }
                    else
                        Log += "展示了";                       
                    foreach (var v in cardMsg.card_ids)
                    {
                        var c = new Card(v);
                        Log += c.Name + ",";
                    }                        
                    Log += Environment.NewLine;
                    break;
                case MessageType.SKILLMSG:
                    var skillMsg = parameters[0] as network.SkillMsg;
                    srcPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => { return u != null && u.id == skillMsg.src_id; });
                    Log += srcPlayer.nickname;
                    if (skillMsg.dst_ids.Count > 0) Log += "对";
                    foreach (var v in skillMsg.dst_ids)
                    {
                        dstPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                            u => { return u != null && u.id == v; });
                        Log += dstPlayer.nickname + ",";
                    }                        
                    Log += string.Format("使用了技能{0}" + Environment.NewLine, skillMsg.skill_id);                    
                    break;
                case MessageType.GOSSIP:
                    var gossip = parameters[0] as network.Gossip;
                    srcPlayer = BattleData.Instance.PlayerInfos.DefaultIfEmpty(null).FirstOrDefault(
                        u => { return u != null && u.id == gossip.id; });
                    Log += string.Format("{0}: {1}" + Environment.NewLine, srcPlayer.nickname, gossip.txt);
                    break;
            }
        }
    }
}

