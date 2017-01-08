using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.UI
{
    [InitializeOnLoad]
    public sealed class WindowFactory : ScriptableObject
    {
        private static WindowFactory instance = null;
        private static object locker = new object();
        public static WindowFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = AssetDatabase.LoadAssetAtPath<WindowFactory>(ConfigFilePath);
                    if (instance == null)
                    {
#if UNITY_EDITOR
                        instance = CreateInstance<WindowFactory>();
                        AssetDatabase.CreateAsset(instance, ConfigFilePath);
                        AssetDatabase.Refresh();
#else
                        throw new System.Exception("WindowsConfig.asset is not exist!");
#endif
                    }
                }
                return instance;
            }
        }
        private WindowFactory() { }

        public const string ConfigFilePath = "Assets/Resources/UI/WindowsConfig.asset";
        public string WindowTypePath = "Scripts/Framework/UI/WindowType.cs";
        public string WindowPrefabPath = "UI/";

        private Dictionary<WindowType, GameObject> goPool = new Dictionary<WindowType, GameObject>();

        public GameObject CreateWindows(WindowType type)
        {
            if (!goPool.ContainsKey(type))
                goPool.Add(type, Resources.Load<GameObject>(WindowPrefabPath + type.ToString()));            
            return Instantiate(goPool[type]);
        }
    }
}

