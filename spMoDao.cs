using network;
using System.Collections.Generic;
using Framework.Message;

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

        public spMoDao()
        {
            for (uint i = 802; i <= 806; i++)
                Skills.Add(i, Skill.GetSkill(i));
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
                case 803:
                case 804:
                case 805:
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
                case 803:
                case 804:
                case 805:
                    if (skill.SkillID == 803 || skill.SkillID == 805)
                        return true;
                    if (skill.SkillID == 804 && BattleData.Instance.MainPlayer.gem > 0)
                        return true;
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

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 2:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1 && Card.GetCard(cardIDs[0]).Name == Card.CardName.魔弹)
                    {
                        for (int i = BattleData.Instance.PlayerIdxOrder.Count - 1; i >= 0; i--)
                        {
                            if (BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]].team !=
                                BattleData.Instance.MainPlayer.team)
                            {
                                if (BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]].id == playerIDs[0])
                                    return true;
                                break;
                            }
                        }
                    }
                    break;

                case 803:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                    {
                        for (int i = BattleData.Instance.PlayerIdxOrder.Count - 1; i >= 0; i--)
                        {
                            if (BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]].team !=
                                BattleData.Instance.MainPlayer.team)
                            {
                                if (BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]].id == playerIDs[0])
                                    return true;
                                break;
                            }
                        }
                        for (int i = 0; i < BattleData.Instance.PlayerIdxOrder.Count; i++)
                        {
                            if (BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]].team !=
                                BattleData.Instance.MainPlayer.team)
                            {
                                if (BattleData.Instance.PlayerInfos[BattleData.Instance.PlayerIdxOrder[i]].id == playerIDs[0])
                                    return true;
                                break;
                            }
                        }
                    }
                    return false;
                case 804:
                    if (playerIDs.Count == 2)
                        return true;
                    return false;
                case 805:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                        return true;
                    return false;
                case 806:
                    if (cardIDs.Count > 0)
                        return true;
                    return false;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
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
                case 803:
                case 804:
                case 805:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    break;
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
                    break;
            }
            switch (state)
            {
                case 803:
                case 805:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家以及卡牌", Skills[state].SkillName));
                    break;
                case 804:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家", Skills[state].SkillName));
                    break;
                case 806:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, "法力护盾: 请选择舍弃的法术牌");
                    return;
            }

            base.UIStateChange(state, msg, paras);
        }

    }
}
