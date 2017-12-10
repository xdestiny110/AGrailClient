using Framework.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Framework.Network;
using System.Collections;

namespace AGrail
{
    public class ReConBox : WindowsBase
	{
        [SerializeField]
        private Button ReCon;

        public override WindowType Type
        {
            get
            {
                return WindowType.ReConBox;
            }
        }

        public override void Awake()
        {
            ReCon.onClick.AddListener(delegate { onBtnReConClick(); });
            base.Awake();
        }
        private void onBtnReConClick()
        {
            ReCon.interactable = false;
            GameManager.TCPInstance.Close();
            GameManager.initTCP();
            UserData.Instance.Login(PlayerPrefs.GetString("username", "wrong"), PlayerPrefs.GetString("password", "wrong"));
            StartCoroutine(Loading());
        }

        IEnumerator Loading()
        {
            yield return new WaitForSeconds(3);
            GameManager.UIInstance.PopWindow(WinMsg.None);
            if (SceneManager.GetActiveScene().buildIndex == 2)
                SceneManager.LoadScene(1);
            else
            {
                GameManager.UIInstance.PopAllWindow();
                GameManager.UIInstance.PushWindow(WindowType.Lobby, WinMsg.None);
            }
        }
    }
}
