using Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Framework.AssetBundle;

namespace AGrail
{
    public class Loading : WindowsBase
    {
        [SerializeField]
        private Text progress;

        public override WindowType Type
        {
            get
            {
                return WindowType.Loading;
            }
        }

        public override void Awake()
        {
            Debug.Log("Show loading UI");
            Invoke("refreshRate", 0.5f);
            base.Awake();
        }

        private void refreshRate()
        {
            progress.text = AssetBundleManager.Instance.Progress.ToString();
        }
    }
}

