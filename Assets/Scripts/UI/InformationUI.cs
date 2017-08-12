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
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", ((int)role.RoleID).ToString() + "L");
                if (sprite != null)
                    hero.sprite = sprite;

                var prefab = AssetBundleManager.Instance.LoadAsset("battle", "SkillInfo");
                foreach(var v in role.Skills.Values)
                {
                    var go = Instantiate(prefab);
                    go.transform.parent = skillRoot;
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<SkillInfo>().Skill = v;
                }
            }
        }

        public override void Awake()
        {
            root.localScale = Vector3.one * 0.1f;
            root.DOScale(1, 0.5f).OnComplete(()=> { btnClose.onClick.AddListener(() => { GameManager.UIInstance.PopWindow(WinMsg.None); }); });            

            base.Awake();
        }

        public override void OnDestroy()
        {
            btnClose.onClick.RemoveAllListeners();

            base.OnDestroy();
        }

    }
}
