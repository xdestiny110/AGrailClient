using System.Collections.Generic;
using network;
using Framework.Message;
using System;

namespace AGrail
{
    public class DieWu : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.DieWu;
            }
        }

        public override string RoleName
        {
            get
            {
                return "蝶舞者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public override string HeroName
        {
            get
            {
                return "华胥";
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override bool HasCoverd
        {
            get
            {
                return true;
            }
        }

        public override string Knelt
        {
            get
            {
                return "DiaoLing";
            }
        }

        public DieWu()
        {
            for(uint i = 2401; i <= 2408; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.舞动:
                    return !isCovered;
                case (uint)SkillID.毒粉:
                case (uint)SkillID.朝圣:
                    return isCovered;
                case (uint)SkillID.镜花水月:
                    return isCovered &&
                        (BattleData.Instance.Agent.SelectCards.Count == 0 || Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element == card.Element);
                case (uint)SkillID.倒逆之蝶:                    
                    return (isCovered && additionalState == 24082) || 
                        (additionalState == 0 && !isCovered && BattleData.Instance.Agent.SelectCards.Count != MaxSelectCard(uiState));
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SkillID.凋零:
                    return true;
                case (uint)SkillID.倒逆之蝶:
                    return additionalState == 24081;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SkillID.舞动:
                case (uint)SkillID.蛹化:
                case (uint)SkillID.倒逆之蝶:
                    if (skill.SkillID == (uint)SkillID.舞动)
                        return true;
                    if (skill.SkillID == (uint)SkillID.蛹化)
                        return BattleData.Instance.MainPlayer.gem > 0;
                    if (skill.SkillID == (uint)SkillID.倒逆之蝶)
                        return BattleData.Instance.MainPlayer.gem + BattleData.Instance.MainPlayer.crystal > 0;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.舞动:
                case (uint)SkillID.毒粉:
                case (uint)SkillID.朝圣:
                    return 1;
                case (uint)SkillID.镜花水月:
                    return 2;
                case (uint)SkillID.倒逆之蝶:
                    return Math.Min(BattleData.Instance.MainPlayer.hand_count, 2);                    
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.凋零:
                    return 1;
                case (uint)SkillID.倒逆之蝶:
                    if (additionalState == 24081)
                        return 1;
                    return 0;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.毒粉:
                case (uint)SkillID.朝圣:
                    return cardIDs.Count == 1;
                case (uint)SkillID.镜花水月:
                    return cardIDs.Count == 2;
                case (uint)SkillID.舞动:
                case (uint)SkillID.蛹化:
                    return true;
                case (uint)SkillID.凋零:
                    return BattleData.Instance.Agent.Cmd.args[0] == 2 && playerIDs.Count == 1;
                case (uint)SkillID.倒逆之蝶:
                    return (additionalState == 0 && cardIDs.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 2)) || 
                        (additionalState == 24081 && playerIDs.Count == 1) ||
                        (additionalState == 24082 && cardIDs.Count == 2) || additionalState == 24083;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.舞动:
                case (uint)SkillID.毒粉:
                case (uint)SkillID.朝圣:
                case (uint)SkillID.镜花水月:
                case (uint)SkillID.蛹化:
                case (uint)SkillID.凋零:
                case (uint)SkillID.倒逆之蝶:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        private List<uint> selectCards = new List<uint>();
        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            if (state != (uint)SkillID.倒逆之蝶)
                additionalState = 0;
            var selectList = new List<List<uint>>();
            var mList = new List<string>();
            switch (state)
            {
                case (uint)SkillID.舞动:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                                null, BattleData.Instance.Agent.SelectCards, state, new List<uint>() { 2 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, null, state, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };                    
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择一张牌弃置，不选视为摸牌", Skills[state].SkillName));
                    return;
                case (uint)SkillID.朝圣:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
                        var args = new List<uint>() { 1 };
                        if (BattleData.Instance.Agent.SelectCards.Count > 0)
                            args.Add(BattleData.Instance.Agent.SelectCards[0]);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, args);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, true);
                    return;
                case (uint)SkillID.毒粉:
                case (uint)SkillID.镜花水月:
                    if(BattleData.Instance.Agent.SelectCards.Count == MaxSelectCard(state))
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, 
                            BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, true);
                    return;
                case (uint)SkillID.蛹化:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    return;
                case (uint)SkillID.凋零:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 )
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    if(BattleData.Instance.Agent.Cmd.args[0] == 2)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择一个目标", Skills[state].SkillName));
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 非法术牌请直接取消", Skills[state].SkillName));
                    return;
                case (uint)SkillID.倒逆之蝶:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if (additionalState == 24081 && BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers,
                            selectCards, state, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    else if (additionalState == 24082 && BattleData.Instance.Agent.SelectCards.Count == 2)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, true);
                        foreach (var v in BattleData.Instance.Agent.SelectCards)
                            BattleData.Instance.Agent.SelectArgs.Add(v);
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null,
                            selectCards, state, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    else if (additionalState == 24083)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null,
                            selectCards, state, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    if (additionalState == 0 && msg == UIStateMsg.ClickSkill)                    
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 弃两张牌", Skills[state].SkillName));                    
                    else if (additionalState == 0 && BattleData.Instance.Agent.SelectCards.Count == MaxSelectCard(state))
                    {
                        selectList.Add(new List<uint>() { 1 });
                        mList.Add("对目标角色造成1点法术伤害");
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add("移除2个【茧】,移除1个【蛹】");
                        selectList.Add(new List<uint>() { 3 });
                        mList.Add("对自己造成4点法术伤害,移除1个【蛹】");
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择要发动的技能", Skills[state].SkillName));
                    }
                    else if (additionalState == 0 && msg == UIStateMsg.ClickArgs)
                    {
                        additionalState = BattleData.Instance.Agent.SelectArgs[0] + (uint)SkillID.倒逆之蝶 * 10;
                        selectCards.Clear();
                        selectCards.AddRange(BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.RemoveAllSelectCard();
                    }                        

                    if(additionalState == 24081)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择一个目标角色", Skills[state].SkillName));
                    }
                    else if(additionalState == 24082)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择两张茧", Skills[state].SkillName));
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, true);
                    }
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SkillID
        {
            生命之火 = 2401,
            舞动,
            毒粉,
            朝圣,
            镜花水月,
            凋零,
            蛹化,
            倒逆之蝶
        }
    }
}
