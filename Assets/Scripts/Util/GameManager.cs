using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Network;
using Framework.UI;
using System;

namespace AGrail
{
    public class GameManager : MonoBehaviour
    {
        public static TCP TCPInstance { private set; get; }
        public static UIManager UIInstance { private set; get; }
        public const int Version = 161001;
        private static GameManager instance;
        private event Action UpdateActions;

        public static void AddUpdateAction(Action action)
        {
            instance.UpdateActions += action;
        }

        public static void RemoveUpdateAciont(Action action)
        {
            instance.UpdateActions -= action;
        }        

        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);

            var config = new ServerConfig();
            var coder = new Coder();
            TCPInstance = new TCP(config, coder);
            UpdateActions += TCPInstance.DoActions;
            TCPInstance.Connect();

            UIInstance = new UIManager();            

            SceneManager.LoadScene(1);
        }

        void OnLevelWasLoaded(int level)
        {
            Debug.LogFormat("Level = {0}", level);
            switch (level)
            {
                case 1:
                    UIInstance.PushWindow(WindowType.LoginBox, WinMsg.None);
                    var userDataInst = UserData.Instance;
                    var roomInst = Room.Instance;
                    break;
                case 2:
                    break;
            }
        }

        void Update()
        {
            if (UpdateActions != null)
                UpdateActions();
        }
    }
}

