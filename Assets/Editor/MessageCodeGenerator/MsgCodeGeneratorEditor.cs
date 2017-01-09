using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Message
{
    [CustomEditor(typeof(MsgCodeGenerator))]
    public class MsgCodeGeneratorEditor : Editor
    {
        private ReorderableList reorderableList;

        [MenuItem("Framework/Message Code Generate")]
        public static void generateCode()
        {
            Selection.activeObject = MsgCodeGenerator.Instance;
        }

        void OnEnable()
        {
            var prop = serializedObject.FindProperty("msgtypes");
            reorderableList = new ReorderableList(serializedObject, prop, true, true, true, true);
            reorderableList.drawHeaderCallback = (rect) => { EditorGUI.LabelField(rect, "Message Type"); };
            reorderableList.drawElementCallback = (rect, idx, isActive, isFocused) =>
            {
                var element = prop.GetArrayElementAtIndex(idx);
                rect.y += 2;
                EditorGUI.PropertyField(rect, element);
            };            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            reorderableList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();

            if(GUILayout.Button("Generate Code"))
            {
                writeCode();
            }
        }

        private void writeCode()
        {

        }

    }
}


