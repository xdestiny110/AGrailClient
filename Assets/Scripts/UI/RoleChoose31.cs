using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;

namespace AGrail
{
    public class RoleChoose31 : WindowsBase
    {
        [SerializeField]
        private Transform root;

        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChoose31;
            }
        }

        public override void Awake()
        {
            foreach(var v in RoleChoose.Instance.RoleIDs)
            {
                var go = new GameObject();
                go.transform.SetParent(root);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.AddComponent<RawImage>().texture = Resources.Load<Texture2D>("Hero/" + v.ToString());
                var btn = go.AddComponent<Button>();
                var cb = btn.colors;
                var c = cb.normalColor;
                c.a = 0.5f;
                cb.normalColor = c;
                btn.colors = cb;
                uint roleID = v;
                btn.onClick.AddListener(()=>
                {
                    RoleChoose.Instance.Choose(roleID);
                    GameManager.UIInstance.PopWindow(WinMsg.Resume);
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


