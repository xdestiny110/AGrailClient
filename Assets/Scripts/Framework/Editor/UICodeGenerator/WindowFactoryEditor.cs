using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Framework.UI
{
    [CustomEditor(typeof(WindowFactory))]
    public class WindowFactoryEditor : Editor
    {
        private static List<string> windowTypes = new List<string>();        
        private const string WindowTypePath = "Scripts/Framework/UI/WindowType.cs";
        private const string AutoUIPath = "Scripts/UI/";

        private const string customNamespace = "AGrail";

        [MenuItem("Framework/Window Factory")]        
        public static void GenerateCode()
        {
            List<string> uiPrefabs = EditorTool.AssetPathOfUnityFolder("Prefabs/" + WindowFactory.WindowPrefabPath, false, ".prefab");
            foreach (var v in uiPrefabs)
            {
                var goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(v);
                windowTypes.Add(goPrefab.name);
                //writeWindowBase(goPrefab.name);
            }
            uiPrefabs = EditorTool.AssetPathOfUnityFolder("Resources/" + WindowFactory.WindowPrefabPath, false, ".prefab");
            foreach (var v in uiPrefabs)
            {
                var goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(v);
                windowTypes.Add(goPrefab.name);
                //writeWindowBase(goPrefab.name);
            }
            writeWindowType();
        }

        private static void writeWindowType()
        {
            using (var fw = new FileWriter(EditorTool.UnityPathToSystemPath(WindowTypePath)))
            {
                fw.Append("namespace Framework.UI");
                fw.Append("{");
                fw.Append("    public enum WindowType");
                fw.Append("    {");
                fw.Append("        None = 0,");
                foreach (var v in windowTypes)
                {
                    fw.AppendFormat("        {0},", v);
                }
                fw.Append("    }");
                fw.Append("}");
            }
        }

        private static void writeWindowBase(string name)
        {
            if (File.Exists(EditorTool.UnityPathToSystemPath(AutoUIPath + name)))
                return;
            using (var fw = new FileWriter(EditorTool.UnityPathToSystemPath(AutoUIPath + name)))
            {
                fw.Append("using Framework.UI;");
                fw.Append("using Framework.Message;");
                fw.Append("using UnityEngine;");
                fw.Append("using UnityEngine.UI;");
                fw.Append("using DG.Tweening;");
            }
        }
    }
}


