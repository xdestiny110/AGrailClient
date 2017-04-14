using UnityEngine;
using System.Collections;
using UnityEditor;

namespace Framework.Log
{
    [CustomPropertyDrawer(typeof(LoggerModule))]
    public class LoggerModuleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using(new EditorGUI.PropertyScope(position, label, property))
            {                
                position.height = EditorGUIUtility.singleLineHeight;
                var nameRect = new Rect(position)
                {
                    x = position.x + 30,
                };
                var flagRect = new Rect(position);

                var nameProperty = property.FindPropertyRelative("moduleName");
                var flagProperty = property.FindPropertyRelative("flag");

                nameProperty.stringValue = EditorGUI.TextField(nameRect, nameProperty.stringValue);
                flagProperty.boolValue = EditorGUI.Toggle(flagRect, flagProperty.boolValue);
            };
        }
    }
}


