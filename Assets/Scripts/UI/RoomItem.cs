using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System;

namespace AGrail
{
    public class RoomItem : MonoBehaviour
    {
        [SerializeField]
        private Text roomID;
        [SerializeField]
        private Text roomMode;
        [SerializeField]
        private Text roomName;
        [SerializeField]
        private Text roomPeople;
        [SerializeField]
        private Text roomStatus;
        [SerializeField]
        private Image roomNeedPassword;

        private network.RoomListResponse.RoomInfo roomInfo;
        public network.RoomListResponse.RoomInfo RoomInfo
        {
            set
            {
                roomInfo = value;
                roomID.text = roomInfo.room_id.ToString();
                roomName.text = roomInfo.room_name;
                roomPeople.text = string.Format("{0}/{1}", roomInfo.now_player, roomInfo.max_player);
                roomStatus.text = (roomInfo.playing) ? "进行中" : "等待中";
                switch (roomInfo.role_strategy)
                {
                    case network.ROLE_STRATEGY.ROLE_STRATEGY_31:
                        roomMode.text = "三选一";
                        break;
                    case network.ROLE_STRATEGY.ROLE_STRATEGY_BP:
                        roomMode.text = "Ban-Pick";
                        break;
                    case network.ROLE_STRATEGY.ROLE_STRATEGY_CM:
                        roomMode.text = "CM";
                        GetComponent<Button>().interactable = false;
                        break;
                    case network.ROLE_STRATEGY.ROLE_STRATEGY_RANDOM:
                        roomMode.text = "随机";
                        break;
                    default:
                        roomMode.text = "测试用";
                        break;
                }
                if (roomInfo.first_extension)
                    roomMode.text += " 一扩";
                if (roomInfo.second_extension)
                    roomMode.text += " 三版";
                if (roomInfo.sp_mo_dao)
                    roomMode.text += " SP魔导";
                if (roomInfo.has_password)
                    roomNeedPassword.enabled = true;
                else
                    roomNeedPassword.enabled = false;
            }
        }

        public void OnClick()
        {
            if (roomInfo.has_password)
            {
                GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InputBox, Framework.UI.WinMsg.Pause, -1, Vector3.zero,
                    new Action<string>((str) => { GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.Resume); Lobby.Instance.JoinRoom(roomInfo, str); }),
                    new Action<string>((str) => { GameManager.UIInstance.PopWindow(Framework.UI.WinMsg.Resume); }),
                    "请输入暗号");
            }
            else
                Lobby.Instance.JoinRoom(roomInfo);
        }
    }
}


