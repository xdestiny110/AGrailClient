using UnityEngine;
using Framework.Message;
using XLua;
using System;
using Framework.AssetBundle;
using System.Collections.Generic;
using System.Collections;

namespace Framework.UI
{
    [LuaCallCSharp, RequireComponent(typeof(Canvas)), RequireComponent(typeof(CanvasGroup))]
    public class UIBase : MonoBehaviour, IMessageListener, IMessageListener<MessageType>
    {
        public virtual string Type { get; protected set; }
        [HideInInspector]
        public CanvasGroup CanvasGroup = null;
        [HideInInspector]
        public Canvas Canvas = null;

        private LuaTable scriptEnv;
        private TextAsset luaScript;
        private Action luaDestroy;
        private Action luaOnShow;
        private Action luaOnHide;
        private Action luaOnPause;
        private Action luaOnResume;
        private Action<string, object[]> luaOnEventTriiger;

        private object[] parameters;
        public virtual object[] Parameters
        {
            set
            {
                parameters = value;
            }
            get
            {
                return parameters;
            }
        }

        public virtual void OnShow()
        {
            MessageSystem.Notify("OnUIShow", this);
            if (luaOnShow != null) luaOnShow();
        }

        public virtual void OnHide()
        {
            MessageSystem.Notify("OnUIHide", this);
            if (luaOnHide != null) luaOnHide();
        }

        public virtual void OnPause()
        {
            MessageSystem.Notify("OnUIPause", this);
            if (luaOnPause != null) luaOnPause();
        }

        public virtual void OnResume()
        {
            MessageSystem.Notify("OnUIResume", this);
            if (luaOnResume != null) luaOnResume();
        }

        public virtual void Awake()
        {
            MessageSystem.Notify("OnUICreate", this);
            CanvasGroup = GetComponent<CanvasGroup>();
            Canvas = GetComponent<Canvas>();
            Canvas.renderMode = (Canvas.renderMode != RenderMode.WorldSpace) ?
                RenderMode.ScreenSpaceCamera : RenderMode.WorldSpace;
            Canvas.worldCamera = Camera.main;

            var idx = gameObject.name.IndexOf(" ");
            if (idx > 0)
                gameObject.name = gameObject.name.Substring(0, idx);
        }

        public virtual IEnumerator Start()
        {
            while (AssetBundleManager.Instance.Progress < 1)
                yield return null;

            var op = AssetBundleManager.Instance.LoadAssetAsyn<TextAsset>("lua_ui", gameObject.name);
            yield return op;
            luaScript = op.Asset;
            if (luaScript != null)
            {
                Debug.LogFormat("load lua ui '{0}' script", gameObject.name);
                Type = gameObject.name;
                scriptEnv = MonoRoot.luaEnv.NewTable();
                LuaTable meta = MonoRoot.luaEnv.NewTable();
                meta.Set("__index", MonoRoot.luaEnv.Global);
                scriptEnv.SetMetaTable(meta);
                meta.Dispose();
                scriptEnv.Set("self", this);
                MonoRoot.luaEnv.DoString(luaScript.text, luaScript.name, scriptEnv);
                luaDestroy = scriptEnv.Get<Action>("destroy");
                luaOnShow = scriptEnv.Get<Action>("onShow");
                luaOnHide = scriptEnv.Get<Action>("onHide");
                luaOnPause = scriptEnv.Get<Action>("onPause");
                luaOnResume = scriptEnv.Get<Action>("onResume");
                luaOnEventTriiger = scriptEnv.Get<Action<string, object[]>>("onEventTriiger");
                var luaStart = scriptEnv.Get<Action>("start");
                if (luaStart != null) luaStart();
            }
            else
                Debug.LogErrorFormat("Can not find lua script on {0}", gameObject.name);
        }

        public virtual void OnDestroy()
        {
            MessageSystem.Notify("OnUIDestroy", this);
            if (luaDestroy != null) luaDestroy();
        }

        public virtual void OnEventTrigger(string eventType, params object[] parameters)
        {
            if (luaOnEventTriiger != null) luaOnEventTriiger(eventType, parameters);
            if(Array.Exists(Enum.GetNames(typeof(MessageType)), (s)=> { return s.Equals(eventType); }))
            {
                OnEventTrigger((MessageType)Enum.Parse(typeof(MessageType), eventType));
            }
        }

        public virtual void OnEventTrigger(MessageType eventType, params object[] parameters)
        {
            
        }
    }
}
