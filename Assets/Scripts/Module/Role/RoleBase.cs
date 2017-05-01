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

        public virtual void Check(int agentState, List<uint> cardIDs = null, List<uint> playerIDs = null, uint? skillID = null)
        {
            if (cardIDs == null)
                cardIDs = new List<uint>();
            if (playerIDs == null)
                playerIDs = new List<uint>();
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, false);
            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetCancelCallback, false);
            if (checkCanBuy(agentState))
            {

            }
            if (checkCanExtract(agentState))
            {

            }
            if (checkCanSynthetize(agentState))
            {

            }
            if (checkCanAttack(agentState, cardIDs, playerIDs, skillID))
            {
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        attack(cardIDs[0], playerIDs[0]);
                    }));
            }
            else if (checkCanMagic(agentState, cardIDs, playerIDs, skillID))
            {
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        magic(cardIDs[0], playerIDs[0]);
                    }));
            }
            else if (checkAttacked(agentState, cardIDs, playerIDs, skillID))
            {
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        if (cardIDs.Count > 0)
                        {
                            if(playerIDs.Count > 0)
                                AttackedReply(new Card(cardIDs[0]), playerIDs[0]);
                            else
                                AttackedReply(new Card(cardIDs[0]));
                        }                            
                        else
                            AttackedReply();
                    }));
            }
            else if (checkDiscard(agentState, cardIDs, playerIDs, skillID))
            {
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        drop(cardIDs);
                    }));
            }
            else if (checkModaned(agentState, cardIDs, playerIDs, skillID))
            {
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        if (cardIDs.Count > 0)
                            moDaned(new Card(cardIDs[0]));
                        else
                            moDaned();
                    }));
            }
            else if (checkWeaken(agentState, cardIDs, playerIDs, skillID))
            {
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        weaken(new List<uint>() { 1 });
                    }));
                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentSetOKCallback, true,
                    new UnityEngine.Events.UnityAction(() =>
                    {
                        weaken(new List<uint>() { 0 });
                    }));
            }
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

        protected virtual void weaken(List<uint> args)
        {
            //被虚弱 
            sendReponseMsg((uint)BasicRespondType.RESPOND_WEAKEN, BattleData.Instance.MainPlayer.id,
                null, null, args);
        }

        protected virtual void moDaned(Card card = null)
        {
            //被魔弹
            if (card != null)
            {
                if (card.Element != Card.CardElement.light)
                    sendReponseMsg((uint)BasicRespondType.RESPOND_BULLET, BattleData.Instance.MainPlayer.id,
                        null, new List<uint>() { card.ID }, new List<uint>() { 0 });
                else
                    sendReponseMsg((uint)BasicRespondType.RESPOND_BULLET, BattleData.Instance.MainPlayer.id,
                        null, new List<uint>() { card.ID }, new List<uint>() { 1 });
            }
            else
                sendReponseMsg((uint)BasicRespondType.RESPOND_BULLET, BattleData.Instance.MainPlayer.id,
                    null, null, new List<uint>() { 2 });
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

        public virtual void AttackedReply(Card card = null, uint? dstID = null)
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

        protected virtual bool checkCanAttack(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
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
                        return true;
                }
            }
            return false;
        }

        protected virtual bool checkCanMagic(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
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
                            return true;
                        else
                        {
                            //确认是否是下一个对方玩家
                        }
                    }
                }
            }
            return false;
        }

        protected virtual bool checkCanBuy(int agentState)
        {
            if (agentState.Check(PlayerAgentState.CanSpecial) &&
                BattleData.Instance.MainPlayer.max_hand - BattleData.Instance.MainPlayer.hand_count >= 3)
                return true;
            return false;
        }

        protected virtual bool checkCanExtract(int agentState)
        {
            return false;
        }

        protected virtual bool checkCanSynthetize(int agentState)
        {
            if (agentState.Check(PlayerAgentState.CanSpecial) &&
                BattleData.Instance.MainPlayer.max_hand - BattleData.Instance.MainPlayer.hand_count >= 3 &&
                BattleData.Instance.Gem[(int)BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[(int)BattleData.Instance.MainPlayer.team] >= 3)
                return true;
            return false;
        }

        protected virtual bool checkWeaken(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //虚弱响应
            if (agentState.Check(PlayerAgentState.Weaken))
                return true;
            return false;
        }

        protected virtual bool checkAttacked(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //应战
            //还得斟酌下...必中还没弄进去
            if (agentState.Check(PlayerAgentState.Attacked))
            {
                if (cardIDs.Count == 1 && skillID == null)
                {
                    Card c1 = new Card(cardIDs[0]);
                    if (c1.Element == Card.CardElement.light)
                        return true;
                    Card c2 = new Card(BattleData.Instance.Agent.Cmd.args[1]);
                    if (playerIDs.Count == 1 && playerIDs[0] != BattleData.Instance.Agent.Cmd.args[3] && 
                        BattleData.Instance.GetPlayerInfo(playerIDs[0]).team != BattleData.Instance.MainPlayer.team &&
                        ((c1.Element == Card.CardElement.darkness || c1.Element == c2.Element) && BattleData.Instance.Agent.Cmd.args[0] < 1))
                        return true;
                }
                if (cardIDs.Count == 0 && skillID == null && playerIDs.Count == 0)
                    return true;
            }
            return false;
        }

        protected virtual bool checkModaned(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //魔弹响应
            if (agentState.Check(PlayerAgentState.MoDaned))
            {
                var card = new Card(cardIDs[0]);
                if (card.Name == Card.CardName.魔弹 || card.Name == Card.CardName.圣光)
                    return true;
            }
            return false;
        }

        protected virtual bool checkDiscard(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //弃牌
            if (agentState.Check(PlayerAgentState.Discard))
            {
                if (cardIDs.Count > 0 && cardIDs.Count < BattleData.Instance.Agent.Cmd.args[1])
                    return true;
            }
            return false;
        }

        protected virtual bool checkSkill(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            //确认技能
            //肯定得重写，但先设成virtual
            return false;
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
