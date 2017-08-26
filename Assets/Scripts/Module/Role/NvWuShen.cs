using network;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class NvWuShen : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.NvWuShen;
            }
        }

        public override string RoleName
        {
            get
            {
                return "女武神";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }

        public override string HeroName
        {
            get
            {
                return "米涅瓦";
            }
        }

        public override uint Star
        {
            get
            {
                return 35;
            }
        }

        public override string Knelt
        {
            get
            {
                return "YingLingXingTai";
            }
        }

        public NvWuShen()
        {
            for (uint i = 2501; i <= 2505; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.英灵召唤:
                    return card.Type == Card.CardType.magic;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SkillID.秩序之印:
                    if (skill.SkillID == (uint)SkillID.秩序之印)
                        return true;
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.英灵召唤:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.英灵召唤:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.秩序之印:
                case (uint)SkillID.英灵召唤:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case (uint)SkillID.秩序之印:
                    if(msg == UIStateMsg.ClickSkill)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    return;
                case (uint)SkillID.英灵召唤:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                        new List<uint>() { 1, (BattleData.Instance.Agent.SelectCards.Count > 0) ? (uint)1 : 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 1, (BattleData.Instance.Agent.SelectCards.Count > 0) ? (uint)1 : 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0, 0 });
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SkillID.军神威光:
                    //偷个懒，去掉选择能量的步骤
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if(msg == UIStateMsg.ClickArgs)
                    {
                        if (BattleData.Instance.Agent.SelectArgs[0] == 2)
                        {
                            if (BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 2)
                                BattleData.Instance.Agent.SelectArgs.AddRange(new List<uint>() { 0, 2 });
                            else if(BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] >= 1)
                                BattleData.Instance.Agent.SelectArgs.AddRange(new List<uint>() { 1, 1 });
                            else
                                BattleData.Instance.Agent.SelectArgs.AddRange(new List<uint>() { 2, 0 });
                        }
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    var selectList = new List<List<uint>>();
                    var mList = new List<string>();
                    selectList.Add(new List<uint>() { 1 });
                    mList.Add("+1治疗");
                    if(BattleData.Instance.Gem[BattleData.Instance.MainPlayer.team] +
                        BattleData.Instance.Crystal[BattleData.Instance.MainPlayer.team] > 1)
                    {
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add("移除我方战绩区2星石,无视上限+2治疗");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SkillID
        {
            神圣追击 = 2501,
            秩序之印,
            和平行者,
            军神威光,
            英灵召唤
        }
    }
}
