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
        private Toggle sixPeople;
        [SerializeField]
        private Toggle CmMode;
        [SerializeField]
        private Toggle bpMode;
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

            CmMode.interactable = UserData.Instance.IsVIP;
            bpMode.interactable = UserData.Instance.IsVIP;          

            base.Awake();
        }

        private void Start()
        {
            roomTitle.text= RandomRoomText();
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
        public void OnCmToggled()
        {
            if(CmMode.isOn)
            {
                seatCM.isOn = true;
                sixPeople.isOn = true;
                fourPeople.interactable = false;
            }
            else
            {
                fourPeople.interactable = true;
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

        private string RandomRoomText()
        {
            string[] rs = new string[]
            {
                "友谊才是第一呀",
                "脸不行就先洗洗",
                "要友善对待新人哟",
                "新速查手册好多攻略呀！",
                "官网竟然是codifygrail.cn！",
                "玩了这么久，写点攻略呗",
                "mvp真的不选我吗",
                "战绩系统要推出啦！",
                "Q群184705348欢迎你"
            };

            var ridx = UnityEngine.Random.Range(0, 9);
            return rs[ridx];
        }
    }
}


