using UnityEngine;
using Framework.UI;
using UnityEngine.UI;
using System.Linq;
using System;
using DG.Tweening;

namespace AGrail
{
    public class CreateRoomUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
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
        [SerializeField]
        private Toggle seat3Combo;
        [SerializeField]
        private Toggle seatCM;
        [SerializeField]
        private Toggle seat2Combo;

        public override WindowType Type
        {
            get
            {
                return WindowType.CreateRoomUI;
            }
        }

        public override void Awake()
        {
            root.localPosition = new Vector3(1280, 0, 0);
            root.DOLocalMoveX(0, 1);

            base.Awake();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void On4PToggled()
        {
            if (fourPeople.isOn)
            { 
            seat3Combo.interactable = false;
            seatCM.interactable = false;
                if (seat3Combo.isOn || seatCM.isOn)
                    seat2Combo.isOn = true;
            }
            else
            {
                seat3Combo.interactable = true;
                seatCM.interactable = true;
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


