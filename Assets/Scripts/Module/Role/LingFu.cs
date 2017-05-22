using UnityEngine;
using System.Collections;
using System;
using network;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class LingFu : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.LingFu;
            }
        }

        public override string RoleName
        {
            get
            {
                return "灵符师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public override bool HasCoverd
        {
            get
            {
                return true;
            }
        }

        public LingFu()
        {
            for (uint i = 1801; i <= 1805; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 8:
                    return isCovered;
                case 1801:
                    return card.Element == Card.CardElement.thunder && !isCovered;
                case 1802:
                    return card.Element == Card.CardElement.wind && !isCovered;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1801:                    
                case 1802:
                case 1804:
                    return true;                    
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 1801:
                case 1802:
                case 10:
                case 11:
                    return skill.SkillID >= 1801 && skill.SkillID <= 1802;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 8:
                case 1801:
                case 1802:
                case 1803:
                case 1804:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1801:
                case 1802:
                case 1804:
                    return 2;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 8:
                    return cardIDs.Count == 1;
                case 1801:
                case 1802:
                    return cardIDs.Count == 1 && playerIDs.Count == 2;
                case 1804:
                    return (playerIDs.Count == 1 && BattleData.Instance.Agent.Cmd.args[0] == 0) ||
                        (playerIDs.Count <= 2 && playerIDs.Count >= 1 && BattleData.Instance.Agent.Cmd.args[0] == 1);
                case 1805:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 8:
                case 1801:
                case 1802:
                case 1805:
                    return true;
                case 5:
                    if (BattleData.Instance.Agent.Cmd.args[0] == 1803)
                        return true;
                    break;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }       
                
        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 5:
                    if(BattleData.Instance.Agent.Cmd.args[0] == 1803)
                    {
                        OKAction = () =>
                        {
                            Drop(BattleData.Instance.Agent.SelectCards);
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        };
                        CancelAction = () =>
                        {
                            sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD, BattleData.Instance.MainPlayer.id,
                                null, null, new List<uint>() { 0 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        };
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 请选择卡牌", Skills[1803].SkillName));
                        return;
                    }
                    break;
                case 8:
                    if (BattleData.Instance.Agent.Cmd.args[0] == 1804)
                    {
                        OKAction = () =>
                        {
                            sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD_COVER, BattleData.Instance.MainPlayer.id,
                                null, BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        };
                        CancelAction = () =>
                        {
                            sendReponseMsg((uint)BasicRespondType.RESPOND_DISCARD_COVER, BattleData.Instance.MainPlayer.id,
                                null, null, new List<uint>() { 0 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        };
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 请选择妖力", Skills[1804].SkillName));
                        return;
                    }
                    break;
                case 1801:
                case 1802:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家以及卡牌", Skills[state].SkillName));
                    return;
                case 1804:
                    OKAction = () =>
                    {
                        sendReponseMsg(1804, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers,
                            null, new List<uint>() { (BattleData.Instance.Agent.SelectPlayers.Count == 1) ? (uint)0 : 1 });
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择一个目标。若想展示火系妖力，则选择两个目标。", Skills[state].SkillName));
                    return;
                case 1805:
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
            }
            base.UIStateChange(state, msg, paras);
        }

    }
}
