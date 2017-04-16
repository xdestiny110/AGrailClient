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
        private GameObject waitAnyClick;
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
            GameManager.AddUpdateAction(showLoginInput);
            state = UserData.Instance.State;
            MessageSystem<MessageType>.Regist(MessageType.LoginState, this);
            base.Awake();
        }

        public override void OnDestroy()
        {
            GameManager.RemoveUpdateAciont(showLoginInput);
            MessageSystem<MessageType>.UnRegist(MessageType.LoginState, this);
            base.OnDestroy();
        }

        public override void OnHide()
        {
            var go = new GameObject();
            go.name = "GameTitle";
            var canvas = go.AddComponent<Canvas>();
            canvas.sortingOrder = 99;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            titleImg.SetParent(go.transform);
            titleImg.transform.DOLocalMoveY(330, 1);
            titleImg.transform.DOScaleX(0.8f, 1);
            titleImg.transform.DOScaleY(0.8f, 1);
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

        private void showLoginInput()
        {
            if (Input.anyKeyDown && waitAnyClick.activeSelf)
            {
                waitAnyClick.SetActive(false);
                loginInput.SetActive(true);
            }
        }

        private LoginState state
        {
            set
            {
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

