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
                heroProperty.sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("battle_texture", role.RoleProperty.ToString());
                var sprite = AssetBundleManager.Instance.LoadAsset<Sprite>("hero_l", ((int)role.RoleID).ToString() + "L");
                if (sprite != null)
                    hero.sprite = sprite;


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
