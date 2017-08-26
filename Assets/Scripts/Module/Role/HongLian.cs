using System.Collections.Generic;
using network;
using Framework.Message;
using System.Linq;

namespace AGrail
{
    public class HongLian : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.HongLian;
            }
        }

        public override string RoleName
        {
            get
            {
                return "红莲骑士";
            }
        }

        public override Card.CardProperty RoleProperty
        {
            get
            {
                return Card.CardProperty.血;
            }
        }

        public override string HeroName
        {
            get
            {
                return "斯卡雷特";
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

        public override string Knelt
        {
            get
            {
                return "ReXueFeiTeng";
            }
        }

        public override uint MaxHealCount
        {
            get
            {
                return 4;
            }
        }

        public HongLian()
        {
            for (uint i = 2801; i <= 2803; i++)
                Skills.Add(i, Skill.GetSkill(i));
            for (uint i = 2806; i <= 2809; i++)
                Skills.Add(i, Skill.GetSkill(i));
        }

        public override bool CanSelect(uint uiState, Card card, bool isCovered)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return card.Type == Card.CardType.magic;
            }
            return base.CanSelect(uiState, card, isCovered);
        }

        public override bool CanSelect(uint uiState, SinglePlayerInfo player)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return BattleData.Instance.Agent.SelectCards.Count == 2;
            }
            return base.CanSelect(uiState, player);
        }

        public override bool CanSelect(uint uiState, Skill skill)
        {
            switch (uiState)
            {
                case 10:
                case 11:
                case (uint)SkillID.腥红十字:
                    if (skill.SkillID == (uint)SkillID.腥红十字 &&
                        BattleData.Instance.MainPlayer.gem + BattleData.Instance.MainPlayer.crystal > 0)
                        return (BattleData.Instance.MainPlayer.yellow_token >= 1 && Util.HasCard(Card.CardType.magic, BattleData.Instance.MainPlayer.hands,2) );
                    return false;
            }
            return base.CanSelect(uiState, skill);
        }

        public override uint MaxSelectCard(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return 2;
            }
            return base.MaxSelectCard(uiState);
        }

        public override uint MaxSelectPlayer(uint uiState)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红十字:
                    return 1;
            }
            return base.MaxSelectPlayer(uiState);
        }

        public override bool CheckOK(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红圣约:
                case (uint)SkillID.杀戮盛宴:
                case (uint)SkillID.戒骄戒躁:
                    return true;
                case (uint)SkillID.腥红十字:
                    return cardIDs.Count == 2 && playerIDs.Count == 1;
            }
            return base.CheckOK(uiState, cardIDs, playerIDs, skillID);
        }

        public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
        {
            switch (uiState)
            {
                case (uint)SkillID.腥红圣约:
                case (uint)SkillID.血腥祷言:
                case (uint)SkillID.杀戮盛宴:
                case (uint)SkillID.戒骄戒躁:
                case (uint)SkillID.腥红十字:
                    return true;
            }
            return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
        }

        private List<uint> selectPlayers = new List<uint>();
        private List<uint> selectHealCnt = new List<uint>();
        public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
        {
            List<List<uint>> selectList = new List<List<uint>>();
            List<string> explainList = new List<string>();
            switch (state)
            {
                case (uint)SkillID.腥红圣约:
                case (uint)SkillID.杀戮盛宴:
                case (uint)SkillID.戒骄戒躁:
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
                case (uint)SkillID.血腥祷言:
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if(additionalState == 0 && BattleData.Instance.Agent.SelectArgs.Count == 0)
                    {
                        selectList.Clear();
                        explainList.Clear();
                        for (uint i = 0; i <= BattleData.Instance.MainPlayer.heal_count; i++)
                        {
                            selectList.Add(new List<uint>() { i });
                            explainList.Add(i + "个治疗");
                        }
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                            string.Format( StateHint.GetHint(state),allies[0].nickname ) );
                    }
                    else if (additionalState == 0 && BattleData.Instance.Agent.SelectArgs.Count == 1)
                    {
                        selectPlayers.Clear();
                        selectHealCnt.Clear();
                        selectPlayers.Add(allies[0].id);
                        selectHealCnt.Add(BattleData.Instance.Agent.SelectArgs[0]);
                        BattleData.Instance.Agent.SelectArgs.Clear();
                        if (allies.Count == 1)
                        {
                            IsStart = true;
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                                selectPlayers, null, selectHealCnt);
                        }
                        else
                        {
                            additionalState = 28031;
                            selectList.Clear();
                            explainList.Clear();
                            for (uint i = 0; i <= BattleData.Instance.MainPlayer.heal_count - selectHealCnt[0]; i++)
                            {
                                selectList.Add(new List<uint>() { i });
                                explainList.Add(i + "个治疗");
                            }
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, explainList);
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint,
                                string.Format(StateHint.GetHint(state,1), allies[1].nickname));
                        }
                    }
                    else if(additionalState == 28031 && BattleData.Instance.Agent.SelectArgs.Count == 1)
                    {
                        IsStart = true;
                        if (selectHealCnt[0] == 0)
                        {
                            selectHealCnt.Clear();
                            selectPlayers.Clear();
                        }
                        selectPlayers.Add(allies[1].id);
                        selectHealCnt.Add(BattleData.Instance.Agent.SelectArgs[0]);
                        sendReponseMsg(state, BattleData.Instance.MainPlayer.id,
                            selectPlayers, null, selectHealCnt);
                        additionalState = 0;
                    }
                    CancelAction = () =>
                    {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        if(additionalState == 0)
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
                        }
                        else
                        {
                            MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                            additionalState = 0;
                            BattleData.Instance.Agent.RemoveSelectPlayer(0);
                        }
                    };
                    return;
                case (uint)SkillID.腥红十字:
                    if(BattleData.Instance.Agent.SelectCards.Count == 2 && BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
                            BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
                        BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    }
                    CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
                    MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
            }
            base.UIStateChange(state, msg, paras);
        }

        private List<SinglePlayerInfo> allies
        {
            get
            {
                return BattleData.Instance.PlayerInfos.Where((s) => { return s.team == BattleData.Instance.MainPlayer.team && s.id != BattleData.Instance.MainPlayer.id; }).ToList();
            }
        }

        private enum SkillID
        {
            腥红圣约 = 2801,
            腥红信仰,
            血腥祷言,
            杀戮盛宴 = 2806,
            热血沸腾,
            戒骄戒躁,
            腥红十字
        }
    }
}


