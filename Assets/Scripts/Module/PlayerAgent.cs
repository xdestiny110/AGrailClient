﻿using UnityEngine;
using System.Collections;
using Framework;
using Framework.Message;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class PlayerAgent : IMessageListener<MessageType>, IDisposable
    {
        private List<uint> selectPlayers = new List<uint>();
        private List<uint> selectCards = new List<uint>();
        private uint? selectSkill = null;
        private int agentState = 0;
        public int AgentState
        {
            get { return agentState; }
            set
            {
                agentState = value;
                MessageSystem<MessageType>.Notify(MessageType.AgentStateChange, agentState);
            }
        }
        
        public RoleBase PlayerRole { get; private set; }
        public network.Command RespCmd { get; set; }

        public PlayerAgent(uint roleID) : base()
        {
            PlayerRole = RoleFactory.Create(roleID);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectCard, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectPlayer, this);
            MessageSystem<MessageType>.Regist(MessageType.AgentSelectSkill, this);
        }

        public void Reset()
        {
            selectPlayers.Clear();
            selectCards.Clear();
            selectSkill = null;
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            int it;
            switch (eventType)
            {
                case MessageType.AgentSelectCard:
                    var cardID = (uint)parameters[0];
                    it = selectCards.FindIndex(c => { return c == cardID; });
                    if (it > 0)
                        selectCards.RemoveAt(it);
                    else
                        selectCards.Add(cardID);
                    PlayerRole.Check(AgentState, selectCards, selectPlayers, selectSkill);
                    break;
                case MessageType.AgentSelectPlayer:
                    var playerID = (uint)parameters[0];
                    it = selectPlayers.FindIndex(c => { return c == playerID; });
                    if (it > 0)
                        selectPlayers.RemoveAt(it);
                    else
                        selectPlayers.Add(playerID);
                    PlayerRole.Check(AgentState, selectCards, selectPlayers, selectSkill);
                    break;
                case MessageType.AgentSelectSkill:
                    break;
            }
        }

        private bool IsDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool Disposing)
        {
            if (!IsDisposed)
            {
                if (Disposing)
                {

                }
                MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectCard, this);
                MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectPlayer, this);
                MessageSystem<MessageType>.UnRegist(MessageType.AgentSelectSkill, this);
            }
            IsDisposed = true;
        }

        ~PlayerAgent()
        {
            Dispose(false);
        }
    }
}


