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

        public override bool CheckSkill(int agentState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            if (agentState.Check(PlayerAgentState.CanMagic) && skillID.HasValue && playerIDs.Count == 1)
            {
                if (skillID == 303 && playerIDs[0] != BattleData.Instance.MainPlayer.id &&
                    BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem > 1)
                    return true;
                if(cardIDs.Count == 1 && skillID == 302 && playerIDs[0] != BattleData.Instance.MainPlayer.id)
                {
                    var c = Card.GetCard(cardIDs[0]);
                    if (c.HasSkill(Skills[302].SkillName))
                        return true;
                }
            }
            if(agentState.Check(PlayerAgentState.SkillResponse))
            {
                if (BattleData.Instance.Agent.Cmd.respond_id == 301 && cardIDs.Count == 0)
                    return true;
                if(BattleData.Instance.Agent.Cmd.respond_id == 305 && cardIDs.Count == 1)
                {
                    var c = Card.GetCard(cardIDs[0]);
                    if (c.Type == Card.CardType.magic)
                        return true;
                }
                //if(BattleData.Instance.Agent.Cmd.respond_id == 42)
            }            
            return false;
        }

        protected override void useSkill(uint skillID, uint srcID, List<uint> dstID = null, List<uint> cardIds = null, List<uint> args = null)
        {
            switch (skillID)
            {
                case 301:
                    sendReponseMsg(301, srcID, null, null, args);
                    break;
                case 302:
                    sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, srcID, dstID, cardIds, 302);
                    break;
                case 303:
                    sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, srcID, dstID, null, 303);
                    break;
                case 305:
                    sendReponseMsg(305, srcID, null, cardIds, args);
                    break;
                default:
                    break;
            }
        }
    }
}


