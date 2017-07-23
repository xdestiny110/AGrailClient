using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Network;
using Framework.UI;
using System;
using Framework.AssetBundle;

namespace AGrail
{
    public class GameManager : MonoBehaviour
    {
        public static TCP TCPInstance { private set; get; }
        public static UIManager UIInstance { private set; get; }
        public const int Version = 161001;
        private static GameManager instance;
        private Framework.Log.LogHandler lh;
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

            lh = new Framework.Log.LogHandler();
            
            var config = new ServerConfig();
            var coder = new Coder();
            TCPInstance = new TCP(config, coder);
            UpdateActions += TCPInstance.DoActions;
            TCPInstance.Connect();

            UIInstance = new UIManager();
            var userDataInst = UserData.Instance;
            var roomInst = Lobby.Instance;
            var battleInst = BattleData.Instance;
            var dialogInst = Dialog.Instance;

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            AssetBundleManager.Instance.LoadManifestAsyn(m => { /*SceneManager.LoadScene(1);*/ } , () => {  });
            UIInstance.PushWindowFromResource(WindowType.Loading, WinMsg.None);
        }

        void OnLevelWasLoaded(int level)
        {
            Debug.LogFormat("Level = {0}", level);
            switch (level)
            {
                case 1:
                    UIInstance.ClearAllWindow();
                    UIInstance.PushWindow(WindowType.LoginBox, WinMsg.None);
                    break;
                case 2:
                    UIInstance.ClearAllWindow();
                    UIInstance.PushWindow(WindowType.BattleUIMobile, WinMsg.None);
                    break;
            }
        }

        void Update()
        {
            if (UpdateActions != null)
                UpdateActions();
        }

        void OnApplicationQuit()
        {
            TCPInstance.Close();
            lh.Close();
        }
    }
}

