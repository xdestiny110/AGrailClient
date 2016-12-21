using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
namespace Editor.UIGen
{
    public class UICoderMenu
    {
        const string UIResourcePath = "Resources/UI";        

        [MenuItem("AppEditor/GenerateUICode")]
        public static void DoGenerate()
        {
            List<string> uiAssetsPath = EditorTool.AssetPathOfUnityFolder(UIResourcePath, ".prefab");
            foreach(string uiAsset in uiAssetsPath)
            {
                UICoder.Instance.Generate(uiAsset);
            }

            UIFactoryCoder.Instance.Generate(uiAssetsPath);

            AssetDatabase.Refresh();
        }
    }
}