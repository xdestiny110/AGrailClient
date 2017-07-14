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
        private Transform titleImg;        

        public override WindowType Type
        {
            get
            {
                return WindowType.LoginBox;
            }
        }

        public override void Awake()
        {
            state = UserData.Instance.State;
            MessageSystem<MessageType>.Regist(MessageType.LoginState, this);
            base.Awake();
        }

        public override void OnDestroy()
        {
            MessageSystem<MessageType>.UnRegist(MessageType.LoginState, this);
            base.OnDestroy();
        }

        public override void OnHide()
        {
            var go = new GameObject();
            go.transform.localPosition = Vector3.zero;
            go.name = "GameTitle";
            var canvas = go.AddComponent<Canvas>();
            canvas.sortingOrder = 99;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = Camera.main;
            titleImg.SetParent(go.transform);
            titleImg.transform.DOLocalMoveY(Screen.height / 800.0f * 330, 1);
            titleImg.transform.DOScaleX(Screen.width / 1200.0f * 0.8f, 1);
            titleImg.transform.DOScaleY(Screen.height / 800.0f * 0.8f, 1);
            root.transform.DOLocalMoveX(-1280, 1).OnComplete(() => { base.OnHide(); gameObject.SetActive(false); });
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
                        if (PlayerPrefs.HasKey("username") && PlayerPrefs.HasKey("password"))
                        {
                            inptUserName.text = PlayerPrefs.GetString("username");
                            inptPassword.text = PlayerPrefs.GetString("password");
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

