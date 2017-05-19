using UnityEngine;
using UnityEditor;

namespace Framework
{
    public class CleanPlayerPref
    {
        [MenuItem("Framework/Clean PlayerPref")]        
        public static void Clean()
        {
            PlayerPrefs.DeleteAll();
        }
        [MenuItem("Framework/Get world position")]
        public static void GetWorldPos()
        {
            var go = Selection.activeGameObject;
            Debug.LogFormat("World postion = {0}", go.transform.position);
        }
    }

}
