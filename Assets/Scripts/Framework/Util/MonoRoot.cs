using Framework.AssetBundle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using Framework.Log;

namespace Framework
{
    public class MonoRoot : MonoBehaviour
    {
        public static MonoRoot Instance { get; private set; }
        public static LuaEnv luaEnv = new LuaEnv();

        private List<Action> updateAction = new List<Action>();
        private List<Action> onceAction = new List<Action>();
        private List<Timer> timers = new List<Timer>();

        [SerializeField]
        private bool isUseLogHandler = true;
        private LogHandler logHandler = null;

        public bool AddUpdateAction(Action ac)
        {
            if (updateAction.Contains(ac))
                return false;
            updateAction.Add(ac);
            return true;
        }

        public bool RemoveUpdateAction(Action ac)
        {
            if (!updateAction.Contains(ac))
                return false;
            updateAction.Remove(ac);
            return true;
        }

        public bool AddOnceAction(Action ac)
        {
            if (onceAction.Contains(ac))
                return false;
            onceAction.Add(ac);
            return true;
        }

        public bool RemoveOnceAction(Action ac)
        {
            if (!onceAction.Contains(ac))
                return false;
            onceAction.Remove(ac);
            return true;
        }

        public void AddTimer(Timer timer)
        {
            timers.Add(timer);
        }

        public void RemoveTimer(Timer timer)
        {
            timers.Remove(timer);
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);

            if(isUseLogHandler)
                logHandler = new LogHandler();

            luaEnv.AddBuildin("luapb", XLua.LuaDLL.Lua.LoadLuaProtobuf);
            luaEnv.AddLoader((ref string filename) => {
                var asset = AssetBundleManager.Instance.LoadAsset<TextAsset>("lua_ui", filename);
                if(asset == null)
                    asset = AssetBundleManager.Instance.LoadAsset<TextAsset>("lua_logic", filename);
                if (asset == null)
                    asset = AssetBundleManager.Instance.LoadAsset<TextAsset>("lua_data", filename);
                if (asset == null)
                    asset = AssetBundleManager.Instance.LoadAsset<TextAsset>("lua_network", filename);
                if (asset == null)
                    asset = AssetBundleManager.Instance.LoadAsset<TextAsset>("lua_util", filename);
                if(asset != null && !string.IsNullOrEmpty(asset.text))
                    return asset.bytes;
                Debug.LogErrorFormat("There is no lua script {0}. Maybe read from resources.", filename);
                return null;
            });
        }

        void Update()
        {
            updateAction.ForEach(ac => ac());

            onceAction.ForEach(ac => ac());
            onceAction.Clear();

            for(int i = 0; i < timers.Count;)
            {
                var timeNow = timers[i].IsRealTime ? DateTime.Now.GetMiliSecFrom1970() : Time.realtimeSinceStartup * 1000;
                if(timeNow - timers[i].RegistTime > timers[i].DelayTime)
                {
                    timers[i].Exec();
                    timers.RemoveAt(i);
                    continue;
                }
                i++;
            }
        }

        void OnDestroy()
        {
            if(logHandler != null)
                logHandler.Close();
        }
    }
}

