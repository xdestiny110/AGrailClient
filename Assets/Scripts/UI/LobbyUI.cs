using UnityEngine;
using Framework.UI;
using Framework.Message;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

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
		[SerializeField]
		private GameObject title;

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
			MessageSystem<MessageType>.Regist(MessageType.ERROR, this);
			root.localPosition = new Vector3(Screen.width, 0, 0);
			root.DOLocalMoveX(0, 1.0f);
			Lobby.Instance.GetRoomList ();

			MessageSystem<MessageType>.Notify(MessageType.PlayBGM);

			base.Awake();
		}

		public override void OnDestroy()
		{
			MessageSystem<MessageType>.UnRegist(MessageType.RoomList, this);
			MessageSystem<MessageType>.UnRegist(MessageType.EnterRoom, this);
			MessageSystem<MessageType>.UnRegist(MessageType.ERROR, this);
			base.OnDestroy();
		}

		public override void OnHide()
		{
			MessageSystem<MessageType>.UnRegist(MessageType.RoomList, this);
			MessageSystem<MessageType>.UnRegist(MessageType.EnterRoom, this);
			MessageSystem<MessageType>.UnRegist(MessageType.ERROR, this);
			root.DOLocalMoveX(-Screen.width, 1.0f).OnComplete(() => { gameObject.SetActive(false); base.OnHide(); });
		}

		public override void OnResume()
		{
			MessageSystem<MessageType>.Regist(MessageType.RoomList, this);
			MessageSystem<MessageType>.Regist(MessageType.EnterRoom, this);
			MessageSystem<MessageType>.Regist(MessageType.ERROR, this);
			gameObject.SetActive(true);
			root.localPosition = new Vector3(-Screen.width, 0, 0);
			root.DOLocalMoveX(0, 1.0f);
			Lobby.Instance.GetRoomList ();
			base.OnShow();
		}

		public override void OnEventTrigger(MessageType eventType, params object[] parameters)
		{
			switch (eventType)
			{
			case MessageType.RoomList:
				roomInfos = Lobby.Instance.RoomInfo;
				break;
			case MessageType.EnterRoom:
				if (Lobby.Instance.SelectRoom.playing)
					SceneManager.LoadScene(2);
				else
					GameManager.UIInstance.PushWindow(WindowType.ReadyRoom, WinMsg.Hide);
				break;
			case MessageType.ERROR:
				var errorProto = parameters[0] as network.Error;
				if (errorProto.id == 31)
					GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InputBox, Framework.UI.WinMsg.Pause, -1, Vector3.zero,
						new Action<string>((str) => { GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.Resume); }),
						new Action<string>((str) => { GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.Resume); }),
						"瞎蒙果然是不行的~");
				break;
			}
		}

		public void OnBtnRefreshClick()
		{
			Lobby.Instance.GetRoomList();
		}

		public void OnBtnCreateClick()
		{
			GameManager.UIInstance.PushWindow(WindowType.CreateRoomUI, WinMsg.Hide);
		}

		private Coroutine coroHandle = null;
		private IEnumerator addRoomItem(List<network.RoomListResponse.RoomInfo> roomInfos)
		{
			if (roomInfos == null) yield break;
			foreach (var v in roomInfos)
			{
				var go = Instantiate(roomItemPrefab);
				go.transform.SetParent(content);
				go.SetActive(false);
				go.transform.localPosition = roomItemPrefab.transform.localPosition;
				go.transform.localRotation = roomItemPrefab.transform.localRotation;
				go.transform.localScale = roomItemPrefab.transform.localScale;
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