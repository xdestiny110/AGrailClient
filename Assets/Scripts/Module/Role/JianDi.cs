using System.Collections.Generic;
using network;
using Framework.Message;
using System;

namespace AGrail
{
    public class JianDi : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.JianDi;
            }
        }

        public override string RoleName
        {
            get
            {
                return "剑帝";
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

        public override bool HasCoverd
        {
            get
            {
                return true;
            }
        }

        public JianDi()
        {
            for (uint i = 1901; i <= 1907; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 1905:
                case 1906:
                    return isCovered;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1903:
                    return player.id != BattleData.Instance.Agent.Cmd.args[0];
            }
            return base.CanSelect(uiState, player);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1905:
                case 1906:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1903:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1903:
                    return playerIDs.Count == 1;
                case 1905:
                case 1906:
                    return cardIDs.Count == 1;
                case 1907:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1903:
                case 1905:
                case 1906:
                case 1907:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 1903:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers,
                            null, new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    if (msg == UIStateMsg.ClickPlayer)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (BattleData.Instance.Agent.SelectPlayers.Count > 0)
                        {
                            var s = BattleData.Instance.GetPlayerInfo(BattleData.Instance.Agent.SelectPlayers[0]);
                            var selectList = new List<List<uint>>();
                            for (uint i = Math.Min(3, BattleData.Instance.MainPlayer.yellow_token); i > 0 ; i--)
                                selectList.Add(new List<uint>() { i });
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "剑气数量", selectList, "个剑气");
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                string.Format("{0}: 请选择剑气数量", Skills[state].SkillName));
                        }
                    }
                    if (BattleData.Instance.Agent.SelectPlayers.Count <= 0)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 请选择目标玩家", Skills[state].SkillName));
                    return;
                case 1905:
                case 1906:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, 
                            BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择剑魂", Skills[state].SkillName));
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, true);                    
                    return;
                case 1907:
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
