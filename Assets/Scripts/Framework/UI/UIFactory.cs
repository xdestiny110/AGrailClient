using UnityEngine;
using System.Collections.Generic;
using Framework.AssetBundle;
using System.Collections;
using System;

namespace Framework.UI
{
    public sealed class UIFactory : Singleton<UIFactory>
    {
        public UIFactory() { }

        public const string UIPrefabPath = "UI";

        private Dictionary<string, GameObject> goPool = new Dictionary<string, GameObject>();

        public GameObject CreateUI(string uiName, bool isResource = false)
        {
            if (!goPool.ContainsKey(uiName))
            {
                if (!isResource)
                    goPool.Add(uiName, AssetBundleManager.Instance.LoadAsset(UIPrefabPath, uiName.ToString()));
                else
                    goPool.Add(uiName, Resources.Load<GameObject>(UIPrefabPath + "/" + uiName.ToString()));
            }
            return UnityEngine.Object.Instantiate(goPool[uiName]);
        }

        public IEnumerator CreateUIAnyn(string uiName,  bool isResource = false)
        {
            if (!goPool.ContainsKey(uiName))
            {
                if (!isResource)
                {
                    var op = AssetBundleManager.Instance.LoadAssetAsyn<GameObject>(UIPrefabPath, uiName);
                    yield return op;
                    if (!string.IsNullOrEmpty(op.Error))
                        Debug.LogError(op.Error);
                    goPool.Add(uiName, op.Asset);
                }
                else
                {
                    var req = Resources.LoadAsync<GameObject>(UIPrefabPath + "/" + uiName);
                    yield return req;
                    if (req.asset == null)
                        Debug.LogErrorFormat("Can not load {0}/{1} from resource", UIPrefabPath, uiName);
                    goPool.Add(uiName, req.asset as GameObject);
                }
            }
        }

        //public bool allWindowReady = false;
        //public IEnumerator PreloadAllWindow()
        //{
        //    allWindowReady = false;
        //    foreach(var v in Enum.GetNames(typeof(WindowType)))
        //    {
        //        var assetbundleReq = AssetBundleManager.Instance.LoadAssetAsynCoro<GameObject>(UIPrefabPath, v);
        //        if (assetbundleReq == null || assetbundleReq.asset == null) continue;
        //        yield return assetbundleReq;
        //        goPool.Add((WindowType)Enum.Parse(typeof(WindowType), v), assetbundleReq.asset as GameObject);
        //        try
        //        {
        //            //应对awake中会出现的报错
        //            var go = GameObject.Instantiate(assetbundleReq.asset);
        //            GameObject.Destroy(go);
        //        }
        //        catch (Exception e) { }
        //    }
        //    allWindowReady = true;
        //}
    }
}

