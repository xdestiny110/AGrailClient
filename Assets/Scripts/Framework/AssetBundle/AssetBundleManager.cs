using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

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

        private string activePlatform
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

        public bool IsError = false;
        public string ErrorInfo = null;
        private float progress = 0;
        public float Progress
        {
            get
            {
                if (wwws.Count > 0)
                {
                    progress = 0;
                    foreach (var v in wwws)
                        progress += v.progress;
                    progress /= wwws.Count;
                }
                return progress * 100;
            }
        }
        private const string remoteSrv = "http://101.201.155.94:5061/";
        private const string version = "20170901";
        private Dictionary<string, UnityEngine.AssetBundle> bundles = new Dictionary<string, UnityEngine.AssetBundle>();
        private List<WWW> wwws = new List<WWW>();

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
                if (cb != null)
                    cb(null);
                progress = 100;
                yield break;
            }

            //验证版本
            if (!IgnoreBundleServer)
            {
                yield return StartCoroutine(GetVersionInfo());
                if (IsError)
                {
                    if (errCb != null)
                        errCb();
                    yield break;
                }
            }

            //读取远端与本地check file
            List<CheckFile> localCheckFile = new List<CheckFile>();
            List<CheckFile> remoteCheckFile = new List<CheckFile>();
            if (File.Exists(Path.Combine(Application.persistentDataPath, "CheckFile")))
                yield return StartCoroutine(readCheckFile(Path.Combine(PersistentDataPath, "CheckFile"), localCheckFile));
            else
                yield return StartCoroutine(readCheckFile(Path.Combine(StreamingAssetsPath, "CheckFile"), localCheckFile));
            if (!IgnoreBundleServer)
                yield return StartCoroutine(readCheckFile(remoteSrv + activePlatform + "/CheckFile", remoteCheckFile));

            if (IsError)
            {
                if (errCb != null)
                    errCb();
                yield break;
            }

            //比较check file
            List<CheckFile> finalCheckFile = new List<CheckFile>();
            if (remoteCheckFile.Count > 0)
            {
                for (int i = 0; i < remoteCheckFile.Count; i++)
                {
                    var idx = localCheckFile.IndexOf(remoteCheckFile[i]);
                    if (idx >= 0)
                        finalCheckFile.Add(new CheckFile()
                        { name = localCheckFile[idx].name, hash = localCheckFile[idx].hash, location = localCheckFile[idx].location });
                    else
                        finalCheckFile.Add(new CheckFile()
                        { name = remoteCheckFile[i].name, hash = remoteCheckFile[i].hash, location = CheckFile.Location.Remote });
                }
                //删除本地冗余的资源
                foreach(var v in localCheckFile)
                {
                    if (!finalCheckFile.Contains(v) && v.location == CheckFile.Location.Persistent)
                        File.Delete(Path.Combine(Application.persistentDataPath, v.name));
                }
            }
            else
                finalCheckFile = localCheckFile;

            //下载新资源
            var coros = new List<Coroutine>();
            foreach (var v in finalCheckFile)
            {
                switch (v.location)
                {
                    case CheckFile.Location.Remote:
                        coros.Add(StartCoroutine(saveAssetBundle(remoteSrv + activePlatform + "/" + v.name, v.name)));
                        break;
                }
            }
            foreach (var v in coros)
                yield return v;

            //更新本地checkfile
            finalCheckFile.ForEach(cf => { cf.location = (cf.location == CheckFile.Location.Remote) ? CheckFile.Location.Persistent : cf.location; });
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, "CheckFile"), FileMode.Create, FileAccess.Write))
            {
                var json = JsonConvert.SerializeObject(finalCheckFile, Formatting.Indented);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);
                fs.Write(bytes, 0, bytes.Length);
            }

            //读取所有bundle
            foreach (var v in finalCheckFile)
            {
                switch (v.location)
                {
                    case CheckFile.Location.Local:
                        yield return StartCoroutine(downloadAssetBundle(Path.Combine(StreamingAssetsPath, v.name), v.name));
                        break;
                    case CheckFile.Location.Persistent:
                        yield return StartCoroutine(downloadAssetBundle(Path.Combine(PersistentDataPath, v.name), v.name));
                        break;
                }
            }

            progress = 1.001f;
            coros.Clear();
            wwws.Clear();
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

        public void LoadAssetAsyn<T>(string assetbundleName, string assetName, Action<T> cb) where T : UnityEngine.Object
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

        public IEnumerator LoadAssetAsynCoro<T>(string assetbundleName, string assetName, Action<T> cb) where T : UnityEngine.Object
        {
            assetbundleName = assetbundleName.ToLower();
            if (bundles.ContainsKey(assetbundleName))
            {
                var req = bundles[assetbundleName].LoadAssetAsync<T>(assetName);
                yield return req;
                if(cb != null)
                    cb(req.asset as T);
            }
            else
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
        }

        public AssetBundleRequest LoadAssetAsynCoro<T>(string assetbundleName, string assetName) where T : UnityEngine.Object
        {
            assetbundleName = assetbundleName.ToLower();
#if UNITY_EDITOR

            if (SimulationMode)
                LoadAsset(assetbundleName, assetName);
            else if (bundles.ContainsKey(assetbundleName))
                return bundles[assetbundleName].LoadAssetAsync<T>(assetName);
            else
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
            return null;
#else
            if (bundles.ContainsKey(assetbundleName))
                return bundles[assetbundleName].LoadAssetAsync<T>(assetName);
            else
                Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetbundleName, assetName);
            return null;
