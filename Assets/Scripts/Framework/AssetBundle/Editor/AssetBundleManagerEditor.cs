using System.Collections;
using System.Collections.Generic;
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

        [MenuItem("Framework/AssetBundle/Simulation Mode")]
        static void simulationMode()
        {
            AssetBundleManager.SimulationMode = !AssetBundleManager.SimulationMode;
        }

        [MenuItem("Framework/AssetBundle/Simulation Mode", true)]
        static bool validateSimulationMode()
        {
            Menu.SetChecked("Framework/AssetBundle/Simulation Mode", AssetBundleManager.SimulationMode);
            return true;
        }


    }
}
