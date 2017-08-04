using UnityEngine;
using System.Collections;
using System;
using network;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class TianShi : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.TianShi;
            }
        }

        public override string RoleName
        {
            get
            {
                return "天使";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.圣;
            }
        }

        public TianShi()
        {
            for (uint i = 701; i <= 706; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case 701:
                    return card.HasSkill(uiState);
                case 702:
                    return card.Element == Card.CardElement.water;
                case 703:
                    return card.Element == Card.CardElement.wind;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 701:
                    foreach(var v in player.basic_cards)
                    {
                        var c = Card.GetCard(v);
                        if (c.Name == Card.CardName.圣盾 || c.HasSkill(701))
                            return false;
                    }
                    return true;
                case 703:
                case 705:
                    return player.basic_cards.Count > 0;
                case 702:
                case 704:
                    return true;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 701:
                case 702:
                case 703:
                case 10:
                case 11:
                    if (skill.SkillID == 701)
                        return Util.HasCard(601, BattleData.Instance.MainPlayer.hands);
                    if (skill.SkillID == 702)
                        return Util.HasCard(Card.CardElement.water, BattleData.Instance.MainPlayer.hands); 
                    if (skill.SkillID == 703)
                        return Util.HasCard(Card.CardElement.wind, BattleData.Instance.MainPlayer.hands);
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 701:
                case 702:
                case 703:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 701:
                case 703:
                case 704:
                case 705:
                    return 1;
                case 702:
                    return 2;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 701:
                case 703:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                        return true;
                    return false;
                case 702:
                    if (cardIDs.Count == 1 && playerIDs.Count >= 1)
                        return true;
                    return false;
                case 704:
                case 705:
                    if (playerIDs.Count == 1)
                        return true;
                    return false;
                case 706:
                    return true;                    
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 701:
                case 702:
                case 703:                
                case 705:
                case 706:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 701:
                case 702:                                        
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };                    
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家以及卡牌", Skills[state].SkillName));
                    return;
                case 703:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    if (msg == UIStateMsg.ClickPlayer)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (BattleData.Instance.Agent.SelectPlayers.Count > 0)
                        {
                            var s = BattleData.Instance.GetPlayerInfo(BattleData.Instance.Agent.SelectPlayers[0]);
                            var selectList = new List<List<uint>>();
                            foreach (var v in s.basic_cards)
                                selectList.Add(new List<uint>() { v });
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Card", selectList);
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                string.Format("{0}: 请选择要移除的基础效果", Skills[state].SkillName));
                        }
                    }
                    if (BattleData.Instance.Agent.SelectPlayers.Count <= 0)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 请选择目标玩家以及卡牌", Skills[state].SkillName));
                    return;
                case 704:
                    OKAction = () => 
                    {                        
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                        string.Format("{0}: 请选择目标玩家为其增加一点治疗", Skills[state].SkillName));
                    return;
                case 705:
                    OKAction = () =>
                    {
                        //默认有能量才会触发该技能
                        //优先使用水晶
                        uint useGem = 1;
                        if (BattleData.Instance.MainPlayer.crystal > 0)
                            useGem = 0;
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, null,
                            new List<uint>() { useGem, BattleData.Instance.Agent.SelectArgs[0] });
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    if (msg == UIStateMsg.ClickPlayer)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if (BattleData.Instance.Agent.SelectPlayers.Count > 0)
                        {
                            var s = BattleData.Instance.GetPlayerInfo(BattleData.Instance.Agent.SelectPlayers[0]);
                            var selectList = new List<List<uint>>();
                            foreach (var v in s.basic_cards)
                                selectList.Add(new List<uint>() { v });
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Card", selectList);
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                string.Format("{0}: 请选择要移除的基础效果", Skills[state].SkillName));
                        }
                    }
                    if (BattleData.Instance.Agent.SelectPlayers.Count <= 0)
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 请选择目标玩家", Skills[state].SkillName));
                    return;
                case 706:
                    OKAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0, 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };                   
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        var selectList = new List<List<uint>>();
                        for (uint i = 0; i <= Math.Min(BattleData.Instance.MainPlayer.gem, BattleData.Instance.Agent.Cmd.args[0]); i++)
                        {
                            for (uint j = 0; j <= Math.Min(BattleData.Instance.MainPlayer.crystal, BattleData.Instance.Agent.Cmd.args[0] - i); j++)
                                selectList.Add(new List<uint>() { i, j });
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Energy", selectList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format("{0}: 请选择要消耗的能量", Skills[state].SkillName));
                    }
                   
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

    }
}
