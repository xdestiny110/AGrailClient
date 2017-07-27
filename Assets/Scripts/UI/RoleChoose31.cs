using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;

namespace AGrail
{
    public class RoleChoose31 : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private List<GameObject> heros;

        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChoose31;
            }
        }

        public override void Awake()
        {
            for(int i = 0; i < 3; i++)
            {
                var roleID = RoleChoose.Instance.RoleIDs[i];
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_m", roleID.ToString() + "M");
                if(sprite != null)                    
                    heros[i].GetComponent<Image>().sprite = sprite;
                heros[i].GetComponentInChildren<Text>().text = RoleFactory.Create(roleID).RoleName;
                heros[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    RoleChoose.Instance.Choose(roleID);
                    SceneManager.LoadScene(2);
                });
            }
            base.Awake();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}


