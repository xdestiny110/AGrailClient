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

        public Mation Mation
        {
            set
            {
                skillDescription.alignment = TextAnchor.MiddleLeft;
                skillDescription.text = value.MationDesc;
                skillDescription.text = skillDescription.text.Replace("\\n", "\n");
                skillName.text = value.MationName;
            }
        }
    }
}
