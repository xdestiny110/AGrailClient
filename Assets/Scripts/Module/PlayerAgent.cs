using UnityEngine;
using System.Collections;
using Framework;
using Framework.Message;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class PlayerAgent
    {
        public List<uint> SelectPlayers = new List<uint>();
        public List<uint> SelectCards = new List<uint>();
        public uint? SelectSkill = null;
        public List<uint> SelectArgs = new List<uint>();

        private int agentState = (int)PlayerAgentState.Idle;
        public UIStateMachine FSM = new UIStateMachine("AgnetUI");
        public int AgentState
        {
            get { return agentState; }
            set
            {
                Debug.LogFormat("Agent state = 0x{0}", Convert.ToString(value, 16));
                if (agentState == (int)PlayerAgentState.CanResign)
                    agentState |= value;
                else
                    agentState = value;
                MessageSystem<MessageType>.Notify(MessageType.AgentStateChange);
                FSM.ChangeState(calUIState(agentState), true, UIStateMsg.Init);
            }
        }

        public RoleBase PlayerRole { get; private set; }
        public network.Command Cmd { get; set; }

        public PlayerAgent(uint roleID) : base()
        {
            PlayerRole = RoleFactory.Create(roleID);
        }

        public void AddSelectPlayer(uint playerID)
        {
            if (!SelectPlayers.Contains(playerID))
            {
                SelectPlayers.Add(playerID);
                while (SelectPlayers.Count > PlayerRole.MaxSelectPlayer(FSM.Current.StateNumber))
                    SelectPlayers.RemoveAt(0);
            }
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            FSM.HandleMessage(UIStateMsg.ClickPlayer);
        }

        public void RemoveSelectPlayer(uint playerID)
        {
            if (SelectPlayers.Contains(playerID))
                SelectPlayers.Remove(playerID);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            FSM.HandleMessage(UIStateMsg.ClickPlayer);
        }

        public void RemoveAllSelectPlayer()
        {
            SelectPlayers.Clear();
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectPlayer);
            FSM.HandleMessage(UIStateMsg.ClickPlayer);
        }

        public void AddSelectCard(uint cardID)
        {
            if (!SelectCards.Contains(cardID))
            {
                SelectCards.Add(cardID);
                while (SelectCards.Count > PlayerRole.MaxSelectCard(FSM.Current.StateNumber))
                    SelectCards.RemoveAt(0);
            }
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            FSM.HandleMessage(UIStateMsg.ClickCard);
        }

        public void RemoveSelectCard(uint cardID)
        {
            if (SelectCards.Contains(cardID))
                SelectCards.Remove(cardID);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            FSM.HandleMessage(UIStateMsg.ClickCard);
        }

        public void RemoveAllSelectCard()
        {
            SelectCards.Clear();
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectCard);
            FSM.HandleMessage(UIStateMsg.ClickCard);
        }

        public void ChangeSelectSkill(uint? skillID)
        {
            SelectSkill = skillID;
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectSkill);
            FSM.HandleMessage(UIStateMsg.ClickSkill);
        }

        public void AddSelectArgs(List<uint> args)
        {
            SelectArgs.Clear();
            SelectArgs.AddRange(args);
            MessageSystem<MessageType>.Notify(MessageType.AgentSelectArgs);
            FSM.HandleMessage(UIStateMsg.ClickArgs);
        }

        private Type calUIState(int state)
        {
            if (state.Check(PlayerAgentState.CanSpecial))
                return typeof(StateNormal);
            if (state.Check(PlayerAgentState.CanAttack) && state.Check(PlayerAgentState.CanMagic))
                return typeof(StateAttackAndMagic);
            if (state.Check(PlayerAgentState.CanAttack) && !state.Check(PlayerAgentState.CanMagic))
                return typeof(StateAttack);
            if (!state.Check(PlayerAgentState.CanAttack) && state.Check(PlayerAgentState.CanMagic))
                return typeof(StateMagic);
            if (state.Check(PlayerAgentState.Attacked))
                return typeof(StateAttacked);
            if (state.Check(PlayerAgentState.MoDaned))
                return typeof(StateModaned);
            if (state.Check(PlayerAgentState.Discard))
                return typeof(StateDrop);
            if (state.Check(PlayerAgentState.Weaken))
                return typeof(StateWeaken);
            if (state.Check(PlayerAgentState.HealCost))
                return typeof(StateHealCost);
            if (state.Check(PlayerAgentState.DiscardCovered))
                return typeof(StateDropCovered);
            if (state.Check(PlayerAgentState.AdditionAction))
                return typeof(StateAdditionAction);
            if (state.Check(PlayerAgentState.SkillResponse))
                return typeof(StateSkill);
            if (state.Check(PlayerAgentState.ActionNone))
                return typeof(StateActionNone);
            if (state.Check(PlayerAgentState.Polling))
                return typeof(StatePolling);
            return typeof(StateIdle);
        }
    }

}


