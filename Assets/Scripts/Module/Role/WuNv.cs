using Framework.Message;
using network;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class WuNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.WuNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "巫女";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }

        public override string Knelt
        {
            get
            {
                return "LiuXue";
            }
        }

        public WuNv()
        {
            for (uint i = 2301; i <= 2304; i++)
                Skills.Add(i, Skill.GetSkill(i));
            Skills.Add(2351, Skill.GetSkill(2351));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.血之悲鸣:
                    return card.HasSkill(uiState);
                case (uint)SkillID.逆流:
                case (uint)SkillID.血之诅咒弃牌:
                    return true;                    
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SkillID.血之悲鸣:
                case (uint)SkillID.同生共死:
                case (uint)SkillID.血之哀伤:
                case (uint)SkillID.血之诅咒:
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
                case (uint)SkillID.血之悲鸣:
                case (uint)SkillID.同生共死:
                case (uint)SkillID.逆流:
                case (uint)SkillID.血之诅咒:
                    if (skill.SkillID == (uint)SkillID.血之诅咒 && BattleData.Instance.MainPlayer.gem > 0)
                        return true;
                    if ((skill.SkillID == (uint)SkillID.逆流 || skill.SkillID == (uint)SkillID.血之悲鸣) && BattleData.Instance.MainPlayer.is_knelt)
                        return true;
                    if (skill.SkillID == (uint)SkillID.同生共死 && additionalState == 0)
                        return true;                    
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.血之悲鸣:
                    return 1;
                case (uint)SkillID.逆流:
                    return Math.Min(2, BattleData.Instance.MainPlayer.hand_count);
                case (uint)SkillID.血之诅咒弃牌:
                    return Math.Min(3, BattleData.Instance.MainPlayer.hand_count);
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.血之悲鸣:
                case (uint)SkillID.同生共死:
                case (uint)SkillID.血之哀伤:
                case (uint)SkillID.血之诅咒:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.血之悲鸣:
                    return playerIDs.Count == 1 && cardIDs.Count == 1;
                case (uint)SkillID.同生共死:
                    return playerIDs.Count == 1;
                case (uint)SkillID.血之哀伤:
                    return (BattleData.Instance.Agent.SelectArgs[0] == 1 && playerIDs.Count == 1) || BattleData.Instance.Agent.SelectArgs[0] == 0;
                case (uint)SkillID.逆流:
                    return Math.Min(2, BattleData.Instance.MainPlayer.hand_count) == cardIDs.Count;
                case (uint)SkillID.血之诅咒:
                    return playerIDs.Count == 1;
                case (uint)SkillID.血之诅咒弃牌:
                    return Math.Min(3, BattleData.Instance.MainPlayer.hand_count) == cardIDs.Count;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.血之悲鸣:                    
                case (uint)SkillID.同生共死:                    
                case (uint)SkillID.血之哀伤:                    
                case (uint)SkillID.逆流:                    
                case (uint)SkillID.血之诅咒:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case (uint)SkillID.血之悲鸣:
                    OKAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    var selectList = new List<List<uint>>();
                    var mList = new List<string>();
                    for(uint i = 1; i<= 3; i++)
                    {
                        selectList.Add(new List<uint>() { i });
                        mList.Add("点伤害");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "血之悲鸣伤害", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家以及伤害", Skills[state].SkillName));
                    return;
                case (uint)SkillID.同生共死:
                    OKAction = () =>
                    {
                        additionalState = 1;
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, state, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 选择目标", Skills[state].SkillName));
                    return;
                case (uint)SkillID.血之哀伤:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (BattleData.Instance.Agent.SelectArgs[0] == 2)
                        {
                            additionalState = 0;
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        }                            
                        else
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                                BattleData.Instance.Agent.SelectPlayers, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择技能选项以及目标玩家", Skills[state].SkillName));
                    return;
                case (uint)SkillID.逆流:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, BattleData.Instance.Agent.SelectCards, state, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 弃2张牌", Skills[state].SkillName));
                    return;
                case (uint)SkillID.血之诅咒:
                    OKAction = () =>
                    {                        
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 弃3张牌并选择目标玩家", Skills[state].SkillName));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SkillID
        {
            血之悲鸣 = 2301,
            同生共死,
            血之哀伤,
            逆流,
            血之诅咒 = 2351,
            血之诅咒弃牌,
        }

    }
}
