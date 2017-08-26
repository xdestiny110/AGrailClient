using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class AnSha : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.AnSha;
            }
        }

        public override string RoleName
        {
            get
            {
                return "暗杀者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }

        public override string HeroName
        {
            get
            {
                return "无念";
            }
        }

        public override uint Star
        {
            get
            {
                return 30;
            }
        }

        public override string Knelt
        {
            get
            {
                return "QianXing";
            }
        }

        public AnSha()
        {
            for (uint i = 501; i <= 503; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            if (uiState == 502 && card.Element == Card.CardElement.water)
                return true;
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 502:
                    if (cardIDs.Count > 0)
                        return true;
                    return false;
                case 503:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 502:
                case 503:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            if (uiState == 502)
                return 7;
            return base.MaxSelectCard(uiState);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 502:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 503:
                    OKAction = () =>
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

    }
}
