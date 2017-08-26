using System.Collections.Generic;
using Framework.Message;
using network;

namespace AGrail
{
    public class MoJian : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoJian;
            }
        }

        public override string RoleName
        {
            get
            {
                return "魔剑";
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
                return "美狄亚";
            }
        }

        public override uint Star
        {
            get
            {
                return 35;
            }
        }

        public override string Knelt
        {
            get
            {
                return "AnYing";
            }
        }

        public override bool IsStart
        {
            get
            {
                return base.IsStart;
            }

            set
            {
                if (!value)
                    additionalState = 0;
                base.IsStart = value;
            }
        }

        public MoJian()
        {
            for (uint i = 901; i <= 906; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 3:
                    if (BattleData.Instance.NowPlayerID == BattleData.Instance.MainPlayer.id && card.Element == Card.CardElement.light)
                        return false;
                    break;
                case 905:
                    return card.Type == Card.CardType.magic;
                case 10:
                case 11:
                case 1:
                    if (additionalState == 901 &&
                        (card.Element != Card.CardElement.fire || card.Type != Card.CardType.attack))
                        return false;
                    return card.Type != Card.CardType.magic;      
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 905:
                    return BattleData.Instance.Agent.SelectCards.Count == 2;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 905:
                case 10:
                case 11:
                    if (skill.SkillID == 905 && BattleData.Instance.MainPlayer.is_knelt && Util.HasCard(Card.CardType.magic, BattleData.Instance.MainPlayer.hands, 2))
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 905:
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 905:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override void AdditionAction()
        {
            if (BattleData.Instance.Agent.SelectArgs.Count == 1)
            {
                switch (BattleData.Instance.Agent.SelectArgs[0])
                {
                    case 901:
                        additionalState = 901;
                        break;
                    default:
                        additionalState = 0;
                        break;
                }
            }
            base.AdditionAction();
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 905:
                    return cardIDs.Count == 2 && playerIDs.Count == 1;
                case 901:
                case 902:
                case 906:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 901:
                case 902:
                case 905:
                case 906:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 902:
                case 906:
                    OKAction = () =>
                    {
                        if (state == 901) additionalState = 901;
                        if (state == 902) IsStart = true;
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
                case 905:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 2)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
