using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.AssetBundles;

namespace Framework.AssetBundle
{
    public class AssetBundleManagerEditor : Editor
    {
        [MenuItem("Framework/AssetBundle/Options Panel")]
        static void showPanel()
        {
            AssetBundleBrowserMain.ShowWindow();
        }

        [MenuItem(AssetBundleManager.SimulateModeStr)]
        static void simulationMode()
        {
            AssetBundleManager.SimulationMode = !AssetBundleManager.SimulationMode;
        }

        [MenuItem(AssetBundleManager.SimulateModeStr, true)]
        static bool validateSimulationMode()
        {
            Menu.SetChecked(AssetBundleManager.SimulateModeStr, AssetBundleManager.SimulationMode);
            return true;
        }

        [MenuItem(AssetBundleManager.IgnoreBundleServerStr)]
        static void ignoreBundleServer()
        {
            AssetBundleManager.IgnoreBundleServer = !AssetBundleManager.IgnoreBundleServer;
        }

        [MenuItem(AssetBundleManager.IgnoreBundleServerStr, true)]
        static bool validateIgnoreBundleServer()
        {
            Menu.SetChecked(AssetBundleManager.IgnoreBundleServerStr, AssetBundleManager.IgnoreBundleServer);
            return true;
        }

        private const string textureBasePath = "Textures";
        private const string texturePrefabPath = "TexturePrefabs";
        [MenuItem("Framework/AssetBundle/Build Texture Prefab")]
        static void buildTexturePrefab()
        {            
            List<string> texturePrefabs = EditorTool.AssetPathOfUnityFolder(textureBasePath, true, ".png", ".jpg");
            foreach(var v in texturePrefabs)
            {

            }
            var texturePrefabSystemPath = EditorTool.UnityPathToSystemPath(texturePrefabPath);
            if (!Directory.Exists(texturePrefabSystemPath))
                Directory.CreateDirectory(texturePrefabSystemPath);
            EditorTool.DeleteDirContent(texturePrefabSystemPath);
        }
    }
}
