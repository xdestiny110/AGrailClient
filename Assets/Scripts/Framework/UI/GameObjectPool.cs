using UnityEngine;
using LitJson;
using System.Collections.Generic;
using System;

namespace Framework
{
    namespace UI
    {
        /// <summary>
        /// 由json配置表获取resource中的预制体
        /// </summary>
        public class GameObjectPool
        {
            private static GameObjectPool instance;
            private static object locker = new object();
            public static GameObjectPool Instance
            {
                get
                {
                    if (instance == null)
                    {
                        lock (locker)
                        {
                            if (instance == null)
                            {
                                instance = new GameObjectPool();
                            }
                        }
                    }
                    return instance;
                }
            }

            private string jsonPath = "UIPrefab/UIPrefabPath";
            private Dictionary<WindowType, string> windowPrefabPath = new Dictionary<WindowType, string>();
            private Dictionary<WindowType, GameObject> windowDict = new Dictionary<WindowType, GameObject>();
            private GameObjectPool()
            {
                var jsonText = Resources.Load<TextAsset>(jsonPath);
                var jsonPrefabPath = JsonMapper.ToObject<Dictionary<string, string>>(jsonText.text);
                foreach (var v in jsonPrefabPath)
                    windowPrefabPath.Add((WindowType)Enum.Parse(typeof(WindowType), v.Key), v.Value);
            }

            public GameObject InstanceGO(WindowType windowType)
            {
                GameObject prefab;
                if (windowDict.ContainsKey(windowType))
                    prefab = windowDict[windowType];
                else
                {
                    if (windowPrefabPath.ContainsKey(windowType))
                    {
                        prefab = Resources.Load<GameObject>(windowPrefabPath[windowType]);
                        windowDict.Add(windowType, prefab);
                    }
                    else
                    {
                        Debug.LogError(string.Format("Can not find panel: {0}", windowType));
                        return null;
                    }
                }

                var go = UnityEngine.Object.Instantiate(prefab) as GameObject;
                go.name = windowType.ToString();
                var rectTransform = go.GetComponent<RectTransform>();
                if (rectTransform != null)
                    rectTransform = prefab.GetComponent<RectTransform>();
                go.transform.localPosition = prefab.transform.localPosition;
                go.transform.localRotation = prefab.transform.localRotation;
                go.transform.localScale = prefab.transform.localScale;

                //确保有canvas group
                if (go.GetComponent<CanvasGroup>() == null)
                    go.AddComponent<CanvasGroup>();

                return go;

            }
        }
    }
}


