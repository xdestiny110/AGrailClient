using UnityEngine;
using System.Collections;
using Framework.UI;
using System;
using UnityEngine.UI;
using Framework.Message;
using System.Collections.Generic;

namespace AGrail
{
    public class ChooseArgsUI : WindowsBase
    {
        [SerializeField]
        private Dropdown args;
        [SerializeField]
        private Text title;

        private List<List<uint>> argsCache = new List<List<uint>>();

        public override WindowType Type
        {
            get
            {
                return WindowType.ChooseArgsUI;
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
                var t = value[0].ToString();
                argsCache = (List<List<uint>>)value[1];
                if (t == "Skill")
                {
                    title.text = "选择技能";
                    for (int i = 0; i < argsCache.Count; i++)                    
                        args.options.Add(new Dropdown.OptionData() { text = Skill.GetSkill(argsCache[i][0]).SkillName });
                }
                if(t == "Card")
                {
                    title.text = "选择卡牌";
                    for (int i = 0; i < argsCache.Count; i++)
                        args.options.Add(new Dropdown.OptionData() { text = Card.GetCard(argsCache[i][0]).Name.ToString() });
                }
                if(t == "Energy")
                {
                    title.text = "选择能量";
                    for (int i = 0; i < argsCache.Count; i++)                    
                        args.options.Add(new Dropdown.OptionData()
                        {
                            text = string.Format("{0}个宝石与{1}个星石", argsCache[argsCache.Count - 1][0], argsCache[argsCache.Count - 1][1])
                        });                    
                }
            }
        }

        public void OnValueChanged(int idx)
        {
            BattleData.Instance.Agent.SelectArgs.Clear();
            BattleData.Instance.Agent.SelectArgs.AddRange(argsCache[idx]);
        }
    }
}

