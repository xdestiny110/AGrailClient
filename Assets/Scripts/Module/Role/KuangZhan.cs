using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class KuangZhan : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.KuangZhan;
            }
        }

        public override string RoleName
        {
            get
            {
                return "狂战士";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }

        public override string HeroName
        {
            get
            {
                return "阿基特";
            }
        }

        public override uint Star
        {
            get
            {
                return 30;
            }
        }

        public KuangZhan()
        {
            for (uint i = 201; i <= 203; i++)
                Skills.Add(i, Skill.GetSkill(i));
            Skills.Add(205, Skill.GetSkill(205));
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 201:
                case 202:
                case 205:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 201:
                case 202:
                case 205:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 201:
                case 202:
                case 205:
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


