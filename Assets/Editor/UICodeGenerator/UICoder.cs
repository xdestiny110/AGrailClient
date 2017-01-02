using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

namespace Framework.UI
{
    [InitializeOnLoad]
    public sealed class UICoder : ScriptableObject
    {
        private static UICoder instance = null;
        private static object locker = new object();
        public static UICoder Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = CreateInstance<UICoder>();
                        }
                    }
                }
                return instance;
            }
        }
        private UICoder() { }

        [ReadOnly]
        public string ConfigFilePath = "Assets/Editor/UICodeGenerator/Config.asset";
        [SerializeField]
        private string UIPrefabPath = "Resources/UI";
        [SerializeField]
        private string UICodePath = "Scripts/UI/Auto";
        [SerializeField]
        private List<UIBehaviour> Widgets = new List<UIBehaviour>();

        private Dictionary<string,string> widgetPathes = new Dictionary<string, string>();

        public void GenerateCode()
        {
            widgetPathes.Clear();

            List<string> uiPrefabs = EditorTool.AssetPathOfUnityFolder(UIPrefabPath, ".prefab");
            foreach(var v in uiPrefabs)
            {
                var goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(v);
                var go = GameObject.Instantiate(goPrefab);
                realizePrefab(go.transform);
            }
            AssetDatabase.Refresh();
        }

        private void realizePrefab(Transform trans, List<string> list = null)
        {
            if (list == null)
                list = new List<string>();
            list.Add(trans.name);

            foreach(var v in Widgets)
            {
                if (trans.GetComponent(v.GetType()) != null){
                    widgetPathes.Add(trans.name + v.GetType().ToString(), list.Aggregate((s1, s2) => { return s1 + "/" + s2; }));
                }
            }

            for (int i = 0; i < trans.childCount; i++)
                realizePrefab(trans.GetChild(i), list);

            list.RemoveAt(list.Count - 1);
        }
    }
}