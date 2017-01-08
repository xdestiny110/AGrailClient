using UnityEngine;
using Framework.UI;
using Framework.Network;
using Framework.Message;
using UnityEngine.UI;
using DG.Tweening;

namespace AGrail
{
    public class LoginBox : WindowsBase
    {
        [SerializeField]
        private GameObject waitAnyClick;
        [SerializeField]
        private GameObject loginInput;
        [SerializeField]
        private InputField inptUserName;
        [SerializeField]
        private InputField inptPassword;
        [SerializeField]
        private Button btnLogin;

        public override void Awake()
        {
            GameManager.AddUpdateAction(showLoginInput);
            MessageSystem.Regist(MessageType.Protobuf, this);
        }

        public override void OnDestroy()
        {
            GameManager.RemoveUpdateAciont(showLoginInput);
        }

        public override void OnHide()
        {
            transform.DOMoveX(-800, 1.0f).OnComplete(()=> { gameObject.SetActive(false); });
        }

        public override void OnShow()
        {
            transform.DOMoveX(800, 1.0f).OnComplete(() => { gameObject.SetActive(false); });
        }

        public override void OnPause()
        {
            
        }

        public override void OnResume()
        {
            
        }

        public override void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            switch (eventType)
            {
                case MessageType.Protobuf:
                    var proto = (Protobuf)parameters[0];
                    switch (proto.ProtoID)
                    {
                        case ProtoNameIds.LOGINRESPONSE:                            
                            break;
                    }
                    break;
            }
        }

        public void Login()
        {
            var request = new network.LoginRequest() { asGuest = false, user_id = inptUserName.text, user_password = inptPassword.text, version = GameManager.Version };
            GameManager.TCPInstance.Send(new Protobuf() { Proto = request, ProtoID = ProtoNameIds.LOGINREQUEST });
        }

        private void showLoginInput()
        {
            if (Input.anyKeyDown && waitAnyClick.activeSelf)
            {
                waitAnyClick.SetActive(false);
                loginInput.SetActive(true);
            }
        }
    }

}

