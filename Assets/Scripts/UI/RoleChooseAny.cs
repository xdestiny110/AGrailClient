using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace AGrail
{
    public class RoleChooseAny : WindowsBase
    {
        [SerializeField]
        private Transform root;
        [SerializeField]
        private Dropdown roleList;
        [SerializeField]
        private Button btnOK;

        private Regex re = new Regex(@"(?<=\()\d+");

        public override WindowType Type
        {
            get
            {
                return WindowType.RoleChooseAny;
            }
        }

        public override void Awake()
        {
            foreach (var v in RoleChoose.Instance.RoleIDs)
                roleList.options.Add(new Dropdown.OptionData() { text = RoleFactory.Create(v).RoleName + "(" + v.ToString() + ")" });
            roleList.RefreshShownValue();
            base.Awake();
        }

        public void OnBtnOKClick()
        {
            var m = re.Matches(roleList.options[roleList.value].text);
            RoleChoose.Instance.Choose(uint.Parse(m[0].Value));
            SceneManager.LoadScene(2);
        }
    }
}


