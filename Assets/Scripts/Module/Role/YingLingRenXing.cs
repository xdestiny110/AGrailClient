using System.Collections.Generic;
using network;
using Framework.Message;
using System;

namespace AGrail
{
    public class YingLingRenXing : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.YingLingRenXing;
            }
        }

        public override string RoleName
        {
            get
            {
                return "英灵人形";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.咏;
            }
        }

        public override string HeroName
        {
            get
            {
                return "零";
            }
        }

        public override uint Star
        {
            get
            {
                return 40;
            }
        }

        public override bool HasYellow
        {
            get
            {
                return true;
            }
        }

        public override bool HasBlue
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
                return "XuShiBengFa";
            }
        }
        public YingLingRenXing()
        {
            for (uint i = 2702; i <= 2707; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 2704:
                    return !BattleData.Instance.Agent.SelectCards.Exists(c => { return Card.GetCard(c).Element == card.Element; }) ||
                        BattleData.Instance.Agent.SelectCards.Contains(card.ID);
                case 2703:
                    return BattleData.Instance.Agent.SelectCards.Count == 0 || BattleData.Instance.Agent.SelectCards.Contains(card.ID) ||
                        BattleData.Instance.Agent.SelectCards.Exists(c => { return Card.GetCard(c).Element == card.Element; });
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 2706:
                    return true;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            return false;
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 2703:
                case 2704:
                    return BattleData.Instance.MainPlayer.max_hand;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 2706:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 2703:
                    return cardIDs.Count > 1 && (!(cardIDs.Count > 1 &&
                        BattleData.Instance.MainPlayer.is_knelt && BattleData.Instance.MainPlayer.yellow_token > 1));
                case 2704:
                    return cardIDs.Count > 1 && (!(cardIDs.Count > 1 &&
                        BattleData.Instance.MainPlayer.is_knelt && BattleData.Instance.MainPlayer.blue_token > 1));
                case 2705:
                    return true;
                case 2706:
                    return playerIDs.Count ==1;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 2701:
                case 2703:
                case 2704:
                case 2705:
                case 2706:
                    return true;
                case 27051:
                    return false;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            var selectList = new List<List<uint>>();
            var mList = new List<string>();
            switch (state)
            {
                case 2701:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if(msg == UIStateMsg.ClickArgs)
                    {
                        if (BattleData.Instance.Agent.SelectArgs[0] == 2)
                        {
                            sendReponseMsg(2701, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        }
                        else
                        {
                            BattleData.Instance.Agent.Cmd.respond_id = 2704;
                            BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                        }
                        return;
                    }
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(2071, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    selectList.Clear();
                    mList.Clear();
                    if (BattleData.Instance.MainPlayer.blue_token > 0 )
                    {
                        selectList.Add(new List<uint>() { 1 });
                        mList.Add("魔纹融合");
                    }
                    if (BattleData.Instance.MainPlayer.yellow_token > 0)
                    {
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add("怒火压制");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 2704:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    OKAction = () =>
                    {
                        if (BattleData.Instance.MainPlayer.is_knelt && (BattleData.Instance.MainPlayer.blue_token > 1))
                            sendReponseMsg(2701, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                                new List<uint>() { 2, BattleData.Instance.Agent.SelectArgs[0] });
                        else
                            sendReponseMsg(2701, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                                new List<uint>() { 2, 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.Cmd.respond_id = 2701;
                        BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                    };
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        sendReponseMsg(2701, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 2, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }

                    if (BattleData.Instance.Agent.SelectCards.Count > 1 &&
                        BattleData.Instance.MainPlayer.is_knelt && (BattleData.Instance.MainPlayer.blue_token > 1))
                    {
                        selectList.Clear();
                        mList.Clear();
                        for (uint i = BattleData.Instance.MainPlayer.blue_token; i > 0; i--)
                        {
                            selectList.Add(new List<uint>() { i - 1 });
                            mList.Add( i-1 +"个魔纹");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,1));
                    }
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 2703:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    OKAction = () =>
                    {
                        if (BattleData.Instance.MainPlayer.is_knelt && (BattleData.Instance.MainPlayer.yellow_token > 1))
                        sendReponseMsg(2703, BattleData.Instance.MainPlayer.id,null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] } );
                        else
                            sendReponseMsg(2703, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                                new List<uint>() { 1, 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg(2703, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        sendReponseMsg(2703, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    if (BattleData.Instance.Agent.SelectCards.Count >1 &&
                        BattleData.Instance.MainPlayer.is_knelt && BattleData.Instance.MainPlayer.yellow_token > 1)
                    {
                        selectList.Clear();
                        mList.Clear();
                        for (uint i = BattleData.Instance.MainPlayer.yellow_token; i > 0; i--)
                        {
                            selectList.Add(new List<uint>() { i - 1 });
                            mList.Add( i-1 + "个战纹");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state,1));
                    }
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case 2705:
                    OKAction = () =>
                    {
                        IsStart = true;
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
                case 27051:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        sendReponseMsg(27051, BattleData.Instance.MainPlayer.id,null,null, new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    selectList.Clear();
                    mList.Clear();
                    for (uint i = 0; i < 4; i++)
                    {
                        selectList.Add(new List<uint>() { i });
                        mList.Add(i+"战"+ (3-i) +"魔");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,StateHint.GetHint(state));
                    return;
                case 2706:
                    if(BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;

            }
            base.UIStateChange(state, msg, paras);
        }
        private enum SkillID
        {
            战纹掌控 = 2701,
            怒火压制,
            战纹碎击,
            魔纹融合,
            符文改造,
            双重回响,
            符文调整 = 27051
        }
    }
}
