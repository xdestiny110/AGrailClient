using System.Collections.Generic;
using network;
using Framework.Message;

namespace AGrail
{
    public class SiLing : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.SiLing;
            }
        }

        public override string RoleName
        {
            get
            {
                return "死灵法师";
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
                return "塔格奥";
            }
        }

        public override uint Star
        {
            get
            {
                return 35;
            }
        }

        public override uint MaxHealCount
        {
            get
            {
                return 5;
            }
        }

        public SiLing()
        {
            for (uint i = 1301; i <= 1305; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 1303:
                    return card.Element == Card.CardElement.earth;
                case 1304:
                    return BattleData.Instance.Agent.SelectArgs.Count == 1 &&
                        (BattleData.Instance.Agent.SelectCards.Count == 0 ||
                        card.Element == Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element);
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1304:
                    return BattleData.Instance.Agent.SelectCards.Count >= 2;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case 1303:
                case 1304:
                case 1305:
                    if (skill.SkillID == 1303)
                        return Util.HasCard(Card.CardElement.earth, BattleData.Instance.MainPlayer.hands);
                    if (skill.SkillID == 1305)
                        return BattleData.Instance.MainPlayer.gem > 0;
                    if (skill.SkillID == 1304 && BattleData.Instance.MainPlayer.heal_count >= 2)
                        return Util.HasCard("same", BattleData.Instance.MainPlayer.hands,2);
                    return skill.SkillID == 1303;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1303:
                    return 1;
                case 1304:
                    return BattleData.Instance.MainPlayer.max_hand;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1304:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1301:
                case 1305:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1301:
                case 1303:
                case 1304:
                case 1305:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 1301:
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
                case 1303:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1304:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    if(BattleData.Instance.Agent.SelectArgs.Count == 0)
                    {
                        var selectList = new List<List<uint>>();
                        var explainList = new List<string>();
                        for (uint i = BattleData.Instance.MainPlayer.heal_count; i >= 2; i--)
                        {
                            selectList.Add(new List<uint>() { i });
                            explainList.Add(i + "个治疗");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    }
                    else
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, string.Format(StateHint.GetHint(state,1), BattleData.Instance.Agent.SelectArgs[0]));
                        if (BattleData.Instance.Agent.SelectCards.Count >= 2 && BattleData.Instance.Agent.SelectPlayers.Count == 1)
                        {
                            sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                                BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards,
                                state, BattleData.Instance.Agent.SelectArgs);
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            return;
                        }
                    }
                    return;
                case 1305:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
