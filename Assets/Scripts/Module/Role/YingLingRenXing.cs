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
                return Card.CardProperty.幻;
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
                case 2701:
                    return true;
                case 2703:
                case 2704:
                    return cardIDs.Count > 1 ;
                case 2705:
                case 27051:
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
                    OKAction = () =>
                    {
                        if (BattleData.Instance.Agent.SelectArgs[0] == 1)
                        {
                            sendReponseMsg(2701, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                            BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                        }
                        else
                        {
                            BattleData.Instance.Agent.Cmd.respond_id = 2704;
                            BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(2071, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    if (BattleData.Instance.MainPlayer.yellow_token > 0 )
                    {
                        selectList.Add(new List<uint>() { 1 });
                        mList.Add(" 怒火压制");
                    }
                    if (BattleData.Instance.MainPlayer.blue_token > 0 )
                    { 
                        selectList.Add(new List<uint>() { 2 });
                        mList.Add(" 魔纹融合");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "选择技能", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, string.Format("请选择发动技能"));
                    return;

                case 2704:
                    OKAction = () =>
                    {
                        if (BattleData.Instance.MainPlayer.is_knelt)
                        sendReponseMsg(2701, BattleData.Instance.MainPlayer.id,null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 2, BattleData.Instance.Agent.SelectArgs[0] });
                        else
                            sendReponseMsg(2701, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 2, 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.Cmd.respond_id = 2701;
                        BattleData.Instance.Agent.FSM.ChangeState<StateSkill>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    if (BattleData.Instance.MainPlayer.is_knelt && (BattleData.Instance.MainPlayer.blue_token > 1))
                    {
                        
                        for (uint i = 0; i<BattleData.Instance.MainPlayer.blue_token; i++)
                        {
                            selectList.Add(new List<uint>() { i });
                            mList.Add("个魔纹");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "额外魔纹", selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择额外翻转的魔纹数量以及异系卡牌", Skills[state].SkillName));
                    }
                    else { 
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择异系卡牌", Skills[state].SkillName));
                    }
                    return;

                case 2703:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (BattleData.Instance.MainPlayer.is_knelt)
                        sendReponseMsg(2703, BattleData.Instance.MainPlayer.id,null, BattleData.Instance.Agent.SelectCards,
                            new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] } );
                        else
                            sendReponseMsg(2703, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards,
                                new List<uint>() { 1, 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(2703, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    if (BattleData.Instance.MainPlayer.is_knelt && (BattleData.Instance.MainPlayer.yellow_token>1))
                    {
                        for (uint i = 0; i < BattleData.Instance.MainPlayer.yellow_token; i++)
                        {
                            selectList.Add(new List<uint>() { i });
                            mList.Add("个战纹");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "额外战纹", selectList, mList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择额外翻转的战纹数量以及同系卡牌", Skills[state].SkillName));
                    }
                    else { 
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择同系卡牌", Skills[state].SkillName));
                    }
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
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, "是否发动符文改造");
                    return;
                case 27051:
                    OKAction = () =>
                    {
                        sendReponseMsg(27051, BattleData.Instance.MainPlayer.id,null,null, new List<uint>() { 1, BattleData.Instance.Agent.SelectArgs[0] });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                    selectList.Clear();
                    mList.Clear();
                    for (uint i = 0; i < 4; i++)
                    {
                        selectList.Add(new List<uint>() { i });
                        mList.Add("个战纹");
                    }
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "战纹数量", selectList, mList);
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("请选择调整后的战纹数量"));
                    return;
                case 2706:
                    OKAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家或点取消", Skills[state].SkillName));
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
