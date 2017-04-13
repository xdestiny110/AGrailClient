using Framework.UI;
using Framework.Message;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

namespace AGrail
{
    public class BattleUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Text turn;
        [SerializeField]
        private Transform[] morales = new Transform[2];
        [SerializeField]
        private Transform[] energy = new Transform[2];
        [SerializeField]
        private Transform[] grail = new Transform[2];
        [SerializeField]
        private Transform leftPlayerStatus;
        [SerializeField]
        private Transform rightPlayerStatus;
        [SerializeField]
        private Transform ShowCardArea;
        [SerializeField]
        private Text dialog;        
        [SerializeField]
        private GameObject playerStatusPrefab;


        [SerializeField]
        private Texture2D[] icons = new Texture2D[3];

        private List<PlayerStatus> players = new List<PlayerStatus>();

        public override WindowType Type
        {
            get
            {
                return WindowType.Battle;
            }
        }

        public override void Awake()
        {
            players.Clear();            
            MessageSystem<MessageType>.Regist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GemChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.Regist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHeroName, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerTeamChange, this);            
            MessageSystem<MessageType>.Regist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerYellowChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerBlueChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerCoveredChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerEnergeChange, this);
            MessageSystem<MessageType>.Regist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.Regist(MessageType.LogChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);

            GameManager.AddUpdateAction(onESCClick);

            root.localPosition = new Vector3(1280, 0, 0);
            root.DOLocalMoveX(0, 1.0f);
            base.Awake();
        }        

        public override void OnDestroy()
        {
            players.Clear();
            MessageSystem<MessageType>.UnRegist(MessageType.MoraleChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GemChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.CrystalChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.GrailChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerIsReady, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerNickName, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHeroName, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerTeamChange, this);            
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHandChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerHealChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerYellowChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerBlueChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerCoveredChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerKneltChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerEnergeChange, this);            
            MessageSystem<MessageType>.UnRegist(MessageType.PlayerBasicAndExCardChange, this);
            MessageSystem<MessageType>.UnRegist(MessageType.LogChange, this);
            MessageSystem<MessageType>.Regist(MessageType.CARDMSG, this);

            GameManager.RemoveUpdateAciont(onESCClick);

            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.MoraleChange:
                    moraleChange((Team)parameters[0], (uint)parameters[1]);
                    break;
                case MessageType.GemChange:
                    gemChange((Team)parameters[0], (int)parameters[1]);
                    break;
                case MessageType.CrystalChange:
                    crystalChange((Team)parameters[0], (int)parameters[1]);                    
                    break;
                case MessageType.GrailChange:
                    grailChange((Team)parameters[0], (uint)parameters[1]);
                    break;
                case MessageType.PlayerIsReady:
                    checkPlayer((int)parameters[0]);
                    players[(int)parameters[0]].IsReady = (bool)parameters[1];
                    break;
                case MessageType.PlayerNickName:
                    checkPlayer((int)parameters[0]);
                    players[(int)parameters[0]].UserName = (string)parameters[1];
                    break;
                case MessageType.PlayerHeroName:
                    break;
                case MessageType.PlayerTeamChange:
                    checkPlayer((int)parameters[0]);
                    players[(int)parameters[0]].Team = (Team)(uint)parameters[1];
                    break;
                case MessageType.PlayerBasicAndExCardChange:
                    players[(int)parameters[0]].BasicAndExCards = (parameters[1] as List<uint>).Union(parameters[2] as List<uint>).ToList();
                    break;
                case MessageType.PlayerEnergeChange:
                    players[(int)parameters[0]].Energy = new KeyValuePair<uint, uint>((uint)parameters[1], (uint)parameters[2]);
                    break;
                case MessageType.PlayerHandChange:
                    players[(int)parameters[0]].HandCount = new KeyValuePair<uint, uint>((uint)parameters[1], (uint)parameters[2]);
                    break;
                case MessageType.PlayerHealChange:
                    players[(int)parameters[0]].HealCount = (uint)parameters[1];
                    break;
                case MessageType.LogChange:
                    dialog.text = Dialog.Instance.Log;
                    break;
                case MessageType.CARDMSG:
                    var cardMsg = parameters[0] as network.CardMsg;
                    for (int i = 0; i < ShowCardArea.childCount; i++)
                        Destroy(ShowCardArea.GetChild(i).gameObject);
                    foreach(var v in cardMsg.card_ids)
                    {
                        var card = new Card(v);
                        var go = new GameObject();
                        go.transform.parent = ShowCardArea;
                        go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>(card.AssetPath);
                    }
                    break;
            }
        }

        private void onESCClick()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Lobby.Instance.LeaveRoom();
                GameManager.UIInstance.PopWindow(WinMsg.Show);
            }
        }

        private void checkPlayer(int idx)
        {            
            while(players.Count<= idx)
            {
                var go = Instantiate(playerStatusPrefab);
                players.Add(go.GetComponent<PlayerStatus>());
                if (players.Count > Lobby.Instance.SelectRoom.max_player / 2)
                    go.transform.parent = rightPlayerStatus;
                else
                    go.transform.parent = leftPlayerStatus;
            }
        }

        private void moraleChange(Team team, uint morale)
        {
            morales[(int)team].DOScaleX(morale / 15.0f, 0.2f);
        }

        private void gemChange(Team team, int diffGem)
        {
            if (diffGem > 0)
            {
                for (int i = 0; i < diffGem; i++)
                {
                    var go = new GameObject();
                    go.AddComponent<RawImage>().texture = icons[0];
                    go.transform.parent = energy[(int)team];
                    go.transform.SetSiblingIndex(0);
                }
            }
            else
            {
                for (int i = 0; i < Mathf.Abs(diffGem); i++)
                    Destroy(energy[(int)team].GetChild(i).gameObject);
            }
        }

        private void crystalChange(Team team, int diffCrystal)
        {
            if (diffCrystal > 0)
            {
                for (int i = 0; i < diffCrystal; i++)
                {
                    var go = new GameObject();
                    go.AddComponent<RawImage>().texture = icons[1];
                    go.transform.parent = energy[(int)team];
                    go.transform.SetSiblingIndex(energy[(int)team].childCount - 1);
                }
            }
            else
            {
                for (int i = energy[(int)team].childCount - 1; i > energy[(int)team].childCount - 1 + diffCrystal; i--)
                    Destroy(energy[(int)team].GetChild(i).gameObject);
            }
        }

        private void grailChange(Team team, uint diffGrail)
        {
            for (int i = 0; i < diffGrail; i++)
            {
                var go = new GameObject();
                go.AddComponent<RawImage>().texture = icons[2];
                go.transform.parent = grail[(int)team];
            }
        }

        private void cardMsg()
        {

        }

    }
}


