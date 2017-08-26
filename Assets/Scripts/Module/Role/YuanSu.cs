using Framework.Message;
using network;
using System.Collections.Generic;

namespace AGrail
{
    public class YuanSu : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.YuanSu;
            }
        }

        public override string RoleName
        {
            get
            {
                return "元素师";
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
                return "索尔斯";
            }
        }

        public override uint Star
        {
            get
            {
                return 35;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public YuanSu()
        {
            for (uint i = 1101; i <= 1108; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            if (additionalState != 0)
                return false;
            bool check = false;
            if (uiState>=1101 && uiState<=1105)
            {

                switch (BattleData.Instance.Agent.SelectCards.Count)
                {
                    case 0:
                        if (card.HasSkill(uiState)) check = true;
                        break;
                    case 1:
                        check = true;
                        break;
                    case 2:
                        if (BattleData.Instance.Agent.SelectCards.Contains(card.ID))
                        {
                        if (!card.HasSkill(uiState)) check = true;
                        else if (Util.HasCard(uiState, BattleData.Instance.Agent.SelectCards, 2))check = true;
                        }
                        break;
                    default:break;
                }
            }
            if (check) {

                switch (uiState)
                {
                    case 1101:
                        return card.Element == Card.CardElement.wind;
                    case 1102:
                        return card.Element == Card.CardElement.water;
                    case 1103:
                        return card.Element == Card.CardElement.fire;
                    case 1104:
                        return card.Element == Card.CardElement.earth;
                    case 1105:
                        return card.Element == Card.CardElement.thunder;
                }
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                    return BattleData.Instance.Agent.SelectCards.Count >= 1;
                case 1106:
                case 1107:
                    return true;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case 2:
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                case 1106:
                case 1107:
                    if (skill.SkillID == 1106 && BattleData.Instance.MainPlayer.yellow_token == 3)
                        return true;
                    if (skill.SkillID == 1107 && BattleData.Instance.MainPlayer.gem > 0)
                        return true;
                    if (skill.SkillID >= 1101 && skill.SkillID <= 1105)
                        return Util.HasCard(skill.SkillID, BattleData.Instance.MainPlayer.hands);
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                case 1106:
                case 1107:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch(uiState)
            {
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                    return cardIDs.Count >= 1 && cardIDs.Count <= 2 && playerIDs.Count == 1;
                case 1106:
                case 1107:
                    return playerIDs.Count == 1;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1101:
                case 1102:
                case 1103:
                case 1104:
                case 1105:
                case 1106:
                case 1107:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        private List<uint> selectPlayers = new List<uint>();
        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            if (state != 1102)
                additionalState = 0;
            switch (state)
            {
                case 1101:
                case 1103:
                case 1104:
                case 1105:
                    if(BattleData.Instance.Agent.SelectCards.Count >= 1 && BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1106:
                case 1107:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1108:
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
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("是否发动{0}", Skills[state].SkillName));
                    return;
                case 1102:
                    OKAction = () =>
                    {
                        if (additionalState == 11021)
                        {
                            selectPlayers.Add(BattleData.Instance.Agent.SelectPlayers[0]);
                            sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                                selectPlayers, BattleData.Instance.Agent.SelectCards, state);
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                        else
                        {
                            additionalState = 11021;
                            selectPlayers.Clear();
                            selectPlayers.Add(BattleData.Instance.Agent.SelectPlayers[0]);
                            BattleData.Instance.Agent.RemoveSelectPlayer(BattleData.Instance.Agent.SelectPlayers[0]);
                        }
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    if (additionalState == 11021)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,1));
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
