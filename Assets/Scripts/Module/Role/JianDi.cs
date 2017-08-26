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

        public override string HeroName
        {
            get
            {
                return "卡特琳娜";
            }
        }

        public override uint Star
        {
            get
            {
                return 45;
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
                    return BattleData.Instance.Agent.SelectArgs.Count == 1 &&
                        player.id != BattleData.Instance.Agent.Cmd.args[0];
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
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    if(BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectArgs.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers,
                            null, new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    else if(BattleData.Instance.Agent.SelectArgs.Count == 0)
                    {
                        var selectList = new List<List<uint>>();
                        var mList = new List<string>();
                        for (uint i = Math.Min(3, BattleData.Instance.MainPlayer.yellow_token); i > 0; i--)
                        {
                            selectList.Add(new List<uint>() { i });
                            mList.Add(i + "个剑气");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    }
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,1));
                    return;
                case 1905:
                case 1906:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null,
                            BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.AgentHandChange, false);
                        return;
                    }
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
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
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
