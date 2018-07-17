using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.AssetBundle
{
    public abstract class AssetBundleLoadOperation : IEnumerator
    {
        public object Current { get; protected set; }
        public float Progress { get; protected set; }
        public string Error { get; protected set; }

        public void Reset()
        {
            throw new NotSupportedException("Not support calling Reset() on iterator.");
        }

        public abstract bool MoveNext();
    }

    public abstract class AssetLoadOperation<T> : AssetBundleLoadOperation
    {
        public string BundleName { protected set; get; }
        public string AssetName { protected set; get; }
        public T Asset { protected set; get; }

        public AssetLoadOperation(string bundleName, string assetName)
        {
            BundleName = bundleName;
            AssetName = assetName;
            Asset = default(T);
        }

        public AssetLoadOperation(string error)
        {
            Error = error;
        }
    }

    public class LoadAssetOperation<T> : AssetLoadOperation<T> where T : UnityEngine.Object
    {
        private AssetBundleRequest assetBundleRequest;

        public LoadAssetOperation(string bundleName, string assetName) : base(bundleName, assetName)
        {
            var bundle = AssetBundleManager.Instance.GetLoadedBundle(bundleName);
            if (bundle != null)
                assetBundleRequest = bundle.LoadAssetAsync<T>(assetName);
        }

        public LoadAssetOperation(string error) : base(error) { }

        public override bool MoveNext()
        {
            if (assetBundleRequest == null)
                return false;
            if(assetBundleRequest.isDone)
            {
                Asset = assetBundleRequest.asset as T;
                if (Asset == null)
                    Error = string.Format("Can not load asset {0}/{1}", BundleName, AssetName);
                return false;
            }
            Progress = assetBundleRequest.progress;
            return true;
        }
    }

#if UNITY_EDITOR
    public class LoadAssetSimulation<T> : AssetLoadOperation<T> where T : UnityEngine.Object
    {
        public LoadAssetSimulation(string bundleName, string assetName) : base(bundleName, assetName)
        {
            var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, assetName);
            if (assetPaths.Length == 0)
            {
                Error = string.Format("There is no asset with name {0}/{1}", bundleName, assetName);
                return;
            }
            Asset = AssetDatabase.LoadAssetAtPath<T>(assetPaths[0]);
            Progress = 1;
        }

        public LoadAssetSimulation(string error) : base(error) { }

        public override bool MoveNext()
        {
            return false;
        }
    }
#endif
}
