using UnityEngine;
using Framework.UI;
using Framework.Network;
using UnityEngine.UI;
using network;

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
        }

        public override void OnDestroy()
        {
            GameManager.RemoveUpdateAciont(showLoginInput);
        }

        public override void OnHide()
        {
            
        }

        public override void OnPause()
        {
            
        }

        public override void OnResume()
        {
            
        }

        public override void OnShow()
        {
            
        }

        public void Login()
        {
            var request = new LoginRequest() { asGuest = false, user_id = inptUserName.text, user_password = inptPassword.text, version = GameManager.Version };
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

