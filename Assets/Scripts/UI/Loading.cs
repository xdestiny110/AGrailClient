using Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;

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
            base.Awake();
        }

        void Start()
        {
            StartCoroutine(refreshRate());
        }

        private IEnumerator refreshRate()
        {
            var val = AssetBundleManager.Instance.Progress;
            while (val < 100)
            {
                progress.text = "Loading: " + val.ToString() + "%";
                yield return new WaitForSeconds(0.3f);
                val = AssetBundleManager.Instance.Progress;
            }
            progress.text = "Loading: " + val.ToString() + "%";
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
        }
    }
}

