using Framework.UI;
using Framework.Message;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

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

            GameManager.RemoveUpdateAciont(onESCClick);

            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.MoraleChange:
                    morales[(int)parameters[0]].DOScaleX((int)parameters[1] / 15.0f, 0.2f);
                    break;
                case MessageType.GemChange:
                    int diffGem = (int)parameters[1];
                    if (diffGem > 0)
                    {
                        for (int i = 0; i < diffGem; i++)
                        {
                            var go = new GameObject();
                            go.AddComponent<RawImage>().texture = icons[0];
                            go.transform.parent = energy[(int)parameters[0]];
                            go.transform.SetSiblingIndex(0);
                        }
                    }
                    else
                    {

                        for (int i = 0; i < diffGem; i++)
                            Destroy(energy[(int)parameters[0]].GetChild(i).gameObject);
                    }
                    break;
                case MessageType.CrystalChange:
                    int diffCrystal = (int)parameters[1];
                    if (diffCrystal > 0)
                    {
                        for (int i = 0; i < diffCrystal; i++)
                        {
                            var go = new GameObject();
                            go.AddComponent<RawImage>().texture = icons[1];
                            go.transform.parent = energy[(int)parameters[0]];
                            go.transform.SetSiblingIndex(energy[(int)parameters[0]].childCount - 1);
                        }
                    }
                    else
                    {
                        for (int i = energy[(int)parameters[0]].childCount - 1; i < energy[(int)parameters[0]].childCount - 1 + diffCrystal; i++)
                            Destroy(energy[(int)parameters[0]].GetChild(i).gameObject);
                    }
                    break;
                case MessageType.GrailChange:
                    for(int i = 0; i < (int)parameters[1]; i++)
                    {
                        var go = new GameObject();
                        go.AddComponent<RawImage>().texture = icons[2];
                        go.transform.parent = grail[(int)parameters[0]];
                    }
                    break;
                case MessageType.PlayerIsReady:
                    checkPlayer((int)parameters[0]);
                    players[(int)parameters[0]].IsReady = (bool)parameters[1];
                    break;
                case MessageType.PlayerNickName:
                    players[(int)parameters[0]].UserName = (string)parameters[1];
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
    }
}


