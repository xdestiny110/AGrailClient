using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using Framework.EventSystem;
using System;

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
        private string UIAutoCodePath = "Scripts/UI/Auto";
        [SerializeField]
        private bool generateCustomCode = false;
        [SerializeField]
        private string UICustomCodePath = "Scripts/UI/Custom";

        private Dictionary<string,string> widgetPathes = new Dictionary<string, string>();

        public void GenerateCode()
        {
            List<string> uiPrefabs = EditorTool.AssetPathOfUnityFolder(UIPrefabPath, ".prefab");
            foreach(var v in uiPrefabs)
            {
                widgetPathes.Clear();
                var goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(v);
                var go = GameObject.Instantiate(goPrefab);
                realizePrefab(go.transform);
                writeCode(go.name);
            }
            AssetDatabase.Refresh();
        }

        private void realizePrefab(Transform trans, List<string> list = null)
        {
            if (list == null)
                list = new List<string>();
            list.Add(trans.name);

            var comps = trans.GetComponents<CustomUIComponent>();
            if(comps != null)
            {
                foreach(var v in comps)
                {
                    widgetPathes.Add(v.UIName, list.Aggregate((s1, s2) => { return s1 + "/" + s2; }));
                }
            }

            for (int i = 0; i < trans.childCount; i++)
                realizePrefab(trans.GetChild(i), list);

            list.RemoveAt(list.Count - 1);
        }

        private void writeCode(string clazzName)
        {
            using(var fw = new FileWriter(EditorTool.UnityPathToSystemPath(UIAutoCodePath + "/" + clazzName + ".cs")))
            {
                fw.Append("using UnityEngine;");
                fw.Append("using UnityEditor;");
                fw.Append("using UnityEngine.UI;");
                fw.Append("");
                fw.Append("namespace Framework.UI");
                fw.Append("{");
                fw.AppendFormat("    public sealed partail class {0} : Window, IEventListener", clazzName);
                fw.Append("    {");
                foreach (var v in widgetPathes)
                {
                    fw.AppendFormat("        public const string {0} = \"{1}\";", v.Key, v.Value);
                }
                fw.Append("    }");
                fw.Append("}");
            }

            if (generateCustomCode)
            {
                using (var fw = new FileWriter(EditorTool.UnityPathToSystemPath(UICustomCodePath + "/" + clazzName + ".cs")))
                {
                    fw.Append("using UnityEngine;");
                    fw.Append("using UnityEditor;");
                    fw.Append("using UnityEngine.UI;");
                    fw.Append("");
                    fw.Append("namespace Framework.UI");
                    fw.Append("{");
                    fw.AppendFormat("    public sealed partail class {0} : Window, IEventListener", clazzName);
                    fw.Append("    {");
                    fw.Append("");
                    fw.Append("        protected override void onActualShow()");
                    fw.Append("        {");
                    fw.Append("");
                    fw.Append("        }");
                    fw.Append("        protected override void onDestroy()");
                    fw.Append("        {");
                    fw.Append("");
                    fw.Append("        }");
                    fw.Append("        protected override void registEvent()");
                    fw.Append("        {");
                    fw.Append("");
                    fw.Append("        }");
                    fw.Append("        protected override void unregistEvent()");
                    fw.Append("        {");
                    fw.Append("");
                    fw.Append("        }");
                    fw.Append("        public void OnEventTrigger(EventSystem.EventType eventType, params object[] parameters)");
                    fw.Append("        {");
                    fw.Append("");
                    fw.Append("        }");
                    fw.Append("");
                    fw.Append("    }");
                    fw.Append("}");
                }
            }            
        }
    }
}