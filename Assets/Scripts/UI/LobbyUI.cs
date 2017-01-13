using UnityEngine;
using Framework.UI;
using Framework.Message;
using DG.Tweening;
using System.Collections.Generic;

namespace AGrail
{   
    public class LobbyUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private GameObject roomItemPrefab;        
        [SerializeField]
        private GameObject loadingIcon;
        [SerializeField]
        private Transform content;

        private List<network.RoomListResponse.RoomInfo> roomInfo
        {
            set
            {
                if(value == null)
                {
                    for (int i = 0; i < content.childCount; i++)
                        Destroy(content.GetChild(i).gameObject);
                    loadingIcon.SetActive(true);
                }
                else
                {
                    loadingIcon.SetActive(false);
                    foreach(var v in value)
                    {
                        var go = Instantiate(roomItemPrefab);
                    }
                }
            }
        }

        public override WindowType Type
        {
            get
            {
                return WindowType.Lobby;
            }
        }

        public override void Awake()
        {
            MessageSystem.Regist(MessageType.RoomList, this);
            root.localPosition = new Vector3(1280, 0, 0);
            root.DOLocalMoveX(0, 1.0f);
            Lobby.Instance.GetRoomList();
            roomInfo = Lobby.Instance.RoomInfo;            
            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem.UnRegist(MessageType.RoomList, this);
            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.RoomList:
                    roomInfo = Lobby.Instance.RoomInfo;
                    break;
            }
        }

    }
}


