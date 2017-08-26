using System.Collections.Generic;
using network;
using Framework.Message;

namespace AGrail
{
    public class GongNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.GongNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "弓之女神";
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
                return "安娜";
            }
        }

        public override uint Star
        {
            get
            {
                return 30;
            }
        }

        public GongNv()
        {
            for (uint i = 301; i <= 305; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 302:
                    return card.HasSkill(302);
                case 305:
                    if (card.Type == Card.CardType.magic)
                        return true;
                    break;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 302:
                    return BattleData.Instance.Agent.SelectCards.Count == 1;
                case 303:
                    return true;

            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 302:
                case 303:
                case 11:
                case 10:
                    if (skill.SkillID == 302)
                        return Util.HasCard(302, BattleData.Instance.MainPlayer.hands);
                    if (skill.SkillID == 303)
                        return BattleData.Instance.MainPlayer.gem + BattleData.Instance.MainPlayer.crystal > 0;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override void AdditionAction()
        {
            if(BattleData.Instance.Agent.SelectArgs.Count == 1 && BattleData.Instance.Agent.SelectArgs[0] == 303)
            {
                sendReponseMsg((uint)BasicRespondType.RESPOND_ADDITIONAL_ACTION, BattleData.Instance.MainPlayer.id,
                    null, null, new List<uint>() { 303 });
                return;
            }
            base.AdditionAction();
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 301:
                    return true;
                case 305:
                    return cardIDs.Count == 1;
                case 302:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                        return true;
                    break;
                case 303:
                    if (playerIDs.Count == 1)
                        return true;
                    break;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 301:
                case 302:
                case 303:
                case 305:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 302:
                case 305:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 302:
                case 303:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 301:
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
                case 305:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 302:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
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
                case 303:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
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


