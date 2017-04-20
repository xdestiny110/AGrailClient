using UnityEngine;
using Framework.UI;
using DG.Tweening;
using Framework.Message;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AGrail
{
    public class BattleUIQT : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Text turn;
        [SerializeField]
        private Text[] morales = new Text[2];        
        [SerializeField]
        private Transform[] energy = new Transform[2];
        [SerializeField]
        private Transform[] grail = new Transform[2];
        [SerializeField]
        private List<Transform> playerAnchor = new List<Transform>();
        [SerializeField]
        private Transform ShowCardArea;
        [SerializeField]
        private GameObject playerStatusPrefab;

        public override WindowType Type
        {
            get
            {
                return WindowType.BattleQT;
            }
        }

        public override void Awake()
        {
            GameManager.AddUpdateAction(onESCClick);
            Dialog.Instance.Reset();

            root.localPosition = new Vector3(1280, 0, 0);
            root.DOLocalMoveX(0, 1.0f);
            base.Awake();
        }

        public override void OnDestroy()
        {
            GameManager.RemoveUpdateAciont(onESCClick);
            base.OnDestroy();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {

        }

        private void onESCClick()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Lobby.Instance.LeaveRoom();
                GameManager.UIInstance.PopWindow(WinMsg.Show);
            }
        }
    }

}
