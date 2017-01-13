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
    }

}
