using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class JianSheng : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.JianSheng;
            }
        }

        public override string RoleName
        {
            get
            {
                return "风之剑圣";
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
                return "维斯特姆";
            }
        }

        public override uint Star
        {
            get
            {
                return 30;
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
                if(!value)
                    additionalState = 0;
                base.IsStart = value;
            }
        }

        public JianSheng()
        {
            for (uint i = 101; i <= 105; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            if (additionalState == 103 && uiState==1)
                return card.Element == Card.CardElement.wind;
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 102:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 102:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void AdditionAction()
        {
            if (BattleData.Instance.Agent.SelectArgs.Count == 1)
            {
                switch(BattleData.Instance.Agent.SelectArgs[0])
                {
                    case 103:
                        additionalState = 103;
                        break;
                    default:
                        additionalState = 0;
                        break;
                }
            }
            base.AdditionAction();
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 102:
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
                    MessageSystem<MessageType>.Notify(MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}


