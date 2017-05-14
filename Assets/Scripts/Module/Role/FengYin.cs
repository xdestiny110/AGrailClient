using UnityEngine;
using System.Collections;
using System;
using network;
using System.Collections.Generic;
using Framework.Message;

namespace AGrail
{
    public class FengYin : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.FengYin;
            }
        }

        public override string RoleName
        {
            get
            {
                return "封印师";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.幻;
            }
        }

        public FengYin()
        {
            for (uint i = 401; i <= 408; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                    return card.HasSkill(uiState);                
            }
            return base.CanSelect(uiState, card);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                case 407:
                    return (player.team != BattleData.Instance.MainPlayer.team);
                case 408:
                    return player.basic_cards.Count > 0;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                case 407:
                case 408:
                case 10:
                case 11:
                    if (skill.SkillID >= 401 && skill.SkillID <= 405)
                        return true;
                    else if ((skill.SkillID == 407 || skill.SkillID == 408) &&
                        BattleData.Instance.MainPlayer.gem + BattleData.Instance.MainPlayer.crystal >= 1)
                        return true;
                    break;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                case 407:
                case 408:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                        return true;
                    return false;
                case 407:
                case 408:
                    if (playerIDs.Count == 1)
                        return true;
                    return false;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                case 407:
                case 408:
                    return true;
            }            
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 401:
                case 402:
                case 403:
                case 404:
                case 405:
                case 407:                
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state,
                            BattleData.Instance.Agent.SelectArgs);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    return;
                case 408:
                    OKAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectArgs, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => 
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);                        
                    };
                    if(msg == UIStateMsg.ClickPlayer)
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        if(BattleData.Instance.Agent.SelectPlayers.Count > 0)
                        {                            
                            var s = BattleData.Instance.GetPlayerInfo(BattleData.Instance.Agent.SelectPlayers[0]);
                            var selectList = new List<List<uint>>();
                            foreach (var v in s.basic_cards)
                                selectList.Add(new List<uint>() { v });
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowArgsUI, "Card", selectList);
                        }
                    }
                return;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}


