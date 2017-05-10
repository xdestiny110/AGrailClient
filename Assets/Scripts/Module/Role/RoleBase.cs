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
        public virtual uint MaxEnergyCount { get { return 3; } }
        public virtual bool HasYellow { get { return false; } }
        public virtual bool HasBlue { get { return false; } }
        public virtual bool HasCoverd { get { return false; } }
        public virtual string Knelt { get { return null; } }
        public Dictionary<uint, Skill> Skills = new Dictionary<uint, Skill>();        

        public virtual void Attack(uint card, uint dstID)
        {
            //普通攻击
            sendActionMsg(BasicActionType.ACTION_ATTACK, BattleData.Instance.MainPlayer.id,
                new List<uint>() { dstID }, new List<uint>() { card });
        }

        public virtual void Magic(uint card, uint dstID)
        {
            //基础法术
            sendActionMsg(BasicActionType.ACTION_MAGIC, BattleData.Instance.MainPlayer.id,
                new List<uint>() { dstID }, new List<uint>() { card });
        }

        public virtual void Weaken(List<uint> args)
        {
            //被虚弱 
            sendReponseMsg((uint)BasicRespondType.RESPOND_WEAKEN, BattleData.Instance.MainPlayer.id,
                null, null, args);
        }

        public virtual void MoDaned(Card card = null)
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

        public virtual void Heal(uint srcID, List<uint> args = null)
        {
            //治疗 
            sendReponseMsg((uint)BasicRespondType.RESPOND_HEAL, srcID, null, null, args);
        }

        public virtual void Drop(List<uint> NIDs)
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

        public virtual void Buy(uint gemNum, uint cristalNum)
        {
            //购买        
            sendActionMsg(BasicActionType.ACTION_SPECIAL, BattleData.Instance.MainPlayer.id,
                null, null, 0, new List<uint>() { gemNum, cristalNum });
        }

        public virtual void Synthetize(uint gemNum, uint cristalNum)
        {
            //合成
            sendActionMsg(BasicActionType.ACTION_SPECIAL, BattleData.Instance.MainPlayer.id,
                null, null, 1, new List<uint>() { gemNum, cristalNum });
        }

        public virtual void Extract(uint gemNum, uint cristalNum)
        {
            //提炼
            sendActionMsg(BasicActionType.ACTION_SPECIAL, BattleData.Instance.MainPlayer.id,
                null, null, 2, new List<uint>() { gemNum, cristalNum });
        }

        public virtual void AdditionAction()
        {
            sendReponseMsg((uint)BasicRespondType.RESPOND_ADDITIONAL_ACTION, 
                BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
        }

        public virtual bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1:
                    if (playerIDs.Count == 1 && cardIDs.Count == 1)
                        return true;
                    break;
                case 2:
                    if(cardIDs.Count == 1 && playerIDs.Count == 1)
                    {
                        if (Card.GetCard(cardIDs[0]).Name != Card.CardName.魔弹)
                            return true;
                        else
                        {
                            foreach (var v in BattleData.Instance.PlayerIdxOrder)
                            {
                                if (BattleData.Instance.PlayerInfos[v].team != BattleData.Instance.MainPlayer.team)
                                {
                                    if (BattleData.Instance.PlayerInfos[v].id == playerIDs[0])
                                        return true;
                                    break;
                                }
                            }
                        }    
                    }
                    break;
                case 3:
                    if (cardIDs.Count == 1)
                    {
                        if (playerIDs.Count == 0 && Card.GetCard(cardIDs[0]).Element == Card.CardElement.light)
                            return true;
                        if (playerIDs.Count == 1 && Card.GetCard(cardIDs[0]).Element != Card.CardElement.light)
                            return true;
                    }
                    break;
                case 4:
                    if (cardIDs.Count == 1)
                        return true;
                    break;
                case 5:
                    if (cardIDs.Count > 0)
                        return true;
                    break;
                case 6:
                    return true;
                case 12:
                case 13:
                case 14:
                case 15:
                    return true;                    
            }
            return false;
        }

        public virtual bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 3:
                case 4:
                case 6:
                case 12:
                case 13:
                case 14:
                    return true;
            }
            return false;
        }

        public virtual bool CheckResign(uint uiState)
        {
            switch (uiState)
            {
                case 15:
                    return true;
            }
            if (BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.CanResign))
                return true;
            return false;
        }
        
        public virtual bool CheckBuy(uint uiState)
        {
            var m = BattleData.Instance.MainPlayer;
            if (uiState == 10 && m.max_hand - m.hand_count >= 3 &&
                BattleData.Instance.Gem[m.team] + BattleData.Instance.Crystal[m.team] <= 4)
                return true;
            return false;
        }

        public virtual bool CheckExtract(uint uiState)
        {
            var m = BattleData.Instance.MainPlayer;
            if (uiState == 10 && m.gem + m.crystal < BattleData.Instance.Agent.PlayerRole.MaxEnergyCount &&
                BattleData.Instance.Gem[m.team] + BattleData.Instance.Crystal[m.team] > 0)
                return true;
            return false;
        }

        public virtual bool CheckSynthetize(uint uiState)
        {
            var m = BattleData.Instance.MainPlayer;
            if (uiState == 10 && m.max_hand - m.hand_count >= 3 &&
                BattleData.Instance.Gem[m.team] + BattleData.Instance.Crystal[m.team] >= 3)
                return true;
            return false;
        }

        //判断能否选择牌/角色/技能
        public virtual bool CanSelect(uint uiState, Card card)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                    if (card.Element != Card.CardElement.light)
                        return true;
                    break;
                case 1:
                    if (card.Type == Card.CardType.attack)
                        return true;
                    break;
                case 2:
                    if (card.Type == Card.CardType.magic && card.Element != Card.CardElement.light)
                        return true;
                    break;
                case 3:
                    if (card.Element == Card.CardElement.light ||
                        (((card.Element == Card.CardElement.darkness || card.Element == Card.GetCard(BattleData.Instance.Agent.Cmd.args[1]).Element) && 
                            BattleData.Instance.Agent.Cmd.args[0] < 1 && card.Type == Card.CardType.attack)))
                        return true;
                    break;
                case 4:
                    if (card.Name == Card.CardName.魔弹 || card.Element == Card.CardElement.light)
                        return true;
                    break;
                case 5:
                    return true;                    
            }
            return false;
        }

        public virtual bool CanSelect(uint uiState, network.SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1:
                    if (player.team != BattleData.Instance.MainPlayer.team &&
                        !(player.role_id == (uint)RoleID.AnSha && player.is_knelt))
                        return true;
                    break;
                case 2:
                    return true;
                case 3:
                    if (player.team != BattleData.Instance.MainPlayer.team && player.id != BattleData.Instance.Agent.Cmd.args[3])
                        return true;
                    break;
            }
            return false;
        }

        public virtual bool CanSelect(uint uiState, Skill skill)
        {
            return false;
        }

        public virtual uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {                
                case 1:
                case 2:
                case 3:
                    return 1;
            }
            return 0;
        }

        public virtual uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1:
                case 2:
                case 3:
                case 4:                
                case 10:
                case 11:
                    return 1;
                case 5:
                    return BattleData.Instance.Agent.Cmd.args[1];
            }
            return 0;
        }

        public System.Action OKAction = null;
        public System.Action CancelAction = null;
        public System.Action ResignAction = null;
        public virtual void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<List<uint>> selectList;
            uint tGem, tCrystal ;
            switch (state)
            {
                case 1:
                    //攻击
                    OKAction = () =>
                    {
                        Attack(BattleData.Instance.Agent.SelectCards[0], BattleData.Instance.Agent.SelectPlayers[0]);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 2:
                    //魔法
                    OKAction = () =>
                    {
                        Magic(BattleData.Instance.Agent.SelectCards[0], BattleData.Instance.Agent.SelectPlayers[0]);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 3:
                    //应战
                    if (BattleData.Instance.Agent.SelectCards.Count > 0)
                    {
                        if (BattleData.Instance.Agent.SelectPlayers.Count > 0)
                            OKAction = () =>
                            {
                                AttackedReply(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]),
                                    BattleData.Instance.Agent.SelectPlayers[0]);
                                BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            };
                        else
                            OKAction = () =>
                            {
                                AttackedReply(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]));
                                BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            };
                    }
                    CancelAction = () =>
                    {
                        AttackedReply();
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 4:
                    //魔弹响应
                    OKAction = () =>
                    {
                        MoDaned(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]));
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MoDaned();
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 5:
                    //弃牌
                    OKAction = () =>
                    {
                        Drop(BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 6:
                    //虚弱
                    OKAction = () =>
                    {
                        Weaken(new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        Weaken(new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 7:
                    //治疗
                    break;
                case 12:
                    //购买
                    if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] == 4)
                    {
                        selectList = new List<List<uint>>();
                        selectList.Add(new List<uint>() { 1, 0 });
                        selectList.Add(new List<uint>() { 0, 1 });
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Energy", selectList);
                        OKAction = () => 
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                            BattleData.Instance.Agent.PlayerRole.Buy(BattleData.Instance.Agent.SelectArgs[0], BattleData.Instance.Agent.SelectArgs[1]);
                        };
                        CancelAction = () => 
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                            BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                        };
                    }
                    else
                    {
                        BattleData.Instance.Agent.PlayerRole.Buy(1, 1);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    }
                    break;
                case 13:
                    //提炼
                    tGem = BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team];
                    tCrystal = BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team];
                    var gem = BattleData.Instance.MainPlayer.gem;
                    var crystal = BattleData.Instance.MainPlayer.crystal;
                    var maxEnergyCnt = BattleData.Instance.Agent.PlayerRole.MaxEnergyCount;
                    selectList = new List<List<uint>>();
                    if (maxEnergyCnt - gem - crystal >= 2)
                    {
                        if (tGem >= 2)
                            selectList.Add(new List<uint>() { 2, 0 });
                        if (tGem >= 1 && tCrystal >= 1)
                            selectList.Add(new List<uint>() { 1, 1 });
                        if (tCrystal >= 2)
                            selectList.Add(new List<uint>() { 0, 2 });
                    }
                    else
                    {
                        if (tGem >= 1)
                            selectList.Add(new List<uint>() { 1, 0 });
                        if (tCrystal >= 1)
                            selectList.Add(new List<uint>() { 0, 1 });
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Energy", selectList);
                    OKAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.PlayerRole.Extract(BattleData.Instance.Agent.SelectArgs[0], BattleData.Instance.Agent.SelectArgs[1]);
                    };
                    CancelAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                    };
                    break;
                case 14:
                    //合成
                    tGem = BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team];
                    tCrystal = BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team];
                    selectList = new List<List<uint>>();
                    if (tGem >= 3)
                        selectList.Add(new List<uint>() { 0, 3 });
                    if (tGem >= 1 && tCrystal >= 2)
                        selectList.Add(new List<uint>() { 1, 2 });
                    if (tGem >= 2 && tCrystal >= 1)
                        selectList.Add(new List<uint>() { 2, 1 });
                    if (tCrystal >= 3)
                        selectList.Add(new List<uint>() { 3, 0 });
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Energy", selectList);
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.PlayerRole.Synthetize(BattleData.Instance.Agent.SelectArgs[0], BattleData.Instance.Agent.SelectArgs[1]);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                    };
                    break;
                case 15:
                    //额外行动
                    selectList = new List<List<uint>>();
                    foreach (var v in BattleData.Instance.Agent.Cmd.args)
                        selectList.Add(new List<uint>() { v });
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Skill", selectList);
                    OKAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        AdditionAction();
                    };
                    ResignAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.SelectArgs.Clear();
                        BattleData.Instance.Agent.SelectArgs.Add((uint)BasicActionType.ACTION_NONE);
                        AdditionAction();
                    };
                    break;
            }
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
