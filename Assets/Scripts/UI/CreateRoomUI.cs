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
                allow_guest = false,
                max_player = fourPeople.isOn ? 4 : 6,
                role_strategy = (network.ROLE_STRATEGY)(roleChoose.value + 1),
                room_name = roomTitle.text,
                password = password.text,
                seat_mode = 1,
                first_extension = false,
                second_extension = false,
                silence = false,
                sp_mo_dao = false,
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


