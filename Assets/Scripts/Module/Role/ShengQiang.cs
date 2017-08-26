using System.Collections.Generic;
using network;
using Framework.Message;
using System;

namespace AGrail
{
    public class ShengQiang : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShengQiang;
            }
        }

        public override string RoleName
        {
            get
            {
                return "圣枪";
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
                return "斯庇尔";
            }
        }

        public override uint Star
        {
            get
            {
                return 35;
            }
        }

        public override uint MaxHealCount
        {
            get
            {
                return 3;
            }
        }

        public ShengQiang()
        {
            for (uint i = 1001; i <= 1007; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 1001:
                    return card.Element == Card.CardElement.water;
                case 1002:
                    return card.Type == Card.CardType.magic;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 1002:
                    return BattleData.Instance.Agent.SelectCards.Count == 1 &&
                        player.id != BattleData.Instance.PlayerID && player.heal_count > 0;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch(uiState)
            {
                case 10:
                case 11:
                case 1001:
                case 1002:
                case 1003:
                    if (skill.SkillID == 1003)
                        return BattleData.Instance.MainPlayer.gem > 0;
                    if (skill.SkillID == 1001)
                        return Util.HasCard(Card.CardElement.water, BattleData.Instance.MainPlayer.hands);
                    if (skill.SkillID == 1002)
                        return Util.HasCard(Card.CardType.magic, BattleData.Instance.MainPlayer.hands);
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 1001:
                case 1002:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 1002:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1002:
                    return cardIDs.Count == 1 && playerIDs.Count == 1;
                case 1003:
                case 1004:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 1001:
                case 1002:
                case 1003:
                case 1004:
                case 1005:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 1001:
                    if(BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1002:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 1003:
                    if (msg == UIStateMsg.ClickSkill)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, null, null, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    return;
                case 1004:
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
                case 1005:
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    var selectList = new List<List<uint>>();
                    var explainList = new List<string>();
                    for (uint i = Math.Min(4, BattleData.Instance.MainPlayer.heal_count); i >= 1; i--)
                    {
                        selectList.Add(new List<uint>() { i });
                        explainList.Add(i + "个治疗");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

    }
}
