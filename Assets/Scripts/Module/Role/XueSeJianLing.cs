using System.Collections.Generic;
using Framework.Message;
using network;
using UnityEngine;

namespace AGrail
{
    public class XueSeJianLing : RoleBase
    {
        public override RoleID RoleID
        {
            get { return RoleID.XueSeJianLing; }
        }

        public override string RoleName
        {
            get { return "血色剑灵"; }
        }

        public override string ShortName
        {
            get { return "血剑"; }
        }

        public override Card.CardProperty RoleProperty
        {
            get { return Card.CardProperty.血; }
        }

        public override string HeroName
        {
            get { return "罗丝菲莉"; }
        }

        public override uint Star
        {
            get { return 40; }
        }

        public override bool HasYellow
        {
            get { return true; }
        }

        public XueSeJianLing()
        {
            for (uint i = 3401; i <= 3405; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        private List<uint> selectPlayers = new List<uint>();

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint) SkillID.散华轮舞:
                    if (additionalState == (uint) AdditionalState.散华轮舞选项2)
                        return true;
                    return false;
            }

            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint) SkillID.血染蔷薇:
                    if (additionalState == (uint) AdditionalState.血染蔷薇2阶段)
                        return player.team == BattleData.Instance.MainPlayer.team;
                    return true;
                case (uint) SkillID.血气屏障:
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
                case (uint) SkillID.血染蔷薇:
                    if (skill.SkillID == (uint) SkillID.血染蔷薇)
                        return BattleData.Instance.MainPlayer.yellow_token >= 2;
                    return false;
            }

            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint) SkillID.散华轮舞:
                    if (additionalState == (uint) AdditionalState.散华轮舞选项2)
                        return (uint) Mathf.Max(0, (int) BattleData.Instance.MainPlayer.hand_count - 4);
                    return 0;
            }

            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint) SkillID.血染蔷薇:
                    return 1;
                case (uint) SkillID.血气屏障:
                    return 1;
            }

            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint) SkillID.赤色一闪:
                    return BattleData.Instance.MainPlayer.yellow_token >= 1;
                case (uint) SkillID.血染蔷薇:
                    return false;
                case (uint) SkillID.血气屏障:
                    return BattleData.Instance.Agent.SelectPlayers.Count == 1;
                case (uint) SkillID.散华轮舞:
                    if (additionalState == (uint) AdditionalState.散华轮舞选项2)
                        return cardIDs.Count == Mathf.Max(0, (int) BattleData.Instance.MainPlayer.hand_count - 4);
                    return false;
            }

            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint) SkillID.赤色一闪:
                case (uint) SkillID.血染蔷薇:
                case (uint) SkillID.血气屏障:
                case (uint) SkillID.散华轮舞:
                    return true;
            }

            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            if (state != (uint) SkillID.血染蔷薇 && state != (uint) SkillID.散华轮舞)
                additionalState = (uint) AdditionalState.无;
            Debug.LogWarningFormat("TGe: state = {0};additionalState = {1}", state.ToString(),
                additionalState.ToString());
            switch (state)
            {
                case (uint) SkillID.血染蔷薇:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        if (additionalState == (uint) AdditionalState.无)
                        {
                            additionalState = (uint) AdditionalState.血染蔷薇2阶段;
                            selectPlayers.Clear();
                            selectPlayers.Add(BattleData.Instance.Agent.SelectPlayers[0]);
                            BattleData.Instance.Agent.RemoveSelectPlayer(BattleData.Instance.Agent.SelectPlayers[0]);
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                StateHint.GetHint(state, 1));
                        }
                        else if (additionalState == (uint) AdditionalState.血染蔷薇2阶段)
                        {
                            additionalState = (uint) AdditionalState.无;
                            selectPlayers.Add(BattleData.Instance.Agent.SelectPlayers[0]);
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id, selectPlayers);
                            selectPlayers.Clear();
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                    }
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            StateHint.GetHint(state, 0));

                    CancelAction = () =>
                    {
                        additionalState = (uint) AdditionalState.无;
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };

                    return;
                case (uint) SkillID.散华轮舞:
                    if (additionalState == (uint) AdditionalState.无)
                    {
                        var selectList = new List<List<uint>>();
                        var mList = new List<string>();
                        MessageSystem<Framework.Message.MessageType>.Notify(
                            Framework.Message.MessageType.CloseNewArgsUI);
                        if (msg == UIStateMsg.ClickArgs)
                        {
                            if (BattleData.Instance.Agent.SelectArgs[0] == 1)
                                additionalState = (uint) AdditionalState.散华轮舞选项1;
                            else
                                additionalState = (uint) AdditionalState.散华轮舞选项2;
                            BattleData.Instance.Agent.Cmd.respond_id = (uint) SkillID.散华轮舞;
                            BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                            return;
                        }

                        CancelAction = () =>
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType
                                .CloseNewArgsUI);
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() {0});
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        };
                        selectList.Clear();
                        mList.Clear();
                        selectList.Add(new List<uint>() {1});
                        mList.Add("花费水晶+2鲜血");
                        if (BattleData.Instance.MainPlayer.gem > 0)
                        {
                            selectList.Add(new List<uint>() {2});
                            mList.Add("花费宝石+2鲜血 弃到4");
                        }

                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI,
                            selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            StateHint.GetHint(state));
                        return;
                    }
                    else if (additionalState == (uint) AdditionalState.散华轮舞选项1)
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() {2});
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    }
                    else if (additionalState == (uint) AdditionalState.散华轮舞选项2)
                    {
                        if (MaxSelectCard(state) > 0)
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                string.Format(StateHint.GetHint(state, 1), MaxSelectCard(state)));
                            OKAction = () =>
                            {
                                IsStart = true;
                                sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null,
                                    BattleData.Instance.Agent.SelectCards,
                                    new List<uint>() {1});
                                BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            };
                            CancelAction = () =>
                            {
                                sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null,
                                    new List<uint>() {0});
                                BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                            };
                        }
                        else
                        {
                            IsStart = true;
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() {1});
                            BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        }
                    }

                    return;
                case (uint) SkillID.血气屏障:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        StateHint.GetHint(state));
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, selectPlayers,
                            null, new List<uint>() {1});
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() {0});
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    return;
            }

            base.UIStateChange(state, msg, paras);
        }

        private enum SkillID
        {
            血色荆棘 = 3401,
            赤色一闪 = 3402,
            血染蔷薇 = 3403,
            血气屏障 = 3404,
            散华轮舞 = 3405,
            血蔷薇庭院,
        }

        private enum AdditionalState
        {
            无 = 0,
            血染蔷薇2阶段 = 34031,
            散华轮舞选项1 = 34051,
            散华轮舞选项2 = 34052,
        }
    }
}