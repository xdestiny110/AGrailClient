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

        public virtual void Check(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, false);
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetCancelCallback, false);
            checkCanAttack(agentState, cardIDs, playerIDs, skillID);
            checkCanMagic(agentState, cardIDs, playerIDs, skillID);
            checkCanSpecial(agentState, cardIDs, playerIDs, skillID);
            checkAttacked(agentState, cardIDs, playerIDs, skillID);
            checkModaned(agentState, cardIDs, playerIDs, skillID);
            checkWeaken(agentState, cardIDs, playerIDs, skillID);
            checkDiscard(agentState, cardIDs, playerIDs, skillID);
            checkSkill(agentState, cardIDs, playerIDs, skillID);
        }

        protected virtual void attack(uint card, uint dstID)
        {
            //普通攻击
            sendActionMsg(BasicActionType.ACTION_ATTACK, BattleData.Instance.MainPlayer.id, 
                new List<uint>() { dstID }, new List<uint>() { card });
        }

        protected virtual void magic(uint card, uint dstID)
        {
            //基础法术
            sendActionMsg(BasicActionType.ACTION_MAGIC, BattleData.Instance.MainPlayer.id,
                new List<uint>() { dstID }, new List<uint>() { card });
        }

        protected virtual void weaken(uint srcID, List<uint> args = null)
        {
            //被虚弱 
            sendReponseMsg((uint)BasicRespondType.RESPOND_WEAKEN, srcID, null, null, args);
        }

        protected virtual void moDaned(uint srcID, Card card = null)
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

        protected virtual void heal(uint srcID, List<uint> args = null)
        {
            //治疗 
            sendReponseMsg((uint)BasicRespondType.RESPOND_HEAL, srcID, null, null, args);
        }

        protected virtual void drop(List<uint> NIDs)
        {
            //弃牌
            sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD, BattleData.Instance.MainPlayer.id,
                null, NIDs, new List<uint>() { 1 });
        }

        protected virtual void attackedReply(Card card = null, uint? dstID = null)
        {
            //应战
            var srcID = BattleData.Instance.MainPlayer.id;
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

        protected virtual void buy(uint srcID, uint gemNum, uint cristalNum)
        {
            //购买        
            sendActionMsg(BasicActionType.ACTION_SPECIAL, srcID, null, null, 0, new List<uint>() { gemNum, cristalNum });
        }

        protected virtual void synthetize(uint srcID, uint gemNum, uint cristalNum)
        {
            //合成
            sendActionMsg(BasicActionType.ACTION_SPECIAL, srcID, null, null, 1, new List<uint>() { gemNum, cristalNum });
        }

        protected virtual void extract(uint srcID, uint gemNum, uint cristalNum)
        {
            //提炼
            sendActionMsg(BasicActionType.ACTION_SPECIAL, srcID, null, null, 2, new List<uint>() { gemNum, cristalNum });
        }

        protected virtual void useSkill(uint skillID, uint srcID, List<uint> dstID = null, List<uint> cardIds = null, List<uint> args = null)
        {

        }

        protected virtual void checkCanAttack(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //基础攻击
            if (agentState.Check(PlayerAgentState.CanAttack))
            {
                //是攻击牌，是敌方，且能够攻击（潜行之类的）
                if (skillID == null && cardIDs.Count == 1 && playerIDs.Count == 1)
                {
                    var card = new Card(cardIDs[0]);
                    var player = BattleData.Instance.PlayerInfos.Find(p => { return p.id == playerIDs[0]; });
                    if (card.Type == Card.CardType.attack && player.team != BattleData.Instance.MainPlayer.team &&
                        !(player.role_id == (uint)RoleID.AnSha && player.is_knelt))
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                            new System.Action(() =>
                            {
                                attack(card.ID, player.id);
                            }));
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetCancelCallback, false);
                    }
                }
            }
        }

        protected virtual void checkCanMagic(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //基础魔法
            if (agentState.Check(PlayerAgentState.CanMagic))
            {
                if (skillID == null && cardIDs.Count == 1 && playerIDs.Count == 1)
                {
                    var card = new Card(cardIDs[0]);
                    var player = BattleData.Instance.PlayerInfos.Find(p => { return p.id == playerIDs[0]; });
                    if (card.Type == Card.CardType.magic && card.Element != Card.CardElement.light)
                    {
                        if (card.Name != Card.CardName.魔弹)
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                                new System.Action(() =>
                                {
                                    magic(card.ID, player.id);
                                }));
                        }
                        else
                        {
                            //确认是否是下一个对方玩家

                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                                new System.Action(() =>
                                {

                                }));
                        }
                    }
                }
            }
        }

        protected virtual void checkCanSpecial(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //特殊行动
            if (agentState.Check(PlayerAgentState.CanSpecial))
            {
                var mainPlayer = BattleData.Instance.MainPlayer;
                if (mainPlayer.max_hand - mainPlayer.hands.Count >= 3)
                {
                    //可以买
                    if (BattleData.Instance.Gem[(int)mainPlayer.team] + BattleData.Instance.Crystal[(int)mainPlayer.team] > 0)
                    {
                        //可以合
                    }
                }
                if (BattleData.Instance.Gem[(int)mainPlayer.team] + BattleData.Instance.Crystal[(int)mainPlayer.team] > 0)
                {
                    //可以提
                }
            }
        }

        protected virtual void checkWeaken(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //虚弱响应
            if (agentState.Check(PlayerAgentState.Weaken))
            {
                
            }
        }

        protected virtual void checkAttacked(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //应战
            if (agentState.Check(PlayerAgentState.Attacked))
            {
                if((cardIDs.Count == 1 && playerIDs.Count == 1 && skillID == null && playerIDs[0] != BattleData.Instance.Agent.RespCmd.src_id) ||
                    cardIDs.Count == 1 && playerIDs.Count == 0 && skillID == null)
                {
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                        new System.Action(() =>
                        {
                            attackedReply(new Card(cardIDs[0]), playerIDs[0]);
                        }));
                }
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetCancelCallback, true,
                    new System.Action(() =>
                    {
                        attackedReply();
                    }));
            }
        }

        protected virtual void checkModaned(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //魔弹响应
            if (agentState.Check(PlayerAgentState.MoDaned))
            {
                var card = new Card(cardIDs[0]);
                if (card.Name == Card.CardName.魔弹)
                {
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                        new System.Action(() =>
                        {
                        }));
                }
                else if (card.Name == Card.CardName.圣光)
                {
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                        new System.Action(() =>
                        {
                        }));
                }
            }
        }        

        protected virtual void checkDiscard(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //弃牌
            if (agentState.Check(PlayerAgentState.Discard))
            {
                if (cardIDs.Count > 0)
                {
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                        new System.Action(() =>
                        {
                            drop(cardIDs);
                        }));
                }
            }
        }

        protected virtual void checkSkill(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //确认技能
            //肯定得重写，但先设成virtual
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
