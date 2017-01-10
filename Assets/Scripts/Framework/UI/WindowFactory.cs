using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.UI
{
    [InitializeOnLoad]
    public sealed class WindowFactory : Singleton<WindowFactory>
    {
        public WindowFactory() { }

        public const string WindowPrefabPath = "UI/";

        private Dictionary<WindowType, GameObject> goPool = new Dictionary<WindowType, GameObject>();

        public GameObject CreateWindows(WindowType type)
        {
            if (!goPool.ContainsKey(type))
                goPool.Add(type, Resources.Load<GameObject>(WindowPrefabPath + type.ToString()));            
            return GameObject.Instantiate(goPool[type]);
        }
    }
}

