using Framework.Message;
using network;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class GeDouJia : RoleBase
    {
        private const uint BAISHIDOUSHEN = 2046;
        private const uint XULICANGYAN = 2025;

        public override RoleID RoleID
        {
            get
            {
                return RoleID.GeDouJia;
            }
        }

        public override string RoleName
        {
            get
            {
                return "格斗家";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override string Knelt
        {
            get
            {
                return "BaiShi";
            }
        }

        public GeDouJia()
        {
            for (uint i = 2001; i <= 2006; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 2006:
                    return true;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 2003:
                    return player.team != BattleData.Instance.MainPlayer.team;
            }
            return base.CanSelect(uiState, player);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 2006:
                    return Math.Max(0, BattleData.Instance.MainPlayer.hand_count - 3);
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 2003:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 2002:
                case 2004:
                case 2005:                
                case XULICANGYAN:
                case BAISHIDOUSHEN:
                    return true;
                case 2003:
                    return playerIDs.Count == 1;
                case 2006:
                    return cardIDs.Count == Math.Max(0, BattleData.Instance.MainPlayer.hand_count - 3);
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 2002:
                case 2003:
                case 2004:
                case 2005:
                case 2006:
                case BAISHIDOUSHEN:
                case XULICANGYAN:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            var selectList = new List<List<uint>>();
            var mList = new List<string>();
            switch (state)
            {
                case 2003:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家", Skills[state].SkillName));
                    return;
                case 2002:
                case 2005:
                case XULICANGYAN:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(XULICANGYAN, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(XULICANGYAN, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    if(state == 2002 || state == XULICANGYAN)
                    {
                        selectList.Add(new List<uint>() { 1 });
                        mList.Add(" 蓄力一击");
                    }                        
                    if (state == 2005 || state == XULICANGYAN)
                    {
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add(" 苍炎之魂");
                    }                        
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "选择技能", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, string.Format("请选择发动技能"));
                    return;
                case 2004:
                case 2006:                
                    OKAction = () =>
                    {
                        if (BattleData.Instance.Agent.SelectCards.Count == 0)
                            sendReponseMsg(BAISHIDOUSHEN, BattleData.Instance.MainPlayer.id, null, null,
                                new List<uint>() { 1 });
                        else
                            sendReponseMsg(BAISHIDOUSHEN, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                                new List<uint>() { 2 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {                        
                        sendReponseMsg(BAISHIDOUSHEN, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    if(state == 2004)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, 
                            string.Format("是否发动百式幻龙拳"));
                    else if (state == 2006)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("斗神天驱: 弃到三张牌并点击确定"));
                    return;
                case BAISHIDOUSHEN:
                    OKAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (BattleData.Instance.Agent.SelectArgs[0] == 1)
                            BattleData.Instance.Agent.Cmd.respond_id = 2004;
                        else
                            BattleData.Instance.Agent.Cmd.respond_id = 2006;
                        BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(BAISHIDOUSHEN, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    selectList.Add(new List<uint>() { 1 });
                    mList.Add(" 百式幻龙拳");
                    selectList.Add(new List<uint>() { 2 });
                    mList.Add(" 斗神天驱");
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "选择技能", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, string.Format("请选择发动技能"));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
