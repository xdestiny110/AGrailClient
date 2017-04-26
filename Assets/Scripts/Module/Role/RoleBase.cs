using UnityEngine;
using System.Collections;
using network;
using System.Collections.Generic;
using Framework.Network;
using Framework.Message;

namespace AGrail
{
    public abstract class RoleBase
    {
        public abstract RoleID RoleID { get; }
        public abstract string RoleName { get; }
        public abstract Card.CardProperty RoleProperty { get; }
        public virtual uint MaxHealCount { get { return 2; } }
        public virtual bool HasYellow { get { return false; } }
        public virtual bool HasBlue { get { return false; } }
        public virtual bool HasCoverd { get { return false; } }
        public virtual string Knelt { get { return null; } }

        public virtual void Attack(uint srcID, Card card, uint dstID)
        {
            //普通攻击
            sendActionMsg(BasicActionType.ACTION_ATTACK, srcID, new List<uint>() { dstID }, new List<uint>() { card.ID });
        }

        public virtual void Magic(uint srcID, Card card, uint dstID)
        {
            //基础法术
            sendActionMsg(BasicActionType.ACTION_MAGIC, srcID, new List<uint>() { dstID }, new List<uint>() { card.ID });
        }

        public virtual void Weaken(uint srcID, List<uint> args = null)
        {
            //被虚弱 
            sendReponseMsg((uint)BasicRespondType.RESPOND_WEAKEN, srcID, null, null, args);
        }

        public virtual void MoDaned(uint srcID, Card card = null)
        {
            //被魔弹
            if (card != null)
            {
                if (card.Element != Card.CardElement.light)
                    sendReponseMsg((uint)BasicRespondType.RESPOND_BULLET, srcID, null, new List<uint>() { card.ID }, new List<uint>() { 0 });
                else
                    sendReponseMsg((uint)BasicRespondType.RESPOND_BULLET, srcID, null, new List<uint>() { card.ID }, new List<uint>() { 1 });
            }
            else
                sendReponseMsg((uint)BasicRespondType.RESPOND_BULLET, srcID, null, null, new List<uint>() { 2 });
        }

        public virtual void Heal(uint srcID, List<uint> args = null)
        {
            //治疗 
            sendReponseMsg((uint)BasicRespondType.RESPOND_HEAL, srcID, null, null, args);
        }

        public virtual void Drop(uint srcID, List<uint> NIDs)
        {
            //弃牌
            sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD, srcID, null, NIDs, new List<uint>() { 1 });
        }

        public virtual void AttackedReply(uint srcID, Card card = null, uint? dstID = null)
        {
            //应战
            if (!dstID.HasValue && card == null)
            {
                //放弃
                sendReponseMsg((uint)BasicRespondType.RESPOND_REPLY_ATTACK, srcID, null, null, new List<uint>() { 2 });
            }
            else
            {
                if (dstID.HasValue && card.Element != Card.CardElement.light)
                {
                    //应战
                    sendReponseMsg((uint)BasicRespondType.RESPOND_REPLY_ATTACK, srcID, new List<uint>() { dstID.Value }, new List<uint>() { card.ID }, new List<uint> { 0 });
                }
                else
                {
                    //圣光
                    sendReponseMsg((uint)BasicRespondType.RESPOND_REPLY_ATTACK, srcID, null, new List<uint>() { card.ID }, new List<uint>() { 1 });
                }
            }
        }

        public virtual void Buy(uint srcID, uint gemNum, uint cristalNum)
        {
            //购买        
            sendActionMsg(BasicActionType.ACTION_SPECIAL, srcID, null, null, 0, new List<uint>() { gemNum, cristalNum });
        }

        public virtual void Synthetize(uint srcID, uint gemNum, uint cristalNum)
        {
            //合成
            sendActionMsg(BasicActionType.ACTION_SPECIAL, srcID, null, null, 1, new List<uint>() { gemNum, cristalNum });
        }

        public virtual void Extract(uint srcID, uint gemNum, uint cristalNum)
        {
            //提炼
            sendActionMsg(BasicActionType.ACTION_SPECIAL, srcID, null, null, 2, new List<uint>() { gemNum, cristalNum });
        }

        public virtual void UseSkill(uint skillID, uint srcID, List<uint> dstID = null, List<uint> cardIds = null, List<uint> args = null)
        {

        }

        public virtual void Check(int agentState ,List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            if((agentState | (int)PlayerAgentState.CanAttack) != 0)
            {
                //是攻击牌，是敌方，且能够攻击（潜行之类的）
                if(skillID == null && cardIDs.Count == 1 && playerIDs.Count == 1)
                {
                    var card = new Card(cardIDs[0]);
                    var player = BattleData.Instance.PlayerInfos.Find(p => { return p.id == playerIDs[0]; });
                    if(card.Type == Card.CardType.attack && player.team != BattleData.Instance.MainPlayer.team &&
                        !(player.role_id == (uint)RoleID.AnSha && player.is_knelt))
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                            new System.Action(() => 
                            {
                            }));
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetCancelCallback, true,
                            new System.Action(() => 
                            {
                            }));
                    }
                }
            }
            if((agentState | (int)PlayerAgentState.CanMagic) != 0)
            {

            }

            //啥都不满足...
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, false);
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetCancelCallback, false);
        }

        protected void sendActionMsg(BasicActionType actionType, uint srcID, List<uint> dstID = null, List<uint> cardIDs = null, uint? actionID = null, List<uint> args = null)
        {
            Action proto = new Action() { action_type = (uint)actionType, src_id = srcID };
            if (dstID != null && dstID.Count > 0)
            {
                foreach (var v in dstID)
                    proto.dst_ids.Add(v);
            }

            if (cardIDs != null && cardIDs.Count > 0)
            {
                foreach (var v in cardIDs)
                    proto.card_ids.Add(v);
            }

            if (actionID.HasValue)
            {
                proto.action_id = actionID.Value;
                if (args != null && args.Count > 0)
                {
                    foreach (var v in args)
                        proto.args.Add(v);
                }
            }
            
            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.ACTION });
        }

        protected void sendReponseMsg(uint respondID, uint srcID, List<uint> dstID = null, List<uint> cardIDs = null, List<uint> args = null)
        {
            Respond proto = new Respond() { respond_id = respondID, src_id = srcID };
            if (dstID != null)
            {
                foreach (var v in dstID)
                    proto.dst_ids.Add(v);
            }
            if (cardIDs != null)
            {
                foreach (var v in cardIDs)
                    proto.card_ids.Add(v);
            }
            if (args != null)
            {
                foreach (var v in args)
                    proto.args.Add(v);
            }

            GameManager.TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.RESPOND });
        }
    }
}
