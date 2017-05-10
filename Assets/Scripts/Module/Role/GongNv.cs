using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using network;
using Framework.Message;

namespace AGrail
{
    public class GongNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.GongNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "弓之女神";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }

        public GongNv()
        {
            for (uint i = 301; i <= 305; i++)
                Skills.Add(i, Skill.GetSkill(i));            
        }

        public override bool CanSelect(uint uiState, Card card)
        {
            switch (uiState)
            {
                case 302:
                    if (card.HasSkill(302))
                        return true;
                    break;
                case 305:
                    if (card.Type == Card.CardType.magic)
                        return true;
                    break;
            }
            return base.CanSelect(uiState, card);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case 302:
                case 303:
                    if (player.id != BattleData.Instance.MainPlayer.id)
                        return true;
                    break;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            if (uiState == 10 || uiState == 11 || uiState == 2 || uiState == 302 || uiState == 303)
            {
                if (skill.SkillID == 302)
                    return true;
                if (skill.SkillID == 303 && BattleData.Instance.MainPlayer.gem + BattleData.Instance.MainPlayer.crystal > 0)
                    return true;
            }                
            return base.CanSelect(uiState, skill);
        }

        public override void AdditionAction()
        {            
            if(BattleData.Instance.Agent.SelectArgs.Count == 1 && BattleData.Instance.Agent.SelectArgs[0] == 303)
            {
                //狙击的返回不会写....
                //sendReponseMsg(304, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                return;
            }
            base.AdditionAction();
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 301:
                case 305:
                    return true;                    
                case 302:
                    if (cardIDs.Count == 1 && playerIDs.Count == 1)
                        return true;
                    break;
                case 303:
                    if (playerIDs.Count == 1)
                        return true;
                    break;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 301:
                case 302:
                case 303:
                case 305:
                    return true;                    
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case 302:
                case 305:
                    return 1;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case 302:
                case 303:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            switch (state)
            {
                case 301:
                    OKAction = () =>
                    {
                        sendReponseMsg(301, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(301, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
                case 302:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, 302);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    break;
                case 303:
                    OKAction = () =>
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, 303);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    break;
                case 305:
                    OKAction = () =>
                    {
                        sendReponseMsg(305, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(305, BattleData.Instance.MainPlayer.id, null, BattleData.Instance.Agent.SelectCards, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    break;
            }
            base.UIStateChange(state, msg, paras);
        }
    }
}


