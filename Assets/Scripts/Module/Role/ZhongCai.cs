using System.Collections.Generic;
using network;
using Framework.Message;

namespace AGrail
{
    public class ZhongCai : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ZhongCai;
            }
        }

        public override string RoleName
        {
            get
            {
                return "仲裁者";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }

        public override string HeroName
        {
            get
            {
                return "赛尔娜";
            }
        }

        public override uint Star
        {
            get
            {
                return 35;
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
                return "ShenPan";
            }
        }

        public ZhongCai()
        {
            for (uint i = 1401; i <= 1406; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 10:
                    if (BattleData.Instance.MainPlayer.yellow_token == 4)
                        return false;
                    break;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1403:
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
                case 1403:
                case 1405:
                    return (skill.SkillID == 1403 && BattleData.Instance.MainPlayer.yellow_token > 0) ||
                        (skill.SkillID == 1405 && BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem > 0 && BattleData.Instance.MainPlayer.yellow_token < 4);
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1403:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1401:
                case 1402:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1401:
                case 1402:
                case 1403:
                case 1405:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckBuy(uint uiState)
        {
            switch (uiState)
            {
                case 10:
                    if (BattleData.Instance.MainPlayer.yellow_token == 4)
                        return false;
                    break;
            }
            return base.CheckBuy(uiState);
        }

        public override bool CheckExtract(uint uiState)
        {
            switch (uiState)
            {
                case 10:
                    if (BattleData.Instance.MainPlayer.yellow_token == 4)
                        return false;
                    break;
            }
            return base.CheckExtract(uiState);
        }

        public override bool CheckSynthetize(uint uiState)
        {
            switch (uiState)
            {
                case 10:
                    if (BattleData.Instance.MainPlayer.yellow_token == 4)
                        return false;
                    break;
            }
            return base.CheckSynthetize(uiState);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 1401:
                case 1402:
                    OKAction = () =>
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1403:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 )
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1405:
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, null, state, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    var selectList = new List<List<uint>>() { new List<uint>() { 0 }, new List<uint>() { 1 } };
                    var mList = new List<string>() { "弃掉你的所有手牌", "将手牌补到上限，【战绩区】+1【宝石】" };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}
