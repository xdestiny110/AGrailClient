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
        private ToggleGroup peopleNum;

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
                max_player = 4,
                role_strategy = network.ROLE_STRATEGY.ROLE_STRATEGY_ANY,
                room_name = roomTitle.text,
                first_extension = false,
                second_extension = false,
            };
            Lobby.Instance.CreateRoom(proto);
            GameManager.UIInstance.PopWindow(WinMsg.Pause);
        }

        public void OnCancelClick()
        {
            GameManager.UIInstance.PopWindow(WinMsg.Resume);
        }

    }
}


