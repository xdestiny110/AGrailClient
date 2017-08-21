using Framework.UI;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Framework.AssetBundle;

namespace AGrail
{
    public class InformationUI : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Image hero;
        [SerializeField]
        private Image heroProperty;
        [SerializeField]
        private Transform levelRoot;
        [SerializeField]
        private Image Star4;
        [SerializeField]
        private Image Star5;
        [SerializeField]
        private Text roleName;
        [SerializeField]
        private Text heroName;
        [SerializeField]
        private Transform skillRoot;
        [SerializeField]
        private Button btnClose;

        public override WindowType Type
        {
            get
            {
                return WindowType.InfomationUI;
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
                var role = Parameters[0] as RoleBase;
                roleName.text = role.RoleName;
                heroName.text = role.HeroName;
                heroProperty.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", role.RoleProperty.ToString());
                Star4.gameObject.SetActive(role.Star>30);
                Star4.sprite = role.Star == 35 ? AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "star_half")
                                               : AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "star");
                Star5.gameObject.SetActive(role.Star>40);
                Star5.sprite = role.Star == 45 ? AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "star_half")
                                               : AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", "star");
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", ((int)role.RoleID).ToString() + "L");
                if (sprite != null)
                    hero.sprite = sprite;

                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "SkillInfo");
                foreach(var v in role.Mations.Values)
                {
                    var go = Instantiate(prefab);
                    go.transform.parent = skillRoot;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<SkillInfo>().Mation = v;
                }
            }
        }

        public override void Awake()
        {
            var graphics = root.GetComponentsInChildren<Graphic>();
            foreach(var v in graphics)
            {
                if (root.name == v.name) continue;
                var cSrc = v.color;
                cSrc.a = 0;
                var cDst = v.color;
                cDst.a = 1;
                v.color = cSrc;
                DOTween.To(() => v.color, x => v.color = x, cDst, 1).SetOptions(true);
            }
            btnClose.onClick.AddListener(() => { GameManager.UIInstance.PopWindow(WinMsg.None); });
            base.Awake();
        }

        public override void OnDestroy()
        {
            btnClose.onClick.RemoveAllListeners();

            base.OnDestroy();
        }

    }
}
