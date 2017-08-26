using UnityEngine;
using System.Collections.Generic;
using Framework.AssetBundle;
using System.Collections;
using System;

namespace Framework.UI
{
    public sealed class WindowFactory : Singleton<WindowFactory>
    {
        public WindowFactory() { }

        public const string WindowPrefabPath = "UI";

        private Dictionary<WindowType, GameObject> goPool = new Dictionary<WindowType, GameObject>();

        public GameObject CreateWindows(WindowType type, bool isResource = false)
        {
            if (!goPool.ContainsKey(type))
            {
                if (!isResource)
                {
                    //var guid = System.Guid.NewGuid();
                    //TimeScope.Start(guid, "create window " + type.ToString());
                    goPool.Add(type, AssetBundleManager.Instance.LoadAsset(WindowPrefabPath, type.ToString()));
                    //TimeScope.Stop(guid);
                }

                else
                    goPool.Add(type, Resources.Load<GameObject>(WindowPrefabPath + "/" + type.ToString()));
            }
            return GameObject.Instantiate(goPool[type]);
        }

        public IEnumerator CreateWindowsAnyn(WindowType type,  bool isResource = false)
        {
            if (!goPool.ContainsKey(type))
            {
                if (!isResource)
                {
                    var assetbundleReq = AssetBundleManager.Instance.LoadAssetAsynCoro<GameObject>(WindowPrefabPath, type.ToString());
                    yield return assetbundleReq;
                    goPool.Add(type, assetbundleReq.asset as GameObject);
                }
                else
                {
                    var req = Resources.LoadAsync<GameObject>(WindowPrefabPath + "/" + type.ToString());
                    yield return req;
                    goPool.Add(type, req.asset as GameObject);
                }
            }
        }

        public bool allWindowReady = false;
        public IEnumerator PreloadAllWindow()
        {
            allWindowReady = false;
            foreach(var v in Enum.GetNames(typeof(WindowType)))
            {
                var assetbundleReq = AssetBundleManager.Instance.LoadAssetAsynCoro<GameObject>(WindowPrefabPath, v);
                if (assetbundleReq == null || assetbundleReq.asset == null) continue;
                yield return assetbundleReq;
                goPool.Add((WindowType)Enum.Parse(typeof(WindowType), v), assetbundleReq.asset as GameObject);
                try
                {
                    //应对awake中会出现的报错
                    var go = GameObject.Instantiate(assetbundleReq.asset);
                    GameObject.Destroy(go);
                }
                catch (Exception e) { }
            }
            allWindowReady = true;
        }
    }
}

