using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AssetBundles;

namespace Framework.AssetBundle
{
    public class AssetBundleManagerEditor : Editor
    {
        [MenuItem("Framework/AssetBundle")]
        static void ShowPanel()
        {
            AssetBundleBrowserMain.ShowWindow();
        }
    }
}
