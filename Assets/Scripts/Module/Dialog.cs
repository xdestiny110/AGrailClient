using UnityEngine;
using System.Collections;
using Framework;
using Framework.Message;
using System;

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
            switch (eventType)
            {
                case MessageType.HITMSG:
                    var hitMsg = parameters[0] as network.HitMsg;                    
                    Log += string.Format("{0}攻击了{1}" + Environment.NewLine, hitMsg.src_id, hitMsg.dst_id);
                    break;
                case MessageType.HURTMSG:
                    var hurtMsg = parameters[0] as network.HurtMsg;
                    Log += string.Format("{0}对{1}造成{2}点伤害" + Environment.NewLine,
                        BattleData.Instance.PlayerInfos[(int)hurtMsg.src_id].nickname,
                        hurtMsg.dst_idSpecified ? BattleData.Instance.PlayerInfos[(int)hurtMsg.dst_id].nickname : "自己",
                        hurtMsg.hurt);
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    Log += string.Format(
                        (cardMsg.type == (uint)network.CardMsgType.CM_SHOW) ?
                        "{0}对{1}展示了{2}." + Environment.NewLine : "{0}对{1}使用了{2}." + Environment.NewLine,
                        BattleData.Instance.PlayerInfos[(int)cardMsg.src_id].nickname,
                        cardMsg.dst_idSpecified ? BattleData.Instance.PlayerInfos[(int)cardMsg.dst_id].nickname : "自己",
                        cardMsg.card_ids);
                    break;
                case MessageType.SKILLMSG:
                    var skillMsg = parameters[0] as network.SkillMsg;
                    Log += string.Format("{0}对{1}使用了技能{2}" + Environment.NewLine, skillMsg.src_id, skillMsg.dst_ids[0], skillMsg.skill_id);
                    break;
                case MessageType.GOSSIP:
                    var gossip = parameters[0] as network.Gossip;
                    Log += string.Format("{0}: {1}" + Environment.NewLine, BattleData.Instance.PlayerInfos[(int)gossip.id].nickname, gossip.txt);
                    break;
            }
        }
    }
}

