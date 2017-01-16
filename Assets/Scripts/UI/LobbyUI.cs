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

        private List<network.RoomListResponse.RoomInfo> roomInfos
        {
            set
            {
                for (int i = 0; i < content.childCount; i++)
                    Destroy(content.GetChild(i).gameObject);
                if (value == null)
                {                    
                    loadingIcon.SetActive(true);
                }
                else
                {
                    loadingIcon.SetActive(false);
                    foreach(var v in value)
                    {
                        //Debug.Log(Time.realtimeSinceStartup);
                        var go = Instantiate(roomItemPrefab);
                        //Debug.Log(Time.realtimeSinceStartup);
                        go.transform.parent = content;
                        var script = go.GetComponent<RoomItem>();
                        script.RoomInfo = v;
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
            if(Lobby.Instance.RoomInfo == null)
                Lobby.Instance.GetRoomList();
            else
                roomInfos = Lobby.Instance.RoomInfo;            
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
                    roomInfos = Lobby.Instance.RoomInfo;
                    break;
            }
        }

    }
}


