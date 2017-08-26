using Framework.Message;
using network;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class MoNv : RoleBase
    {
        public override RoleID RoleID
        {
            get
            {
                return RoleID.MoNv;
            }
        }

        public override string RoleName
        {
            get
            {
                return "苍炎魔女";
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
                return "莉莉丝";
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
                return "LieYan";
            }
        }

		public MoNv()
		{
			for (uint i = (uint)SkillID.CANG_YAN_FA_DIAN; i <= (uint)SkillID.MO_NENG_FAN_ZHUAN; i++)
				Skills.Add(i, Skill.GetSkill(i));
		}

		public override bool CanSelect(uint uiState, Card card, bool isCovered)
		{
			Card.CardElement realElement;

			if ((BattleData.Instance.MainPlayer.is_knelt) && (card.Type ==  Card.CardType.attack) && (card.Element != Card.CardElement.light && card.Element != Card.CardElement.darkness && card.Element != Card.CardElement.water))
				realElement = Card.CardElement.fire;
			else
				realElement = card.Element;
			
			switch (uiState)
			{
				case 3:	//应战
					if (card.Element == Card.CardElement.light ||
					     (((realElement == Card.CardElement.darkness || realElement == Card.GetCard (BattleData.Instance.Agent.Cmd.args [1]).Element)
					     && BattleData.Instance.Agent.Cmd.args [0] < 1
					     && card.Type == Card.CardType.attack)))
						return true;
					else
						return false;
				case (uint)SkillID.CANG_YAN_FA_DIAN:
					return (realElement == Card.CardElement.fire);
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
					return ((BattleData.Instance.Agent.SelectCards.Count < 2) &&
						(realElement == Card.CardElement.fire)) || BattleData.Instance.Agent.SelectCards.Contains(card.ID);
				case (uint)SkillID.TI_SHEN_WAN_OU:	
					return card.Type == Card.CardType.magic;
				case (uint)SkillID.TONG_KU_LIAN_JIE:	
				case (uint)SkillID.TONG_KU_LIAN_JIE_CARD:	
					return true;
				case (uint)SkillID.MO_NENG_FAN_ZHUAN:
					return card.Type == Card.CardType.magic;
			}
			return base.CanSelect(uiState, card, isCovered);
		}

		public override bool CanSelect(uint uiState, SinglePlayerInfo player)
		{
			switch (uiState)
			{
				case (uint)SkillID.TI_SHEN_WAN_OU:
					return BattleData.Instance.Agent.SelectCards.Count == 1 &&
                        player.team == BattleData.Instance.MainPlayer.team &&
                        BattleData.Instance.PlayerID != player.id;
				case (uint)SkillID.CANG_YAN_FA_DIAN:
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
                    return BattleData.Instance.Agent.SelectCards.Count == MaxSelectCard(uiState);
                case (uint)SkillID.TONG_KU_LIAN_JIE:
					return player.team != BattleData.Instance.MainPlayer.team;
                case (uint)SkillID.MO_NENG_FAN_ZHUAN:
                    return player.team != BattleData.Instance.MainPlayer.team && BattleData.Instance.Agent.SelectCards.Count > 1;
            }
			return base.CanSelect(uiState, player);
		}

		public override bool CanSelect(uint uiState, Skill skill)
		{
			switch (uiState)
			{
				case 10:
				case 11:
				case (uint)SkillID.CANG_YAN_FA_DIAN:
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
				case (uint)SkillID.TONG_KU_LIAN_JIE:
				case (uint)SkillID.YONG_SHENG_YIN_SHI_JI:
					if (skill.SkillID == (uint)SkillID.CANG_YAN_FA_DIAN)
                        return Util.HasCard(Card.CardElement.fire, BattleData.Instance.MainPlayer.hands) || (BattleData.Instance.MainPlayer.is_knelt);
                    if (skill.SkillID == (uint)SkillID.TIAN_HUO_DUAN_KONG)
						return ( (BattleData.Instance.MainPlayer.yellow_token > 0 && Util.HasCard(Card.CardElement.fire, BattleData.Instance.MainPlayer.hands,2)) || (BattleData.Instance.MainPlayer.is_knelt) );
					if (skill.SkillID == (uint)SkillID.TONG_KU_LIAN_JIE)
						return (BattleData.Instance.MainPlayer.crystal + BattleData.Instance.MainPlayer.gem > 0);
					return false;
				case (uint)SkillID.TI_SHEN_WAN_OU:
				case (uint)SkillID.MO_NENG_FAN_ZHUAN:
					return false;
						
				default:	
					return base.CanSelect(uiState, skill);
			}
		}
		public override uint MaxSelectCard(uint uiState)
		{
			switch (uiState)
			{
				case (uint)SkillID.CANG_YAN_FA_DIAN:
					return 1;
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
					return 2;
				case (uint)SkillID.TI_SHEN_WAN_OU:
					return 1;
				case (uint)SkillID.TONG_KU_LIAN_JIE:
					//return Math.Max(0, BattleData.Instance.MainPlayer.hand_count - 3);
					return 0;
				case (uint)SkillID.MO_NENG_FAN_ZHUAN:
					return BattleData.Instance.MainPlayer.hand_count;	

				default:
					return base.MaxSelectCard(uiState);
			}

		}

		public override uint MaxSelectPlayer(uint uiState)
		{
			switch (uiState)
			{
				case (uint)SkillID.CANG_YAN_FA_DIAN:
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
				case (uint)SkillID.TI_SHEN_WAN_OU:
				case (uint)SkillID.TONG_KU_LIAN_JIE:
				case (uint)SkillID.MO_NENG_FAN_ZHUAN:
					return 1;	
				default:
					return base.MaxSelectPlayer(uiState);
			}
		}

		public override bool CheckCancel(uint uiState, List<uint> cardIDs, List<uint> playerIDs, uint? skillID)
		{
			switch (uiState)
			{
				case (uint)SkillID.CANG_YAN_FA_DIAN:
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
				case (uint)SkillID.TI_SHEN_WAN_OU:
				case (uint)SkillID.TONG_KU_LIAN_JIE:
				case (uint)SkillID.MO_NENG_FAN_ZHUAN:
				case (uint)SkillID.MO_NV_ZHI_NU:
					return true;
			}
			return base.CheckCancel(uiState, cardIDs, playerIDs, skillID);
		}

		public override void UIStateChange(uint state, UIStateMsg msg, params object[] paras)
		{
			List<List<uint>> selectList = new List<List<uint>>();
			switch (state)
			{
			case 1:	//躺斩
				if (BattleData.Instance.MainPlayer.is_knelt) {
					MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.SendHint, StateHint.GetHint(StateEnum.Attack));
                        if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1) {
						sendActionMsg (BasicActionType.ACTION_ATTACK_SKILL, BattleData.Instance.MainPlayer.id,
							BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, (uint)SkillID.MO_NV_ZHI_NU_ATTACK);
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
					}
				}
				else base.UIStateChange (state, msg, paras);
					return;
				case (uint)SkillID.CANG_YAN_FA_DIAN:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
							BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
					CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
					MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
				
				case (uint)SkillID.TIAN_HUO_DUAN_KONG:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 2)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
							BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, state);
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle>(UIStateMsg.Init, true);
                        return;
                    };
					CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
					MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;

				case (uint)SkillID.TI_SHEN_WAN_OU:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1 && BattleData.Instance.Agent.SelectCards.Count == 1)
                    {
                        sendReponseMsg (state, BattleData.Instance.MainPlayer.id,
							BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, new List<uint>() { 1 });
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
                        return;
                    };
					CancelAction = () => {
						sendReponseMsg(state, BattleData.Instance.MainPlayer.id, null, null, new List<uint>() { 0 });
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
					};
					MessageSystem<Framework.Message.MessageType>.Notify (Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
				case (uint)SkillID.TONG_KU_LIAN_JIE:
                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
                        sendActionMsg(BasicActionType.ACTION_MAGIC_SKILL, BattleData.Instance.MainPlayer.id,
							BattleData.Instance.Agent.SelectPlayers, null, state, new List<uint>() { 1 });
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
						return;
					};
					CancelAction = () => { BattleData.Instance.Agent.FSM.BackState(UIStateMsg.Init); };
					MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
				case (uint)SkillID.MO_NENG_FAN_ZHUAN:

                    if (BattleData.Instance.Agent.SelectPlayers.Count == 1)
                    {
						sendReponseMsg (state, BattleData.Instance.MainPlayer.id,
							BattleData.Instance.Agent.SelectPlayers, BattleData.Instance.Agent.SelectCards, new List<uint> () { 1 });
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
                        return;
					};
					CancelAction = () => {
						sendReponseMsg (state, BattleData.Instance.MainPlayer.id, null, null, new List<uint> () { 0 });
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
					};
					MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
				case (uint)SkillID.MO_NV_ZHI_NU:
                    //MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                    if (msg == UIStateMsg.ClickArgs) {
                        IsStart = true;						
						sendReponseMsg (state, BattleData.Instance.MainPlayer.id,
							null, null, new List<uint> () { 1, BattleData.Instance.Agent.SelectArgs[0] });
						MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
						return;
					};
					CancelAction = () => {
                        MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.CloseNewArgsUI);
                        sendReponseMsg (state, BattleData.Instance.MainPlayer.id, null, null, new List<uint> () { 0 });
						BattleData.Instance.Agent.FSM.ChangeState<StateIdle> (UIStateMsg.Init, true);
					};
					selectList = new List<List<uint>>() { new List<uint>() { 0 }, new List<uint>() { 1 },
						new List<uint>() { 2 }};
					var mList = new List<string>() { "0张手牌","1张手牌","2张手牌"};
					MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.ShowNewArgsUI, selectList, mList);
					MessageSystem<Framework.Message.MessageType>.Notify(Framework.Message.MessageType.SendHint, StateHint.GetHint(state));
                    return;
				default:
					base.UIStateChange (state, msg, paras);
					return;
			}
		}
		private enum SkillID
		{
			CANG_YAN_FA_DIAN=3001,
			TIAN_HUO_DUAN_KONG=3002,
			MO_NV_ZHI_NU=3003,
			MO_NV_ZHI_NU_ATTACK=30031,
			TI_SHEN_WAN_OU=3004,
			YONG_SHENG_YIN_SHI_JI=3005,
			TONG_KU_LIAN_JIE=3006,
			TONG_KU_LIAN_JIE_CARD=30061,
			MO_NENG_FAN_ZHUAN=3007
		}
    }
}


