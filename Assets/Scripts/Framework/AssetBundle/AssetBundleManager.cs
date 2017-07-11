using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace Framework.AssetBundle
{
    public class AssetBundleManager : MonoBehaviour
    {
        private static AssetBundleManager instance = null;
        public static AssetBundleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("AssetBundleMgr");
                    instance = go.AddComponent<AssetBundleManager>();
                    DontDestroyOnLoad(go);
                    instance.init();                    
                }
                return instance;
            }
        }

        public const string SimulateModeStr = "Framework/AssetBundle/Simulation Mode";
        private static bool? simulationMode = null;
        public static bool SimulationMode
        {
            get
#if !UNITY_EDITOR
            {
                return false;
            }
#else
            {
            if (simulationMode == null)
                    simulationMode = UnityEditor.EditorPrefs.GetBool(SimulateModeStr, true);
                return simulationMode.Value;
            }
            set
            {
                simulationMode = value;
                UnityEditor.EditorPrefs.SetBool(SimulateModeStr, value);
            }
#endif
        }

        public const string IgnoreBundleServerStr = "Framework/AssetBundle/Ignore Bundle Server";
        private static bool? ignoreBundleServer = null;
        public static bool IgnoreBundleServer
        {
            get
#if !UNITY_EDITOR
            {
                return false;
            }
#else
            {
                if (ignoreBundleServer == null)
                    ignoreBundleServer = UnityEditor.EditorPrefs.GetBool(IgnoreBundleServerStr, true);
                return ignoreBundleServer.Value;
            }
            set
            {
                ignoreBundleServer = value;
                UnityEditor.EditorPrefs.SetBool(IgnoreBundleServerStr, value);
            }
#endif
        }

        private string manifestFileName
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString();
#else
                return Application.platform.ToString();
#endif
            }
        }

        private const string remoteSrv = "http://10.0.2.114:7888/";
        private AssetBundleManifest localManifest = null;
        private AssetBundleManifest remoteManifest = null;
        private Dictionary<string, UnityEngine.AssetBundle> bundles = new Dictionary<string, UnityEngine.AssetBundle>();


        void init()
        {            
            Debug.LogFormat("SimulationMode = {0}", SimulationMode);
            Caching.maximumAvailableDiskSpace = 200 * 1024 * 1024;
        }

        public void LoadManifestAsyn(LoadManifestCB cb)
        {
            StartCoroutine(LoadManifestCoro(cb));
        }

        public IEnumerator LoadManifestCoro(LoadManifestCB cb)
        {
            if (SimulationMode)
            {
                cb.Cb(null);
                yield break;
            }

            UnityWebRequest oldWww = UnityWebRequest.GetAssetBundle(Application.streamingAssetsPath + "/" + manifestFileName);
            yield return oldWww.Send();
            if (!oldWww.isError)
                localManifest = (oldWww.downloadHandler as DownloadHandlerAssetBundle).assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            else
                Debug.LogError("local manifest is null!");

            if (!IgnoreBundleServer)
            {
                UnityWebRequest newWww = UnityWebRequest.GetAssetBundle(remoteSrv + manifestFileName + "/" + manifestFileName);
                yield return newWww.Send();
                if (!newWww.isError)
                    remoteManifest = (newWww.downloadHandler as DownloadHandlerAssetBundle).assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                else
                    Debug.LogError("remote manifest is null!");
                if (SimulationMode)
                    yield break;
            }

            //两个Manifest进行比较
            //若有不同则针对不同进行下载
            //这里相当于直接加载及解压了所有AssetBundles
            if(remoteManifest != null)
            {
                foreach (var v in remoteManifest.GetAllAssetBundles())
                    yield return StartCoroutine(downloadAssetBundle(v));
                cb.Cb(remoteManifest);
            }
            else
            {
                foreach (var v in localManifest.GetAllAssetBundles())
                    yield return StartCoroutine(downloadAssetBundle(v));
                cb.Cb(localManifest);
            }
        }

        public GameObject LoadAsset(string assetbundleName, string assetName)
        {
            assetbundleName = assetbundleName.ToLower();
#if UNITY_EDITOR
            if (SimulationMode)
            {
                var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetbundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
                    return null;
                }
                return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPaths[0]);
            }
            else if (bundles.ContainsKey(assetbundleName))
                return bundles[assetbundleName].LoadAsset<GameObject>(assetName);
            else
            {
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
                return null;
            }
#else
            if (bundles.ContainsKey(assetbundleName))
                return bundles[assetbundleName].LoadAsset<GameObject>(assetName);
            else
            {
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
                return null;
            }
#endif
        }

        public void LoadAssetAsyn(string assetbundleName, string assetName, LoadAssetCB cb)
        {
            assetbundleName = assetbundleName.ToLower();
#if UNITY_EDITOR
            if (SimulationMode)
                LoadAsset(assetbundleName, assetbundleName);
            else
                StartCoroutine(LoadAssetAsynCoro(assetbundleName, assetName, cb));
#else
            StartCoroutine(LoadAssetAsynCoro(assetbundleName, assetName, cb));
#endif
        }

        public IEnumerator LoadAssetAsynCoro(string assetbundleName, string assetName, LoadAssetCB cb)
        {
            assetbundleName = assetbundleName.ToLower();
            if (bundles.ContainsKey(assetbundleName))
            {
                var req = bundles[assetbundleName].LoadAssetAsync<GameObject>(assetName);
                yield return req;
                cb.Cb(req.asset as GameObject);
            }
            else
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
        }

        private IEnumerator downloadAssetBundle(string bundleName)
        {
            string uri = "";
            UnityWebRequest www = null;
            AssetBundleManifest manifest = null;
            if (remoteManifest == null || (localManifest.GetAllAssetBundles().Contains(bundleName) &&
                remoteManifest.GetAssetBundleHash(bundleName) == localManifest.GetAssetBundleHash(bundleName)))
            {
                manifest = localManifest;
                uri = Application.streamingAssetsPath + "/" + bundleName;
            }
            else
            {
                manifest = remoteManifest;
                uri = remoteSrv + "/" + manifestFileName + "/" + bundleName;
            }

            www = UnityWebRequest.GetAssetBundle(uri, manifest.GetAssetBundleHash(bundleName), 0);
            yield return www.Send();
            if (www.isError)
                Debug.LogErrorFormat("Download bundle {0} from {1} failed.", bundleName, uri);
            else
            {
                Debug.LogFormat("Download bundle {0} from {1} succeed.", bundleName, uri);
                bundles.Add(bundleName, DownloadHandlerAssetBundle.GetContent(www));
            }
        }
    }

    public class LoadManifestCB
    {
        public Action<AssetBundleManifest> Cb = null;
    }

    public class LoadAssetCB
    {
        public Action<GameObject> Cb = null;
    }
}


