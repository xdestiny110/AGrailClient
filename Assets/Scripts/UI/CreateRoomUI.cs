using UnityEngine;
using Framework.UI;
using UnityEngine.UI;
using Framework.Network;

namespace AGrail
{
    public class CreateRoomUI : WindowsBase
    {
        [SerializeField]
        private InputField roomTitle;
        [SerializeField]
        private Dropdown roleChoose;
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
        private Dropdown seatModeChoose;

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
                role_strategy = (network.ROLE_STRATEGY)(roleChoose.value + 1),
                room_name = roomTitle.text,
                password = password.text,
                seat_mode = seatModeChoose.value + 1,
                first_extension = firstExtension.isOn,
                second_extension = firstExtension.isOn,
                silence = false,
                sp_mo_dao = spMoDaoExtension.isOn,
            };
            Lobby.Instance.CreateRoom(proto);
            GameManager.UIInstance.PopWindow(WinMsg.Pause);
            GameManager.UIInstance.PushWindow(WindowType.BattleQT, WinMsg.Hide);
        }

        public void OnCancelClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.Resume);
        }

    }
}


