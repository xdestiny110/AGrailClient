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
                //服务器没完成的情况下暂时先忽略
                return true;
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
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsPlayer:
                        return "StandaloneWindows";
                    case RuntimePlatform.Android:
                        return "Android";
                    default:
                        return "Error";
                }
#endif
            }
        }

        private bool noCoro = true;
        public float Progress
        {
            get
            {
                float val = 0;
                foreach(var www in wwws)
                    val += www.progress;
                foreach (var req in reqs)
                    val += req.downloadProgress;
                var cnt = wwws.Count + reqs.Count;
                val /= (cnt == 0) ? 1 : cnt;
                if (cnt == 0 && !noCoro)
                    val = 100;
                return val;
            }
        }

        private const string remoteSrv = "http://10.0.2.114:7888/";
        private AssetBundleManifest localManifest = null;
        private AssetBundleManifest remoteManifest = null;
        private Dictionary<string, UnityEngine.AssetBundle> bundles = new Dictionary<string, UnityEngine.AssetBundle>();
        private List<WWW> wwws = new List<WWW>();
        private List<UnityWebRequest> reqs = new List<UnityWebRequest>();

        void init()
        {
            Debug.LogFormat("SimulationMode = {0}", SimulationMode);
            Caching.maximumAvailableDiskSpace = 200 * 1024 * 1024;
        }

        void Awake()
        {
            init();
        }

        public void LoadManifestAsyn(Action<AssetBundleManifest> cb, Action errCb)
        {
            StartCoroutine(LoadManifestCoro(cb, errCb));
        }

        public IEnumerator LoadManifestCoro(Action<AssetBundleManifest> cb, Action errCb)
        {
            if (SimulationMode)
            {
                noCoro = false;
                if (cb != null)
                    cb(null);
                yield break;
            }
            localManifest = null;
            remoteManifest = null;

            yield return StartCoroutine(downloadAssetBundleManifest(true));
            if(!IgnoreBundleServer)
                yield return StartCoroutine(downloadAssetBundleManifest(false));

            if (isError)
            {
                if (errCb != null)
                    errCb();
                yield break;
            }

            //两个Manifest进行比较
            //若有不同则针对不同进行下载
            //这里相当于直接加载及解压了所有AssetBundles
            if(remoteManifest != null)
            {
                foreach (var v in remoteManifest.GetAllAssetBundles())
                {
                    if(localManifest != null)
                    {
                        if(localManifest.GetAssetBundleHash(v) != remoteManifest.GetAssetBundleHash(v))
                            yield return StartCoroutine(downloadAssetBundle(remoteManifest, v, false));
                        else
                            yield return StartCoroutine(downloadAssetBundle(localManifest, v, true));
                    }
                    else
                        yield return StartCoroutine(downloadAssetBundle(remoteManifest, v, false));
                    if (isError)
                    {
                        if (errCb != null)
                            errCb();
                        yield break;
                    }
                }
                if(cb != null)
                    cb(remoteManifest);
            }
            else if(localManifest != null)
            {
                foreach (var v in localManifest.GetAllAssetBundles())
                {
                    yield return StartCoroutine(downloadAssetBundle(localManifest, v, true));
                    if (isError)
                    {
                        if (errCb != null)
                            errCb();
                        yield break;
                    }
                }
                if (cb != null)
                    cb(localManifest);
            }
            else
            {
                if (errCb != null)
                    errCb();
                Debug.LogError("localMainifest is null!");
            }
        }

        public T LoadAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            assetBundleName = assetBundleName.ToLower();
#if UNITY_EDITOR
            if (SimulationMode)
            {
                var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetBundleName, assetName);
                    return null;
                }
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
            }
            else if (bundles.ContainsKey(assetBundleName))
                return bundles[assetBundleName].LoadAsset<T>(assetName);
            else
            {
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetBundleName, assetName);
                return null;
            }
