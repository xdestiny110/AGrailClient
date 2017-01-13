using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using System;

namespace AGrail
{
    public class RoomItem : MonoBehaviour, IPointerClickHandler
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
            }
        }        

        public void OnPointerClick(PointerEventData eventData)
        {
            if (roomInfo.has_password)
            {

            }
            else
                Lobby.Instance.JoinRoom(roomInfo.room_id);
                     
        }
    }
}


