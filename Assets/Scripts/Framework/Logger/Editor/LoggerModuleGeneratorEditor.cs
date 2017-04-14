using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace Framework.Log
{
    [CustomEditor(typeof(LoggerModuleGenerator))]
    public class LoggerModuleGeneratorEditor : Editor
    {
        private ReorderableList moduleList;
        private bool foldModuleList;

        [MenuItem("Framework/Logger Module")]
        public static void generateCode()
        {
            Selection.activeObject = LoggerModuleGenerator.Instance;
        }

        void OnEnable()
        {
            buildReorderableList(ref moduleList, "loggerModules", "Logger Module");            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            foldModuleList = EditorGUILayout.Foldout(foldModuleList, "Const Message Type");
            if (foldModuleList)
                moduleList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Generate Code"))
            {
                writeCode();
            }
        }

        private void buildReorderableList(ref ReorderableList list, string propName, string header, bool enable = true)
        {
            var prop = serializedObject.FindProperty(propName);
            list = new ReorderableList(serializedObject, prop, true, true, enable, enable);
            var p = list.serializedProperty;
            list.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, header); };            
            list.drawElementCallback = (rect, idx, isActive, isFocused) =>
            {
                var element = prop.GetArrayElementAtIndex(idx);
                rect.y += 2;
                if (!enable)
                    GUI.enabled = false;                
                EditorGUI.PropertyField(rect, element, GUIContent.none);
                if (!enable)
                    GUI.enabled = true;
            };
        }

        private void writeCode()
        {
            using (var fw = new FileWriter(EditorTool.UnityPathToSystemPath(LoggerModuleGenerator.LoggerModulePath)))
            {
                fw.Append("namespace Framework.Message");
                fw.Append("{");
                fw.Append("    public enum MessageType");
                fw.Append("    {");
                fw.Append("        // Logger Module");
                //foreach (var v in MsgCodeGenerator.Instance.msgTypesConst)
                //    fw.Append("        " + v.ToString() + ",");
                fw.Append("    }");
                fw.Append("}");
            }
        }

    }
}


