using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AGrail
{
    public class SkillInfo : MonoBehaviour
    {
        [SerializeField]
        private Text skillName;
        [SerializeField]
        private Text skillDescription;
        [SerializeField]
        private Text skillTag;

        public Mation Mation
        {
            set
            {
                skillTag.text = value.MationTag;
                skillTag.text = skillTag.text.Replace("【", "");
                skillTag.text = skillTag.text.Replace("】", "　");
                skillDescription.alignment = TextAnchor.MiddleLeft;
                skillDescription.text = value.MationDesc;
                skillDescription.text = skillDescription.text.Replace("\\n", "\n");
                skillName.text = value.MationName;
            }
        }
    }
}
