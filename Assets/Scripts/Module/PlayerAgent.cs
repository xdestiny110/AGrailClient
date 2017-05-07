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
        public int AgentState
        {
            get { return agentState; }
            set
            {
                Debug.LogFormat("Agent state = {0}", value);
                reset();
                agentState = value;
                MessageSystem<MessageType>.Notify(MessageType.AgentStateChange);
                AgentUIState = calUIState(agentState);                
            }
        }

        private uint agentUIState = 0;
        public uint AgentUIState
        {
            get { return agentUIState; }
            set
            {
                Debug.LogFormat("Agent UI state = {0}", value);
                agentUIState = value;
                MessageSystem<MessageType>.Notify(MessageType.AgentUIStateChange);
            }
        }
        
        public RoleBase PlayerRole { get; private set; }
        public network.Command Cmd { get; set; }

        public PlayerAgent(uint roleID) : base()
        {
            PlayerRole = RoleFactory.Create(roleID);         
        }

        private void reset()
        {
            SelectPlayers.Clear();
            SelectCards.Clear();
            SelectSkill = null;
            SelectArgs.Clear();
        }

        private uint calUIState(int state)
        {
            if (state.Check(PlayerAgentState.CanSpecial))
                return 10;
            if (state.Check(PlayerAgentState.CanAttack) && state.Check(PlayerAgentState.CanMagic))
                return 11;
            if (state.Check(PlayerAgentState.CanAttack) && !state.Check(PlayerAgentState.CanMagic))
                return 1;
            if (!state.Check(PlayerAgentState.CanAttack) && state.Check(PlayerAgentState.CanMagic))
                return 2;
            if (state.Check(PlayerAgentState.Attacked))
                return 3;
            if (state.Check(PlayerAgentState.MoDaned))
                return 4;
            if (state.Check(PlayerAgentState.Discard))
                return 5;
            if (state.Check(PlayerAgentState.Weaken))
                return 6;
            if (state.Check(PlayerAgentState.HealCost))
                return 7;
            if (state.Check(PlayerAgentState.AdditionAction))
                return 42;
            if (state.Check(PlayerAgentState.SkillResponse))
                return Cmd.respond_id;
            return 0;
        }
    }    

}


