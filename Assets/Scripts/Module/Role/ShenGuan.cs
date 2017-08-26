using System.Collections.Generic;
using network;
using System;
using Framework.Message;

namespace AGrail
{
    public class ShenGuan : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShenGuan;
            }
        }

        public override string RoleName
        {
            get
            {
                return "神官";
            }
        }

        public override string HeroName
        {
            get
            {
                return "莉格蕾朵";
            }
        }

        public override uint Star
        {
            get
            {
                return 40;
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }

        public override uint MaxHealCount
        {
            get
            {
                return 6;
            }
        }

        public ShenGuan()
        {
            for (uint i = 1501; i <= 1506; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣祈福:
                    return card.Type == Card.CardType.magic;
                case (uint)SKILLID.水之神力:
                    return card.Element == Card.CardElement.water;
                case (uint)SKILLID.水之神力给牌:
                    return true;
                case (uint)SKILLID.神圣领域:
                    return additionalState == 15061 || additionalState == 15062;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SKILLID.水之神力:
                    return BattleData.Instance.Agent.SelectCards.Count == 1 &&
                        player.team == BattleData.Instance.MainPlayer.team &&
                        BattleData.Instance.PlayerID != player.id;
                case (uint)SKILLID.神圣契约:
                    return BattleData.Instance.Agent.SelectArgs.Count == 1 &&
                        player.team == BattleData.Instance.MainPlayer.team &&
                        BattleData.Instance.PlayerID != player.id;
                case (uint)SKILLID.神圣领域:
                    if(additionalState == 15061)
                        return BattleData.Instance.Agent.SelectCards.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 2);
                    if(additionalState == 15062)
                        return player.team == BattleData.Instance.MainPlayer.team && player.id != BattleData.Instance.PlayerID &&
                            BattleData.Instance.Agent.SelectCards.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 2);
                    return false;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SKILLID.神圣祈福:
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.神圣领域:
                    if (skill.SkillID == 1506 && BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem > 0)
                        return true;
                    if (skill.SkillID == 1502)
                        return Util.HasCard(Card.CardType.magic, BattleData.Instance.MainPlayer.hands, 2);
                    if (skill.SkillID == 1503)
                        return Util.HasCard(Card.CardElement.water, BattleData.Instance.MainPlayer.hands);
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣祈福:
                    return 2;
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.水之神力给牌:
                    return 1;
                case (uint)SKILLID.神圣领域:
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SKILLID.水之神力:
                    return 1;
                case (uint)SKILLID.神圣契约:
                    return 1;
                case (uint)SKILLID.神圣领域:
                    if (additionalState == 15061 || additionalState == 15062)
                        return 1;
                    return 0;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣启示:
                    return true;
                case (uint)SKILLID.神圣祈福:
                    return cardIDs.Count == 2;
                case (uint)SKILLID.水之神力:
                    return cardIDs.Count == 1 && playerIDs.Count == 1 && BattleData.Instance.MainPlayer.hand_count > 1;
                case (uint)SKILLID.水之神力给牌:
                    return cardIDs.Count == 1;
                case (uint)SKILLID.神圣契约:
                    return playerIDs.Count == 1;
                case (uint)SKILLID.神圣领域:
                    if (additionalState == 15061 || additionalState == 15062)
                        return playerIDs.Count == 1;
                    return cardIDs.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 2);
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SKILLID.神圣启示:
                case (uint)SKILLID.神圣祈福:
                case (uint)SKILLID.水之神力:
                case (uint)SKILLID.神圣契约:
                case (uint)SKILLID.神圣领域:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<List<uint>> selectList = new List<List<uint>>();
            List<string> explainList = new List<string>();
            if (state != (uint)SKILLID.水之神力 && state != (uint)SKILLID.神圣领域)
                additionalState = 0;
            switch (state)
            {
                case (uint)SKILLID.神圣启示:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SKILLID.神圣祈福:
                    if (BattleData.Instance.Agent.SelectCards.Count == 2)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null,
                            BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SKILLID.水之神力:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SKILLID.水之神力给牌:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择要给予的牌", Skills[state].SkillName));
                    return;
                case (uint)SKILLID.神圣契约:
                    if(BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        IsStart = true;
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers,
                            null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if(BattleData.Instance.Agent.SelectArgs.Count == 0)
                    {
                        selectList.Clear();
                        for (uint i = Math.Min(4, BattleData.Instance.MainPlayer.heal_count); i >= 1; i--)
                        {
                            selectList.Add(new List<uint>() { i });
                            explainList.Add(i + "个治疗");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    }
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,1));
                    return;
                case (uint)SKILLID.神圣领域:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if (BattleData.Instance.Agent.SelectCards.Count == Math.Min(BattleData.Instance.MainPlayer.hand_count, 2) &&
                        BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        if(additionalState == 15061)
                        {
                            sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state, new List<uint>() { 1 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                        else if(additionalState == 15062)
                        {
                            sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state, new List<uint>() { 2 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                        return;
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    if (BattleData.Instance.Agent.SelectArgs.Count == 1)
                    {
                        if (BattleData.Instance.Agent.SelectArgs[0] == 1)
                        {
                            additionalState = 15061;
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,1));
                        }

                        else
                        {
                            additionalState = 15062;
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,2));
                        }
                    }
                    else
                    {
                        selectList.Clear();
                        var mList = new List<string>();
                        if (BattleData.Instance.MainPlayer.heal_count > 0)
                        {
                            selectList.Add(new List<uint>() { 1 });
                            mList.Add("(移除你的1[治疗])对目标角色造成2点法术伤害");
                        }
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add("你+2[治疗],目标队友+1[治疗]");
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    }
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SKILLID
        {
            神圣启示 = 1501,
            神圣祈福,
            水之神力,
            圣使守护,
            神圣契约,
            神圣领域,
            水之神力给牌 = 1531
        }

    }
}
