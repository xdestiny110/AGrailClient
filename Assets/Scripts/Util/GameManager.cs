using UnityEngine;
using UnityEngine.SceneManagement;
using Framework.Network;
using Framework.UI;
using System;
using Framework.AssetBundle;
using System.Collections;
using Framework.Message;

namespace AGrail
{
    public class GameManager : MonoBehaviour
    {
        public static TCP TCPInstance { private set; get; }
        public static UIManager UIInstance { private set; get; }
        public const int Version = 171203;
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
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);
            instance = this;
            DontDestroyOnLoad(this);
            lh = new Framework.Log.LogHandler();
            initTCP();
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
        public static void initTCP()
        {
            var config = new ServerConfig();
            var coder = new Coder();
            TCPInstance = new TCP(config, coder);
            instance.UpdateActions += TCPInstance.DoActions;
            TCPInstance.Connect();
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

        private float timer = 5.0f;
        public static uint heart = 0;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var topWin = UIInstance.PeekWindow();
                if(topWin != null)
                {
                    Vector3 pt = new Vector3();
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(topWin.transform as RectTransform, Input.mousePosition, Camera.main, out pt);
                    var prefab = AssetBundleManager.Instance.LoadAsset("ui", "ClickCircle");
                    var ClickCircle = Instantiate(prefab);
                    ClickCircle.name = "ClickCircle";
                    ClickCircle.transform.SetParent(instance.transform);
                    ClickCircle.transform.localPosition = pt;
                    ClickCircle.transform.localScale = new Vector3(1, 1, 1);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                var topWin = UIInstance.PeekWindow();                
                if (topWin != null)
                {
                    Vector3 pt = new Vector3();
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(topWin.transform as RectTransform, Input.mousePosition, Camera.main, out pt);
                    var prefab = AssetBundleManager.Instance.LoadAsset("ui", "Draging");
                    var Draging = Instantiate(prefab);
                    Draging.name = "Draging";
                    Draging.transform.SetParent(instance.transform);
                    Draging.transform.localPosition = pt;
                    Draging.transform.localScale = new Vector3(1, 1, 1);
                }
            }

            if (UIInstance.PeekWindowType() != WindowType.LoginBox && UIInstance.PeekWindowType() != WindowType.ReConBox)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    heart++;
                    var proto = new network.HeartBeat();
                    TCPInstance.Send(new Protobuf() { Proto = proto, ProtoID = ProtoNameIds.HEARTBEAT });
                    if (heart >= 2)
                    {
                        heart = 0;
                        UIInstance.PushWindow(WindowType.ReConBox, WinMsg.None,59);
                    }
                    timer = 5.0f;
                }
            }
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

