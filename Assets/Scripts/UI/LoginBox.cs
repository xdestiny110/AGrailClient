using UnityEngine;
using Framework.UI;
using Framework.Message;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace AGrail
{
    public class LoginBox : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private GameObject btnStart;
        [SerializeField]
        private GameObject loginInput;
        [SerializeField]
        private InputField inptUserName;
        [SerializeField]
        private InputField inptPassword;
        [SerializeField]
        private Text txtStatus;
        [SerializeField]
        private Button btnLogin;
        [SerializeField]
        private Button btnSwitchAccount;
        [SerializeField]
        private Transform titleImg;
        [SerializeField]
        private Text txtVersion;

        public override WindowType Type
        {
            get
            {
                return WindowType.LoginBox;
            }
        }

        public override void Awake()
        {
            txtVersion.text = "Ver." + Application.version;
            state = UserData.Instance.State;
            MessageSystem<MessageType>.Regist(MessageType.LoginState, this);
            btnSwitchAccount.onClick.AddListener(() =>
            {
                btnStart.SetActive(false);
                loginInput.SetActive(true);
            });
            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.LoginState, this);
            base.OnDestroy();
        }

        public override void OnHide()
        {
            root.transform.DOLocalMoveX(-1280, 1.0f);
        }

        public override void OnShow()
        {
            root.transform.localPosition = new Vector3(-1280, 0, 0);
            root.transform.DOLocalMoveX(0, 1.0f);
            base.OnShow();
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.LoginState:
                    state = UserData.Instance.State;
                    break;
            }
        }

        public void Login()
        {
            UserData.Instance.Login(inptUserName.text, inptPassword.text);
        }

        public void OnBtnStartClick()
        {
            if ((PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password")))
                Login();
            else
            {
                btnStart.SetActive(false);
                loginInput.SetActive(true);
            }
        }

        private LoginState _state;
        private LoginState state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                switch (value)
                {
                    case LoginState.Prepare:
                        txtStatus.text = "连接服务器...";
                        break;
                    case LoginState.Ready:
                        txtStatus.text = "";
                        btnStart.SetActive(true);
                        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
                        {
                            inptUserName.text = PlayerPrefs.GetString("username");
                            inptPassword.text = PlayerPrefs.GetString("password");
                            btnSwitchAccount.gameObject.SetActive(true);
                        }
                        break;
                    case LoginState.Update:
                        txtStatus.text = "有新版本啦~快去下载吧";
                        break;
                    case LoginState.Forbidden:
                        txtStatus.text = "账号被封禁";
                        break;
                    case LoginState.Wrong:
                        txtStatus.text = "账号密码错误";
                        break;
                    case LoginState.Logining:
                        txtStatus.text = "登录中...";
                        break;
                    case LoginState.Success:
                        PlayerPrefs.SetString("username", inptUserName.text);
                        PlayerPrefs.SetString("password", inptPassword.text);
                        GameManager.UIInstance.PushWindow(WindowType.Lobby, WinMsg.Hide);
                        break;
                }
            }
        }
    }

}

