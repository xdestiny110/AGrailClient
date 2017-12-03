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
        [SerializeField]
        private Button button0;
        [SerializeField]
        private Button button1;
        [SerializeField]
        private Button button2;
        [SerializeField]
        private Button button3;
        [SerializeField]
        private Button button4;
        [SerializeField]
        private Button button5;

        public override WindowType Type
        {
            get
            {
                return WindowType.HandBook;
            }
        }

        public override void Awake()
        {
            button0.onClick.AddListener(delegate { GenerateHero(0); });
            button1.onClick.AddListener(delegate { GenerateHero(1); });
            button2.onClick.AddListener(delegate { GenerateHero(2); });
            button3.onClick.AddListener(delegate { GenerateHero(3); });
            button4.onClick.AddListener(delegate { GenerateHero(4); });
            button5.onClick.AddListener(delegate { GenerateHero(5); });
            GenerateHero(0);
            base.Awake();
        }
        public void GenerateHero(uint p)
        {
            for (int i = 0; i < SkillArea.childCount; i++)
            {
                Destroy(SkillArea.GetChild(i).gameObject);
            }
            var prefab = AssetBundleManager.Instance.LoadAsset("lobby", "HeroIco");
            for (uint a = 1; a < 33; a++)
            {
                uint i = a;
                if (a == 32) i = 108;
                var role = RoleFactory.Create(i);
                if(p==0 || role.RoleProperty == (Card.CardProperty)p)
                { 
                var go = Instantiate(prefab);
                go.transform.SetParent(SkillArea);
                go.transform.localScale = new Vector3(1, 1, 1);
                go.transform.GetComponent<Image>().sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_h", i.ToString() + "h");
                go.transform.GetComponent<Button>().onClick.AddListener(
                    () => { GameManager.UIInstance.PushWindow(Framework.UI.WindowType.InfomationUI, Framework.UI.WinMsg.None, -1, Vector3.zero, role); });
                go.transform.GetChild(0).GetComponent<Text>().text = role.RoleName;
                }
            }
        }
    }
}