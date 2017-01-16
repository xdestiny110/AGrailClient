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
                Morale[(int)Team.Blue] = value.blue_moraleSpecified ? value.blue_morale : Morale[(int)Team.Blue];
                Morale[(int)Team.Red] = value.red_moraleSpecified ? value.red_morale : Morale[(int)Team.Red];
                Gem[(int)Team.Blue] = value.blue_gemSpecified ? value.blue_gem : Gem[(int)Team.Blue];
                Gem[(int)Team.Red] = value.red_gemSpecified ? value.red_gem : Gem[(int)Team.Red];
                Crystal[(int)Team.Blue] = value.blue_crystalSpecified ? value.blue_crystal : Crystal[(int)Team.Blue];
                Crystal[(int)Team.Red] = value.red_crystalSpecified ? value.red_crystal : Crystal[(int)Team.Red];
                Grail[(int)Team.Blue] = value.blue_grailSpecified ? value.blue_grail : Grail[(int)Team.Blue];
                Grail[(int)Team.Red] = value.red_grailSpecified ? value.red_grail : Grail[(int)Team.Red];

                foreach(var v in value.player_infos)
                {
                    var player = PlayerInfos.DefaultIfEmpty(null).FirstOrDefault((u) => { return u.id == v.id; });
                    if (player != null)
                    {
                        player.team = v.teamSpecified ? v.team : player.team;
                        player.role_id = v.role_idSpecified ? v.role_id : player.role_id;
                        
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


