using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UScene
{
    public class SceneProxy
    {
        private static object locker = new object();
        private static SceneProxy instance;
        private SceneProxy() { }
        public static SceneProxy Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                            instance = new SceneProxy();
                    }
                }
                return instance;
            }
        }


        Dictionary<SceneDefine.SceneType, List<System.Action>> sceneActions = new Dictionary<SceneDefine.SceneType, List<System.Action>>();

        public void AddProxy(SceneDefine.SceneType type, System.Action action)
        {
            List<System.Action> actions = null;
            if(!sceneActions.TryGetValue(type, out actions))
            {
                actions = new List<System.Action>();
                sceneActions.Add(type, actions);
            }
            if(!actions.Contains(action))
            {
                actions.Add(action);
            }
        }

        public void RemoveProxy(SceneDefine.SceneType type, System.Action action)
        {
            List<System.Action> actions = null;
            if(sceneActions.TryGetValue(type, out actions))
            {
                if(actions.Contains(action))
                {
                    actions.Remove(action);
                }
            }
        }

        public void Init()
        {
            Module.MonoRoot.Instance.AddLevelAction(OnLevelChange);
            //这些回调看以后是否需要，可以由外部模块直接注册
            //AddProxy(SceneDefine.SceneType.Room, OnVideoRoomLevelLoaded);
            //AddProxy(SceneDefine.SceneType.Lobby, OnLobbyLoaded);
            //AddProxy(SceneDefine.SceneType.Login, OnSquareLoaded);
        }

        private void OnLevelChange()
        {
            SceneDefine.SceneType type = SceneDefine.CurrentSceneType();
            InitWindow(type);
            SceneMgr.Instance.OnLevelLoadDone(type);
            List<System.Action> actions = null;
            if(sceneActions.TryGetValue(type, out actions))
            {
                actions.ForEach(ac => ac());
            }
        }

        void OnSquareLoaded()
        {

        }

        void OnLobbyLoaded()
        {

        }

        void OnVideoRoomLevelLoaded()
        {
            //加载资源 并配置资源 
        }

        void InitWindow(SceneDefine.SceneType type)
        {
            UI.WinStack loginStack = null;
            InitWinStack(type, out loginStack);
            if (loginStack != null)
            {
                ShowStackStartWin(loginStack);
            }
            else
            {
                ShowStackStartWin(null);
            }
        }

        void ShowStackStartWin(UI.WinStack excludeStack)
        {
            foreach (UI.WinStack stack in UI.WinStack.all.Values)
            {
                if (stack != excludeStack)
                {
                    UI.WinStack.Show(stack.identity, stack.startWinName, UI.WinMsg.Null);
                }
            }
        }

        //记录下登陆界面所在的stack
        void InitWinStack(SceneDefine.SceneType type,  out UI.WinStack loginStack)
        {
            loginStack = null;
            UI.WinStack.Destroy();
            foreach (Common.Config.WinStackAttrib attrib in Common.Config.WinStackAttrib.all)
            {
                SceneDefine.SceneType configType = SceneDefine.SceneNameToType(attrib.SceneName);
                if (configType == type)
                {
                    UI.WinStack.AddStack(attrib.Identity, attrib.Position, attrib.Rotation, attrib.scale, attrib.StartWinName, attrib.group);
                    if (attrib.StartWinName == UI.WinFactory.LOGIN)
                    {
                        loginStack = UI.WinStack.GetStack(attrib.Identity);
                    }
                }
            }
        }
    }
}