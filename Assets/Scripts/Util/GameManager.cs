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

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;

            AssetBundleManager.Instance.LoadManifestAsyn(m => { /*SceneManager.LoadScene(1);*/ } , () => {  });
            UIInstance.PushWindowFromResource(WindowType.Loading, WinMsg.None);
        }

        private int previousSceneIdx = -1;
        private void SceneManager_sceneUnloaded(Scene arg0)
        {
            previousSceneIdx = arg0.buildIndex;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Debug.LogFormat("From Level {0} to level {1}", previousSceneIdx, arg0.buildIndex);
            switch (arg0.buildIndex)
            {
                case 1:
                    UIInstance.ClearAllWindow();
                    if (previousSceneIdx == 2)
                        UIInstance.PushWindow(WindowType.Lobby, WinMsg.None);
                    else
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

