using UnityEngine;
using UnityEditor;
using Framework;

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
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = CreateInstance<WindowFactory>();
                        }
                    }
                }
                return instance;
            }
        }
        private WindowFactory() { }               
                
        public string ConfigFilePath = "Assets/Resources/UI/WindowsConfig.asset";        
        public string WindowTypePath = "Scripts/Framework/UI/WindowType.cs";        
        public string WindowPrefabPath = "UI/";

        public GameObject CreateWindows(WindowType type)
        {
            var res = Resources.Load<GameObject>(WindowPrefabPath + type.ToString());
            return Instantiate(res);
        }
    }
}