#else
            if (bundles.ContainsKey(assetBundleName))
                return bundles[assetBundleName].LoadAsset<T>(assetName);
            else
            {
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetBundleName, assetName);
                return null;
            }
#endif
        }

        public GameObject LoadAsset(string assetBundleName, string assetName)
        {
            return LoadAsset<GameObject>(assetBundleName, assetName);
        }

        public void LoadAssetAsyn(string assetbundleName, string assetName, Action<GameObject> cb)
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

        public IEnumerator LoadAssetAsynCoro(string assetbundleName, string assetName, Action<GameObject> cb)
        {
            assetbundleName = assetbundleName.ToLower();
            if (bundles.ContainsKey(assetbundleName))
            {
                var req = bundles[assetbundleName].LoadAssetAsync<GameObject>(assetName);
                yield return req;
                if(cb != null)
                    cb(req.asset as GameObject);
            }
            else
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
        }

        private bool isError = false;
        private IEnumerator downloadAssetBundleManifest(bool isLocal)
        {
            if (isLocal)
            {
                var uri =
#if UNITY_EDITOR || UNITY_STANDALONE
                 "file://" + Application.streamingAssetsPath + "/" + manifestFileName;
#elif UNITY_ANDROID
                "jar:file://" + Application.dataPath + "!/assets/" + manifestFileName;
#endif
                using(var www = new WWW(uri))
                {
                    wwws.Add(www);
                    noCoro = false;
                    yield return www;
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogErrorFormat("Can not get manifest! Uri = {0}. Error = {1}", uri, www.error);
                        isError = true;
                    }
                    else
                        localManifest = www.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    wwws.Remove(www);
                }
            }
            else
            {
                var uri = remoteSrv + manifestFileName + "/" + manifestFileName;
                using(var req = UnityWebRequest.GetAssetBundle(uri))
                {
                    reqs.Add(req);
                    noCoro = false;
                    yield return req.Send();
                    if (req.isError)
                    {
                        Debug.LogErrorFormat("Can not get manifest! Uri = {0}.", uri);
                        isError = true;
                    }
                    else
                        remoteManifest = DownloadHandlerAssetBundle.GetContent(req).LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                    reqs.Remove(req);
                }
            }
        }

        private IEnumerator downloadAssetBundle(AssetBundleManifest manifest, string bundleName, bool isLocal)
        {
            if (isLocal)
            {
                var uri =
#if UNITY_EDITOR || UNITY_STANDALONE
                 "file://" + Application.streamingAssetsPath + "/" + bundleName;
#elif UNITY_ANDROID
                "jar:file://" + Application.dataPath + "!/assets/" + bundleName;
#endif
                using (var www = WWW.LoadFromCacheOrDownload(uri, manifest.GetAssetBundleHash(bundleName)))
                {
                    wwws.Add(www);
                    noCoro = false;
                    yield return www;
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogErrorFormat("Download bundle {0} from {1} failed.", bundleName, uri);
                        isError = true;
                    }
                    else
                    {
                        Debug.LogFormat("Download bundle {0} from {1} succeed.", bundleName, uri);
                        bundles.Add(bundleName, www.assetBundle);
                    }
                    wwws.Remove(www);
                }
            }
            else
            {
                var uri = remoteSrv + "/" + manifestFileName + "/" + bundleName;
                using (var req = UnityWebRequest.GetAssetBundle(uri, manifest.GetAssetBundleHash(bundleName), 0))
                {
                    reqs.Add(req);
                    noCoro = false;
                    yield return req.Send();
                    if (req.isError)
                    {
                        Debug.LogErrorFormat("Download bundle {0} from {1} failed.", bundleName, uri);
                        isError = true;
                    }
                    else
                    {
                        Debug.LogFormat("Download bundle {0} from {1} succeed.", bundleName, uri);
                        bundles.Add(bundleName, DownloadHandlerAssetBundle.GetContent(req));
                    }
                    reqs.Remove(req);
                }
            }
        }
    }
}


