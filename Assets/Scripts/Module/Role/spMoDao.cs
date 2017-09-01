using network;
using System.Collections.Generic;
using Framework.Message;
using System;

namespace AGrail
{
    public class spMoDao : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.spMoDao;
            }
        }

        public override string RoleName
        {
            get
            {
                return "sp魔导师";
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
                return "妮亚";
            }
        }

        public override uint Star
        {
            get
            {
                return 30;
            }
        }

        public spMoDao()
        {
            for (uint i = 802; i <= 806; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {   if(uiState == 806)
                    return cardIDs.Count > 0;
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 4:
                    if (card.Name == Card.CardName.魔弹 || card.Element == Card.CardElement.light ||
                        card.Element == Card.CardElement.fire || card.Element == Card.CardElement.earth)
                        return true;
                    return false;

                case 803:
                    return card.Element == Card.CardElement.fire || card.Element == Card.CardElement.earth;
                case 805:
                case 806:
                    return card.Type == Card.CardType.magic;

            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 2:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1 && Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Name == Card.CardName.魔弹)
                    {
                        for (int i = BattleData.Instance.PlayerIdxOrder.Count - 1; i >= 0; i--)
                        {
                            var target = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[i]);
                            if (target.team != BattleData.Instance.MainPlayer.team)
                            {
                                if (target.id == player.id)
                                    return true;
                                break;
                            }
                        }
                        for (int i = 0; i < BattleData.Instance.PlayerIdxOrder.Count; i++)
                        {
                            var target = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[i]);
                            if (target.team != BattleData.Instance.MainPlayer.team)
                            {
                                if (target.id == player.id)
                                    return true;
                                break;
                            }
                        }
                        return false;
                    }
                    else
                        break;

                case 805:
                    return BattleData.Instance.Agent.SelectCards.Count == 1 && player.team != BattleData.Instance.MainPlayer.team;

                case 803:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        for (int i = BattleData.Instance.PlayerIdxOrder.Count - 1; i >= 0; i--)
                        {
                            var target = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[i]);
                            if (target.team != BattleData.Instance.MainPlayer.team)
                            {
                                if (target.id == player.id)
                                    return true;
                                break;
                            }
                        }
                        for (int i = 0; i < BattleData.Instance.PlayerIdxOrder.Count; i++)
                        {
                            var target = BattleData.Instance.GetPlayerInfo((uint)BattleData.Instance.PlayerIdxOrder[i]);
                            if (target.team != BattleData.Instance.MainPlayer.team)
                            {
                                if (target.id == player.id)
                                    return true;
                                break;
                            }
                        }
                    }
                    return false;
                case 804:
                    return player.team != BattleData.Instance.MainPlayer.team;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case 805:
                case 803:
                case 804:
                    if (skill.SkillID == 805)
                        return Util.HasCard(Card.CardType.magic, BattleData.Instance.MainPlayer.hands);

                    if (skill.SkillID == 803)
                        return (Util.HasCard(Card.CardElement.fire, BattleData.Instance.MainPlayer.hands) || Util.HasCard(Card.CardElement.earth, BattleData.Instance.MainPlayer.hands));

                    if (skill.SkillID == 804)
                        return BattleData.Instance.MainPlayer.gem > 0;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 803:
                case 805:
                    return 1;
                case 806:
                    return 7;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 803:
                    return 1;
                case 804:
                    return 2;
                case 805:
                    return 1;

            }
            return base.MaxSelectPlayer(uiState);
        }
        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 803:
                case 804:
                case 805:
                case 806:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 806:
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
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 805:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 804:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 2)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    return;
                case 803:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
