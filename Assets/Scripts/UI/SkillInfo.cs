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

        public Skill Skill
        {
            set
            {
                skillDescription.alignment = TextAnchor.MiddleLeft;
                skillDescription.text = value.Description;
                skillDescription.text = skillDescription.text.Replace("\\n", "\n");
                skillName.text = value.SkillName;
            }
        }
    }
}
