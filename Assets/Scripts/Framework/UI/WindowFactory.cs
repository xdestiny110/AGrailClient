using UnityEngine;
using System.Collections.Generic;
using Framework.AssetBundle;

namespace Framework.UI
{
    public sealed class WindowFactory : Singleton<WindowFactory>
    {
        public WindowFactory() { }

        public const string WindowPrefabPath = "UI";

        private Dictionary<WindowType, GameObject> goPool = new Dictionary<WindowType, GameObject>();

        public GameObject CreateWindows(WindowType type, bool isResource = false)
        {
            if (!goPool.ContainsKey(type))
            {
                if (!isResource)
                    goPool.Add(type, AssetBundleManager.Instance.LoadAsset(WindowPrefabPath, type.ToString()));
                else
                    goPool.Add(type, Resources.Load<GameObject>(WindowPrefabPath + "/" + type.ToString()));
            }
            return GameObject.Instantiate(goPool[type]);
        }
    }
}

