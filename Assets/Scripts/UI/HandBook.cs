using UnityEngine;
using System.Collections;
using Framework.UI;
using System.Linq;
using UnityEngine.UI;
using System.Collections.Generic;
using Framework.AssetBundle;
using UnityEngine.SceneManagement;
using Framework.Message;

namespace AGrail
{


    public class HandBook : WindowsBase
    {
        [SerializeField]
        private Transform SkillArea;

        public override WindowType Type
        {
            get
            {
                return WindowType.HandBook;
            }
        }

        public override void Awake()
        {
            var prefab = AssetBundleManager.Instance.LoadAsset("lobby", "HeroIco");
            for (uint a=1;a<33;a++)
            {
                uint i = a;
                if (a == 32) i = 108;
                var go = Instantiate(prefab);
                go.transform.SetParent(SkillArea);
                var role = RoleFactory.Create(i);
                go.transform.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_h", i.ToString() + "h");
                go.transform.GetComponent<Button>().onClick.AddListener(
                    () => { GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InfomationUI, Framework.UI.WinMsg.None, -1, Vector3.zero, role); });
                go.transform.GetChild(0).GetComponent<Text>().text = role.RoleName;
            }
            base.Awake();
        }

        public void test()
        {
            GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InfomationUI, Framework.UI.WinMsg.None, -1, Vector3.zero, 1);
        }

    }
}