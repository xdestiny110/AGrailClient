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

        public GameObject CreateWindows(WindowType type)
        {
            if (!goPool.ContainsKey(type))
                goPool.Add(type, AssetBundleManager.Instance.LoadAsset(WindowPrefabPath, type.ToString()));
            return GameObject.Instantiate(goPool[type]);
        }
    }
}

