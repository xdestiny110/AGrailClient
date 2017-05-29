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
            switch (uiState)
            {
                case 1101:
                    return card.Element == Card.CardElement.wind && (card.HasSkill(uiState) || BattleData.Instance.Agent.SelectCards.Count > 0);
                case 1102:
                    return card.Element == Card.CardElement.water && (card.HasSkill(uiState) || BattleData.Instance.Agent.SelectCards.Count > 0);
                case 1103:
                    return card.Element == Card.CardElement.fire && (card.HasSkill(uiState) || BattleData.Instance.Agent.SelectCards.Count > 0);
                case 1104:
                    return card.Element == Card.CardElement.earth && (card.HasSkill(uiState) || BattleData.Instance.Agent.SelectCards.Count > 0);
                case 1105:
                    return card.Element == Card.CardElement.thunder && (card.HasSkill(uiState) || BattleData.Instance.Agent.SelectCards.Count > 0);                    
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
                        return true;
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
                    return BattleData.Instance.MainPlayer.max_hand;
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
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家以及独有技卡牌", Skills[state].SkillName));
                    return;
                case 1106:
                case 1107:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                       string.Format("{0}: 请选择目标玩家", Skills[state].SkillName));
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
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择给予治疗的对象", Skills[state].SkillName));                    
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 选择伤害的对象", Skills[state].SkillName));                    
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
