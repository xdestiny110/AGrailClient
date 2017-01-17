using System;
using System.Linq;
using Framework;
using Framework.Message;
using Framework.Network;
using System.Collections.Generic;

namespace AGrail
{
    public class BattleData : Singleton<BattleData>, IMessageListener
    {
        public int RoomID { get; private set; }
        public int PlayerID { get; private set; }
        public uint Pile { get; private set; }
        public uint Discard { get; private set; }
        public bool IsStarted { get; private set; }
        public uint[] Morale = new uint[2];
        public uint[] Gem = new uint[2];
        public uint[] Crystal = new uint[2];
        public uint[] Grail = new uint[2];
        public List<network.SinglePlayerInfo> PlayerInfos = new List<network.SinglePlayerInfo>();        

        public BattleData() : base()
        {
            MessageSystem.Regist(MessageType.GAMEINFO, this);
            reset();
        }

        public void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.GAMEINFO:
                    gameInfo = (network.GameInfo)parameters[0];                    
                    break;
            }
        }

        private void reset()
        {
            Morale = new uint[2];
            Gem = new uint[2];
            Crystal = new uint[2];
            Grail = new uint[2];
            PlayerInfos.Clear();
        }

        private network.GameInfo gameInfo
        {
            set
            {                
                RoomID = value.room_idSpecified ? value.room_id : RoomID;
                PlayerID = value.player_idSpecified ? value.player_id : PlayerID;
                Pile = value.pileSpecified ? value.pile : Pile;
                Discard = value.discardSpecified ? value.discard : Discard;
                IsStarted = value.is_startedSpecified ? value.is_started : IsStarted;
                if (value.blue_moraleSpecified)
                {
                    Morale[(int)Team.Blue] = value.blue_morale;
                    MessageSystem.Notify(MessageType.MoraleChange, Team.Blue);
                }
                if (value.red_moraleSpecified)
                {
                    Morale[(int)Team.Red] = value.red_morale;
                    MessageSystem.Notify(MessageType.MoraleChange, Team.Red);
                }
                if (value.blue_gemSpecified)
                {
                    Gem[(int)Team.Blue] = value.blue_gem;
                    MessageSystem.Notify(MessageType.GemChange, Team.Blue);
                }
                if (value.red_gemSpecified)
                {
                    Gem[(int)Team.Red] = value.red_gem;
                    MessageSystem.Notify(MessageType.GemChange, Team.Red);
                }
                if (value.blue_crystalSpecified)
                {
                    Crystal[(int)Team.Blue] = value.blue_crystal;
                    MessageSystem.Notify(MessageType.CrystalChange, Team.Blue);
                }
                if (value.red_crystalSpecified)
                {
                    Crystal[(int)Team.Red] = value.red_crystal;
                    MessageSystem.Notify(MessageType.CrystalChange, Team.Red);
                }
                if (value.blue_grailSpecified)
                {
                    Grail[(int)Team.Blue] = value.blue_grail;
                    MessageSystem.Notify(MessageType.GrailChange, Team.Blue);
                }
                if (value.red_grailSpecified)
                {
                    Grail[(int)Team.Red] = value.red_grail;
                    MessageSystem.Notify(MessageType.GrailChange, Team.Red);
                }

                foreach(var v in value.player_infos)
                {
                    var player = PlayerInfos.DefaultIfEmpty(null).FirstOrDefault((u) => { return u.id == v.id; });
                    if (player != null)
                    {
                        var idx = PlayerInfos.IndexOf(player);
                        if (v.teamSpecified)
                        {
                            player.team = v.team;
                            MessageSystem.Notify(MessageType.PlayerTeamChange, idx);
                        }
                        if (v.role_idSpecified)
                        {
                            player.role_id = v.role_id;
                            MessageSystem.Notify(MessageType.PlayerRoleChange, idx);
                        }
                        if (v.readySpecified)
                        {
                            player.ready = v.ready;
                            MessageSystem.Notify(MessageType.PlayerIsReady, idx);
                        }
                        if(v.nicknameSpecified)
                        {
                            player.nickname = v.nickname;
                            MessageSystem.Notify(MessageType.PlayerNickName, idx);
                        }
                        if (v.hand_countSpecified)
                        {
                            player.hand_count = v.hand_count;
                            MessageSystem.Notify(MessageType.PlayerHandChange, idx);
                        }
                        if(v.max_handSpecified)
                        {
                            player.max_hand = v.max_hand;
                            MessageSystem.Notify(MessageType.PlayerHandMaxChange, idx);
                        }
                        if (v.heal_countSpecified)
                        {
                            player.heal_count = v.heal_count;
                            MessageSystem.Notify(MessageType.PlayerHealChange, idx);
                        }
                        if (v.gemSpecified)
                        {
                            player.gem = v.gem;
                            MessageSystem.Notify(MessageType.PlayerGemChange, idx);
                        }
                        if (v.crystalSpecified)
                        {
                            player.crystal = v.crystal;
                            MessageSystem.Notify(MessageType.PlayerCrystalChange, idx);
                        }
                    }
                    else
                        PlayerInfos.Add(v);
                }
            }
        }
    }

    public enum Team
    {
        Blue = 0,
        Red,
        Other,
    }
}


