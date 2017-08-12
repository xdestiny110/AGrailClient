using UnityEngine;
using Framework.UI;
using UnityEngine.UI;
using Framework.Network;
using System.Linq;
using System;

namespace AGrail
{
    public class CreateRoomUI : WindowsBase
    {
        [SerializeField]
        private InputField roomTitle;
        [SerializeField]
        private ToggleGroup roleChoose;
        [SerializeField]
        private Toggle fourPeople;
        [SerializeField]
        private InputField password;
        [SerializeField]
        private Toggle firstExtension;
        [SerializeField]
        private Toggle thirdExtension;
        [SerializeField]
        private Toggle spMoDaoExtension;
        [SerializeField]
        private ToggleGroup seatModeChoose;

        public override WindowType Type
        {
            get
            {
                return WindowType.CreateRoomUI;
            }
        }

        public void OnOKClick()
        {            
            var proto = new network.CreateRoomRequest()
            {
                allow_guest = true,
                max_player = fourPeople.isOn ? 4 : 6,
                role_strategy = (network.ROLE_STRATEGY)Enum.Parse(typeof(network.ROLE_STRATEGY), roleChoose.ActiveToggles().First().name),
                room_name = roomTitle.text,
                password = password.text,
                seat_mode = (int)Enum.Parse(typeof(network.SEAT_MODE), seatModeChoose.ActiveToggles().First().name),
                first_extension = firstExtension.isOn,
                second_extension = firstExtension.isOn,
                silence = false,
                sp_mo_dao = spMoDaoExtension.isOn,
            };
            Lobby.Instance.CreateRoom(proto);
            GameManager.UIInstance.PopWindow(WinMsg.None);
            GameManager.UIInstance.PushWindow(WindowType.ReadyRoom, WinMsg.None);
        }

        public void OnCancelClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.Show);
        }

    }
}


