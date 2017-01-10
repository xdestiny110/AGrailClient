using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Framework.UI
{
    [CustomEditor(typeof(WindowFactory))]
    public class WindowFactoryEditor : Editor
    {
        private List<string> windowTypes = new List<string>();

        [MenuItem("Framework/Window Factory")]
        public static void ShowInspector()
        {
            Selection.activeObject = WindowFactory.Instance;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Window Code"))
            {
                GenerateCode();
            }
        }

        private void GenerateCode()
        {
            List<string> uiPrefabs = EditorTool.AssetPathOfUnityFolder("Resources/" + WindowFactory.Instance.WindowPrefabPath, ".prefab");
            foreach (var v in uiPrefabs)
            {
                var goPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(v);
                var go = GameObject.Instantiate(goPrefab);
                go.name = goPrefab.name;
                windowTypes.Add(go.name);
                DestroyImmediate(go);
            }
            writeWindowType();
        }

        private void writeWindowType()
        {
            using (var fw = new FileWriter(EditorTool.UnityPathToSystemPath(WindowFactory.Instance.WindowTypePath)))
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

        private void writeWindowBase()
        {

        }

    }
}