#endif
        }

        private IEnumerator GetVersionInfo()
        {
            var www = new WWW(remoteSrv + "version.txt");
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                IsError = true;
                ErrorInfo = www.error;
                Debug.LogErrorFormat("WWW error occur. Error = {0}", www.error);
                yield break;
            }

            if (www.text != version)
            {
                IsError = true;
                ErrorInfo = "需要更新版本";
                Debug.LogErrorFormat("Need update.");
                yield break;
            }
        }

        private IEnumerator readCheckFile(string filePath, List<CheckFile> checkFile)
        {
            Debug.LogFormat("Download check file from {0}", filePath);
            using (var www = new WWW(filePath))
            {
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    IsError = true;
                    ErrorInfo = www.error;
                    Debug.LogErrorFormat("WWW error occur. Error = {0}", www.error);
                    yield break;
                }
                checkFile.AddRange(JsonConvert.DeserializeObject<List<CheckFile>>(www.text));
            }
        }

        private IEnumerator downloadAssetBundle(string bundlePath, string bundleName)
        {
            Debug.LogFormat("Download asset bundle {0} from {1}", bundleName, bundlePath);
            using (var www = new WWW(bundlePath))
            {
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    IsError = true;
                    ErrorInfo = www.error;
                    Debug.LogErrorFormat("WWW error occur. Error = {0}", www.error);
                    yield break;
                }
                bundles[bundleName] = www.assetBundle;
            }
        }

        private IEnumerator saveAssetBundle(string bundlePath, string bundleName)
        {
            Debug.LogFormat("Save asset bundle {0} from {1}", bundleName, bundlePath);
            var www = new WWW(bundlePath);
            wwws.Add(www);
            yield return www;
            if (!string.IsNullOrEmpty(www.error))
            {
                IsError = true;
                ErrorInfo = www.error;
                Debug.LogErrorFormat("WWW error occur. Error = {0}", www.error);
                yield break;
            }
            using (FileStream fs = new FileStream(Path.Combine(Application.persistentDataPath, bundleName), FileMode.Create, FileAccess.Write))
            {
                fs.Write(www.bytes, 0, www.bytes.Length);
            }
        }

        private string StreamingAssetsPath
        {
            get
            {
                return
#if UNITY_EDITOR || UNITY_STANDALONE
                 "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
                "jar:file://" + Application.dataPath + "!/assets/";
#endif
            }
        }

        private string PersistentDataPath
        {
            get
            {
                return
#if UNITY_EDITOR || UNITY_STANDALONE
                    "file:///" + Application.persistentDataPath + "/";
#elif UNITY_ANDROID
                    "file://" + Application.persistentDataPath + "/";
#endif
            }
        }
    }
}


