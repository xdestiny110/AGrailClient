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
        public abstract string HeroName { get; }
        public virtual uint MaxHealCount { get { return 2; } }
        public virtual uint MaxEnergyCount { get { return 3; } }
        public virtual bool HasYellow { get { return false; } }
        public virtual bool HasBlue { get { return false; } }
        public virtual bool HasCoverd { get { return false; } }
        public virtual string Knelt { get { return null; } }
        public virtual bool IsStart { set; get; }
        public Dictionary<uint, Skill> Skills = new Dictionary<uint, Skill>();
        public Dictionary<uint, Mation> Mations { get { return Mation.GetMation((uint)RoleID); } }
        public abstract uint Star { get; }
        public bool attackable = false;
        //断线重连应对机制
        public bool NoSelection = false;

        //记录一些特殊状态
        //由于当初没想好导致必须要在Role中维护这个状态...这个要比较小心
        protected uint additionalState { get; set; }

        public virtual bool CheckunActional(uint uiState)
        {
            if (CheckBuy(uiState)) return false;
            NoSelection = false;
            if (uiState == 11 || uiState == 10)
            {
                foreach (var v in BattleData.Instance.MainPlayer.hands)
                    if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, Card.GetCard(v), false))
                        return false;
                foreach (var v in BattleData.Instance.Agent.PlayerRole.Skills.Values)
                    if (BattleData.Instance.Agent.PlayerRole.CanSelect(BattleData.Instance.Agent.FSM.Current.StateNumber, v))
                        return false;
                NoSelection = true;
                return true;
            }
            return false;
        }

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

        public virtual void Heal(uint srcID, List<uint> args)
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
            attackable = true;
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
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                        return true;
                    break;
                case 5:
                case 8:
                    if (cardIDs.Count == BattleData.Instance.Agent.Cmd.args[1])
                        return true;
                    break;
                //case 6:
                //case 7:
                //return true;
                case 1602:
                    return true;


                    //case 3105:	//激昂狂想曲-代价
                    //case 3106:	//胜利交响诗-代价
                    // case 31061:	//胜利交响诗-效果
                    // return true;
                    //case 31052:	//激昂狂想曲-效果



            }
            return false;
        }

        public virtual bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 3:
                case 4:
                //case 6:
                case 7:
                case 12:
                case 13:
                case 14:
                case 1602:
                case 3105:  //激昂狂想曲-代价
                            //case 31052:	//激昂狂想曲-效果
                case 3106:  //胜利交响诗-代价
                            //case 31061:	//胜利交响诗-效果
                    return true;
                case 5:
                    if (BattleData.Instance.Agent.Cmd.args[0] == 801 || BattleData.Instance.Agent.Cmd.args[0] == 805 ||
                        BattleData.Instance.Agent.Cmd.args[0] == 29051)
                        return true;
                    return false;
            }
            return false;
        }

        public virtual bool CheckResign(uint uiState)
        {
            if (CheckunActional(uiState)) return true;
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
            if (uiState == 10 && m.max_hand - m.hand_count >= 3 && !IsStart)
                return true;
            return false;
        }

        public virtual bool CheckExtract(uint uiState)
        {
            var m = BattleData.Instance.MainPlayer;
            if (uiState == 10 && m.gem + m.crystal < BattleData.Instance.Agent.PlayerRole.MaxEnergyCount &&
                BattleData.Instance.Gem[m.team] + BattleData.Instance.Crystal[m.team] > 0 && !IsStart)
                return true;
            return false;
        }

        public virtual bool CheckSynthetize(uint uiState)
        {
            var m = BattleData.Instance.MainPlayer;
            if (uiState == 10 && m.max_hand - m.hand_count >= 3 &&
                BattleData.Instance.Gem[m.team] + BattleData.Instance.Crystal[m.team] >= 3 && !IsStart)
                return true;
            return false;
        }

        //判断能否选择牌/角色/技能
        public virtual bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                    if (card.Element != Card.CardElement.light && !isCovered)
                        return true;
                    break;
                case 1:
                    if (card.Type == Card.CardType.attack && !isCovered)
                        return true;
                    break;
                case 2:
                    if (card.Type == Card.CardType.magic && card.Element != Card.CardElement.light && !isCovered)
                        return true;
                    break;
                case 3:
                    if ((card.Element == Card.CardElement.light ||
                        (((card.Element == Card.CardElement.darkness || card.Element == Card.GetCard(BattleData.Instance.Agent.Cmd.args[1]).Element) &&
                            BattleData.Instance.Agent.Cmd.args[0] < 1 && card.Type == Card.CardType.attack))) && !isCovered)
                        return true;
                    break;
                case 4:
                    if ((card.Name == Card.CardName.魔弹 || card.Element == Card.CardElement.light) && !isCovered)
                        return true;
                    break;
                case 5:
                    if (BattleData.Instance.Agent.Cmd.args[0] == 801 || BattleData.Instance.Agent.Cmd.args[0] == 805)
                        return !isCovered && card.Type == Card.CardType.magic;
                    return !isCovered;
                case 8:
                    return isCovered;
            }
            return false;
        }

        public virtual bool CanSelect(uint uiState, network.SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1 &&
                        player.team != BattleData.Instance.MainPlayer.team &&
                        !(player.role_id == (uint)RoleID.AnSha && player.is_knelt))
                    {
                        if (BattleData.Instance.MainPlayer.ex_cards.Contains(1001) && player.role_id != (uint)RoleID.YongZhe)
                            return false;
                        return true;
                    }
                    break;
                case 2:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        if (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Name == Card.CardName.魔弹)
                        {
                            foreach (var v in BattleData.Instance.PlayerIdxOrder)
                            {
                                var info = BattleData.Instance.GetPlayerInfo((uint)v);
                                if (info.team != BattleData.Instance.MainPlayer.team)
                                {
                                    if (info.id == player.id)
                                        return true;
                                    break;
                                }
                            }
                            return false;
                        }
                        else if (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Name == Card.CardName.圣盾)
                        {
                            foreach (var v in player.basic_cards)
                            {
                                Card c = Card.GetCard(v);
                                if (c.Name.ToString() == "圣盾" || c.HasSkill(701))
                                    return false;
                            }
                            return true;
                        }
                        else if (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Name == Card.CardName.虚弱)
                        {
                            foreach (var v in player.basic_cards)
                            {
                                if (Card.GetCard(v).Name.ToString() == "虚弱")
                                    return false;
                            }
                            return true;
                        }
                        else
                            return true;
                    }
                    return false;
                case 3:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1 && Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element != Card.CardElement.light)
                        return player.team != BattleData.Instance.MainPlayer.team && player.id != BattleData.Instance.Agent.Cmd.args[3];
                    break;
                case 31052: //激昂狂想曲-效果
                            //if (BattleData.Instance.Agent.SelectArgs [0] == 1)
                            ///	return player.team != BattleData.Instance.MainPlayer.team;
                    //else
                    return player.team != BattleData.Instance.MainPlayer.team;

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
                case 31052: //激昂狂想曲-效果
                    return 2;

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
                case 8:
                    return BattleData.Instance.Agent.Cmd.args[1];
            }
            return 0;
        }

        public System.Action OKAction = null;
        public System.Action CancelAction = null;
        public System.Action ResignAction = null;
        public virtual void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<string> mList;
            List<List<uint>> selectList;
            List<string> explainList;
            uint tGem, tCrystal;
            switch (state)
            {
                case (uint)StateEnum.Idle:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    break;
                case (uint)StateEnum.Attack:
                    //攻击
                    if (!attackable && BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.CanResign))
                    {
                        sendActionMsg(BasicActionType.ACTION_NONE, BattleData.Instance.MainPlayer.id, null, null, null, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        break;
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(StateEnum.Attack));
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        Attack(BattleData.Instance.Agent.SelectCards[0], BattleData.Instance.Agent.SelectPlayers[0]);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    }
                    break;
                case (uint)StateEnum.Magic:
                    //魔法
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                         StateHint.GetHint(StateEnum.Magic));
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        Magic(BattleData.Instance.Agent.SelectCards[0], BattleData.Instance.Agent.SelectPlayers[0]);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    }
                    break;
                case (uint)StateEnum.Attacked:
                    //应战
                    if (BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        if (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element == Card.CardElement.light)
                        {
                            AttackedReply(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]));
                            return;
                        }
                        else
                        {
                            if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                            {
                                AttackedReply(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]),
                                    BattleData.Instance.Agent.SelectPlayers[0]);
                                return;
                            }
                            else
                                MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                    StateHint.GetHint(StateEnum.Attacked, 1));
                        }
                    }
                    else
                    {
                          // MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                          //  StateHint.GetHint(StateEnum.Attacked));
                    }
                      
                    CancelAction = () =>
                    {
                        AttackedReply();
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case (uint)StateEnum.Modaned:
                    //魔弹响应
                    if (BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        MoDaned(Card.GetCard(BattleData.Instance.Agent.SelectCards[0]));
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () =>
                    {
                        MoDaned();
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(StateEnum.Modaned));
                    break;
                case (uint)StateEnum.Drop:
                    //弃牌
                    OKAction = () =>
                    {
                        Drop(BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD, BattleData.Instance.MainPlayer.id, null, null,
                            new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format(StateHint.GetHint(StateEnum.Drop, (int)BattleData.Instance.Agent.Cmd.args[0]),BattleData.Instance.Agent.Cmd.args[1]));
                    break;
                case (uint)StateEnum.Weaken:
                    //虚弱
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        Weaken(BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    selectList = new List<List<uint>>();
                    explainList = new List<string>();
                    selectList.Add(new List<uint>() { 1 }); selectList.Add(new List<uint>() { 0 });
                    explainList.Add("摸牌并正常行动"); explainList.Add("跳过本回合");
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(StateEnum.Weaken));
                    break;
                case (uint)StateEnum.Heal:
                    //治疗
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format(StateHint.GetHint(StateEnum.Heal), BattleData.Instance.Agent.Cmd.args[1]));
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        Heal(BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () =>
                    {
                        Heal(BattleData.Instance.MainPlayer.id, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 8:
                    //弃盖牌
                    OKAction = () =>
                    {
                        sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD_COVER, BattleData.Instance.MainPlayer.id,
                            null, BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format(StateHint.GetHint(StateEnum.DropCover), BattleData.Instance.Agent.Cmd.args[1]));
                    break;
                case 10:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(StateEnum.Any));
                    break;
                case 11:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(StateEnum.AttackAndMagic));
                    break;
                case 12:
                    //购买
                    if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] == 4)
                    {
                        if (msg == UIStateMsg.ClickArgs)
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            BattleData.Instance.Agent.PlayerRole.Buy(BattleData.Instance.Agent.SelectArgs[0], BattleData.Instance.Agent.SelectArgs[1]);
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            return;
                        }
                        CancelAction = () =>
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                        };
                        selectList = new List<List<uint>>() { new List<uint>() { 1, 0 }, new List<uint>() { 0, 1 } };
                        explainList = new List<string>() { "1个宝石", "1个水晶" };
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    }
                    else if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] + BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] == 5)
                    {
                        if (msg == UIStateMsg.ClickArgs)
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            BattleData.Instance.Agent.PlayerRole.Buy(0, 0);
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            return;
                        }
                        CancelAction = () =>
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                        };
                        selectList = new List<List<uint>>() { new List<uint>() { 0, 0 } };
                        explainList = new List<string>() { "不增加星石" };
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    }
                    else
                    {
                        BattleData.Instance.Agent.PlayerRole.Buy(1, 1);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    }
                    break;
                case 13:
                    //提炼
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.PlayerRole.Extract(BattleData.Instance.Agent.SelectArgs[0], BattleData.Instance.Agent.SelectArgs[1]);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    tGem = BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team];
                    tCrystal = BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team];
                    var gem = BattleData.Instance.MainPlayer.gem;
                    var crystal = BattleData.Instance.MainPlayer.crystal;
                    var maxEnergyCnt = BattleData.Instance.Agent.PlayerRole.MaxEnergyCount;
                    selectList = new List<List<uint>>();
                    explainList = new List<string>();
                    if (maxEnergyCnt - gem - crystal >= 2)
                    {
                        if (tGem >= 2)
                        {
                            selectList.Add(new List<uint>() { 2, 0 });
                            explainList.Add("2个宝石");
                        }
                        if (tGem >= 1 && tCrystal >= 1)
                        {
                            selectList.Add(new List<uint>() { 1, 1 });
                            explainList.Add("1个宝石与1个水晶");
                        }
                        if (tCrystal >= 2)
                        {
                            selectList.Add(new List<uint>() { 0, 2 });
                            explainList.Add("2个水晶");
                        }
                    }
                    if (maxEnergyCnt - gem - crystal >= 1)
                    {
                        if (tGem >= 1)
                        {
                            selectList.Add(new List<uint>() { 1, 0 });
                            explainList.Add("1个宝石");
                        }
                        if (tCrystal >= 1)
                        {
                            selectList.Add(new List<uint>() { 0, 1 });
                            explainList.Add("1个水晶");
                        }
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                    };
                    break;
                case 14:
                    //合成
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.PlayerRole.Synthetize(BattleData.Instance.Agent.SelectArgs[0], BattleData.Instance.Agent.SelectArgs[1]);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    tGem = BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team];
                    tCrystal = BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team];
                    selectList = new List<List<uint>>();
                    explainList = new List<string>();
                    if (tCrystal >= 3)
                    {
                        selectList.Add(new List<uint>() { 0, 3 });
                        explainList.Add("3个水晶");
                    }
                    if (tGem >= 1 && tCrystal >= 2)
                    {
                        selectList.Add(new List<uint>() { 1, 2 });
                        explainList.Add("1个宝石2个水晶");
                    }
                    if (tGem >= 2 && tCrystal >= 1)
                    {
                        selectList.Add(new List<uint>() { 2, 1 });
                        explainList.Add("2个宝石1个水晶");
                    }
                    if (tGem >= 3)
                    {
                        selectList.Add(new List<uint>() { 3, 0 });
                        explainList.Add("3个宝石");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.ClickBtn);
                    };
                    break;
                case 15:
                    //额外行动
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        AdditionAction();
                        return;
                    }
                    selectList = new List<List<uint>>();
                    explainList = new List<string>();
                    foreach (var v in BattleData.Instance.Agent.Cmd.args)
                    {
                        selectList.Add(new List<uint>() { v });
                        explainList.Add(Skill.GetSkill(v).SkillName);
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    ResignAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.SelectArgs.Clear();
                        BattleData.Instance.Agent.SelectArgs.Add((uint)BasicActionType.ACTION_NONE);
                        AdditionAction();
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(StateEnum.AdditionalAction));
                    break;
                case 1602:
                    //响应威力赐福
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(state, 1));
                    break;
                case 3105:  //激昂狂想曲-代价
                    if (msg == UIStateMsg.ClickArgs)
                    {

                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, new List<uint>() {
                        BattleData.Instance.Agent.SelectArgs [0] / 4 + 1,
                        BattleData.Instance.Agent.SelectArgs [0] - 1,
                        3 - BattleData.Instance.Agent.SelectArgs [0]
                        });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        //MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.CloseArgsUI);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        return;

                    }
                ;
                    CancelAction = () =>
                    {
                        //MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.CloseArgsUI);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    selectList = new List<List<uint>>();

                    mList = new List<string>();
                    if (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] > 1)
                    {
                        selectList.Add(new List<uint>() { 1 });
                        mList.Add("移除我方战绩区2【水晶】");
                    }
                    if ((BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] > 0) && (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] > 0))
                    {
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add("移除我方战绩区1【水晶】1【宝石】");
                    }
                    if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] > 1)
                    {
                        selectList.Add(new List<uint>() { 3 });
                        mList.Add("移除我方战绩区2【宝石】");
                    }
                    uint watch2 = BattleData.Instance.MainPlayer.role_id;
                    if (BattleData.Instance.MainPlayer.role_id != 31)
                    {
                        selectList.Add(new List<uint>() { 4 });
                        mList.Add("将永恒乐章转移给吟游诗人");
                    }

                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 31052: //激昂狂想曲-效果
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        if (((BattleData.Instance.Agent.SelectArgs[0] == 1) && (BattleData.Instance.Agent.SelectPlayers.Count == 2)) || (BattleData.Instance.Agent.SelectArgs[0] == 2))
                        {
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                                BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectArgs);
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            //MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.CloseArgsUI);
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            return;
                        }


                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);

                    if (BattleData.Instance.Agent.SelectPlayers.Count == 2)
                    {
                        selectList = new List<List<uint>>() { new List<uint>() { 1 }, new List<uint>() { 2 } };
                        mList = new List<string>() { "对这两名对手各造成1点伤害", "弃2张牌" };
                    }
                    else
                    {
                        selectList = new List<List<uint>>() { new List<uint>() { 3 }, new List<uint>() { 2 } };
                        mList = new List<string>() { "若要造成伤害，请先选择两位对手", "弃2张牌" };
                    }

                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(3105, 1));
                    return;
                case 3106:  //胜利交响诗-代价
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        //MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        return;
                    };
                    CancelAction = () =>
                    {
                        //MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    selectList = new List<List<uint>>() { new List<uint>() { 1 } };
                    mList = new List<string>() { "对吟游诗人造成3点法术伤害" };
                    if (BattleData.Instance.MainPlayer.role_id != 31)
                    {
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add("将永恒乐章转移给吟游诗人");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 31061: //胜利交响诗-效果
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { BattleData.Instance.Agent.SelectArgs[0] / 3 + 1, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        //MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.CloseArgsUI);
                        return;
                    }
                ;
                    selectList = new List<List<uint>>();
                    mList = new List<string>();
                    uint Gem = BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team];
                    uint Crystal = BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team];

                    //gem = BattleData.Instance.MainPlayer.gem;
                    //crystal = BattleData.Instance.MainPlayer.crystal;
                    //maxEnergyCnt = BattleData.Instance.Agent.PlayerRole.MaxEnergyCount;
                    if (BattleData.Instance.Agent.PlayerRole.MaxEnergyCount - BattleData.Instance.MainPlayer.gem - BattleData.Instance.MainPlayer.crystal > 0)
                    {
                        if (BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] > 0)
                        {
                            selectList.Add(new List<uint>() { 2 });
                            mList.Add("提炼我方战绩区的1【宝石】");
                        }

                        if (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] > 0)
                        {
                            selectList.Add(new List<uint>() { 1 });
                            mList.Add("提炼我方战绩区的1【水晶】");
                        }
                    }
                    selectList.Add(new List<uint>() { 3 });
                    mList.Add("+1【治疗】,我方战绩区+1【宝石】");

                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(3106, 1));

                    return;
                case 999:
                    return;
            }
            if (BattleData.Instance.Agent.AgentState.Check(PlayerAgentState.CanResign))
                ResignAction = () =>
                {
                    sendActionMsg(BasicActionType.ACTION_NONE, BattleData.Instance.MainPlayer.id, null, null, null, null);
                    BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                };
            else if (CheckunActional(state))
            {
                if (CheckExtract(state))
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint("UnActional", 1));
                else
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint("UnActional"));
                ResignAction = () =>
                    {
                        NoSelection = false;
                        sendActionMsg(BasicActionType.ACTION_UNACTIONAL, BattleData.Instance.MainPlayer.id, null, null, null, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
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
