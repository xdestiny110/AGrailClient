using UnityEngine;
using System.Collections;
using System;
using network;
using System.Collections.Generic;

namespace AGrail
{
    public class JianSheng : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.JianSheng;
            }
        }

        public override string RoleName
        {
            get
            {
                return "风之剑圣";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.技;
            }
        }

        public JianSheng()
        {
            for (uint i = 101; i <= 105; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card)
        {
            if (additionalState == 103 &&
                (card.Element != Card.CardElement.wind || card.Type != Card.CardType.attack))
                return false;
            return base.CanSelect(uiState, card);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            if (additionalState == 103 &&
                cardIDs.Count == 1 && playerIDs.Count == 1)
                return true;
            switch (uiState)
            {
                case 102:
                    return true;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case 102:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckResign(uint uiState)
        {
            if (additionalState == 103)
                return true;
            return base.CheckResign(uiState);
        }

        public override void AdditionAction()
        {
            if (BattleData.Instance.Agent.SelectArgs.Count == 1)
            {
                switch(BattleData.Instance.Agent.SelectArgs[0])
                {                    
                    case 103:
                        additionalState = 103;
                        break;
                    default:
                        additionalState = 0;
                        break;
                }
            }
            base.AdditionAction();
        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {            
            switch (state)
            {
                case 102:
                    OKAction = () => { sendReponseMsg(102, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 }); };
                    CancelAction = () => { sendReponseMsg(102, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 }); };
                    return;
            }
            base.UIStateChange(state, msg, paras);
            if (additionalState == 103)
            {
                //这代码真傻...应该做成list的
                //但懒得改了
                var t1 = OKAction;
                var t2 = ResignAction;
                OKAction = () => { t1(); additionalState = 0; };
                ResignAction = () => { t2(); additionalState = 0; };
            }
        }
    }
}


