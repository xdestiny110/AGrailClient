using UnityEngine;
using System.Collections;
using Framework;
using Framework.Message;
using System;
using System.Collections.Generic;

namespace AGrail
{
    public class PlayerAgent : Singleton<PlayerAgent>, IMessageListener<MessageType>
    {
        public List<uint> SelectPlayers = new List<uint>();
        public List<uint> SelectCards = new List<uint>();
        public uint? SelectSkill = null;

        private RoleBase playerRole = null;
        public RoleBase PlayerRole
        {
            get
            {
                if(playerRole == null)
                {
                    var s = BattleData.Instance.PlayerInfos.Find(p => { return p.id == BattleData.Instance.PlayerID; });
                    if(s != null)
                        playerRole = RoleFactory.Create(s.role_id);
                }
                return playerRole;
            }
        }

        public PlayerAgent() : base()
        {
            MessageSystem<MessageType>.Regist(MessageType.SelectCard, this);
            MessageSystem<MessageType>.Regist(MessageType.SelectPlayer, this);
            MessageSystem<MessageType>.Regist(MessageType.SelectSkill, this);
        }

        public void Reset()
        {
            SelectPlayers.Clear();
            SelectCards.Clear();
            SelectSkill = null;
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            int it;
            switch (eventType)
            {
                case MessageType.SelectCard:
                    var cardID = (uint)parameters[0];
                    it = SelectCards.FindIndex(c => { return c == cardID; });
                    if (it > 0)
                    {
                        SelectCards.RemoveAt(it);
                    }
                    else
                    {
                        SelectCards.Add(cardID);
                    }                        
                    break;
                case MessageType.SelectPlayer:
                    var playerID = (uint)parameters[0];
                    it = SelectPlayers.FindIndex(c => { return c == playerID; });
                    if (it > 0)
                    {
                        SelectCards.RemoveAt(it);
                    }
                    else
                    {
                        SelectCards.Add(playerID);
                    }
                    break;
                case MessageType.SelectSkill:
                    break;
            }
        }
    }

    public enum PlayerAgentState
    {
        Idle = 0x1,
        Attacked = 0x2,
        MoDaned = 0x4,
        Weaken = 0x8,
        HealCost = 0x10,
        Discard = 0x20,
        StartUp = 0x40,

        CanSpecial = 0x80,
        CanAttack = 0x100,
        CanMagic = 0x200,
        CanSkill = 0x400,
        
    }
}


