using UnityEditor;
using UnityEngine;

namespace Framework.UI
{
    [CustomEditor(typeof(UICoder))]
    public class UICoderEditor : Editor
    {
        [MenuItem("Framework/Generate UI Code")]
        public static void Edit()
        {
            Selection.activeObject = UICoder.Instance;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Generate UI code"))
            {                
                Caching.CleanCache();
                AssetDatabase.CreateAsset(UICoder.Instance, UICoder.Instance.ConfigFilePath);
            }
        }
    }
}