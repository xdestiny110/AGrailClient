using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace Framework.Log
{
    [CustomEditor(typeof(LoggerModuleGenerator))]
    public class LoggerModuleGeneratorEditor : Editor
    {
        private ReorderableList moduleList, frameworkModuleList;
        private bool foldModuleList, foldFrameworkModuleList;

        [MenuItem("Framework/Logger Module")]
        public static void generateCode()
        {
            Selection.activeObject = LoggerModuleGenerator.Instance;
        }
        
        void OnEnable()
        {
            buildReorderableList(ref frameworkModuleList, "frameworkLoggerModules", "Framework Logger Module");
            buildReorderableList(ref moduleList, "loggerModules", "Logger Module");            
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            foldModuleList = EditorGUILayout.Foldout(foldModuleList, "User Define Logger Type");
            if (foldModuleList)
                moduleList.DoLayoutList();
            foldFrameworkModuleList = EditorGUILayout.Foldout(foldFrameworkModuleList, "Framework Logger Type");
            if (foldFrameworkModuleList)
                frameworkModuleList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if(GUILayout.Button("Generate Logger Module Type"))
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
                fw.Append("using System;");
                fw.Append("");
                fw.Append("namespace Framework.Log");
                fw.Append("{");
                fw.Append("    [Serializable]");
                fw.Append("    public class LoggerModule");
                fw.Append("    {");
                fw.Append("        public string moduleName;");
                fw.Append("        public bool flag = true;");
                fw.Append("    }");
                fw.Append("");
                fw.Append("    public enum LoggerModuleType");
                fw.Append("    {");
                fw.Append("        //Framework");
                foreach(var v in LoggerModuleGenerator.Instance.frameworkLoggerModules)
                {
                    fw.Append("        " + v.moduleName + ",");
                }
                fw.Append("");
                fw.Append("        //Custom");
                foreach(var v in LoggerModuleGenerator.Instance.loggerModules)
                {
                    fw.Append("        " + v.moduleName + ",");
                }
                fw.Append("    }");
                fw.Append("}");
            }
        }

    }
}


