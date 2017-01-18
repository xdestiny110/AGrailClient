using UnityEngine;
using Framework.UI;
using Framework.Message;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

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
                if(coroHandle != null)
                {                    
                    StopCoroutine(coroHandle);
                    coroHandle = null;
                }
                for (int i = 0; i < content.childCount; i++)
                    Destroy(content.GetChild(i).gameObject);
                loadingIcon.SetActive(true);
                coroHandle = StartCoroutine(addRoomItem(value));
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
            MessageSystem<MessageType>.Regist(MessageType.RoomList, this);
            MessageSystem<MessageType>.Regist(MessageType.EnterRoom, this);
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
            MessageSystem<MessageType>.UnRegist(MessageType.RoomList, this);
            MessageSystem<MessageType>.UnRegist(MessageType.EnterRoom, this);
            base.OnDestroy();
        }

        public override void OnHide()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.RoomList, this);
            MessageSystem<MessageType>.UnRegist(MessageType.EnterRoom, this);
            root.DOLocalMoveX(-1280, 1.0f).OnComplete(() => { gameObject.SetActive(false); base.OnHide(); });            
        }

        public override void OnShow()
        {
            MessageSystem<MessageType>.Regist(MessageType.RoomList, this);
            MessageSystem<MessageType>.Regist(MessageType.EnterRoom, this);
            gameObject.SetActive(true);
            root.localPosition = new Vector3(1280, 0, 0);
            root.DOLocalMoveX(0, 1.0f);
            if (Lobby.Instance.RoomInfo == null)
                Lobby.Instance.GetRoomList();
            else
                roomInfos = Lobby.Instance.RoomInfo;
            base.OnShow();
        }

        public override void OnPause()
        {
            canvasGroup.interactable = false;
            base.OnPause();
        }

        public override void OnResume()
        {
            canvasGroup.interactable = true;
            base.OnResume();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.RoomList:
                    roomInfos = Lobby.Instance.RoomInfo;
                    break;
                case MessageType.EnterRoom:
                    //GameManager.UIInstance.PushWindow(WindowType)
                    break;
            }
        }

        private Coroutine coroHandle = null;
        private IEnumerator addRoomItem(List<network.RoomListResponse.RoomInfo> roomInfos)
        {
            if (roomInfos == null) yield break;
            foreach (var v in roomInfos)
            {
                var go = Instantiate(roomItemPrefab);
                go.transform.parent = content;
                go.SetActive(false);
                var script = go.GetComponent<RoomItem>();
                script.RoomInfo = v;
                //怒了，一帧只允许创建一个
                //在没有统一管理的资源池与协程池前先这么凑合着
                //以后搞成Bundle异步加载应该会好些
                yield return null;
            }
            for(int i = 0; i < content.childCount; i++)            
                content.GetChild(i).gameObject.SetActive(true);            
            loadingIcon.SetActive(false);
        }

    }
}


