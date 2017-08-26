using network;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class XianZhe : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.XianZhe;
            }
        }

        public override string RoleName
        {
            get
            {
                return "贤者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.咏;
            }
        }

        public override string HeroName
        {
            get
            {
                return "诺雷杰";
            }
        }

        public override uint Star
        {
            get
            {
                return 40;
            }
        }

        public override uint MaxEnergyCount
        {
            get
            {
                return 4;
            }
        }

        public XianZhe()
        {
            for (uint i = 1701; i <= 1704; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 1702:
                case 1703:
                    return !BattleData.Instance.Agent.SelectCards.Exists(c => { return Card.GetCard(c).Element == card.Element; }) ||
                        BattleData.Instance.Agent.SelectCards.Contains(card.ID);
                case 1704:
                    return BattleData.Instance.Agent.SelectCards.Count == 0 || BattleData.Instance.Agent.SelectCards.Contains(card.ID) ||
                        BattleData.Instance.Agent.SelectCards.Exists(c => { return Card.GetCard(c).Element == card.Element; });
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1702:
                case 1704:
                    return BattleData.Instance.Agent.SelectCards.Count > 1;
                case 1703:
                    return BattleData.Instance.Agent.SelectCards.Count > 2;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 1702:
                case 1703:
                case 10:
                case 11:
                    if (skill.SkillID == 1702 && BattleData.Instance.MainPlayer.gem > 0)
                        return Util.HasCard("differ", BattleData.Instance.MainPlayer.hands,2);
                    if (skill.SkillID == 1703 && BattleData.Instance.MainPlayer.gem > 0)
                        return Util.HasCard("differ", BattleData.Instance.MainPlayer.hands,3);
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1702:
                case 1703:
                case 1704:
                    return BattleData.Instance.MainPlayer.max_hand;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1702:
                case 1704:
                    return 1;
                case 1703:
                    return 4;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1702:
                case 1704:
                    return cardIDs.Count > 1 && playerIDs.Count == 1;
                case 1703:
                    return cardIDs.Count > 2 && playerIDs.Count <= cardIDs.Count - 2;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1702:
                case 1703:
                case 1704:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 1702:
                    if(BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1703:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1704:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

    }
}
