using System.Collections.Generic;
using network;
using Framework.Message;

namespace AGrail
{
    public class MoQiang : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoQiang;
            }
        }

        public override string RoleName
        {
            get
            {
                return "魔枪";
            }
        }

        public override string HeroName
        {
            get
            {
                return "菲欧娜";
            }
        }

        public override uint Star
        {
            get
            {
                return 40;
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public override string Knelt
        {
            get
            {
                return "HuanYing";
            }
        }

        public MoQiang()
        {
            for (uint i = 2901; i <= 2906; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 10:
                case 11:
                    if (card.Type == Card.CardType.magic)
                        return false;
                    break;
                case (uint)SkillID.暗之障壁:
                    if (BattleData.Instance.Agent.SelectCards.Count == 0)
                        return card.Element == Card.CardElement.thunder || card.Type == Card.CardType.magic;
                    else
                        return (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element == Card.CardElement.thunder && card.Element == Card.CardElement.thunder) ||
                        (Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Type == Card.CardType.magic && card.Type == Card.CardType.magic);
                case (uint)SkillID.充盈:
                    return card.Type == Card.CardType.magic || card.Element == Card.CardElement.thunder;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SkillID.幻影星辰:
                    return true;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case (uint)SkillID.充盈:
                case 10:
                case 11:
                    if (skill.SkillID == (uint)SkillID.充盈 && additionalState != 29011)
                        return ( Util.HasCard(Card.CardType.magic, BattleData.Instance.MainPlayer.hands) || Util.HasCard(Card.CardElement.thunder, BattleData.Instance.MainPlayer.hands) );
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.充盈:
                    return 1;
                case (uint)SkillID.暗之障壁:
                    return BattleData.Instance.MainPlayer.max_hand;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.幻影星辰:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.充盈:
                    return cardIDs.Count == 1;
                case (uint)SkillID.暗之障壁:
                    return cardIDs.Count > 0;
                case (uint)SkillID.幻影星辰:
                    return playerIDs.Count == 1;
                case (uint)SkillID.暗之解放:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.充盈:
                case (uint)SkillID.暗之障壁:
                case (uint)SkillID.幻影星辰:
                case (uint)SkillID.暗之解放:
                case (uint)SkillID.漆黑之枪:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case (uint)SkillID.充盈:
                    if (BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            null, BattleData.Instance.Agent.SelectCards, state, null);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SkillID.暗之障壁:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null,
                            BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SkillID.幻影星辰:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        IsStart = true;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers,
                            null, new List<uint>() { 2 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    additionalState = 0;
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SkillID.暗之解放:
                    OKAction = () =>
                    {
                        IsStart = true;
                        additionalState = 29011;
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
                case (uint)SkillID.漆黑之枪:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        var args = new List<uint>() { 1 };
                        args.Add(BattleData.Instance.Agent.SelectArgs[0]);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, args);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    List<List<uint>> selectList = new List<List<uint>>();
                    var mList = new List<string>();
                    for(uint i = BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem; i > 0; i--)
                    {
                        selectList.Add(new List<uint>() { i });
                        mList.Add(i.ToString() + "个能量");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private enum SkillID
        {
            暗之解放 = 2901,
            幻影星辰,
            黑暗束缚,
            暗之障壁,
            充盈,
            漆黑之枪,
        }
    }
}


