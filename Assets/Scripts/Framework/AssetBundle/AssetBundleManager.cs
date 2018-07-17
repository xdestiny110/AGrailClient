using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Framework.AssetBundle
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class AssetBundleManager : MonoBehaviour
    {
        private static AssetBundleManager instance = null;
        public static AssetBundleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    if (Application.isPlaying)
                    {
                        var go = new GameObject("AssetBundleMgr");
                        instance = go.AddComponent<AssetBundleManager>();
                        DontDestroyOnLoad(go);
                    }
                    else
                        instance = new AssetBundleManager();

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
        private const string remoteSrv = "http://10.0.0.90:5061/";
        private const string version = "20171208";
        private Dictionary<string, UnityEngine.AssetBundle> bundles = new Dictionary<string, UnityEngine.AssetBundle>();
        private List<WWW> wwws = new List<WWW>();

        void Awake()
        {
            Debug.LogFormat("SimulationMode = {0}", SimulationMode);
        }

        public UnityEngine.AssetBundle GetLoadedBundle(string bundleName)
        {
            if (bundles.ContainsKey(bundleName))
                return bundles[bundleName];
            return null;
        }

        public IEnumerator LoadCheckFileAsync()
        {
            if (SimulationMode)
            {
                progress = 100;
                yield break;
            }

            //验证版本
            if (!IgnoreBundleServer)
            {
                yield return StartCoroutine(GetVersionInfo());
                if (IsError)
                    yield break;
            }

            //读取远端与本地check file
            List<CheckFile> localCheckFile = new List<CheckFile>();
            List<CheckFile> remoteCheckFile = new List<CheckFile>();
            if (File.Exists(PlatformPath.PersistFileSysPath("CheckFile")))
                yield return StartCoroutine(readCheckFile(PlatformPath.PersistPath2URL("CheckFile"), localCheckFile));
            else
                yield return StartCoroutine(readCheckFile(PlatformPath.StreamPath2URL("CheckFile"), localCheckFile));
            if (!IgnoreBundleServer)
                yield return StartCoroutine(readCheckFile(remoteSrv + activePlatform + "/CheckFile", remoteCheckFile));

            if (IsError)
                yield break;

            //移动pb文件至Persistent path
            Debug.Log("move pb file to persistent path");
            foreach (var v in localCheckFile)
            {
                if (v.name.EndsWith(".pb") && v.location == CheckFile.Location.Local)
                {
                    var www = new WWW(PlatformPath.StreamPath2URL(v.name));
                    yield return www;
                    v.location = CheckFile.Location.Persistent;
                    using (var fs = new FileStream(PlatformPath.PersistFileSysPath(v.name), FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(www.bytes, 0, www.bytes.Length);
                    }
                }
            }

            //比较check file
            Debug.Log("Compare check file");
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
                foreach (var v in localCheckFile)
                {
                    if (!finalCheckFile.Contains(v) && v.location == CheckFile.Location.Persistent)
                        File.Delete(PlatformPath.PersistFileSysPath(v.name));
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
            Debug.Log("Update check file");
            finalCheckFile.ForEach(cf => { cf.location = (cf.location == CheckFile.Location.Remote) ? CheckFile.Location.Persistent : cf.location; });
            writeCheckFile(PlatformPath.PersistFileSysPath("CheckFile"), finalCheckFile);

            //TODO: 目前直接读取所有bundle, 对于大工程需要改为依赖加载
            Debug.Log("load all bundle");
            foreach (var v in finalCheckFile)
            {
                if (v.name.EndsWith(".pb"))
                    continue;
                switch (v.location)
                {
                    case CheckFile.Location.Local:
                        yield return StartCoroutine(downloadAssetBundle(PlatformPath.StreamPath2URL(v.name), v.name));
                        break;
                    case CheckFile.Location.Persistent:
                        yield return StartCoroutine(downloadAssetBundle(PlatformPath.PersistPath2URL(v.name), v.name));
                        break;
                }
            }

            progress = 1.001f;
            coros.Clear();
            wwws.Clear();
        }

        public Texture LoadAssetTexture(string assetBundleName, string assetName)
        {
            return LoadAsset<Texture>(assetBundleName, assetName);
        }

        public GameObject LoadAssetGameObject(string assetBundleName, string assetName)
        {
            return LoadAsset<GameObject>(assetBundleName, assetName);
        }

        public TextAsset LoadAssetText(string assetBundleName, string assetName)
        {
            return LoadAsset<TextAsset>(assetBundleName, assetName);
        }

        public GameObject LoadAsset(string assetBundleName, string assetName)
        {
            return LoadAsset<GameObject>(assetBundleName, assetName);
        }

        public T LoadAsset<T>(string assetBundleName, string assetName) where T : UnityEngine.Object
        {
            assetBundleName = assetBundleName.ToLower();
#if UNITY_EDITOR
            if (SimulationMode)
            {
                var op = new LoadAssetSimulation<T>(assetBundleName, assetName);
                if (!string.IsNullOrEmpty(op.Error) && !assetBundleName.StartsWith("lua"))
                    Debug.LogError(op.Error);
                return op.Asset;
            }
            else if (bundles.ContainsKey(assetBundleName))
                return bundles[assetBundleName].LoadAsset<T>(assetName);
            else
            {
                if (!assetBundleName.StartsWith("lua"))
                    Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetBundleName, assetName);
                return null;
            }
#else
            if (bundles.ContainsKey(assetBundleName))
                return bundles[assetBundleName].LoadAsset<T>(assetName);
            else
            {
                if(!assetBundleName.StartsWith("lua"))
                    Debug.LogErrorFormat("There is no asset with name {0}/{1}", assetBundleName, assetName);
                return null;
            }
#endif
        }

        public AssetLoadOperation<T> LoadAssetAsyn<T>(string assetbundleName, string assetName)
            where T : UnityEngine.Object
        {
            assetbundleName = assetbundleName.ToLower();
#if UNITY_EDITOR
            if (SimulationMode)
                return new LoadAssetSimulation<T>(assetbundleName, assetName);
#endif
            return new LoadAssetOperation<T>(assetbundleName, assetName);
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

            if (!www.text.StartsWith(version))
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
                LitJson.JsonData data = LitJson.JsonMapper.ToObject(www.text);
                for (int i = 0; i < data.Count; i++)
                {
                    var c = new CheckFile()
                    {
                        name = data[i]["name"].ToString(),
                        hash = data[i]["hash"].ToString(),
                        location = (CheckFile.Location)(int)data[i]["location"]
                    };
                    checkFile.Add(c);
                }
            }
        }

        private void writeCheckFile(string filePath, List<CheckFile> checkFiles)
        {
            var json = LitJson.JsonMapper.ToJson(checkFiles);
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                fs.Write(bytes, 0, bytes.Length);
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
            using (FileStream fs = new FileStream(PlatformPath.PersistFileSysPath(bundleName), FileMode.Create, FileAccess.Write))
            {
                fs.Write(www.bytes, 0, www.bytes.Length);
            }
        }
    }
}


