using UnityEngine;
using System.Collections;
using System;
using network;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class QiDao : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.QiDao;
            }
        }

        public override string RoleName
        {
            get
            {
                return "祈祷师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.咏;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override string HeroName
        {
            get
            {
                return "珐珞";
            }
        }

        public override uint Star
        {
            get
            {
                return 40;
            }
        }

        public override string Knelt
        {
            get
            {
                return "QiDao";
            }
        }

        public QiDao()
        {
            for (uint i = 1601; i <= 1606; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 1601:
                case 1602:
                    return card.HasSkill(uiState);
                case 1604:
                    return true;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1601:
                case 1602:
                case 1604:
                    if (uiState == 1601 || uiState == 1602)
                    foreach (var v in player.basic_cards)
                    {
                        var c = Card.GetCard(v);
                        if (c.HasSkill(uiState))
                            return false;
                    }
                    return BattleData.Instance.Agent.SelectCards.Count == MaxSelectCard(uiState) &&
                        player.team == BattleData.Instance.MainPlayer.team &&
                        player.id != BattleData.Instance.PlayerID;
                case 1605:
                    return true;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 2:
                case 1601:
                case 1602:
                case 1604:
                case 1605:
                    if (skill.SkillID >= 1601 && skill.SkillID <= 1602)
                        return Util.HasCard(skill.SkillID, BattleData.Instance.MainPlayer.hands);
                    if (BattleData.Instance.MainPlayer.yellow_token > 0 && skill.SkillID >= 1604 && skill.SkillID <= 1605)
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1601:
                case 1602:
                    return 1;
                case 1604:
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1601:
                case 1602:
                case 1604:
                case 1605:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1601:
                case 1602:
                    return cardIDs.Count == 1 && playerIDs.Count == 1;
                case 1603:
                    return true;
                case 1604:
                case 1605:
                    return playerIDs.Count == 1;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1601:
                case 1602:
                case 1603:
                case 1604:
                case 1605:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 1601:
                case 1602:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1603:
                    OKAction = () =>
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1604:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 2)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1605:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 )
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

    }
}
