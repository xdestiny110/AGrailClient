using Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Framework.AssetBundle;
using UnityEngine.UI;

namespace AGrail
{
    public class ArgsUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Transform panel;

        public override WindowType Type
        {
            get
            {
                return WindowType.ArgsUI;
            }
        }

        public override object[] Parameters
        {
            get
            {
                return base.Parameters;
            }
            set
            {
                base.Parameters = value;
                var args = (List<List<uint>>)value[0];//参数列表
                var explain = value[1] as List<string>;//说明文字列表
                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "ArgsItem");
                for(int i = 0; i < args.Count; i++)
                {
                    var go = Instantiate(prefab);
                    go.transform.parent = panel;
                    go.transform.localScale = prefab.transform.localScale;
                    go.GetComponentInChildren<Text>().text = explain[i];
                    var idx = i;
                    go.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        BattleData.Instance.Agent.AddSelectArgs(args[idx]);
                    });
                }
            }
        }
    }
}


