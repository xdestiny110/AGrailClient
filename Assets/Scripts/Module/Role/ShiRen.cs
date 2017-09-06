using System.Collections.Generic;
using network;
using Framework.Message;

namespace AGrail
{
    public class ShiRen : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.ShiRen;
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

        public ShiRen()
        {
            for (uint i = 3101; i <= 3107; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override string RoleName
        {
            get
            {
                return "吟游诗人";
            }
        }

        public override string HeroName
        {
            get
            {
                return "詹姆";
            }
        }

        public override uint Star
        {
            get
            {
                return 45;
            }
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.CHEN_LUN_XIE_ZOU_QU:
                    return BattleData.Instance.Agent.SelectCards.Count == 0 ||
                            card.Element == Card.GetCard(BattleData.Instance.Agent.SelectCards[0]).Element;
                    //return true;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            //bool bool1, bool2;
            //bool1 = (player.team != BattleData.Instance.MainPlayer.team);
            //bool2 = (BattleData.Instance.PlayerID != player.id);
            switch (uiState)
            {
                case (uint)SkillID.CHEN_LUN_XIE_ZOU_QU:
                    return player.team != BattleData.Instance.MainPlayer.team && BattleData.Instance.PlayerID != player.id && BattleData.Instance.Agent.SelectCards.Count == 2;
                //return true;
                case (uint)SkillID.BU_XIE_HE_XIAN:
                    return true;
                case (uint)SkillID.XI_WANG_FU_GE_QU:
                    return player.team == BattleData.Instance.MainPlayer.team && BattleData.Instance.PlayerID != player.id;


            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SkillID.BU_XIE_HE_XIAN:
                    if (skill.SkillID == (uint)SkillID.BU_XIE_HE_XIAN)
                        return (BattleData.Instance.MainPlayer.yellow_token > 1);
                    break;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.BU_XIE_HE_XIAN:
                case (uint)SkillID.XI_WANG_FU_GE_QU:
                case (uint)SkillID.CHEN_LUN_XIE_ZOU_QU:
                    return 1;

            }
            return base.MaxSelectPlayer(uiState);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.CHEN_LUN_XIE_ZOU_QU:
                    return 2;
                default:
                    return base.MaxSelectCard(uiState);
            }

        }
        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.BAO_FENG_QIAN_ZOU_QU:
                    return true;
                default:
                    return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
            }

        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.BU_XIE_HE_XIAN:
                case (uint)SkillID.XI_WANG_FU_GE_QU:
                case (uint)SkillID.CHEN_LUN_XIE_ZOU_QU:
                case (uint)SkillID.BAO_FENG_QIAN_ZOU_QU:
                    return true;
                default:
                    return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
            }

        }

        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<List<uint>> selectList = new List<List<uint>>();
            List<string> explainList = new List<string>();
            switch (state)
            {
                case (uint)SkillID.CHEN_LUN_XIE_ZOU_QU:
                    if ((msg == UIStateMsg.ClickPlayer))
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        //MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseArgsUI);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        return;
                    };
                    CancelAction = () => {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SkillID.BU_XIE_HE_XIAN:
                    if (msg == UIStateMsg.ClickArgs)
                    {
                        uint watch1 = BattleData.Instance.Agent.SelectArgs[0];
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state, new List<uint>() {
                        BattleData.Instance.Agent.SelectArgs [0] % 5,
                        BattleData.Instance.Agent.SelectArgs [0] / 5 + 1
                        });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        //MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.CloseArgsUI);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        return;
                    };
                    CancelAction = () => {
                        //MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.CloseArgsUI);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init);
                    };
                    selectList.Clear();
                    explainList.Clear();
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        for (uint i = BattleData.Instance.MainPlayer.yellow_token; i >= 2; i--)
                        {
                            selectList.Add(new List<uint>() { i });
                            explainList.Add(string.Format("与目标各摸{0:D1}张牌", i - 1));
                        }
                        for (uint i = BattleData.Instance.MainPlayer.yellow_token; i >= 2; i--)
                        {
                            selectList.Add(new List<uint>() { i + 5 });
                            explainList.Add(string.Format("与目标各弃{0:D1}张牌", i));
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state, 1));
                    }
                    //selectList = new List<List<uint>>() { new List<uint>() { 2 }, new List<uint>() { 3 },
                    //	new List<uint>() { 4 },  new List<uint>() { 6 }, new List<uint>() { 7 }, new List<uint>() { 8 }};
                    //var mList = new List<string>() { "与目标摸1","与目标摸2","与目标摸3","与目标弃2","与目标弃3","与目标弃4",};
                    else
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                case (uint)SkillID.XI_WANG_FU_GE_QU:
                    if (msg == UIStateMsg.ClickPlayer)
                    {
						IsStart = true;	
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        //sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 1 });
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
                    CancelAction = () =>
                    {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
                default:
                    base.UIStateChange(state, msg, paras);
                    return;

                case (uint)SkillID.BAO_FENG_QIAN_ZOU_QU:
                    OKAction = () => {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, BattleData.Instance.Agent.SelectPlayers, null, new List<uint>() { 1 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    CancelAction = () => {
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                    };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
        }

        private enum SkillID
        {
            CHEN_LUN_XIE_ZOU_QU = 3101,
            BU_XIE_HE_XIAN = 3102,
            BU_XIE_HE_XIAN_TARGET = 31021,
            GE_YONG_TIAN_FU = 3103,
            BAO_FENG_QIAN_ZOU_QU = 3104,
            JI_ANG_KUANG_XIANG_QU = 3105,
            JI_ANG_KUANG_XIANG_QU_STONE = 31051,
            JI_ANG_KUANG_XIANG_QU_2 = 31052,
            JI_ANG_KUANG_XIANG_QU_HARM = 31053,
            SHENG_LI_JIAO_XIANG_SHI = 3106,
            SHENG_LI_JIAO_XIANG_SHI_2 = 31061,
            SHENG_LI_JIAO_XIANG_SHI_STONE = 31062,
            XI_WANG_FU_GE_QU = 3107
        };
    }
}
